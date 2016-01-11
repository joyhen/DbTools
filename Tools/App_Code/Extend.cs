using System.Data;
using System.Dynamic;
using System.Threading;
using System.Collections.Generic;
using System;

namespace Tools.App_Code
{
    /// <summary>
    /// 扩展方法
    /// </summary>
    public static class Extend
    {
        public static Predicate<T> ToPredicate<T>(this Func<T, bool> source)
        {
            Predicate<T> result = new Predicate<T>(source);
            return result;
        }

        public static string GenCodeFormat(this string codestr)
        {
            if (string.IsNullOrWhiteSpace(codestr) || codestr.Trim().Length == 0)
                return "";

            return codestr.Replace("<", "&lt;")
                          .Replace(">", "&gt;")
                          .Replace("\"", "&quot;");
        }

        public static List<T> MapTo<T>(this DataTable dt) where T : class, new()
        {
            var objects = new List<dynamic>();

            foreach (DataRow row in dt.Rows)
            {
                dynamic obj = new ExpandoObject();

                foreach (DataColumn column in dt.Columns)
                {
                    var x = (IDictionary<string, object>)obj;
                    x.Add(column.ColumnName, row[column.ColumnName]);
                }
                objects.Add(obj);
            }

            var retval = new List<T>();
            foreach (dynamic item in objects)
            {
                var o = new T();
                Mapper<T>.Map(item, o);
                retval.Add(o);
            }

            return retval;
        }

        //[System.Obsolete("无效", true)]
        //public static T Map<T>(this DataRow row) where T : class, new()
        //{
        //    dynamic obj = new ExpandoObject();

        //    foreach (DataColumn column in row.Table.Columns)
        //    {
        //        var x = (IDictionary<string, object>)obj;
        //        x.Add(column.ColumnName, row[column.ColumnName]);
        //    }

        //    var retval = new List<T>();

        //    var o = new T();
        //    Mapper<T>.Map(obj, o);
        //    retval.Add(o);

        //    return o;
        //}

        /// <summary>
        /// 对象数据异步写入缓存
        /// </summary>
        public static T AsyncInsertCache<T>(
            this T listdata, string cacheKey, int cacheMinutes = 30) where T : class
        {
            if (listdata == null) return null;

            var context = System.Web.HttpContext.Current;

            ThreadPool.QueueUserWorkItem((state) =>
            {
                CacheUtil.InsertCachAsync(context, cacheKey, listdata, cacheMinutes);
            });

            return listdata;
        }

        //...

    }
}