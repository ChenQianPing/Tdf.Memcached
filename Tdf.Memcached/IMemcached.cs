using System;

namespace Tdf.Memcached
{
    public interface IMemcached
    {
        void Add(string key, object value);
        void Add(string key, object value, DateTime expiredDateTime);
        void Update(string key, object value);
        void Update(string key, object value, DateTime expiredDateTime);
        void Set(string key, object value);
        void Set(string key, object value, DateTime expiredTime);
        void Delete(string key);
        object Get(string key);
        bool KeyExists(string key);
    }
}
