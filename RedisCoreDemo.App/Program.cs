using StackExchange.Redis;
using System;

namespace RedisCoreDemo.App
{
    class Program
    {
        public static void Main(string[] args)
        {
            var EXPIRED_KEYS_CHANNEL = "__keyevent@0__:expired";
            var host = "23.83.252.113:6379,password=753951";

            var connection = ConnectionMultiplexer.Connect(host);
            ISubscriber subscriber = connection.GetSubscriber();
            subscriber.Subscribe(EXPIRED_KEYS_CHANNEL, (channel, key) =>
            {
                Console.WriteLine($"EXPIRED: {key}");
            }
            );
            Console.WriteLine("Listening for events...");
            Console.ReadKey();
        }
    }
}
