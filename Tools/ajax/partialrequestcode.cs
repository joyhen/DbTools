using System;
using System.Web;
using System.Linq;
using System.ComponentModel;
using System.Collections.Generic;

namespace Tools.ajax
{
    using GenJsonClass;
    using Tools.App_Code;
    using Newtonsoft.Json.Linq;

    public partial class requestaction
    {
        /// <summary>
        /// 获取代码库内容
        /// </summary>
        private void GetCodeLibrary()
        {
            var tablecode = BufHelp.ProtoBufDeserialize<List<KeyValue>>(KeyCenter.CodeFile)
                            ?? new List<KeyValue>();
            Outmsg(true, outdata: tablecode);
        }
        /// <summary>
        /// 从代码库中删除
        /// </summary>
        private void DelCodeLibrary()
        {
            string table = ((dynamic)ActionParama.Arg).tb;
            if (!IsNotEmptyString(table))
            {
                Outmsg("参数无效");
                return;
            }

            var tablecode = BufHelp.ProtoBufDeserialize<List<KeyValue>>(KeyCenter.CodeFile)
                            ?? new List<KeyValue>();

            var query = tablecode.FirstOrDefault(x => x.key == table);  //是否存
            if (query == null)
            {
                Outmsg("非法删除");
                return;
            }

            tablecode.Remove(query);
            var result = BufHelp.ProtoBufSerialize<List<KeyValue>>(tablecode, KeyCenter.CodeFile);
            Outmsg(result, outmsg: "删除失败");
        }
        /// <summary>
        /// 导入到代码库
        /// </summary>
        private void GenCodeLibrary()
        {
            string table = ((dynamic)ActionParama.Arg).tb;
            string field = ((dynamic)ActionParama.Arg).fd;

            if (!IsNotEmptyString(table) || !IsNotEmptyString(field))
            {
                Outmsg("参数无效");
                return;
            }

            var tablecode = BufHelp.ProtoBufDeserialize<List<KeyValue>>(KeyCenter.CodeFile)
                            ?? new List<KeyValue>();

            var query = tablecode.FirstOrDefault(x => x.key == table);  //是否存
            if (query != null)
            {
                tablecode.Remove(query);                                //存在就删除
            }

            tablecode.Add(new KeyValue { key = table, value = field });
            var result = BufHelp.ProtoBufSerialize<List<KeyValue>>(tablecode, KeyCenter.CodeFile);
            Outmsg(result, outmsg: "导入到代码库失败");
        }

        /// <summary>
        /// 生成json
        /// </summary>
        private void MakeJson()
        {
            string codestr = ((dynamic)ActionParama.Arg).pm;
            if (!IsNotEmptyString(codestr))
            {
                Outmsg("参数无效");
                return;
            }

            Outmsg("此方法尚未完善");

            //if (!codestr.Contains("class") && !codestr.Contains("@class") && !codestr.Contains("\""))
            //{

            //}
            //else
            //{

            //}
        }

        /// <summary>
        /// 生成class
        /// </summary>
        private void MakeClass()
        {
            string codestr = ((dynamic)ActionParama.Arg).pm;
            if (!IsNotEmptyString(codestr))
            {
                Outmsg("参数无效");
                return;
            }

            var gen = Prepare(codestr);
            if (gen == null)
            {
                Outmsg("初始化json代码逆向生成器失败");
                return;
            }

            try
            {
                gen.TargetFolder = null;
                gen.SingleFile = true;
                using (var sw = new System.IO.StringWriter())
                {
                    gen.OutputStream = sw;
                    gen.GenerateClasses();
                    sw.Flush();
                    var lastGeneratedString = sw.ToString();

                    CacheUtil.Remove(KeyCenter.GenJsonClass); //清除之前的缓存
                    CacheUtil.InsertCach(KeyCenter.GenJsonClass, lastGeneratedString);
                    Outmsg(true);
                }
            }
            catch (Exception ex)
            {
                Outmsg("Unable to generate the code: " + ex.Message);
            }
        }

        private JsonClassGenerator Prepare(string jsonText)
        {
            var gen = new JsonClassGenerator();
            gen.Example = jsonText;
            gen.InternalVisibility = false;         //Internal or public
            gen.ExplicitDeserialization = false;    //明确反序列化
            gen.Namespace = "Example";              //命名空间
            gen.NoHelperClass = true;               //是否生成帮助类
            gen.SecondaryNamespace = null;          //子命名空间
            gen.UseProperties = true;               //还原驼峰结构
            gen.MainClass = "SampleClass";          //类名
            gen.UsePascalCase = true;
            gen.UseNestedClasses = false;
            gen.ApplyObfuscationAttributes = false;
            gen.SingleFile = true;
            gen.ExamplesInDocumentation = false;
            return gen;
        }

        ///...

    }
}