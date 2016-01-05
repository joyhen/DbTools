using System;

namespace Tools.App_Code
{
    /// <summary>
    /// 键值中心
    /// </summary>
    public class KeyCenter
    {
        /// <summary>
        /// 数据库链接字符串键
        /// </summary>
        public const string DBKey = "DbTools";
        /// <summary>
        /// Ajax GET方式参数对象名
        /// </summary>
        public const string KeyAjaxActionNameWhenGet = "ajaxparam";

        /// <summary>
        /// 键前缀
        /// </summary>
        public static readonly string KeyStrPrefix = "XYTools_";

        /// <summary>
        /// 数据库链接配置文件
        /// </summary>
        public const string DBConfigFile = "config.bin";
        /// <summary>
        /// 表业务信息文件
        /// </summary>
        public const string TableBusinessFile = "db.bin";

    }
}