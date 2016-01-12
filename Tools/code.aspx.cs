using System;
using System.Linq;
using System.Collections.Generic;

namespace Tools
{
    using Help;
    using System.Data.SqlClient;
    using System.Data;

    public partial class code : BasePage
    {
        protected string coderesult = string.Empty;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                string table = Q("tb");
                string field = Q("fd");

                if (!IsNotEmptyString(table) || !IsNotEmptyString(table)) return;

                var arr = field.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                coderesult = GenCode(table, arr);
                coderesult += string.Format(getdatatemplate.GenCodeFormat(), table, field);

                var col = string.Join(",", arr.Select(x => string.Format("[{0}]", x)).ToArray());
                var val = string.Join(",", arr.Select(x => string.Format("@{0}", x)).ToArray());
                coderesult += string.Format(insertsql, table, col, val, getSqlParameter(arr, table), setSqlParameter(arr));

            }
        }

        /// <summary>
        /// sqlserver 数据库字段类型集合
        /// </summary>
        private List<keyvalue> Sqltype = Enum
            .GetNames(typeof(SqlDbType))
            .Select(x => new keyvalue { key = x, val = x.ToLower() }).ToList();

        private string getSqlParameter(string[] arr, string table)
        {
            var query = arr.Select(x =>
            {
                var thistype = getcurrenttype(table, x);
                var maptype = Sqltype.FirstOrDefault(t => string.Compare(t.val, thistype, ignoreCase: true) == 0); //映射到数据库
                return string.Format("new SqlParameter(\"@{0}\", SqlDbType.{1})", x, maptype.key);
            });

            return string.Join(@",
        ", query);
        }
        private string setSqlParameter(string[] arr)
        {
            var ps = new System.Text.StringBuilder(System.Environment.NewLine);
            for (int i = 0; i < arr.Length; i++)
            {
                ps.AppendLine(string.Format("    parameters[{0}] = paramArg.{1};", i, arr[i]));
            }
            return ps.ToString();
        }

        /// <summary>
        /// 获取当前列的映射类型（小写的）
        /// </summary>
        private string getcurrenttype(string table, string column)
        {
            var cachedata = GetTableDetail(table);
            if (cachedata == null) return "";

            var tbiinfo = cachedata.FirstOrDefault(x => x.name == column);
            return tbiinfo == null ? "" : tbiinfo.type;
        }

        internal class keyvalue
        {
            public string key { get; set; }
            public string val { get; set; }
        }

        private string GenCode(string table, string[] fields)
        {
            var cachedata = GetTableDetail(table);
            if (cachedata == null) return "";

            //表字段的业务场景描述内容
            var tbbizinfo = BufHelp.ProtoBufDeserialize<List<TableInfoTiny>>(KeyCenter.TableBusinessFile)
                            ?? new List<TableInfoTiny>();
            var currentbiz = tbbizinfo.FirstOrDefault(x => x.tablename == table);               //当前表的业务说明信息
            var biztag = currentbiz != null && currentbiz.fieldinfo.Count > 0;

            System.Text.StringBuilder strb = new System.Text.StringBuilder();

            cachedata.ForEach(x =>
            {
                if (!fields.Contains(x.name)) return;

                if (biztag)
                {
                    var _temp = currentbiz.fieldinfo.FirstOrDefault(f => f.name == x.name);
                    strb.AppendFormat(summarytemplate, x.description, (_temp != null ? _temp.biz : ""));
                }
                else
                    strb.AppendFormat(summarytemplate, x.description, "");

                strb.Append(System.Environment.NewLine);
                strb.AppendFormat(propertytemplate, FormatType(x.type, x.isnull == 1), x.name);
            });

            return string.Format(classtemplate, table, strb.ToString().GenCodeFormat());
        }

        private string getdatatemplate
        {
            get
            {
                return @"

public List<DTO_{0}> GetDTO_{0}Data()
{{
    var sql = ""SELECT {1} FROM {0}"";
    var ds = DBHelp.ExecuteDataSet(sql);
    if (CheckDS(ds))
    {{
        var result = ds.Tables[0].MapTo<DTO_{0}>();
        return result.ToList();
    }}
    return null;
}}";
            }
        }

        private string FormatType(string @type, bool isnull)
        {
            if (@type == "nchar" || @type == "variant" || @type == "text" || @type == "ntext" || @type == "nvarchar")
                return "string";
            if (@type == "bit" || @type == "bigint" || @type == "tinyint" || @type == "smallint")
                return isnull ? "int?" : "int";
            if (@type == "bit")
                return isnull ? "bool?" : "bool";
            if (@type == "datetime" || @type == "smalldatetime")
                return isnull ? "DateTime?" : "DateTime";
            if (@type == "binary" || @type == "image")
                return isnull ? "byte?" : "byte";
            if (@type == "money")
                return isnull ? "decimal?" : "decimal";
            if (@type == "xml")
                return "object";

            return @type + (isnull ? "?" : "");
        }
        private string summarytemplate
        {
            get
            {
                return @"
    /// <summary>
    /// {0}
    /// </summary>
    /// <remarks>{1}</remarks>";
            }
        }
        private string propertytemplate
        {
            get
            {
                return @"    public {0} {1} {{ get; set; }}";
            }
        }
        private string classtemplate
        {
            get
            {
                return @"
using System;

public class DTO_{0}
{{  {1}
}}";
            }
        }

        private string insertsql
        {
            get
            {
                return @"

private bool Insert_{0}(DTO_{0} paramArg)
{{
    string sql = ""INSERT INTO [{0}]({1})VALUES({2})"";

    SqlParameter[] parameters = new SqlParameter[]
    {{
        {3}
    }};
    {4}
    var result = DBHelp.ExecuteSql(sql, parameters);
    return result > 0;
}}";
                //return "";
            }
        }

        //...

    }
}