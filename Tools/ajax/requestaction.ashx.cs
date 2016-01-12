using System;
using System.Web;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;

namespace Tools.ajax
{
    using Help;

    public partial class requestaction : IHttpHandler
    {
        /// <summary>
        /// 当前http请求信息
        /// </summary>
        private HttpContext CurrentContext;
        /// <summary>
        /// 当前操作需要的参数对象
        /// </summary>
        protected AjaxParama ActionParama = null;

        public void ProcessRequest(HttpContext context)
        {
            CurrentContext = context;

            #region 对象输出设置
            context.Response.Buffer = true;
            context.Response.ExpiresAbsolute = DateTime.Now.AddMilliseconds(-1);
            context.Response.Expires = 0;
            context.Response.Cache.SetNoStore();                        // 禁用缓存
            context.Response.Clear();                                   // 清除输出
            context.Response.CacheControl = "no-cache";
            context.Response.AppendHeader("Pragma", "No-Cache");
            context.Response.ContentType = "text/json";
            #endregion

            #region 基本请求校验
            var requestinfo = context.Request;                          //HttpRequest

            if (requestinfo.UrlReferrer == null || requestinfo.UrlReferrer.Host != requestinfo.Url.Host)
            {
                Outmsg("非正常请求"); return;
            }
            #endregion

            #region 获取请求对象
            string requestData = string.Empty;                          //请求参数
            string requestType = requestinfo.RequestType.ToUpper();     //请求方式
            if (requestType == "POST")
            {
                var iptStream = HttpContext.Current.Request.InputStream;
                var byts = new byte[iptStream.Length];
                HttpContext.Current.Request.InputStream.Read(byts, 0, byts.Length);
                requestData = System.Text.Encoding.UTF8.GetString(byts);
            }
            else if (requestType == "GET")
            {
                requestData = Q(KeyCenter.KeyAjaxActionNameWhenGet);
            }
            #endregion

            #region 分析请求对象
            if (!string.IsNullOrWhiteSpace(requestData))
            {
                ActionParama = JsonUtils.DeserializeObject<AjaxParama>(requestData);
                if (ActionParama == null) { Outmsg("请求错误"); return; }
            }
            #endregion

            switch (ClientAction)
            {
                case "updateconstr":
                    UpdateConstr();
                    break;
                case "getalltable":
                    GetAllTable();
                    break;
                case "gettableinfo":
                    GetTableInfo();
                    break;
                case "deletefield":
                    DeleteField();
                    break;
                case "addfield":
                    AddField();
                    break;
                case "mark":
                    Mark();
                    break;
                case "scence":
                    Sence();
                    break;
                case "gencodelib":
                    GenCodeLibrary();
                    break;
                case "getcodelib":
                    GetCodeLibrary();
                    break;
                case "deletecodelib":
                    DelCodeLibrary();
                    break;

                case "makejson":
                    MakeJson();
                    break;
                case "makeclass":
                    MakeClass();
                    break;  

                default:
                    Outmsg("请求无效");
                    break;
            }
        }

        #region 业务模块
        /// <summary>
        /// 更新数据库链接
        /// </summary>
        private void UpdateConstr()
        {
            string constr = ((dynamic)ActionParama.Arg).cs;
            if (!IsNotEmptyString(constr))
            {
                Outmsg("参数无效");
                return;
            }

            var model = new KeyValue { key = KeyCenter.DBKey, value = constr };
            var result = BufHelp.ProtoBufSerialize<KeyValue>(model, KeyCenter.DBConfigFile);
            if (result) CacheUtil.Clear();                                                      //清空所有缓存

            Outmsg(result, outmsg: "修改链接字符串出错");
        }
        /// <summary>
        /// 获取数据库表
        /// </summary>
        /// <remarks>过滤掉conflict打头的表</remarks>
        private void GetAllTable()
        {
            var key = MethodBase.GetCurrentMethod().Name;
            var query = GetDataWithCache<List<DataBaseTB>>(new Func<List<DataBaseTB>>(() =>
            {
                var ds = DBHelp.ExecuteDataSet(sql.GetAllTable);
                if (CheckDS(ds))
                {
                    var result = ds.Tables[0].MapTo<DataBaseTB>()
                        .Where(t => t.type.Trim() == "U" && !(t.name.Trim().StartsWith("conflict")));
                    return result.ToList();
                }
                return null;
            }), key);

            if (GetArgParam() == "all")                                                         //取所有表
            {
                Outmsg(query != null, outdata: query);
                return;
            }

            var subcout = query.Take(20).ToList();                                              //取20个表显示
            Outmsg(query != null, outdata: subcout);
        }
        /// <summary>
        /// 获取表信息
        /// </summary>
        private void GetTableInfo()
        {
            string tbname = ((dynamic)ActionParama.Arg).tbname;
            if (!IsNotEmptyString(tbname))
            {
                Outmsg("参数无效");
                return;
            }

            var query = GetTableDetail(tbname);

            if (GetArgParam() != "biz")
            {
                Outmsg(query != null, outdata: query);
                return;
            }

            //业务场景的表信息标记
            var tbBizInfo = BufHelp.ProtoBufDeserialize<List<TableInfoTiny>>(KeyCenter.TableBusinessFile)
                            ?? new List<TableInfoTiny>();

            var currentbiz = tbBizInfo.FirstOrDefault(x => x.tablename == tbname);              //当前表的业务说明信息
            if (currentbiz != null && currentbiz.fieldinfo.Count > 0)
            {
                query.ForEach(x =>
                {
                    var _temp = currentbiz.fieldinfo.FirstOrDefault(f => f.name == x.name);
                    x.biz = _temp != null ? _temp.biz : "";
                });
                Outmsg(true, outdata: query);
            }
            else
                Outmsg(true, outdata: query);
        }

