using System;
using System.Linq;
using System.Collections.Generic;

namespace Tools.App_Code
{
    using ProtoBuf;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;
    using Newtonsoft.Json.Converters;

    /// <summary>
    /// 修改字段的说明和业务场景描述 客户端参数对象
    /// </summary>
    public class MofifyArg : IAjaxArg
    {
        public string table { get; set; }
        public string name { get; set; }
        public string desc { get; set; }
        public string biz { get; set; }
    }
    /// <summary>
    /// 添加字段
    /// </summary>
    public class FiedHandle : MofifyArg
    {
        public string type { get; set; }
        public string len { get; set; }
    }

    /// <summary>
    /// 动态输出时忽略此标记的属性
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class IngoreProperty : System.Attribute
    {
    }
    public class MapCondition
    {
        public MapCondition(string param)
        {
            if (string.IsNullOrWhiteSpace(param))
                throw new Exception("对象无效");

            var arr = param.Split('|');
            if (arr == null || arr.Length == 0)
                throw new Exception("对象无效");

            this._orginal = arr[0];
            this._newname = arr.Length > 1 ? arr[1] : null;
        }

        private string _orginal = null;
        private string _newname = null;

        public string Orginal
        {
            get { return _orginal; }
            private set { _orginal = value; }
        }

        public string NewName
        {
            get { return _newname; }
            private set { _newname = value; }
        }

        //public Tuple<string, string> mapProperty { get; set; }
        public Func<object, object> fn { get; set; }
    }

    /// <summary>
    /// 键值对象
    /// </summary>
    [ProtoContract]
    public class KeyValue
    {
        /// <summary>
        /// 键
        /// </summary>
        [ProtoMember(1)]
        public string key { get; set; }
        /// <summary>
        /// 值
        /// </summary>
        [ProtoMember(2)]
        public string value { get; set; }
    }

    /// <summary>
    /// ajax post 参数对象
    /// </summary>
    public class AjaxParama : IAjaxParama
    {
        public virtual string Action { get; set; }

        public object Arg { get; set; }

        /// <summary>
        /// js对象 转 c#对象
        /// </summary>
        /// <typeparam name="T">目标c#对象类型</typeparam>
        public T GetArg<T>() where T : IAjaxArg, new()
        {
            if (Arg == null) return default(T);

            try
            {
                var _type = Arg.GetType();

                if (_type == typeof(JObject))
                {
                    var jobjectAjaxArg = (JObject)Arg;
                    return jobjectAjaxArg.ToObject<T>(new JsonSerializer
                    {
                        Converters = { new AjaxArgConverter<T>() },
                        NullValueHandling = NullValueHandling.Ignore //add
                    });

                    #region //此方法不健壮（JToken）
                    //string desJson = JsonConvert.SerializeObject(Arg);
                    //return JsonConvert.DeserializeObject<T>(desJson, new JsonSerializerSettings() { });
                    ////return JsonConvert.DeserializeObject<T>(desJson, new AjaxArgConverter());
                    #endregion
                }
                else if (_type == typeof(String))
                {
                    return JsonConvert.DeserializeObject<T>(Arg.ToString());
                }

                return (T)Arg;
            }
            catch
            {
                return default(T);
            }
        }

        /// <summary>
        /// 获取ajax传递过来的对象信息
        /// </summary>
        /// <returns>注意，不可缓存，实时的</returns>
        public List<string> GetAjaxArgName2List<T>() where T : IAjaxArg, new()
        {
            var jobjectAjaxArg = (Newtonsoft.Json.Linq.JObject)Arg;
            return jobjectAjaxArg.Children().Select(x => x.Path).ToList();
        }

        public class AjaxArgConverter<T> : CustomCreationConverter<T> where T : IAjaxArg, new()
        {
            public override T Create(System.Type objectType)
            {
                return new T();
            }
        }
    }

    /// <summary>
    /// 获取数据库表、视图、存储过程名
    /// </summary>    
    public class DataBaseTB
    {
        /// <summary>
        /// 表、视图、存储过程名
        /// </summary>
        public string name { get; set; }
        /// <summary>
        /// 类别
        /// </summary>
        public string type { get; set; }
    }

    /// <summary>
    /// 表信息精简版
    /// </summary>
    [ProtoContract]
    public class TableInfoTiny
    {
        [ProtoMember(1)]
        public string tablename { get; set; }
        [ProtoMember(2)]
        public List<FieldInfo> fieldinfo { get; set; }
    }
    /// <summary>
    /// 字段信息
    /// </summary>
    [ProtoContract]
    public class FieldInfo
    {
        /// <summary>
        /// 字段名
        /// </summary>
        [ProtoMember(1)]
        public string name { get; set; }
        /// <summary>
        /// 字段类型
        /// </summary>
        [ProtoMember(2)]
        public string type { get; set; }
        /// <summary>
        /// 字段说明
        /// </summary>
        [ProtoMember(3)]
        public string des { get; set; }
        /// <summary>
        /// 出现的业务场景
        /// </summary>
        [ProtoMember(4)]
        public string biz { get; set; }
    }

    /// <summary>
    /// 表信息
    /// </summary>
    public class TableInfo
    {
        /// <summary>
        /// 字段名
        /// </summary>
        public string name { get; set; }
        /// <summary>
        /// 是否是索引
        /// </summary>
        public int isindex { get; set; }
        public bool index { get { return isindex == 1; } }
        /// <summary>
        /// 是否是主键
        /// </summary>
        public int iskey { get; set; }
        public bool key { get { return iskey == 1; } }
        /// <summary>
        /// 类型
        /// </summary>
        public string type { get; set; }
        /// <summary>
        /// 长度
        /// </summary>
        public int length { get; set; }
        /// <summary>
        /// 是否可空
        /// </summary>
        public int isnull { get; set; }
        public bool @null { get { return isnull == 1; } }
        /// <summary>
        /// 默认值
        /// </summary>
        public string defaultvalue { get; set; }
        public string defval
        {
            get
            {
                if (string.IsNullOrWhiteSpace(defaultvalue)) return string.Empty;
                var result = defaultvalue.Replace("(", "").Replace(")", "")
                                         .Replace("（", "").Replace("）", "");
                return result;
            }
        }
        /// <summary>
        /// 描述说明
        /// </summary>
        public string description { get; set; }
        public string des { get { return description; } }
        public string biz { get; set; }
    }

    /// <summary>
    /// 统一接口输出实体
    /// </summary>
    public class ResultModel
    {
        /// <summary>
        /// 错误码
        /// </summary>
        [Obsolete("后期使用", true), JsonIgnore]
        public int code { get; set; }
        /// <summary>
        /// 执行成功与否
        /// </summary>
        /// <remarks>ture:解析data，false:提示msg错误信息</remarks>
        public bool success { set; get; }
        /// <summary>
        /// 成功或错误的提示信息
        /// </summary>
        public string msg { set; get; }
        /// <summary>
        /// 返回数据
        /// </summary>
        public object data { set; get; }
    }
}