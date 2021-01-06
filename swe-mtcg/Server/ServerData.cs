using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using swe_mtcg.Card;

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
        private ConcurrentBag<ICardCollection> _availablePackages;
        private ConcurrentQueue<string> _battleQ;
        private ConcurrentQueue<string> _batteResults;

        static ServerData()
        {
        }

        private ServerData()
        {
            _battleQ = new ConcurrentQueue<string>();
            _batteResults = new ConcurrentQueue<string>();
            _users = new ConcurrentDictionary<string, User.User>();
            _availablePackages = new ConcurrentBag<ICardCollection>();
        }

        // For testing with singleton class
        public void Reset()
        {
            _users = new ConcurrentDictionary<string, User.User>();
            _availablePackages = new ConcurrentBag<ICardCollection>();
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
                string token = _users[tmp.LoginName].Token;
                return token;
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
                return string.Empty;
            }
        }

        public bool AddPackage(string jsonBody)
        {
            // TODO Check for duplicate ids?
            string id;
            string name;
            double damage;
            try
            {
                ICardCollection tmpPackage = new CardCollection(5);
                JArray parsedArr = JArray.Parse(jsonBody);
                foreach (JObject jObjectCard in parsedArr.Children<JObject>())
                {
                    id = jObjectCard.GetValue("Id", StringComparison.OrdinalIgnoreCase).Value<string>();
                    name = jObjectCard.GetValue("Name", StringComparison.OrdinalIgnoreCase).Value<string>();
                    damage = jObjectCard.GetValue("Damage", StringComparison.OrdinalIgnoreCase).Value<double>();
                    tmpPackage.AddCard(AbstractCard.CreateCard(name, damage, id));
                }

                _availablePackages.Add(tmpPackage);
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

            bool succeeded = _availablePackages.TryTake(out ICardCollection tmpColl);
            if (succeeded && tmpColl != null && _users[username].Coins >= 5)
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

            // TODO Check username for invalid character
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

        public string GetScoreboard()
        {
            // TODO Add DB Connection + Scoreboard output
            throw new NotImplementedException();
        }

        public string QueueForBattle(string token)
        {
            string username = GetLoginNameFromToken(token);
            _battleQ.Enqueue(username);
            bool res;
            if (_battleQ.Count >= 2)
            {
                res = Battle();
            }

            string battleResMsg;
            // wait 15 sek for result else timeout
            for (int i = 0; i < 15; i++)
            {
                if (_batteResults.TryDequeue(out battleResMsg))
                {
                    return battleResMsg;
                }
                // Sleep not optimal
                // Possible solution callback with event?
                Thread.Sleep(1000);
            }

            return "Timeout";
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

            Random rd = new Random((int) DateTime.Now.Ticks);
            
            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"Battle player1: {user1} vs. player2: {user2}");
            // Actual battle
            string battleResMsg;
            for (int i = 0; i < 100; i++)
            {
                sb.Append($"Round {i}: ");
                if (user1BattleDeck.Count == 0)
                {
                    _users[user1].Loose();
                    _users[user2].Win();
                    sb.AppendLine($"Player2: {user2} Won.");
                    _batteResults.Enqueue(sb.ToString());
                    _batteResults.Enqueue(sb.ToString());
                    return true;
                }

                if (user2BattleDeck.Count == 0)
                {
                    _users[user1].Loose();
                    _users[user2].Win();
                    sb.AppendLine($"Player1: {user1} Won.");
                    _batteResults.Enqueue(sb.ToString());
                    _batteResults.Enqueue(sb.ToString());
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
            _users[user1].Draw();
            _users[user2].Draw();
            sb.AppendLine("Draw");
            _batteResults.Enqueue(sb.ToString());
            _batteResults.Enqueue(sb.ToString());
            return true;
        }
    }
}