using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Text.Json;

namespace swe_mtcg
{
    // Singleton pattern see: https://csharpindepth.com/Articles/Singleton#cctor
    public class MessageCollection
    {
        private static readonly MessageCollection _instance = new MessageCollection();

        public static MessageCollection Instance
        {
            get { return _instance; }
        }

        // Int is ID, string is Message Content
        private ConcurrentDictionary<int, string> _Messages;
        public int MaxIdx { get; private set; }

        public int Count
        {
            get { return _Messages.Count; }
        }

        static MessageCollection()
        {
        }

        private MessageCollection()
        {
            MaxIdx = 0;
            _Messages = new ConcurrentDictionary<int, string>();
        }

        // For Testing with singleton class
        public void Reset()
        {
            MaxIdx = 0;
            _Messages = new ConcurrentDictionary<int, string>();
        }


        public Tuple<int, string> GetMsgTupleFromJson(string jsonMsg)
        {
            int id;
            string content;
            try
            {
                JObject myJobject = JObject.Parse(jsonMsg);
                // id = myJobject.SelectToken("Id").Value<int>(); -- Case Insensitive!!
                id = myJobject.GetValue("Id", StringComparison.OrdinalIgnoreCase).Value<int>();
                //content = myJobject.SelectToken("Content").Value<string>(); -- Case Insensitive!!
                content = myJobject.GetValue("Content", StringComparison.OrdinalIgnoreCase).Value<string>();
                return Tuple.Create(id, content);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ERROR: {ex.Message}");
                return null;
            }
        }


        public bool UpdateMessage(int id, string content)
        {
            if (_Messages.ContainsKey(id))
            {
                _Messages[id] = content;
                return true;
            }

            return false;
        }

        public bool DeleteMessage(int id)
        {
            if (_Messages.ContainsKey(id))
            {
                string rmvdMsg;
                return _Messages.TryRemove(id, out rmvdMsg);
            }

            return false;
        }

        public void AddMessage(string content)
        {
            bool succeeded = _Messages.TryAdd(MaxIdx, content);
            if (succeeded)
            {
                MaxIdx++;
            }
            // TODO Add return status
        }

        public string GetMessageContent(int id)
        {
            if (_Messages.ContainsKey(id))
            {
                return _Messages[id];
            }

            return string.Empty;
        }

        public string GetMessageAsJson(int id)
        {
            if (MaxIdx < id)
            {
                return "";
            }

            if (_Messages.ContainsKey(id))
            {
                //Serialize
                var jsonObject = new JObject();
                jsonObject.Add("Id", id);
                jsonObject.Add("Content", _Messages[id]);
                return jsonObject.ToString();
            }

            return "";
        }

        public string GetMessagesArrayAsJson()
        {
            if (this.Count == 0)
            {
                return "";
            }

            JArray result = new JArray(from m in _Messages
                select new JObject(new JProperty("Id", m.Key), new JProperty("Content", m.Value)));
            return result.ToString();
        }

        public string GetMsgContentFromJson(string jsonMsg)
        {
            string content;
            try
            {
                JObject myJobject = JObject.Parse(jsonMsg);
                //content = myJobject.SelectToken("Content").Value<string>(); -- Case Insensitive!!
                content = myJobject.GetValue("Content", StringComparison.OrdinalIgnoreCase).Value<string>();
                return content;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ERROR: {ex.Message}");
                return string.Empty;
            }
        }

        public int GetIdFromJson(string jsonMsg)
        {
            int id;
            try
            {
                JObject myJobject = JObject.Parse(jsonMsg);
                id = myJobject.GetValue("Id", StringComparison.OrdinalIgnoreCase).Value<int>();
                return id;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ERROR: {ex.Message}");
                return -1;
            }
        }
    }
}