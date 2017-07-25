using Memcached.ClientLibrary;
using System;
using System.Configuration;

namespace Tdf.Memcached
{
    /// <summary>
    /// 基于Memcached.ClientLibrary封装使用Memchached信息
    /// 读取缓存存放在服务器
    /// </summary>
    public class MemcacheHelper : IMemcached
    {
        /// <summary>
        /// 字段_instance,存放注册的缓存信息
        /// </summary>
        private static MemcacheHelper _instance;

        /// <summary>
        /// 缓存客户端
        /// </summary>
        private readonly MemcachedClient _client;

        /// <summary>
        /// 受保护类型的缓存对象，初始化一个新的缓存对象
        /// </summary>
        public MemcacheHelper()
        {
            // 读取app.Config中需要缓存的服务器地址信息，可以传递多个地址，使用";"分隔
            string[] serverList = ConfigurationManager.AppSettings["MemcachedHost"].Split(new char[] { ';' });
            
            try
            {
                // 初始化池
                var sockIoPool = SockIOPool.GetInstance();
                // 设置服务器列表
                sockIoPool.SetServers(serverList);
                // 各服务器之间负载均衡的设置比例
                sockIoPool.SetWeights(new int[] { 1 });
                // 初始化时创建连接数
                sockIoPool.InitConnections = 3;
                // 最小连接数
                sockIoPool.MinConnections = 3;
                // 最大连接数
                sockIoPool.MaxConnections = 5;
                // 连接的最大空闲时间，下面设置为6个小时（单位ms），超过这个设置时间，连接会被释放掉
                sockIoPool.MaxIdle = 1000 * 60 * 60 * 6;
                // socket连接的超时时间，下面设置表示不超时（单位ms），即一直保持链接状态
                sockIoPool.SocketConnectTimeout = 0;
                // 通讯的超时时间，下面设置为3秒（单位ms）,.Net版本没有实现
                sockIoPool.SocketTimeout = 1000 * 3;
                // 维护线程的间隔激活时间，下面设置为30秒（单位s），设置为0时表示不启用维护线程
                sockIoPool.MaintenanceSleep = 30;
                // 设置SocktIO池的故障标志
                sockIoPool.Failover = true;
                // 是否对TCP/IP通讯使用nalgle算法，.net版本没有实现
                sockIoPool.Nagle = false;
                // socket单次任务的最大时间（单位ms），超过这个时间socket会被强行中端掉，当前任务失败。
                sockIoPool.MaxBusy = 1000 * 10;
                sockIoPool.Initialize();

                // 实例化缓存对象
                _client = new MemcachedClient
                {
                    // 是否启用压缩数据：如果启用了压缩，数据压缩长于门槛的数据将被储存在压缩的形式
                    EnableCompression = false,
                    // 压缩设置，超过指定大小的都压缩 
                    CompressionThreshold = 1024*1024
                };
                
            }
            catch (Exception ex)
            {
                // 错误信息写入事务日志
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// 获取缓存的实例对象，方法调用的时候使用
        /// </summary>
        /// <returns></returns>
        public static MemcacheHelper GetInstance()
        {
            return _instance;
        }

        /// <summary>
        /// 添加缓存信息(如果存在缓存信息则直接重写设置，否则添加)
        /// 使用：MemcacheHelper.GetInstance().Add(key,value)
        /// </summary>
        /// <param name="key">需要缓存的键</param>
        /// <param name="value">需要缓存的值</param>
        public void Add(string key, object value)
        {
            if (_client.KeyExists(key))
            {
                _client.Set(key, value);
            }
            _client.Add(key, value);
        }

        /// <summary>
        /// 添加缓存信息
        /// 使用：MemcacheHelper.GetInstance().Add(key,value,Datetime.Now())
        /// </summary>
        /// <param name="key">需要缓存的键</param>
        /// <param name="value">需要缓存的值</param>
        /// <param name="expiredDateTime">设置的缓存的过时时间</param>
        public void Add(string key, object value, DateTime expiredDateTime)
        {
            _client.Add(key, value, expiredDateTime);
        }

        /// <summary>
        /// 修改缓存的值
        /// 使用：MemcacheHelper.GetInstance().Update(key,value)
        /// </summary>
        /// <param name="key">需要修改的键</param>
        /// <param name="value">需要修改的值</param>
        public void Update(string key, object value)
        {
            _client.Replace(key, value);
        }

        /// <summary>
        /// 修改缓存的值
        /// 使用：MemcacheHelper.GetInstance().Update(key,value,Datetime.Now())
        /// </summary>
        /// <param name="key">需要修改的键</param>
        /// <param name="value">需要修改的值</param>
        /// <param name="expiredDateTime">设置的缓存的过时时间</param>
        public void Update(string key, object value, DateTime expiredDateTime)
        {
            _client.Replace(key, value, expiredDateTime);
        }

        /// <summary>
        /// 设置缓存
        /// 使用：MemcacheHelper.GetInstance().Set(key,value)
        /// </summary>
        /// <param name="key">设置缓存的键</param>
        /// <param name="value">设置缓存的值</param>
        public void Set(string key, object value)
        {
            _client.Set(key, value);
        }

        /// <summary>
        /// 设置缓存，并修改过期时间
        /// 使用：MemcacheHelper.GetInstance().Set(key,value,Datetime.Now())
        /// </summary>
        /// <param name="key">设置缓存的键</param>
        /// <param name="value">设置缓存的值</param>
        /// <param name="expiredTime">设置缓存过期的时间</param>
        public void Set(string key, object value, DateTime expiredTime)
        {
            _client.Set(key, value, expiredTime);
        }

        /// <summary>
        /// 删除缓存
        /// 使用：MemcacheHelper.GetInstance().Delete(key)
        /// </summary>
        /// <param name="key">需要删除的缓存的键</param>
        public void Delete(string key)
        {
            _client.Delete(key);
        }

        /// <summary>
        /// 获取缓存的值
        /// 使用：MemcacheHelper.GetInstance().Get(key)
        /// </summary>
        /// <param name="key">传递缓存中的键</param>
        /// <returns>返回缓存在缓存中的信息</returns>
        public object Get(string key)
        {
            return _client.Get(key);
        }

        /// <summary>
        /// 缓存是否存在
        /// 使用：MemcacheHelper.GetInstance().KeyExists(key)
        /// </summary>
        /// <param name="key">传递缓存中的键</param>
        /// <returns>如果为true，则表示存在此缓存，否则比表示不存在</returns>
        public bool KeyExists(string key)
        {
            return _client.KeyExists(key);
        }

        /// <summary>
        /// 注册Memcache缓存(在Global.asax的Application_Start方法中注册)
        /// 使用：MemcacheHelper.RegisterMemcache();
        /// </summary>
        public static void RegisterMemcache()
        {
            if (_instance == null)
            {
                _instance = new MemcacheHelper();
            }
        }
    }
}
