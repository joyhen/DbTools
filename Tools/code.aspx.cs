using System;
using System.Linq;
using System.Collections.Generic;

namespace Tools
{
    using Tools.App_Code;

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
            }
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

public List<{0}> Get{0}Data()
{{
    var sql = ""SELECT {1} FROM {0}"";
    var ds = DBHelp.ExecuteDataSet(sql);
    if (CheckDS(ds))
    {{
        var result = ds.Tables[0].MapTo<{0}>();
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

public class {0}
{{
    {1}
}}";
            }
        }
    }
}