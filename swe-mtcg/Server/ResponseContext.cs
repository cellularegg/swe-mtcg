using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;

namespace swe_mtcg
{
    public class ResponseContext
    {
        public string Body { get; private set; }
        public string Status{ get; private set; }
        //private string _ContentType;
        public Dictionary<string, string> Headers { get; private set; }

        private ResponseContext(string status, string contentType, string body)
        {
            //this._ContentType = contentType;
            this.Status = status;
            this.Body = body;
            Headers = new Dictionary<string, string>();
            Headers.Add("Content-Type", contentType);
            Headers.Add("Content-Length", Encoding.UTF8.GetBytes(Body).Length.ToString());
        }

        public static ResponseContext GetResponseContext(RequestContext request)
        {
            if (request == null)
            {
                return NullResponse();
            }
            MessageCollection _msgColl = MessageCollection.Instance;
            int msgId = GetMsgIdFromPath(request.Path);
            if (msgId == -100)
            {
                return PageNotFoundResponse();
            }
            switch (request.Method)
            {
                case RequestMethod.GET:
                    string body;
                    if (request.Path == "/hello")
                    {
                        body = "{\"Hello\":\"World\"}";
                        return new ResponseContext("200 OK", "application/json", body);
                    }
                    else if (msgId == -1)
                    {
                        // TODO CHECK FOR EMTPY body -> Send correct "Error"
                        body = _msgColl.GetMessagesArrayAsJson();
                        return new ResponseContext("200 OK", "application/json", body);
                    }
                    else
                    {
                        // TODO CHECK FOR EMTPY body -> Send correct "Error"
                        body = _msgColl.GetMessageAsJson(msgId);
                        if (body != string.Empty)
                        {
                            return new ResponseContext("200 OK", "application/json", body);
                        }
                        else
                        {
                            return new ResponseContext("400 Bad Request", "text/plain", $"Error, requested msg with Id: {msgId} does not exist.\n");
                        }
                    }
                case RequestMethod.POST:
                    // Or if(msgId == -1)
                    if (request.Path == "/messages")
                    {
                        string msgContent = _msgColl.GetMsgContentFromJson(request.Body);
                        if (msgContent == string.Empty)
                        {
                            return new ResponseContext("400 Bad Request", "text/plain", "Error, Msg was not in the correct JOSN Format.\n");
                        }
                        else
                        {
                            _msgColl.AddMessage(msgContent);
                            return new ResponseContext("201 Created", "text/plain", "Message successfully created.\n");
                        }
                    }
                    return PageNotFoundResponse();
                //break;
                case RequestMethod.PUT:
                    // Or if(msgId == -1) You can send PUT updates but have to provide correct json body!
                    // Nearly the same code twice maybe TODO refactor this!
                    if (request.Path == "/messages")
                    {
                        Tuple<int, string> msg = _msgColl.GetMsgTupleFromJson(request.Body);
                        if (msg == null)
                        {
                            return new ResponseContext("400 Bad Request", "text/plain", "Error, Msg was not in the correct JOSN Format.\n");
                        }
                        else
                        {
                            if (_msgColl.UpdateMessage(msg.Item1, msg.Item2))
                            {
                                return new ResponseContext("200 OK", "text/plain", "Message successfully edited.\n");
                            }
                            // Maybe 500 is not the correct status code??
                            return new ResponseContext("500 Internal Server Error", "text/plain", $"Error, when editing msg with Id: {msg.Item1}.\n");
                        }
                    }
                    else if (msgId >= 0)
                    {
                        string msgContent = _msgColl.GetMsgContentFromJson(request.Body);
                        if (msgContent == string.Empty)
                        {
                            return new ResponseContext("400 Bad Request", "text/plain", "Error, Msg was not in the correct JOSN Format.\n");
                        }
                        else
                        {
                            if (_msgColl.UpdateMessage(msgId, msgContent))
                            {
                                return new ResponseContext("200 OK", "text/plain", "Message successfully edited.\n");
                            }
                            // Maybe 500 is not the correct status code??
                            return new ResponseContext("500 Internal Server Error", "text/plain", $"Error, when editing msg with Id: {msgId}.\n");

                        }
                    }
                    return PageNotFoundResponse();
                case RequestMethod.DELETE:
                    // Or if(msgId == -1) You can send PUT updates but have to provide correct json body!
                    if (request.Path == "/messages")
                    {
                        int id = _msgColl.GetIdFromJson(request.Body);
                        if (id == -1)
                        {
                            return new ResponseContext("400 Bad Request", "text/plain", "Error, make sure request is JSON and has Id key.\n");
                        }
                        else
                        {
                            if (_msgColl.DeleteMessage(id))
                            {
                                return new ResponseContext("200 OK", "text/plain", "Message successfully deleted.\n");
                            }
                            // Maybe 500 is not the correct status code??
                            return new ResponseContext("500 Internal Server Error", "text/plain", $"Error, when deleting msg with Id: {id}.\n");
                        }
                    }
                    else if (msgId >= 0)
                    {
                        if (_msgColl.DeleteMessage(msgId))
                        {
                            return new ResponseContext("200 OK", "text/plain", "Message successfully deleted.\n");
                        }
                        // Maybe 500 is not the correct status code??
                        return new ResponseContext("500 Internal Server Error", "text/plain", $"Error, when deleting msg with Id: {msgId}.\n");
                    }
                    return PageNotFoundResponse();
                default:
                    return MethodNotAllowedResponse();
            }
        }

