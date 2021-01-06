using System;
using System.Collections.Concurrent;
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
        public string NameTag { get; set; }
        public string Bio { get; set; }
        public string Status { get; set; }
        public double Elo { get; private set; }
        public int TotalGames { get; private set; }
        public int Wins { get; private set; }
        public int Looses { get; private set; }
        public double Coins { get; set; }
        public string Password { get; }
        public string Token { get; }
        public ICardCollection Stack { get; set; }
        public ICardCollection Deck { get; set; }

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
            Stack = new CardCollection();
            Deck = new CardCollection(4);
            Wins = 0;
            Looses = 0;
            TotalGames = 0;
        }

        // public User(string loginName, string password, string nameTag = "", string bio = "", string status = "")
        // {
        //     LoginName = loginName;
        //     Password = password;
        //     Token = loginName + "-mtcgToken";
        //     NameTag = nameTag == "" ? loginName : nameTag;
        //     Bio = bio;
        //     Status = status;
        //     Elo = 100;
        //     Coins = 20;
        //     Stack = new CardCollection();
        //     Deck = new CardCollection(4);
        //     Wins = 0;
        //     Looses = 0;
        //     TotalGames = 0;
        // }


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