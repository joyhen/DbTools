using System;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;

namespace Tools.ajax
{
    using Help;

    public partial class requestaction
    {
        #region 公用方法

        /// <summary>
        /// 获取特殊业务的标记参数对象
        /// </summary>
        private string GetArgParam()
        {
            string tag = ActionParama.Arg != null ? ((dynamic)ActionParama.Arg).tag : "";
            return tag;
        }

        /// <summary>
        /// 获取ajax参数对象
        /// </summary>
        /// <param name="checkParam">是否校验（如果有参数，建议校验）</param>
        private T GetActionParamData<T>(bool checkParam = false) where T : class, IAjaxArg, new()
        {
            if (checkParam)
            {
                var listAjaxParamName = ActionParama.GetAjaxArgName2List<T>();
                if (listAjaxParamName == null) return default(T);   //Outmsg(false, "参数解析错误");

                var listAjaxParamTargetModelFieldName = ReflectionMapr<T>.GetAjaxParamName();
                bool exist = listAjaxParamTargetModelFieldName.All(x => listAjaxParamName.Contains(x));

                if (!exist) return default(T);                      //Outmsg(false, "非正常请求");
            }

            var ajaxPraramData = ActionParama.GetArg<T>();
            return ajaxPraramData;
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
        /// 校验数据集中的表
        /// </summary>
        private bool CheckTB(DataSet ds, int tbindex)
        {
            if (CheckDS(ds))
            {
                return ds.Tables[tbindex].Rows.Count > 0;
            }
            return false;
        }

        /// <summary>
        /// 请求响应后的输出
        /// </summary>
        private void Outmsg(string msg)
        {
            Outmsg(false, msg);
        }
        /// <summary>
        /// 请求响应后的输出
        /// </summary>
        /// <remarks>outmsg : "" or default(String)</remarks>
        private void Outmsg(bool success = false, string outmsg = "", object outdata = null)
        {
            string json = JsonUtils.JsonSerializer(new ResultModel
            {
                success = success,
                msg = outmsg,
                data = outdata
            });
            CurrentContext.Response.Write(json);
        }

        /// <summary>
        /// GET请求参数
        /// </summary>
        private string Q(string key)
        {
            if (string.IsNullOrWhiteSpace(key)) return string.Empty;

            var query = CurrentContext.Request.QueryString[key];
            return query ?? string.Empty;
        }
        /// <summary>
        /// GET请求参数
        /// </summary>
        private int QInt(string key)
        {
            int id = 0;
            int.TryParse(Q(key), out id);
            return id;
        }
        /// <summary>
        /// POST请求参数
        /// </summary>
        protected string F(string key)
        {
            if (string.IsNullOrWhiteSpace(key)) return string.Empty;

            var query = CurrentContext.Request.Form[key];
            return query ?? string.Empty;
        }
        /// <summary>
        /// POST请求参数
        /// </summary>
        protected int FInt(string key)
        {
            int id = 0;
            int.TryParse(Q(key), out id);
            return id;
        }

        /// <summary>
        /// 字符串有效性检查
        /// </summary>
        private bool IsNotEmptyString(string str)
        {
            return (!string.IsNullOrWhiteSpace(str) && str.Trim().Length > 0);
        }
        /// <summary>
        /// 字符串转枚举
        /// </summary>
        private static T GetEnumBuyStr<T>(string str)
        {
            try
            {
                T enumOne = (T)Enum.Parse(typeof(T), str);
                return enumOne;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion
    }
}