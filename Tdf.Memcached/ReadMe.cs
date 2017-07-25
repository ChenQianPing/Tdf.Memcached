/*
 * 
 * memcached.exe -d install
 * memcached.exe -d start
 * 不要忘啦去windows服务中把服务启动了。
 * 
 * 
 * Tips:
 * 1.Memcached中的数据都是存储在memcached内置的内存存储空间中，
 * 由于数据仅存在于内存中，因此当某个服务器停止运行或者出现问题之后，
 * 所有存放在服务器上的键/值对都会丢失。
 * 
 * 2.由于Redis只使用单核，而Memcached可以使用多核.
 * 
 * 
 * MemcachedHost：127.0.0.1:11211
 * 默认端口为：11211
 */ 