        /// <summary>
        /// 删除表字段
        /// </summary>
        private void DeleteField()
        {
            string table = ((dynamic)ActionParama.Arg).tb;
            string field = ((dynamic)ActionParama.Arg).fd;
            if (!IsNotEmptyString(table) || !IsNotEmptyString(field))
            {
                Outmsg("参数无效");
                return;
            }

            if (table != "cctable") { Outmsg("您没有删除权限"); return; } //test

            var result = DBHelp.ExecuteSql(string.Format(sql.DeleteField, table, field));
            if (result != 0) { CacheUtil.Remove(KeyCenter.KeyStrPrefix + table); }              //清除缓存

            Outmsg(result != 0, outmsg: "删除字段失败");
        }
        /// <summary>
        /// 添加字段
        /// </summary>
        /// <remarks>需要更新缓存</remarks>
        private void AddField()
        {
            var argdata = GetActionParamData<FiedHandle>(true);
            if (argdata == null)
            {
                Outmsg("参数解析错误");
                return;
            }
            //...
            Outmsg("添加功能暂时未对外开放");
        }
        /// <summary>
        /// 修改字段备注和业务场景信息
        /// </summary>
        private void Mark()
        {
            var argdata = GetActionParamData<MofifyArg>(true);
            if (argdata == null)
            {
                Outmsg("参数解析错误");
                return;
            }

            if (!IsNotEmptyString(argdata.table) ||
                !IsNotEmptyString(argdata.name) ||
                !IsNotEmptyString(argdata.desc))
            {
                Outmsg("参数无效");
                return;
            }

            var query = GetTableDetail(argdata.table);                                //表信息
            if (query == null || query.Count == 0)
            {
                Outmsg("数据表无效");
                return;
            }

            var field = query.FirstOrDefault(x => x.name == argdata.name);                      //字段信息
            if (field == null)
            {
                Outmsg("无效字段");
                return;
            }

            string sqlstr = !IsNotEmptyString(field.description) ?
                            sql.AddFieldProperty :                                              //添加字段说明
                            sql.ModifyFieldProperty;                                            //修改字段说明

            var result = DBHelp.ExecuteSql(string.Format(sqlstr, argdata.table, argdata.name, argdata.desc));
            if (result == 0)
            {
                Outmsg("修改字段说明失败");
                return;
            }

            CacheUtil.Remove(KeyCenter.KeyStrPrefix + argdata.table);                           //清除当前字段所在表的缓存信息
            Outmsg(true, outmsg: "修改字段说明成功");
        }
        /// <summary>
        /// 修改字段的业务场景说明
        /// </summary>
        private void Sence()
        {
            #region check
            var argdata = GetActionParamData<MofifyArg>(true);
            if (argdata == null)
            {
                Outmsg("参数解析错误");
                return;
            }

            if (!IsNotEmptyString(argdata.table) ||
                !IsNotEmptyString(argdata.name) ||
                !IsNotEmptyString(argdata.biz))
            {
                Outmsg("参数无效");
                return;
            }

            var query = GetTableDetail(argdata.table);                                //表信息
            if (query == null || query.Count == 0)
            {
                Outmsg("数据表无效");
                return;
            }

            var field = query.FirstOrDefault(x => x.name == argdata.name);                      //字段信息
            if (field == null)
            {
                Outmsg("无效字段");
                return;
            }
            #endregion

            #region 字段的业务场景描述
            var tbBizInfo = BufHelp.ProtoBufDeserialize<List<TableInfoTiny>>(KeyCenter.TableBusinessFile)
                            ?? new List<TableInfoTiny>();
            //业务属性描述
            var currentbiz = tbBizInfo.FirstOrDefault(x => x.tablename == argdata.table);       //当前表的业务说明信息
            if (currentbiz == null || currentbiz.fieldinfo.Count == 0)                          //表不存在
            {
                var fieldbiz = new TableInfoTiny
                {
                    tablename = argdata.table,
                    fieldinfo = query.Select(x => new FieldInfo
                    {
                        name = x.name,
                        type = x.type,
                        des = x.des,
                        biz = x.name == argdata.name ? argdata.biz.Trim() : ""
                    }).ToList()
                };
                tbBizInfo.RemoveAll(x => x.tablename == argdata.table);
                tbBizInfo.Add(fieldbiz);
            }
            else                                                                                //表存在
            {
                var tempf = currentbiz.fieldinfo.FirstOrDefault(x => x.name == argdata.name);
                tempf.biz = argdata.biz;                                                        //修改业务描述内容
            }

            var result = BufHelp.ProtoBufSerialize<List<TableInfoTiny>>(tbBizInfo, KeyCenter.TableBusinessFile);
            Outmsg(result, outmsg: string.Format("修改字段{0}的业务属性出错", argdata.name));
            #endregion
        }
        #endregion

        /// <summary>
        /// 获取表信息
        /// </summary>
        private List<TableInfo> GetTableDetail(string tbname)
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
        /// 当前操作方法名的客户端方法签名
        /// </summary>
        protected string ClientAction
        {
            get
            {
                if (ActionParama == null)
                    return string.Empty;

                return ActionParama.Action ?? string.Empty;
            }
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}