using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using swe_mtcg.Card;
using swe_mtcg.User;

namespace swe_mtcg
{
    public class ServerData
    {
        private static readonly ServerData _instance = new ServerData();

        public static ServerData Instance
        {
            get { return _instance; }
        }

        private ConcurrentDictionary<string, User.User> _users;

        // bool because it is the smallest data type (1 bit?)
        private Dictionary<string, bool> _allCardIds;
        private ConcurrentBag<ICardCollection> _availablePackages;
        private ConcurrentQueue<string> _battleQ;
        private ConcurrentDictionary<string, TradingDeal> _tradingDeals;

        static ServerData()
        {
        }

        private ServerData()
        {
            _battleQ = new ConcurrentQueue<string>();
            _users = new ConcurrentDictionary<string, User.User>();
            _availablePackages = new ConcurrentBag<ICardCollection>();
            _allCardIds = new Dictionary<string, bool>();
            _tradingDeals = new ConcurrentDictionary<string, TradingDeal>();
        }

        // For testing with singleton class
        public void Reset()
        {
            _battleQ = new ConcurrentQueue<string>();
            _users = new ConcurrentDictionary<string, User.User>();
            _availablePackages = new ConcurrentBag<ICardCollection>();
            _allCardIds = new Dictionary<string, bool>();
            _tradingDeals = new ConcurrentDictionary<string, TradingDeal>();
        }

        public bool RegisterUser(string jsonBody)
        {
            try
            {
                User.User u = JsonConvert.DeserializeObject<User.User>(jsonBody);
                // Maybe need to check for null?
                return _users.TryAdd(u.LoginName, u);
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
                return false;
            }
        }

        public string GetToken(string jsonBody)
        {
            try
            {
                User.User tmp = JsonConvert.DeserializeObject<User.User>(jsonBody);
                string token = "";
                if (_users.ContainsKey(tmp.LoginName) && tmp.Password == _users[tmp.LoginName].Password)
                {
                    token = _users[tmp.LoginName].Token;
                }
                else
                {
                    throw new ArgumentException("Error invalid username or password");
                }

                JObject jsonObject = new JObject {{"token", token}};
                return JsonConvert.SerializeObject(jsonObject);
            }
            // Invalid ArgumentException or JsonReaderException
            catch (Exception e)
            {
                Debug.WriteLine(e);
                return string.Empty;
            }
        }

        public bool AddPackage(string jsonBody)
        {
            string id;
            string name;
            double damage;

            try
            {
                lock (_allCardIds)
                {
                    ICardCollection tmpPackage = new CardCollection(5);
                    JArray parsedArr = JArray.Parse(jsonBody);
                    foreach (JObject jObjectCard in parsedArr.Children<JObject>())
                    {
                        if (jObjectCard.ContainsKey("Id"))
                        {
                            id = jObjectCard.GetValue("Id", StringComparison.OrdinalIgnoreCase).Value<string>();
                        }
                        else
                        {
                            id = "";
                        }

                        name = jObjectCard.GetValue("Name", StringComparison.OrdinalIgnoreCase).Value<string>();
                        damage = jObjectCard.GetValue("Damage", StringComparison.OrdinalIgnoreCase).Value<double>();
                        if (_allCardIds.ContainsKey(id))
                        {
                            return false;
                        }

                        ICard tmp = AbstractCard.CreateCard(name, damage, id);
                        if (tmpPackage.AddCard(tmp))
                        {
                            _allCardIds.Add(tmp.Id.ToString(), true);
                        }
                    }

                    _availablePackages.Add(tmpPackage);
                }

                return true;
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
                return false;
            }
        }


        public bool AcquirePackage(string token)
        {
            string username = this.GetLoginNameFromToken(token);
            if (username == string.Empty)
            {
                return false;
            }

            if (_availablePackages.Count == 0)
            {
                return false;
            }

            if (_users[username].Coins < 5)
            {
                return false;
            }

            bool succeeded = _availablePackages.TryTake(out ICardCollection tmpColl);
            if (succeeded && tmpColl != null)
            {
                _users[username].Coins -= 5;
                foreach (KeyValuePair<string, ICard> card in tmpColl.Cards)
                {
                    _users[username].Stack.AddCard(card.Value);
                }

                return true;
            }

            return false;
        }


        public string GetLoginNameFromToken(string token)
        {
            // Check if user exists & check if token is valid
            if (!token.Contains("-mtcgToken"))
            {
                Debug.WriteLine($"Error invalid token {token}.");
                return string.Empty;
            }

            // Assuming '-' is an invalid Username
            try
            {
                string loginName = token.Split('-')[0];
                if (_users.ContainsKey(loginName))
                {
                    return loginName;
                }

                Debug.WriteLine($"Error user {loginName} does not exist.");
                return string.Empty;
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
                return string.Empty;
            }
        }

