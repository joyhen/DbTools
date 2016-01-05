using System;
using System.Web;
using System.Linq;
using System.Collections.Generic;

namespace Tools.App_Code
{
    public class BufHelp
    {
        /// <summary>
        /// 对象锁
        /// </summary>
        private readonly static Object Locker = new Object();

        /// <summary>
        /// 序列化-表字段业务信息
        /// </summary>
        public static bool ProtoBufSerialize<T>(T model, string filename) where T : class
        {
            try
            {
                string binpath = AppDomain.CurrentDomain.BaseDirectory + @"data\";

                if (!System.IO.Directory.Exists(binpath))
                    System.IO.Directory.CreateDirectory(binpath);

                lock (Locker)
                {
                    using (var file = System.IO.File.Create(binpath + filename))
                    {
                        ProtoBuf.Serializer.Serialize<T>(file, model);
                        return true;
                    }
                }
            }
            catch
            {
                return false;
            }
        }

        public static T ProtoBufDeserialize<T>(string filename) where T : class
        {
            var dbpath = AppDomain.CurrentDomain.BaseDirectory + @"data\" + filename;

            if (System.IO.File.Exists(dbpath))
            {
                lock (Locker)
                {
                    using (var file = System.IO.File.OpenRead(dbpath))
                    {
                        var result = ProtoBuf.Serializer.Deserialize<T>(file);
                        return result;
                    }
                }
            }

            return default(T);
        }
    }
}