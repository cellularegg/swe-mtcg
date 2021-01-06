using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace swe_mtcg
{
    public class HTTPServer
    {
        public const string VERSION = "HTTP/1.1";
        public int port { get; }
        public bool isRunning { get; private set; }
        private TcpListener _tcpListener;

        public HTTPServer(int pPort)
        {
            // TODO Check for valid Port
            port = pPort;
            _tcpListener = new TcpListener(IPAddress.Any, port);
        }


        public void Start()
        {
            isRunning = true;
            _tcpListener.Start();
            try
            {
                while (isRunning)
                {
                    Console.WriteLine("Waiting for connection.");
                    TcpClient client = _tcpListener.AcceptTcpClient();
                    Console.WriteLine("Client connected.");
                    // Thread t = new Thread(new ParameterizedThreadStart(HandleClient));
                    // t.Start(client);
                    ThreadPool.QueueUserWorkItem(HandleClient, client);
                }
            }
            catch (SocketException e)
            {
                Console.WriteLine(e);
                isRunning = false;
                _tcpListener.Stop();
            }


            isRunning = false;
            _tcpListener.Stop();
        }

        private void HandleClient(Object obj)
        {
            TcpClient client = (TcpClient) obj;
            StreamReader sr = new StreamReader(client.GetStream());
            string msg = "";
            try
            {
                while (sr.Peek() != -1)
                {
                    // sr.ReadLine() it gets stuck when receiving POST requests
                    msg += (char) sr.Read();
                }

                RequestContext request = RequestContext.GetRequestContext(msg);
                Console.WriteLine(request);
                ResponseContext response = ResponseContext.GetResponseContext(request);
                StreamWriter writer = new StreamWriter(client.GetStream()) {AutoFlush = true};
                Console.WriteLine(response.GetAsString(true));
                writer.Write(response.GetAsString(false));
                client.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                client.Close();
            }
        }
    }
}