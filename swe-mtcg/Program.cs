using System;

namespace swe_mtcg
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.Title = "mtcg-zelenay-david";
            Console.WriteLine("Starting server on port 10001");
            HTTPServer server = new HTTPServer(10001);
            server.Start();
        }
    }
}