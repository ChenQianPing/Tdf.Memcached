using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tdf.Memcached;

namespace Tests.Memcached
{
    public class TestMemcacheHelper
    {
        public static void TestMethod1()
        {
            // 注册Memcache缓存
            MemcacheHelper.RegisterMemcache();

            // Send
            var msg = new Message
            {
                MessageId = Guid.NewGuid().ToString("N").ToUpper(),
                MessageBody = "60",
                MessageRouter = Guid.NewGuid().ToString("N").ToUpper(),
                MessageTitle = "DetectOmr"
            };

            MemcacheHelper.GetInstance().Add("DetectOmrMsg", msg);

            // Read
            var newMyObj = MemcacheHelper.GetInstance().Get("DetectOmrMsg") as Message;

            if (newMyObj != null)
                Console.WriteLine($@"UserId is {newMyObj.MessageRouter} and Body is {newMyObj.MessageBody}");

            Console.Read();

        }


        public static void TestMethod2()
        {
            // 注册Memcache缓存
            MemcacheHelper.RegisterMemcache();

            // 添加缓存信息(如果存在缓存信息则直接重写设置，否则添加)
            MemcacheHelper.GetInstance().Add("DetectOmr", "50");

            // 缓存是否存在
            var tf = MemcacheHelper.GetInstance().KeyExists("DetectOmr");
            Console.WriteLine(tf);

            if (tf)
            {
                // 获取缓存的值
                var s = MemcacheHelper.GetInstance().Get("DetectOmr");
                Console.WriteLine(s);
            }
            
            Console.Read();
        }

        public static void TestMethod3()
        {
            var values = "0,1,2,3,4,5,6,7,8,9";
            var lstValues = values.Split(Convert.ToChar(",")).ToList();

            var i = 0;
            foreach (var value in lstValues)
            {
                Console.WriteLine($@"i = {i} and value is {value}");
                i++;
            }
            Console.Read();
        }





    }
}
