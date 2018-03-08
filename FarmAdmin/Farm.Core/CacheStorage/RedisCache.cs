using Farm.Infrastructure.Helpers;
using ServiceStack.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Farm.Infrastructure.CacheStorage
{
    public class RedisCache 
    {
        ServiceStackRedis service = null;
        public RedisCache(string host, int port, string password, int expirySeconds, long db)
        {
            service = new ServiceStackRedis(host, port, password, expirySeconds, db);
        }

        public RedisCache(string host)
        {
            service = new ServiceStackRedis(host);
        }

        public RedisCache()
        {
            service = new ServiceStackRedis();
        }

        public void Add<V>(string key, V value)
        {
            service.Set(key, value);
        }

        public void Add<V>(string key, V value, int cacheDurationInSeconds)
        {
            service.Set(key, value, cacheDurationInSeconds);
        }

        public bool ContainsKey<V>(string key)
        {
            return service.ContainsKey(key);
        }

        public V Get<V>(string key)
        {
            return service.Get<V>(key);
        }

        public IEnumerable<string> GetAllKey<V>()
        {
            return service.GetAllKeys();
        }

        public V GetOrCreate<V>(string cacheKey, Func<V> create, int cacheDurationInSeconds = int.MaxValue)
        {
            if (this.ContainsKey<V>(cacheKey))
            {
                return this.Get<V>(cacheKey);
            }
            else
            {
                var result = create();
                this.Add(cacheKey, result, cacheDurationInSeconds);
                return result;
            }
        }

        public void Remove<V>(string key)
        {
            service.Remove(key);
        }
    }

    public class ServiceStackRedis
    {
        private readonly int _expirySeconds = -1;
        private readonly PooledRedisClientManager _redisClientManager;
        public ServiceStackRedis(string host, int port, string password, int expirySeconds, long db)
        {
            _expirySeconds = expirySeconds;
            var hosts = new[] { string.Format("{0}@{1}:{2}", password, host, port) };
            _redisClientManager = new PooledRedisClientManager(hosts, hosts, null, db, 500, _expirySeconds);
        }

        public ServiceStackRedis(string host)
            : this(host, 6379, null, -1, 0)
        {
        }

        public ServiceStackRedis()
            : this("localhost", 6379, null, -1, 0)
        {
        }

        public bool Set<T>(string key, T value)
        {
            if (key == null) throw new ArgumentNullException("key");
            if (_expirySeconds != -1) return Set(key, value, _expirySeconds);
            bool baseType = IsBulitinType(typeof(T));
            if (baseType)
            {
                using (var client = _redisClientManager.GetClient())
                {
                    return client.Set(key, value);
                }
            }
            else
            {
                var json = JsonHelper.ToJson(value);
                using (var client = _redisClientManager.GetClient())
                {
                    return client.Set(key, json);
                }
            }
        }

        public bool Set<T>(string key, T value, int duration)
        {
            if (key == null) throw new ArgumentNullException("key");
            bool baseType = IsBulitinType(typeof(T));
            if (baseType)
            {
                using (var client = _redisClientManager.GetClient())
                {
                    return client.Set(key, value, DateTime.Now.AddSeconds(duration));
                }
            }
            else
            {
                var json = JsonHelper.ToJson(value);
                using (var client = _redisClientManager.GetClient())
                {
                    return client.Set(key, json, DateTime.Now.AddSeconds(duration));
                }
            }
        }

        public T Get<T>(string key)
        {
            if (key == null) throw new ArgumentNullException("key");
            bool baseType = IsBulitinType(typeof(T));
            if (baseType)
            {
                T data = default(T);
                using (var client = _redisClientManager.GetClient())
                {
                    data = client.Get<T>(key);
                }
                return data;
            }
            else
            {
                string data;
                using (var client = _redisClientManager.GetClient())
                {
                    data = client.Get<string>(key);
                }
                return data == null ? default(T) : JsonHelper.FromJson<T>(data);
            }
        }
        public bool Remove(string key)
        {
            using (var client = _redisClientManager.GetClient())
            {
                return client.Remove(key);
            }
        }

        public bool RemoveAll()
        {
            using (var client = _redisClientManager.GetClient())
            {
                try
                {
                    client.FlushDb();
                    return true;
                }
                catch (Exception)
                {
                    return false;
                }
            }
        }

        public bool ContainsKey(string key)
        {
            using (var client = _redisClientManager.GetClient())
            {
                return client.ContainsKey(key);
            }
        }

        public List<string> GetAllKeys()
        {
            using (var client = _redisClientManager.GetClient())
            {
                return client.SearchKeys("SqlSugarDataCache.*");
            }
        }
        private bool IsBulitinType(Type type)
        {
            return (type == typeof(object) || Type.GetTypeCode(type) != TypeCode.Object);
        }
    }
}