        public bool ConfigureDeck(string jsonBody, string token)
        {
            string username = GetLoginNameFromToken(token);
            if (username == string.Empty)
            {
                return false;
            }

            try
            {
                JArray arr = JArray.Parse(jsonBody);
                string[] stringArr = arr.ToObject<string[]>();
                if (stringArr == null)
                {
                    Debug.WriteLine("Error JsonArray conversion to string array failed.");
                    return false;
                }

                if (arr.Count != 4)
                {
                    Debug.WriteLine("Error a configured deck must contain 4 cards.");
                    return false;
                }

                // Check if User owns cards either in deck or in stack
                foreach (string cardId in stringArr)
                {
                    if (!(_users[username].Stack.Cards.ContainsKey(cardId) ||
                          _users[username].Deck.Cards.ContainsKey(cardId)))
                    {
                        Debug.WriteLine($"Error user {username} does not own card {cardId}.");
                        return false;
                    }
                }

                // Remove Cards First
                if (_users[username].Deck.Cards.Count != 0)
                {
                    List<string> cardIds = new List<string>();
                    foreach (KeyValuePair<string, ICard> c in _users[username].Deck.Cards)
                    {
                        cardIds.Add(c.Key);
                    }

                    foreach (string cardId in cardIds)
                    {
                        // Should not fail - maybe should still handle error and maybe cleanup deck?
                        if (!_users[username].MoveCardToStack(cardId))
                        {
                            Debug.WriteLine($"Error moving card {cardId} to stack for user {username}.");
                            return false;
                        }
                    }
                }

                // Add cards to deck
                foreach (var cardId in stringArr)
                {
                    if (!_users[username].MoveCardToDeck(cardId))
                    {
                        Debug.WriteLine($"Error moving card {cardId} to deck for user {username}.");
                        return false;
                    }
                }

                return true;
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
                return false;
            }
        }

        public string GetDeck(string token, bool asJson)
        {
            string username = GetLoginNameFromToken(token);
            if (username == string.Empty)
            {
                return "";
            }

            if (asJson)
            {
                List<ICard> tmpDeck = new List<ICard>();
                foreach (KeyValuePair<string, ICard> card in _users[username].Deck.Cards)
                {
                    tmpDeck.Add(card.Value);
                }

                return JsonConvert.SerializeObject(tmpDeck);
            }

            return _users[username].Deck.ToString();
        }

        public string GetStack(string token, bool asJson)
        {
            string username = GetLoginNameFromToken(token);
            if (username == string.Empty)
            {
                return "";
            }

            if (asJson)
            {
                List<ICard> tmpStack = new List<ICard>();
                foreach (KeyValuePair<string, ICard> card in _users[username].Stack.Cards)
                {
                    tmpStack.Add(card.Value);
                }

                return JsonConvert.SerializeObject(tmpStack);
            }

            return _users[username].Stack.ToString();
        }


        public string QueueForBattle(string token)
        {
            string username = GetLoginNameFromToken(token);
            if (!_users.ContainsKey(username))
            {
                return string.Empty;
            }

            int gameCount = _users[username].MatchHistory.Count;
            _battleQ.Enqueue(username);

            bool res;
            if (_battleQ.Count >= 2)
            {
                res = Battle();
            }

            // string battleResMsg;

            // wait 20 sek for result else timeout
            // Very bad code... -> When Battle takes more than 20 secs all future battles logs are wrong....
            for (int i = 0; i < 200; i++)
            {
                // if (_batteResults.TryDequeue(out battleResMsg))
                // {
                //     return battleResMsg;
                // }
                if (_users[username].MatchHistory.Count > gameCount)
                {
                    // get latest battle
                    BattleDetails bd = _users[username].MatchHistory[_users[username].MatchHistory.Keys.Max()];
                    return JsonConvert.SerializeObject(bd);
                    // return bd.ToString();
                }

                // Sleep not optimal
                // Possible solution callback with event?
                Thread.Sleep(100);
            }

            // unlikely -> if user cannot find opponent to battle
            // dirty fix -> remove a user from Q
            string tmpUser = string.Empty;
            _battleQ.TryDequeue(out tmpUser);
            return string.Empty;
        }

