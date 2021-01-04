using System.Collections.Concurrent;
using System.Security.Principal;
using System.Text;
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
        public double Elo { get; set; }
        public double Coins { get; set; }
        public string Password { get; set; }
        public string Token { get;  }
        public ICardCollection Stack { get; set; }
        public ICardCollection Deck { get;  set; }

        [JsonConstructor]
        public User(string username, string password)
        {
            LoginName = username;
            Password = password;
            Token = username + "-mtcgToken";
            NameTag = "";
            Bio = "";
            Status = "";
            Elo = 100;
            Coins = 20;
            Stack = new CardCollection();
            Deck = new CardCollection(4); 
        }
        public User(string loginName, string password, string nameTag = "", string bio = "", string status = "")
        {
            LoginName = loginName;
            Password = password;
            Token = loginName + "-mtcgToken";
            NameTag = nameTag == "" ? loginName : nameTag;
            Bio = bio;
            Status = status;
            Elo = 100;
            Coins = 20;
            Stack = new CardCollection();
            Deck = new CardCollection(4);
        }
    }
}