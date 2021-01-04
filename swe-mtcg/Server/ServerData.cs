using System;
using System.Collections.Concurrent;
using System.Diagnostics;
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

        static ServerData()
        {
        }

        private ServerData()
        {
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
                Console.WriteLine(e);
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
    }
}