using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;

namespace swe_mtcg
{
    public class ResponseContext
    {
        public string Body { get; private set; }

        public string Status { get; private set; }

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
            // TODO Update accordingly
            // Endpoints:
            // POST /users - register
            // POST /sessions - "login"
            // POST /packages - create packages
            // POST /transactions/packages - acquire deck
            // GET /cards - show all cards (stack)
            // GET /cards?format=plain - show all cards (stack) in plaintext
            // GET /deck - show deck
            // GET /deck?format=plain - show deck in plaintext
            // PUT /deck - configure deck
            // POST /battles - queue up for battle

            // NOT IMPLEMENTED YET IN BACKEND LOGIC
            // Edit user data
            // GET /users/<username> - show userdata
            // PUT /users/<username> - update userdata (LoginName is not changeable)
            // Stats + scoreboard
            // GET /stats - display stats for a user
            // GET /score - displays scoreboard
            // Trading
            // GET /tradings - displays all trading deals
            // POST /tradings - create a trading deal
            // DELETE /tradings/<trade-id> - delete trading deal
            // POST /tradings/<trade-id> - trade with other user

            ServerData serverData = ServerData.Instance;


            switch (request.Method)
            {
                case RequestMethod.GET:
                    // GET /cards - show all cards (stack)
                    // GET /cards?format=plain - show all cards (stack) in plaintext
                    // GET /deck - show deck
                    // GET /deck?format=plain - show deck in plaintext
                    if (request.Path == "/hello")
                    {
                        string body = "{\"Hello\":\"World\"}";
                        return new ResponseContext("200 OK", "application/json", body);
                    }

                    // improvement possible /cards and /deck nearly the same code

                    if (request.Path.Contains("/cards"))
                    {
                        if (request.Path == "/cards")
                        {
                            string body;
                            // Maybe todo refactor because "token extraction" is repeated multiple times
                            string token = string.Empty;
                            if (request.Headers.ContainsKey("authorization"))
                            {
                                token = ResponseContext.GetTokenFromHeader(request.Headers["authorization"]);
                            }

                            if (token == string.Empty)
                            {
                                return UnauthorizedResponse();
                            }

                            body = serverData.GetStack(token, true);
                            return new ResponseContext("200 OK", "application/json", body);
                        }

                        // TODO parse query parameters generically 
                        if (request.Path == "/cards?format=plain")
                        {
                            string body;
                            // Maybe todo refactor because "token extraction" is repeated multiple times
                            string token = string.Empty;
                            if (request.Headers.ContainsKey("authorization"))
                            {
                                token = ResponseContext.GetTokenFromHeader(request.Headers["authorization"]);
                            }

                            if (token == string.Empty)
                            {
                                return UnauthorizedResponse();
                            }

                            body = serverData.GetStack(token, false);
                            return new ResponseContext("200 OK", "text/plain", body);
                        }
                    }

                    if (request.Path.Contains("/deck"))
                    {
                        if (request.Path == "/deck")
                        {
                            string body;
                            string token = string.Empty;
                            if (request.Headers.ContainsKey("authorization"))
                            {
                                token = ResponseContext.GetTokenFromHeader(request.Headers["authorization"]);
                            }

                            if (token == string.Empty)
                            {
                                return UnauthorizedResponse();
                            }

                            body = serverData.GetDeck(token, true);
                            return new ResponseContext("200 OK", "application/json", body);
                        }

                        // TODO parse query parameters generically 
                        if (request.Path == "/deck?format=plain")
                        {
                            string body;
                            // Maybe todo refactor because "token extraction" is repeated multiple times
                            string token = string.Empty;
                            if (request.Headers.ContainsKey("authorization"))
                            {
                                token = ResponseContext.GetTokenFromHeader(request.Headers["authorization"]);
                            }

                            if (token == string.Empty)
                            {
                                return UnauthorizedResponse();
                            }

                            body = serverData.GetDeck(token, false);
                            return new ResponseContext("200 OK", "text/plain", body);
                        }
                    }

                    // Get User Data
                    if (request.Path.Contains("/users/"))
                    {
                        string username = Regex.Replace(request.Path, "^\\/users\\/", "");
                        string token = string.Empty;
                        if (request.Headers.ContainsKey("authorization"))
                        {
                            token = ResponseContext.GetTokenFromHeader(request.Headers["authorization"]);
                        }

                        if (token == string.Empty)
                        {
                            return UnauthorizedResponse();
                        }

                        string body = serverData.GetUserData(username, token);
                        if (body == String.Empty)
                        {
                            return UnauthorizedResponse();
                        }

                        return new ResponseContext("200 OK", "application/json", body);
                    }
                    if (request.Path == "/stats")
                    {
                        string body;
                        // Maybe todo refactor because "token extraction" is repeated multiple times
                        string token = string.Empty;
                        if (request.Headers.ContainsKey("authorization"))
                        {
                            token = ResponseContext.GetTokenFromHeader(request.Headers["authorization"]);
                        }

                        if (token == string.Empty)
                        {
                            return UnauthorizedResponse();
                        }

                        body = serverData.GetStats(token);
                        return new ResponseContext("200 OK", "application/json", body);
                    }
                    if (request.Path == "/score")
                    {
                        string body;
                        body = serverData.GetScoreboard();
                        return new ResponseContext("200 OK", "application/json", body);
                    }


                    return ResponseContext.PageNotFoundResponse(true);

                case RequestMethod.POST:
                    // POST /users - register
                    // POST /sessions - "login"
                    // POST /packages - create packages
                    // POST /transactions/packages - acquire packages
                    // POST /battles - queue up for battle

                    // User registration
                    if (request.Path == "/users")
                    {
                        bool succeeded = serverData.RegisterUser(request.Body);
                        if (succeeded)
                        {
                            return new ResponseContext("201 Created", "application/json",
                                "{ \"msg\": \"User created successfully.\" }\n");
                        }
                        else
                        {
                            return new ResponseContext("400 Bad Request", "application/json",
                                "{ \"msg\": \"Error, username must not contain special characters and must be unique (See server logs for further details).\"}\n");
                        }
                    }
                    // Login
                    else if (request.Path == "/sessions")
                    {
                        string token = serverData.GetToken(request.Body);
                        if (token != String.Empty)
                        {
                            return new ResponseContext("200 OK", "application/json", token);
                        }

                        return new ResponseContext("400 Bad Request", "application/json",
                            "{ \"msg\": \"Error, username or password wrong.\"}\n");
                    }
                    // Create Package
                    else if (request.Path == "/packages")
                    {
                        string token = string.Empty;
                        if (request.Headers.ContainsKey("authorization"))
                        {
                            token = ResponseContext.GetTokenFromHeader(request.Headers["authorization"]);
                        }

                        if (token == string.Empty)
                        {
                            return UnauthorizedResponse();
                        }

                        // Check if admin??? 
                        if (token == "admin-mtcgToken" && serverData.AddPackage(request.Body))
                        {
                            return new ResponseContext("201 Created", "application/json",
                                "{ \"msg\": \"Package created successfully.\" }\n");
                        }

                        return new ResponseContext("400 Bad Request", "application/json",
                            "{ \"msg\": \"Error, make sure package is a valid Json and you are authorized to create packages.\"}\n");
                    }

                    // Acquire Package
                    if (request.Path == "/transactions/packages")
                    {
                        string token = string.Empty;
                        if (request.Headers.ContainsKey("authorization"))
                        {
                            token = ResponseContext.GetTokenFromHeader(request.Headers["authorization"]);
                        }

                        if (token == string.Empty)
                        {
                            return UnauthorizedResponse();
                        }

                        if (serverData.AcquirePackage(token))
                        {
                            return new ResponseContext("200 OK", "application/json",
                                "{\"msg\": \"Package successfully acquired.\"}");
                        }

                        return new ResponseContext("400 Bad Request", "application/json",
                            "{ \"msg\": \"Error, either no packages are available or you do not have enough coins.\"}\n");
                    }

                    // Battle
                    if (request.Path == "/battles")
                    {
                        string body = "";
                        string token = string.Empty;
                        if (request.Headers.ContainsKey("authorization"))
                        {
                            token = ResponseContext.GetTokenFromHeader(request.Headers["authorization"]);
                        }

                        if (token == string.Empty)
                        {
                            return UnauthorizedResponse();
                        }

                        body = serverData.QueueForBattle(token);
                        if (body == String.Empty)
                        {
                            return new ResponseContext("400 Bad Request", "application/json",
                                "{ \"msg\": \"Error... Timeout, there was no other player in battle queue.\"}\n");
                        }
                        else
                        {
                            return new ResponseContext("200 OK", "application/json", body);
                        }
                    }


                    return PageNotFoundResponse();
                //break;
                case RequestMethod.PUT:
                    // PUT /deck - configure deck
                    if (request.Path == "/deck")
                    {
                        string token = string.Empty;
                        if (request.Headers.ContainsKey("authorization"))
                        {
                            token = ResponseContext.GetTokenFromHeader(request.Headers["authorization"]);
                        }

                        if (token == string.Empty)
                        {
                            return UnauthorizedResponse();
                        }

                        if (serverData.ConfigureDeck(request.Body, token))
                        {
                            return new ResponseContext("200 OK", "application/json",
                                "{\"msg\": \"Deck successfully configured.\"}");
                        }

                        return new ResponseContext("400 Bad Request", "application/json",
                            "{ \"msg\": \"Error, deck must contain 4 cards which you must own.\"}\n");
                    }

                    // Update User

                    // Get User Data
                    if (request.Path.Contains("/users/"))
                    {
                        string username = Regex.Replace(request.Path, "^\\/users\\/", "");
                        string token = string.Empty;
                        if (request.Headers.ContainsKey("authorization"))
                        {
                            token = ResponseContext.GetTokenFromHeader(request.Headers["authorization"]);
                        }

                        if (token == string.Empty || (username + "-mtcgToken" != token))
                        {
                            return UnauthorizedResponse();
                        }

                        bool updateSucceeed = serverData.UpdateUserData(token, request.Body);
                        if (!updateSucceeed)
                        {
                            return new ResponseContext("400 Bad Request", "application/json",
                                "{ \"msg\": \"Error, make sure request body is a valid json and you are authorized.\"}\n");
                        }

                        return new ResponseContext("200 OK", "application/json",
                            "{\"msg\": \"User updated successfully.\"}");
                    }

                    return PageNotFoundResponse();
                case RequestMethod.DELETE:
                    // Or if(msgId == -1) You can send PUT updates but have to provide correct json body!
                    // if (request.Path == "/messages")
                    // {
                    //     int id = _msgColl.GetIdFromJson(request.Body);
                    //     if (id == -1)
                    //     {
                    //         return new ResponseContext("400 Bad Request", "text/plain",
                    //             "Error, make sure request is JSON and has Id key.\n");
                    //     }
                    //     else
                    //     {
                    //         if (_msgColl.DeleteMessage(id))
                    //         {
                    //             return new ResponseContext("200 OK", "text/plain", "Message successfully deleted.\n");
                    //         }
                    //
                    //         // Maybe 500 is not the correct status code??
                    //         return new ResponseContext("500 Internal Server Error", "text/plain",
                    //             $"Error, when deleting msg with Id: {id}.\n");
                    //     }
                    // }
                    // else if (msgId >= 0)
                    // {
                    //     if (_msgColl.DeleteMessage(msgId))
                    //     {
                    //         return new ResponseContext("200 OK", "text/plain", "Message successfully deleted.\n");
                    //     }
                    //
                    //     // Maybe 500 is not the correct status code??
                    //     return new ResponseContext("500 Internal Server Error", "text/plain",
                    //         $"Error, when deleting msg with Id: {msgId}.\n");
                    // }

                    return PageNotFoundResponse();
                default:
                    return MethodNotAllowedResponse();
            }
        }

