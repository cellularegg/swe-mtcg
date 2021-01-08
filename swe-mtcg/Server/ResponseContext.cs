using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using NUnit.Framework.Internal.Execution;

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


                    if (request.Path == "/cards")
                    {
                        string body;
                        if (!ResponseContext.GetTokenFromHeader(request.Headers, out string token))
                        {
                            return UnauthorizedResponse();
                        }


                        if (request.QueryParameters.ContainsKey("format") &&
                            request.QueryParameters["format"] == "plain")
                        {
                            body = serverData.GetStack(token, false);
                        }
                        else
                        {
                            body = serverData.GetStack(token, true);
                        }

                        return new ResponseContext("200 OK", "application/json", body);
                    }


                    if (request.Path == "/deck")
                    {
                        string body;
                        if (!ResponseContext.GetTokenFromHeader(request.Headers, out string token))
                        {
                            return UnauthorizedResponse();
                        }

                        if (request.QueryParameters.ContainsKey("format") &&
                            request.QueryParameters["format"] == "plain")
                        {
                            body = serverData.GetDeck(token, false);
                        }
                        else
                        {
                            body = serverData.GetDeck(token, true);
                        }

                        return new ResponseContext("200 OK", "application/json", body);
                    }


                    // Get User Data
                    if (request.Path.Contains("/users/"))
                    {
                        string username = Regex.Replace(request.Path, "^\\/users\\/", "");
                        if (!ResponseContext.GetTokenFromHeader(request.Headers, out string token))
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
                        if (!ResponseContext.GetTokenFromHeader(request.Headers, out string token))
                        {
                            return UnauthorizedResponse();
                        }

                        body = serverData.GetStats(token);
                        return new ResponseContext("200 OK", "application/json", body);
                    }

                    if (request.Path == "/tradings")
                    {
                        //  Maybe check for empty body??
                        string body = serverData.GetTradingDeals();
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
                        if (!ResponseContext.GetTokenFromHeader(request.Headers, out string token))
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
                        if (!ResponseContext.GetTokenFromHeader(request.Headers, out string token))
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
                        if (!ResponseContext.GetTokenFromHeader(request.Headers, out string token))
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

                    if (request.Path.Contains("/tradings"))
                    {
                        // Create Package
                        if (request.Path == "/tradings")
                        {
                            if (!ResponseContext.GetTokenFromHeader(request.Headers, out string token))
                            {
                                return UnauthorizedResponse();
                            }

                            if (serverData.CreateTradingDeal(token, request.Body))
                            {
                                return new ResponseContext("201 Created", "application/json",
                                    "{ \"msg\": \"Trading deal created successfully.\" }\n");
                            }

                            return new ResponseContext("400 Bad Request", "application/json",
                                "{ \"msg\": \"Error, make sure you own the cards, that you want to register for trading.\"}\n");
                        }
                        else
                        {
                            string tradeId = Regex.Replace(request.Path, "^\\/tradings\\/", "");
                            if (!ResponseContext.GetTokenFromHeader(request.Headers, out string token))
                            {
                                return UnauthorizedResponse();
                            }

                            if (serverData.Trade(tradeId, token, request.Body))
                            {
                                return new ResponseContext("200 OK", "application/json",
                                    "{ \"msg\": \"Trade was successful.\" }\n");
                            }

                            return new ResponseContext("400 Bad Request", "application/json",
                                "{ \"msg\": \"Error, make sure you own the cards, that you want to register for trading.\"}\n");
                        }
                    }


                    return PageNotFoundResponse();
                //break;
                case RequestMethod.PUT:
                    // PUT /deck - configure deck
                    if (request.Path == "/deck")
                    {
                        if (!ResponseContext.GetTokenFromHeader(request.Headers, out string token))
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
                        if (!ResponseContext.GetTokenFromHeader(request.Headers, out string token))
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
                    // Delete Trade
                    if (request.Path.Contains("/tradings"))
                    {
                        // remove "/tradings/" from request path to get TradingId
                        string tradeId = Regex.Replace(request.Path, "^\\/tradings\\/", "");
                        if (!ResponseContext.GetTokenFromHeader(request.Headers, out string token))
                        {
                            return UnauthorizedResponse();
                        }


                        if (serverData.DeleteTradingDeal(token, tradeId))
                        {
                            return new ResponseContext("200 OK", "application/json",
                                "{\"msg\": \"The Trading deal has successfully been deleted.\"}");
                        }

                        return new ResponseContext("400 Bad Request", "application/json",
                            "{ \"msg\": \"Error, make sure you provided the correct trade id and that you are the owner of the trade.\"}\n");
                    }

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

        private static bool GetTokenFromHeader(Dictionary<string, string> headers, out string token)
        {
            // header contains: "Basic username-mtcgToken"
            string headerContent;
            if (headers.ContainsKey("authorization"))
            {
                headerContent = headers["authorization"];
            }
            else
            {
                token = string.Empty;
                return false;
            }

            string[] tmp = headerContent.Split(' ');
            if (tmp.Length == 2)
            {
                token = tmp[1];
                return true;
            }

            token = string.Empty;
            return false;
        }


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