        public bool Battle()
        {
            // Not the best solution hopefully it works ^^'
            // Problem: when two battles finish simultaneously user might not get the correct "battle log"
            string user1 = "";
            string user2 = "";
            lock (_battleQ)
            {
                if (_battleQ.Count < 2)
                {
                    return false;
                }

                _battleQ.TryDequeue(out user1);
                _battleQ.TryDequeue(out user2);
            }

            if (user1 == "" || user2 == "")
            {
                return false;
            }

            // Deep copy of Deck for battle
            List<ICard> user1BattleDeck = new List<ICard>();
            foreach (KeyValuePair<string, ICard> card in _users[user1].Deck.Cards)
            {
                user1BattleDeck.Add(AbstractCard.CreateCard(card.Value.Name, card.Value.Damage, card.Key));
            }

            List<ICard> user2BattleDeck = new List<ICard>();
            foreach (KeyValuePair<string, ICard> card in _users[user2].Deck.Cards)
            {
                user2BattleDeck.Add(AbstractCard.CreateCard(card.Value.Name, card.Value.Damage, card.Key));
            }

            BattleDetails battleDetails = new BattleDetails(user1, user2);

            StringBuilder sb = new StringBuilder();
            // sb.AppendLine($"Battle player1: {user1} vs. player2: {user2}");
            // Actual battle
            string battleResMsg;
            for (int i = 0; i < 100; i++)
            {
                Random rd = new Random((int) DateTime.Now.Ticks);
                // sb.Append($"Round {i}: ");
                if (user1BattleDeck.Count == 0)
                {
                    // you can play practice matches against yourself but cannot "cheat" elo by fighting against yourself
                    if (user1 != user2)
                    {
                        _users[user1].Loose();
                        _users[user2].Win();
                    }

                    battleDetails.Winner = user2;
                    _users[user1].MatchHistory.Add(DateTime.Now, battleDetails);
                    _users[user2].MatchHistory.Add(DateTime.Now, battleDetails);

                    // sb.AppendLine($"Player2: {user2} Won.");
                    // _batteResults.Enqueue(sb.ToString());
                    // _batteResults.Enqueue(sb.ToString());
                    return true;
                }

                if (user2BattleDeck.Count == 0)
                {
                    if (user1 != user2)
                    {
                        _users[user1].Loose();
                        _users[user2].Win();
                    }

                    battleDetails.Winner = user1;
                    _users[user1].MatchHistory.Add(DateTime.Now, battleDetails);
                    _users[user2].MatchHistory.Add(DateTime.Now, battleDetails);
                    // sb.AppendLine($"Player1: {user1} Won.");
                    // _batteResults.Enqueue(sb.ToString());
                    // _batteResults.Enqueue(sb.ToString());
                    return true;
                }

                int rdIdx = rd.Next(user1BattleDeck.Count);
                ICard user1Card = user1BattleDeck[rdIdx];
                user1BattleDeck.RemoveAt(rdIdx);

                rdIdx = rd.Next(user2BattleDeck.Count);
                ICard user2Card = user2BattleDeck[rd.Next(user2BattleDeck.Count)];
                user2BattleDeck.RemoveAt(rdIdx);

                int battleResNr = AbstractCard.Battle(user1Card, user2Card, out battleResMsg);
                sb.AppendLine(battleResMsg);
                battleDetails.AddRound(battleResMsg);
                switch (battleResNr)
                {
                    case 0:
                        // draw
                        user1BattleDeck.Add(user1Card);
                        user2BattleDeck.Add(user2Card);
                        break;
                    case 1:
                        // user 1 wins
                        user1BattleDeck.Add(user1Card);
                        user1BattleDeck.Add(user2Card);
                        break;
                    case 2:
                        // user 2 wins
                        user2BattleDeck.Add(user1Card);
                        user2BattleDeck.Add(user2Card);
                        break;
                }
            }

            if (user1 != user2)
            {
                _users[user1].Draw();
                _users[user2].Draw();
            }

            // sb.AppendLine("Draw");
            battleDetails.Winner = "Draw";
            _users[user1].MatchHistory.Add(DateTime.Now, battleDetails);
            _users[user2].MatchHistory.Add(DateTime.Now, battleDetails);
            // _batteResults.Enqueue(sb.ToString());
            // _batteResults.Enqueue(sb.ToString());
            return true;
        }

        public string GetUserData(string username, string token)
        {
            string tmpUsername = this.GetLoginNameFromToken(token);
            if (tmpUsername != username)
            {
                return string.Empty;
            }

            return JsonConvert.SerializeObject(_users[username]);
        }

