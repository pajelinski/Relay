using System.Diagnostics;
using System.Linq;
using Relay.Application;

namespace Relay
{
    using System;

    class Program
    {
        static void Main(string[] args)
        {
            var relay = new Application.Relay();

            var subscribers = Enumerable.Range(0, 9).Select(id => new Subscriber(id));

            foreach (var subscriber in subscribers)
            {
                relay.AddSubscriber(subscriber);
            }

            Console.WriteLine("To start broadcast press any key");

            Console.ReadKey();

            Console.WriteLine("Started broadcast...");
            var run = true;

            for (var messageCount = 0; run; messageCount++)
            {
                relay.Broadcast(new Message($"test message {messageCount}"));
                run = WasQPressed();
            }

            Console.WriteLine("Press any key to quit");
            Console.ReadKey();
        }

        private static bool WasQPressed()
        {
            Console.WriteLine();
            Console.WriteLine("To end broadcast press q");
            Console.WriteLine("To continue press any key");
            Console.WriteLine();
                var consoleKeyInfo = Console.ReadKey(false);
                return consoleKeyInfo.Key != ConsoleKey.Q;
        }
    }
}
