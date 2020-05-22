using StackExchange.Redis;
using System;

namespace RedisCoreDemo.App
{
    public static class RedisKeyspaceNotifications
    {
        /// <summary>
        /// NOTE: For this sample to work, you need to go to the Azure Portal and configure keyspace notifications with "Kxge$" to
        ///       1) turn on expiration notifications (x), 
        ///       2) general command notices (g) and 
        ///       3) Evicted events (e).
        ///       4) STRING operations ($).
        /// IMPORTANT
        ///       1) MAKE SURE YOU UNDERSTAND THE PERFORMANCE IMPACT OF TURNING ON KEYSPACE NOTIFICATIONS BEFORE PROCEEDING
        ///          See http://redis.io/topics/notifications for more details
        ///       2) THIS DOES NOT WORK CORRECTLY ON CLUSTERED REDIS INSTANCES
        ///          See https://github.com/StackExchange/StackExchange.Redis/issues/789 for details
        public static void NotificationsExample(ConnectionMultiplexer connection)
        {
            var subscriber = connection.GetSubscriber();
            int db = 0; //what Redis DB do you want notifications on?
            string notificationChannel = "__keyspace@" + db + "__:*";

            //you only have to do this once, then your callback will be invoked.
            subscriber.Subscribe(notificationChannel, (channel, notificationType) =>
            {
                // IS YOUR CALLBACK NOT GETTING CALLED???? 
                // -> See comments above about enabling keyspace notifications on your redis instance
                var key = GetKey(channel);
                switch (notificationType) // use "Kxge" keyspace notification options to enable all of the below...
                {
                    case "expire": // requires the "Kg" keyspace notification options to be enabled
                        Console.WriteLine("Expiration Set for Key: " + key);
                        break;
                    case "expired": // requires the "Kx" keyspace notification options to be enabled
                        Console.WriteLine("Key EXPIRED: " + key);
                        break;
                    case "rename_from": // requires the "Kg" keyspace notification option to be enabled
                        Console.WriteLine("Key RENAME(From): " + key);
                        break;
                    case "rename_to": // requires the "Kg" keyspace notification option to be enabled
                        Console.WriteLine("Key RENAME(To): " + key);
                        break;
                    case "del": // requires the "Kg" keyspace notification option to be enabled
                        Console.WriteLine("KEY DELETED: " + key);
                        break;
                    case "evicted": // requires the "Ke" keyspace notification option to be enabled
                        Console.WriteLine("KEY EVICTED: " + key);
                        break;
                    case "set": // requires the "K$" keyspace notification option to be enabled for STRING operations
                        Console.WriteLine("KEY SET: " + key);
                        break;
                    default:
                        Console.WriteLine("Unhandled notificationType: " + notificationType);
                        break;
                }
            });

            Console.WriteLine("Subscribed to notifications...");

            // setup for delete notification example
            connection.GetDatabase(db).StringSet("DeleteExample", "Anything");

            // key rename callbacks example
            connection.GetDatabase(db).StringSet("{RenameExample}From", "Anything");
            connection.GetDatabase(db).KeyRename("{RenameExample}From", "{RenameExample}To");

            var random = new Random();
            //add some keys that will expire to test the above callback configured above.
            for (int i = 0; i < 10; i++)
            {
                var expiry = TimeSpan.FromSeconds(random.Next(2, 10));
                connection.GetDatabase(db).StringSet("foo" + i, "bar", expiry);
            }

            // should result in a delete notification callback.
            connection.GetDatabase(db).KeyDelete("DeleteExample");
        }

        private static string GetKey(string channel)
        {
            var index = channel.IndexOf(':');
            if (index >= 0 && index < channel.Length - 1)
                return channel.Substring(index + 1);

            //we didn't find the delimeter, so just return the whole thing
            return channel;
        }
    }
}