        public bool UpdateUserData(string token, string jsonBody)
        {
            string username = this.GetLoginNameFromToken(token);
            if (username == string.Empty || !_users.ContainsKey(username))
            {
                return false;
            }

            try
            {
                JObject myJobject = JObject.Parse(jsonBody);
                string name = myJobject.GetValue("Name", StringComparison.OrdinalIgnoreCase).Value<string>();
                string bio = myJobject.GetValue("Bio", StringComparison.OrdinalIgnoreCase).Value<string>();
                string status = myJobject.GetValue("Status", StringComparison.OrdinalIgnoreCase).Value<string>();
                _users[username].NameTag = name;
                _users[username].Bio = bio;
                _users[username].Status = status;
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"ERROR: {ex.Message}");
                return false;
            }
        }

        public string GetStats(string token)
        {
            string username = GetLoginNameFromToken(token);
            if (username == string.Empty)
            {
                return string.Empty;
            }

            JObject o;
            if (_users[username].TotalGames == 0)
            {
                o = JObject.FromObject(new
                {
                    loginName = _users[username].LoginName,
                    totalGamesPlayed = _users[username].TotalGames,
                    elo = _users[username].Elo,
                    winLooseRatio = 0,
                    wins = _users[username].Wins,
                    looses = _users[username].Looses,
                    draws = _users[username].TotalGames - (_users[username].Looses + _users[username].Wins),
                    matchHistory = from m in _users[username].MatchHistory
                        orderby m.Key descending
                        select new
                        {
                            dateTime = m.Key,
                            rounds = m.Value.Rounds.Count,
                            user1 = m.Value.User1,
                            user2 = m.Value.User2,
                            winner = m.Value.Winner
                        }
                });
                return o.ToString();
            }

            o = JObject.FromObject(new
            {
                loginName = _users[username].LoginName,
                totalGamesPlayed = _users[username].TotalGames,
                elo = _users[username].Elo,
                winrate = _users[username].Wins / _users[username].TotalGames,
                wins = _users[username].Wins,
                looses = _users[username].Looses,
                draws = _users[username].TotalGames - (_users[username].Looses + _users[username].Wins),
                matchHistory = (from m in _users[username].MatchHistory
                    orderby m.Key descending
                    select new
                    {
                        dateTime = m.Key,
                        rounds = m.Value.Rounds.Count,
                        user1 = m.Value.User1,
                        user2 = m.Value.User2,
                        winner = m.Value.Winner
                    }).Take(10)
            });
            return o.ToString();
        }

        public string GetScoreboard()
        {
            JObject o = JObject.FromObject(new
            {
                totalGamesPlayedByAllUsers = _users.Values.Sum(u => u.TotalGames) / 2,
                highestElo = _users.Values.Max(u => u.Elo),
                users = (from u in _users
                    orderby u.Value.Elo descending
                    select new
                    {
                        displayName = u.Value.NameTag,
                        totalGamesPlayed = u.Value.TotalGames,
                        elo = u.Value.Elo,
                        wins = u.Value.Wins,
                        looses = u.Value.Looses,
                        draws = u.Value.TotalGames - (u.Value.Looses + u.Value.Wins),
                    }).Take(5)
            });
            return o.ToString();
        }

        public bool CreateTradingDeal(string token, string jsonBody)
        {
            string username = this.GetLoginNameFromToken(token);
            if (username == string.Empty || !_users.ContainsKey(username))
            {
                return false;
            }

            try
            {
                JObject myJobject = JObject.Parse(jsonBody);
                string tradeId = "";
                double eloWanted = double.MaxValue;
                if (myJobject.ContainsKey("Id"))
                {
                    tradeId = myJobject.GetValue("Id", StringComparison.OrdinalIgnoreCase).Value<string>();
                }

                if (myJobject.ContainsKey("EloWanted"))
                {
                    eloWanted = myJobject.GetValue("EloWanted", StringComparison.OrdinalIgnoreCase).Value<double>();
                }

                string cardToTradeId = myJobject.GetValue("CardToTrade", StringComparison.OrdinalIgnoreCase)
                    .Value<string>();
                string type = myJobject.GetValue("Type", StringComparison.OrdinalIgnoreCase).Value<string>();
                double minimumDamage = myJobject.GetValue("MinimumDamage", StringComparison.OrdinalIgnoreCase)
                    .Value<double>();

                // Check if player has card
                if (!_users[username].Stack.Cards.ContainsKey(cardToTradeId))
                {
                    // Player does not own card or it is currently being used in the deck
                    return false;
                }

                // Remove card from stack
                ICard cardToTrade = _users[username].Stack.RemoveCard(cardToTradeId);
                if (cardToTrade == null)
                {
                    // Error occured when removing card
                    return false;
                }

                // Create new Trading deal with specified requirements
                TradingDeal tradingDeal =
                    new TradingDeal(cardToTrade, username, type, minimumDamage, eloWanted, tradeId);
                if (_tradingDeals.TryAdd(tradingDeal.Id.ToString(), tradingDeal))
                {
                    return true;
                }

                // Adding to Trading Deals failed -> Add Card to player Stack
                _users[username].Stack.AddCard(cardToTrade);
                return false;
            }
            catch (JsonException ex)
            {
                Debug.WriteLine($"ERROR: {ex.Message}");
                return false;
            }
        }

