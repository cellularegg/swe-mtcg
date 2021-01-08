using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using System.Security.Principal;
using System.Text;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using swe_mtcg.Card;

namespace swe_mtcg.User
{
    public class User
    {
        public string LoginName { get; set; }
        [JsonProperty("Name")] public string NameTag { get; set; }
        public string Bio { get; set; }
        public string Status { get; set; }
        public double Elo { get; set; }
        public int TotalGames { get; private set; }
        public int Wins { get; private set; }
        public int Looses { get; private set; }
        public double Coins { get; set; }
        [JsonIgnore] public string Password { get; }
        [JsonIgnore] public string Token { get; }
        [JsonIgnore] public ICardCollection Stack { get; set; }
        [JsonIgnore] public ICardCollection Deck { get; set; }
        [JsonIgnore] public Dictionary<DateTime, BattleDetails> MatchHistory { get; set; }

        [JsonConstructor]
        public User(string username, string password)
        {
            if (!Regex.IsMatch(username, "^[a-zA-Z]+$"))
            {
                throw new ArgumentException(
                    "Error username cannot be an empty string (Special characters are automatically removed).");
            }

            LoginName = username;
            Password = password;
            Token = username + "-mtcgToken";
            NameTag = username;
            Bio = "";
            Status = "";
            Elo = 100;
            Coins = 20;
            MatchHistory = new Dictionary<DateTime, BattleDetails>();
            Stack = new CardCollection();
            Deck = new CardCollection(4);
            Wins = 0;
            Looses = 0;
            TotalGames = 0;
        }

        public bool MoveCardToDeck(string id)
        {
            if (!Stack.Cards.ContainsKey(id))
            {
                return false;
            }

            return Deck.AddCard(Stack.RemoveCard(id));
        }

        public bool MoveCardToStack(string id)
        {
            if (!Deck.Cards.ContainsKey(id))
            {
                return false;
            }

            return Stack.AddCard(Deck.RemoveCard(id));
        }

        public string GetStats()
        {
            // TODO implement DB Connection
            throw new NotImplementedException();
        }

        public void Draw()
        {
            this.TotalGames++;
        }

        public void Loose()
        {
            this.TotalGames++;
            this.Looses++;
            this.Elo -= 3;
        }

        public void Win()
        {
            this.TotalGames++;
            this.Wins++;
            this.Elo += 5;
        }
    }
}