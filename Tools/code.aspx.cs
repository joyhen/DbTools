using System;
using System.Linq;
using System.Collections.Generic;

namespace Tools
{
    using Help;
    using System.Data;
    using System.Data.SqlClient;

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

                coderesult = formdesign(table, arr);
                coderesult += getmodelcode(table, arr);
                coderesult += string.Format(getdatatemplate,
                                            table,
                                            field,
                                            "获取模型" + modeltype + table + "数据",
                                            modeltype);

                coderesult += getinsertcode(arr, table);    //新增操作代码
                coderesult += getupdatecode(arr, table);    //更新操作代码
                coderesult += string.Format(delete, table); //删除操作代码

                coderesult = coderesult.GenCodeFormat();
            }
        }

        /// <summary>
        /// 新增
        /// </summary>
        private string getinsertcode(string[] arr, string table)
        {
            arr = arr.Where(x => x.ToLower() != "id").ToArray(); //过滤掉id字段
            var col = string.Join(",", arr.Select(x => string.Format("[{0}]", x)).ToArray());
            var val = string.Join(",", arr.Select(x => string.Format("@{0}", x)).ToArray());

            var insql = string.Format(insertsql, table, col, val); //插入语句
            var result = string.Format(insert_update,
                                        table,
                                        insql,
                                        getSqlParameter(arr, table),
                                        setSqlParameter(arr),
                                        dbactiontype.Insert.ToString(),
                                        "新增表" + table + "记录",
                                        modeltype);
            return result;
        }

        /// <summary>
        /// 更新
        /// </summary>
        /// <remarks>必须包含id自增主键列</remarks>
        private string getupdatecode(string[] arr, string table)
        {
            if (!arr.Any(x => x.ToLower() == "id")) return null;

            var temparr = arr.Where(x => x.ToLower() != "id").ToArray(); //过滤掉id字段

            var strb = new System.Text.StringBuilder();
            for (int i = 0; i < temparr.Length; i++)
            {
                strb.Append((i == 0 ? "SET " : "\r                        ,") + string.Format("[{0}] = @{0}", temparr[i]));
            }

            var upsql = string.Format(updatesql, table, strb.ToString()); //更新语句
            var result = string.Format(insert_update,
                                        table,
                                        upsql,
                                        getSqlParameter(arr, table),
                                        setSqlParameter(arr),
                                        dbactiontype.Update.ToString(),
                                        "更新表" + table + "记录",
                                        modeltype);
            return result;
        }

        private const string modeltype = "D"; //"DTO_";
        /// <summary>
        /// 模型是否支持Protobuf
        /// </summary>
        private bool SupportProtobuf = false; //true
        private Object Locker = new Object();

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
                ps.AppendLine(string.Format("    parameters[{0}].Value = paramArg.{1};", i, arr[i]));
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
        /// <summary>
        /// 对象模型代码
        /// </summary>
        private string getmodelcode(string table, string[] fields)
        {
            var cachedata = GetTableDetail(table);
            if (cachedata == null) return "";

            //表字段的业务场景描述内容
            var tbbizinfo = BufHelp.ProtoBufDeserialize<List<TableInfoTiny>>(KeyCenter.TableBusinessFile)
                            ?? new List<TableInfoTiny>();
            var currentbiz = tbbizinfo.FirstOrDefault(x => x.tablename == table);               //当前表的业务说明信息
            var biztag = currentbiz != null && currentbiz.fieldinfo.Count > 0;                  //字段是否有业务场景描述

            var tag = 0; //标记
            var strb = new System.Text.StringBuilder();

            cachedata.ForEach(x =>
            {
                if (!fields.Contains(x.name)) return;

                tag++;

                if (biztag)
                {
                    var _temp = currentbiz.fieldinfo.FirstOrDefault(f => f.name == x.name);
                    strb.AppendFormat(summarytemplate, x.description, (_temp != null ? _temp.biz : ""));
                }
                else
                    strb.AppendFormat(summarytemplate, x.description, "");

                if (SupportProtobuf) //支持buf序列化
                {
                    strb.Append(System.Environment.NewLine);
                    strb.Append(string.Format("    [ProtoMember({0})]", tag));
                }

                strb.Append(System.Environment.NewLine);

                strb.AppendFormat(propertytemplate, FormatType(x.type, x.isnull == 1), x.name);
            });

            return string.Format(
                classtemplate,
                table,
                strb.ToString(),
                modeltype,
                DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                SupportProtobuf ? System.Environment.NewLine + "[ProtoContract]" : "",
                SupportProtobuf ? System.Environment.NewLine + "using ProtoBuf;" : "");
        }

        private string getdatatemplate
        {
            get
            {
                return @"

/// <summary>
/// {2}
/// </summary>
public List<{3}{0}> Get{3}{0}Data()
{{
    var sql = @""SELECT {1} FROM {0}"";
    var ds = DBHelp.ExecuteDataSet(sql);
    if (CheckDS(ds))
    {{
        var result = ds.Tables[0].MapTo<{3}{0}>();
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
            if (@type == "bigint")
                return isnull ? "int?" : "int";
            if (@type == "tinyint" || @type == "smallint") //数据库对应tinyint,smallint，映射时不可为Nullable
                return "int";
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
///代码创建时间：{3}

using System;{5}
using System.Data;
using System.Linq;
using System.Data.SqlClient;
using System.Collections.Generic;

/// <summary>
/// 表{0}的领域模型
/// </summary>{4}
public class {2}{0}
{{  {1}
}}";
            }
        }

        /// <summary>
        /// 插入语句
        /// </summary>
        private string insertsql
        {
            get
            {
                return @"string sql = @""INSERT INTO [{0}] ({1}) VALUES ({2})"";";
            }
        }
        /// <summary>
        /// 更新语句
        /// </summary>
        private string updatesql
        {
            get
            {
                return @"string sql = @""UPDATE [{0}]
                     {1}
                  WHERE  [id] = @id"";";
            }
        }
        /// <summary>
        /// 删除语句
        /// </summary>
        private string deletesql
        {
            get
            {
                return @"string sql = ""DELETE {0} WHERE Id=@Id"";";
            }
        }

        private string insert_update
        {
            get
            {
                return @"

/// <summary>
/// {5}
/// </summary>
public bool {4}_{0}({6}{0} paramArg)
{{
    {1}

    SqlParameter[] parameters = new SqlParameter[]
    {{
        {2}
    }};
    {3}
    var result = DBHelp.ExecuteSql(sql, parameters);
    return result > 0;
}}";
            }
        }

        private string delete
        {
            get
            {
                return @"

/// <summary>
/// 删除表{0}记录
/// </summary>
public bool Delete_{0}(int id)
{{
    string sql = ""DELETE {0} WHERE Id=@Id"";

    SqlParameter[] parameters = new SqlParameter[]
    {{
        new SqlParameter(""@Id"", SqlDbType.Int)
    }};
    
    parameters[0].Value = id;

    var result = DBHelp.ExecuteSql(sql, parameters);
    return result > 0;
}}";
            }
        }

        /// <summary>
        /// 表单代码
        /// </summary>
        private string formdesign(string table, string[] fields)
        {
            string template = @"
            <div class=""v52fmbx_dlbox"">
                <dl>
                    <dt>{0}：</dt>
                    <dd>
                        <input name=""{1}"" validate type=""text"" value="""" class=""text nonull ccverify"" />
                        <span class=""tips"">{2}</span>
                    </dd>
                </dl>
            </div>";

            var cachedata = GetTableDetail(table);
            //表字段的业务场景描述内容
            var tbbizinfo = BufHelp.ProtoBufDeserialize<List<TableInfoTiny>>(KeyCenter.TableBusinessFile)
                            ?? new List<TableInfoTiny>();
            var currentbiz = tbbizinfo.FirstOrDefault(x => x.tablename == table);               //当前表的业务说明信息
            var biztag = currentbiz != null && currentbiz.fieldinfo.Count > 0;                  //字段是否有业务场景描述

            var strb = new System.Text.StringBuilder();

            cachedata.ForEach(x =>
            {
                if (!fields.Contains(x.name)) return;

                string tips = string.Empty;
                if (biztag)
                {
                    var _temp = currentbiz.fieldinfo.FirstOrDefault(f => f.name == x.name);
                    tips = _temp != null ? _temp.biz : "";
                }

                strb.AppendFormat(template, x.description, x.name.ToLower(), tips);
                strb.Append(System.Environment.NewLine);
            });

            return strb.ToString();
        }

        internal class keyvalue
        {
            public string key { get; set; }
            public string val { get; set; }
        }
        private enum dbactiontype
        {
            Insert,
            Update,
            Delete
        }

        //...

    }
}