        private static ResponseContext NullResponse(bool asJson = false)
        {
            if (asJson)
            {
                return new ResponseContext("400 Bad Request", "application/json", "{\"msg\": \"Error bad request\"}\n");
            }

            return new ResponseContext("400 Bad Request", "text/plain", "Error bad request\n");
        }

        private static ResponseContext UnauthorizedResponse(bool asJson = false)
        {
            if (asJson)
            {
                return new ResponseContext("401 Unauthorized", "application/json",
                    "{\"msg\": \"Error Unauthorized\"}\n");
            }

            return new ResponseContext("401 Unauthorized", "text/plain", "Error Unauthorized\n");
        }

        private static ResponseContext PageNotFoundResponse(bool asJson = false)
        {
            if (asJson)
            {
                return new ResponseContext("404 Not Found", "application/json",
                    "{\"msg\": \"Error page not found\"}\n");
            }

            return new ResponseContext("404 Not Found", "text/plain", "Error page not found\n");
        }

        private static ResponseContext MethodNotAllowedResponse(bool asJson = false)
        {
            if (asJson)
            {
                return new ResponseContext("405 Method Not Allowed", "application/json",
                    "{\"msg\": \"Error method not allowed\"}\n");
            }

            return new ResponseContext("405 Method Not Allowed", "text/plain", "Error method not allowed\n");
        }

        private static string GetTokenFromHeader(string header)
        {
            // header contains: "Basic username-mtcgToken"
            string[] tmp = header.Split(' ');
            if (tmp.Length == 2)
            {
                return tmp[1];
            }

            return string.Empty;
        }

        // Returns the MessageId from the request path -1 when no msg id is provided and -100 if path is invalid
        // Example: /messages will return -1 and /messages/7 will return 7
        // I think this is not a clean implementation maybe TODO make it clean :)
        // private static int GetMsgIdFromPath(string path)
        // {
        //     // Maybe shorter / better with regex?
        //     string[] splitPath = path.Split('/');
        //     if (splitPath.Length == 2 && splitPath[1] == "messages")
        //     {
        //         return -1;
        //     }
        //
        //     if (splitPath.Length == 3 && splitPath[1] == "messages")
        //     {
        //         int id;
        //         if (int.TryParse(splitPath[2], out id))
        //         {
        //             return id;
        //         }
        //     }
        //
        //     return -100;
        // }

        private static string GetUsrFromPath(string path)
        {
            return Regex.Replace(path, "^\\/users\\/", "");
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