        private static ResponseContext NullResponse()
        {
            return new ResponseContext("400 Bad Request", "text/plain", "Error bad request\n");
        }
        private static ResponseContext PageNotFoundResponse()
        {
            return new ResponseContext("404 Bad Request", "text/plain", "Error page not found\n");
        }
        private static ResponseContext MethodNotAllowedResponse()
        {
            return new ResponseContext("405 Bad Request", "text/plain", "Error method not allowed\n");
        }

        // Returns the MessageId from the request path -1 when no msg id is provided and -100 if path is invalid
        // Example: /messages will return -1 and /messages/7 will return 7
        // I think this is not a clean implementation maybe TODO make it clean :)
        private static int GetMsgIdFromPath(string path)
        {
            // Maybe shorter / better with regex?
            string[] splitPath = path.Split('/');
            if (splitPath.Length == 2 && splitPath[1] == "messages")
            {
                return -1;
            }
            if (splitPath.Length == 3 && splitPath[1] == "messages")
            {
                int id;
                if (int.TryParse(splitPath[2], out id))
                {
                    return id;
                }
            }
            return -100;
        }

        public string GetAsString(bool forDebugging = false)
        {
            StringBuilder sb = new StringBuilder();
            if (forDebugging)
            {

                sb.AppendLine("----------------------------------------------------------");
                sb.AppendLine("ResponseContext:");
                sb.AppendLine("\tStatus: " + Status);
                sb.AppendLine("\tHeaders: ");
                foreach (KeyValuePair<string, string> header in this.Headers)
                {
                    sb.AppendLine($"\t\t {header.Key} : {header.Value}");
                }
                sb.AppendLine("\tBody:");
                // remove \r from \r\n
                string tmpBody = Body.Replace("\r", string.Empty);
                string[] lines = tmpBody.Split('\n');
                foreach (string line in lines)
                {
                    sb.AppendLine("\t\t" + line);
                }
            }
            else
            {
                sb.AppendLine(HTTPServer.VERSION + " " + Status);
                foreach (KeyValuePair<string, string> header in this.Headers)
                {
                    sb.AppendLine(header.Key + ":" + header.Value);
                }
                sb.AppendLine();
                sb.AppendLine(Body);
            }
            return sb.ToString();
        }
    }
}
