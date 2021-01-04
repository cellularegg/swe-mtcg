using System;

namespace swe_mtcg
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.Title = "mtcg-zelenay-david";
            Console.WriteLine("Starting server on port 8080");
            HTTPServer server = new HTTPServer(8080);
            server.Start();
        }
    }
}