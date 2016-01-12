using System;
using System.Data;
using System.Linq;
using System.Collections.Generic;

namespace Tools
{
    using Help;

    public class BasePage : System.Web.UI.Page
    {
        /// <summary>
        /// 获取表信息
        /// </summary>
        protected List<TableInfo> GetTableDetail(string tbname)
        {
            var query = GetDataWithCache<List<TableInfo>>(new Func<List<TableInfo>>(() =>
            {
                var ds = DBHelp.ExecuteDataSet(string.Format(sql.GetTableInfo, tbname));
                if (CheckDS(ds))
                {
                    var result = ds.Tables[0].MapTo<TableInfo>();
                    return result.ToList();
                }
                return null;
            }), key: tbname);

            return query ?? new List<TableInfo>();
        }
        /// <summary>
        /// 获取数据，包含缓存（缓存有就取，没有就执行方法获取）
        /// </summary>
        /// <typeparam name="T">要获取的对象</typeparam>
        /// <param name="fn">缓存没有执行的方法</param>
        private T GetDataWithCache<T>(Func<T> fn, string key) where T : class
        {
            var cachekey = KeyCenter.KeyStrPrefix + key;
            var cachedata = CacheUtil.GetCache(cachekey) as T;

            if (cachedata == null)
            {
                if (fn == null) return null;

                var fr = fn.Invoke().AsyncInsertCache(cachekey);
                return fr;
            }

            return cachedata;
        }
        /// <summary>
        /// 校验数据集
        /// </summary>
        private bool CheckDS(DataSet ds)
        {
            return ds != null && ds.Tables.Count > 0;
        }
        /// <summary>
        /// GET请求参数
        /// </summary>
        protected string Q(string key)
        {
            if (string.IsNullOrWhiteSpace(key)) return string.Empty;

            var query = Request.QueryString[key];
            return query ?? string.Empty;
        }
        /// <summary>
        /// 字符串有效性检查
        /// </summary>
        protected bool IsNotEmptyString(string str)
        {
            return (!string.IsNullOrWhiteSpace(str) && str.Trim().Length > 0);
        }
    }
}