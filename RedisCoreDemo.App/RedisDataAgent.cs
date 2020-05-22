using StackExchange.Redis;
using System;
using System.Collections.Generic;

namespace RedisCoreDemo.App
{
    public class RedisDataAgent
    {
        private static IDatabase _database;
        public RedisDataAgent()
        {
            var connection = RedisConnectionFactory.GetConnection();

            _database = connection.GetDatabase();
        }

        public string GetStringValue(string key)
        {
            return _database.StringGet(key);
        }

        public void SetStringValue(string key, string value)
        {
            _database.StringSet(key, value);
        }

        public void StringSet(string key, string value, TimeSpan? expiry, When when = When.Always, CommandFlags flags = CommandFlags.None)
        {
            _database.StringSet(key, value, expiry, when, flags);
        }

        public void StringSet(KeyValuePair<RedisKey, RedisValue>[] values, When when = When.Always, CommandFlags flags = CommandFlags.None)
        {
            _database.StringSet(values, when, flags);
        }

        public void DeleteStringValue(string key)
        {
            _database.KeyDelete(key);
        }
    }
}
