using StackExchange.Redis;
using System;
using System.Collections.Generic;

namespace RedisCoreDemo.App
{
    public class RedisConnectionFactory
    {
        private static readonly Lazy<ConnectionMultiplexer> Connection;

        private static readonly string REDIS_CONNECTIONSTRING = "REDIS_CONNECTIONSTRING";

        static RedisConnectionFactory()
        {
            var connectionString = "localhost:6379,password=753951"; //config[REDIS_CONNECTIONSTRING];

            if (connectionString == null)
            {
                throw new KeyNotFoundException($"Environment variable for {REDIS_CONNECTIONSTRING} was not found.");
            }

            var options = ConfigurationOptions.Parse(connectionString);

            Connection = new Lazy<ConnectionMultiplexer>(() => ConnectionMultiplexer.Connect(options));
        }

        public static ConnectionMultiplexer GetConnection() => Connection.Value;
    }
}