        public bool Trade(string tradeId, string token, string jsonBody)
        {
            string username = this.GetLoginNameFromToken(token);
            if (username == string.Empty || !_users.ContainsKey(username) || !_tradingDeals.ContainsKey(tradeId))
            {
                return false;
            }

            try
            {
                JObject myJobject = JObject.Parse(jsonBody);
                string cardToTradeId = myJobject.GetValue("CardToTrade", StringComparison.OrdinalIgnoreCase)
                    .Value<string>();
                TradingDeal td;
                if (!_tradingDeals.TryRemove(tradeId, out td))
                {
                    return false;
                }
                
                if (td.Owner == username)
                {
                    _tradingDeals.TryAdd(td.Id.ToString(), td);
                    // Trading with yourself is not allowed
                    return false;
                }
                

                // Trade for Card
                if (cardToTradeId != "")
                {
                    // Check if player has card
                    if (!_users[username].Stack.Cards.ContainsKey(cardToTradeId))
                    {
                        _tradingDeals.TryAdd(td.Id.ToString(), td);

                        // Player does not own card or it is currently being used in the deck
                        return false;
                    }

                    // Check requirements
                    bool tradevalid = td.IsValidTrade(_users[username].Stack.Cards[cardToTradeId]);
                    if (!td.IsValidTrade(_users[username].Stack.Cards[cardToTradeId]))
                    {
                        _tradingDeals.TryAdd(td.Id.ToString(), td);

                        // Card does not fulfill requirements
                        return false;
                    }

                    // Remove card from stack
                    ICard cardFromOther = _users[username].Stack.RemoveCard(cardToTradeId);
                    if (cardFromOther == null)
                    {
                        _tradingDeals.TryAdd(td.Id.ToString(), td);

                        // Error occured when removing card
                        return false;
                    }

                    ICard cardFromTradeOwner = _users[td.Owner].Stack.RemoveCard(cardToTradeId);
                    

                    // Adding card "cannot" fail because the stack is locked when adding a card
                    _users[td.Owner].Stack.AddCard(cardFromOther);
                    _users[username].Stack.AddCard(td.CardToTrade);
                    // Remove trading Deal
                    return true;
                }
                // Trade for Elo
                else
                {
                    // Check if other has enough elo
                    if (_users[username].Elo >= td.EloRequirement)
                    {
                        _users[username].Elo -= td.EloRequirement;
                        _users[td.Owner].Elo += td.EloRequirement;
                        _users[username].Stack.AddCard(td.CardToTrade);
                        return true;
                    }
                    else
                    {
                        _tradingDeals.TryAdd(td.Id.ToString(), td);
                        return false;
                    }
                }
            }
            catch (JsonException ex)
            {
                Debug.WriteLine($"ERROR: {ex.Message}");
                return false;
            }
        }

        public string GetTradingDeals()
        {
            JArray o = JArray.FromObject(
                from td in _tradingDeals
                select new
                {
                    tradingId = td.Key,
                    tradingDealOwner = td.Value.Owner,
                    cardToTrade = td.Value.CardToTrade,
                    typeRequired = td.Value.TypeRequirement,
                    damageRequired = td.Value.DamageRequirement,
                    eloReuired = td.Value.EloRequirement,
                });
            return o.ToString();
        }

        public bool DeleteTradingDeal(string token, string tradingDealId)
        {
            string username = this.GetLoginNameFromToken(token);
            if (username == string.Empty || !_users.ContainsKey(username))
            {
                return false;
            }

            // Check if User is owner of trading deal
            if (_tradingDeals.ContainsKey(tradingDealId) && _tradingDeals[tradingDealId].Owner != username)
            {
                return false;
            }

            if (!_tradingDeals.TryRemove(tradingDealId, out TradingDeal tmpTradingDeal) || tmpTradingDeal == null)
            {
                return false;
            }

            if (_users[username].Stack.AddCard(tmpTradingDeal.CardToTrade))
            {
                return true;
            }
            // Should not occur that a card disappears because AddCard Locks the stack of the player

            return false;
        }
    }
}