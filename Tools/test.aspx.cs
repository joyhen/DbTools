using System;
using System.Data;
using System.Linq;
using System.Data.SqlClient;
using System.Collections.Generic;

namespace Tools
{
    using Help;

    public partial class test : BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                var result = Insert_cctable(new DTO_cctable
                {
                    bb = 2,
                    cc = "fdsa",
                    dd = DateTime.Now, // or null
                    ee = 123m,         // or null
                });
                Response.Write("写入数据库" + (result ? "成功" : "失败"));
            }
        }
        
        /// <summary>
        /// 表cctable的领域模型
        /// </summary>
        public class DTO_cctable
        {
            /// <summary>
            /// 自增主键
            /// </summary>
            /// <remarks></remarks>
            public int Id { get; set; }
            /// <summary>
            /// 
            /// </summary>
            /// <remarks></remarks>
            public int? bb { get; set; }
            /// <summary>
            /// cc的说明哦
            /// </summary>
            /// <remarks>cc的业务场景</remarks>
            public string cc { get; set; }
            /// <summary>
            /// dd说明已经修改了
            /// </summary>
            /// <remarks></remarks>
            public DateTime? dd { get; set; }
            /// <summary>
            /// 
            /// </summary>
            /// <remarks>eed的业务说明</remarks>
            public decimal? ee { get; set; }
        }

        /// <summary>
        /// 获取模型DTO_cctable数据
        /// </summary>
        public List<DTO_cctable> GetDTO_cctableData()
        {
            var sql = "SELECT Id,bb,cc,dd,ee FROM cctable";
            var ds = DBHelp.ExecuteDataSet(sql);
            if (CheckDS(ds))
            {
                var result = ds.Tables[0].MapTo<DTO_cctable>();
                return result.ToList();
            }
            return null;
        }

        /// <summary>
        /// 新增表cctable记录
        /// </summary>
        private bool Insert_cctable(DTO_cctable paramArg)
        {
            string sql = @"INSERT INTO [cctable] ([bb],[cc],[dd],[ee]) VALUES (@bb,@cc,@dd,@ee)";

            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@bb", SqlDbType.Bit),
                new SqlParameter("@cc", SqlDbType.NVarChar),
                new SqlParameter("@dd", SqlDbType.DateTime),
                new SqlParameter("@ee", SqlDbType.Decimal)
            };

            parameters[0].Value = paramArg.bb;
            parameters[1].Value = paramArg.cc;
            parameters[2].Value = paramArg.dd;
            parameters[3].Value = paramArg.ee;

            var result = DBHelp.ExecuteSql(sql, parameters);
            return result > 0;
        }

        /// <summary>
        /// 更新表cctable记录
        /// </summary>
        private bool Update_cctable(DTO_cctable paramArg)
        {
            string sql = @"UPDATE [cctable]
                              SET [bb] = @bb
                                 ,[cc] = @cc
                                 ,[dd] = @dd
                                 ,[ee] = @ee
                           WHERE  [id] = @id";

            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@Id", SqlDbType.Int),
                new SqlParameter("@bb", SqlDbType.Bit),
                new SqlParameter("@cc", SqlDbType.NVarChar),
                new SqlParameter("@dd", SqlDbType.DateTime),
                new SqlParameter("@ee", SqlDbType.Decimal)
            };

            parameters[0].Value = paramArg.Id;
            parameters[1].Value = paramArg.bb;
            parameters[2].Value = paramArg.cc;
            parameters[3].Value = paramArg.dd;
            parameters[4].Value = paramArg.ee;

            var result = DBHelp.ExecuteSql(sql, parameters);
            return result > 0;
        }

        /// <summary>
        /// 删除表cctable记录
        /// </summary>
        private bool Delete_cctable(int id)
        {
            string sql = "DELETE cctable WHERE Id=@Id";

            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@Id", SqlDbType.Int)
            };

            parameters[0].Value = id;

            var result = DBHelp.ExecuteSql(sql, parameters);
            return result > 0;
        }


        //...

    }
}