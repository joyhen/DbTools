using System;

namespace Tools.App_Code
{
    internal sealed class sql
    {
        /// <summary>
        /// 表、视图、存储过程
        /// </summary>
        /// <remarks>标量值函数 type='FN' | 表值函数 type='TF'</remarks>
        public const string GetAllTable = @"SELECT name AS [name], [sysobjects].[type] AS [type] FROM [sysobjects] 
                                            WHERE [sysobjects].[type]='U' or [sysobjects].[type]='V' or [sysobjects].[type]='P'
                                            ORDER BY 
                                            CASE WHEN  [sysobjects].[type]='U' THEN 0 WHEN  [sysobjects].[type]='V' THEN 1 ELSE 2 END , name";
        #region 表信息
        /// <summary>
        /// 表信息
        /// </summary>
        public const string GetTableInfo = @"
            SELECT    
         --       tablename                      = case when a.colorder=1 then d.name else '' end
         -- ,     tabledescription               = case when a.colorder=1 then isnull(f.value,'') else '' end
         -- ,     colorder                       = a.colorder
          /*,*/   name                           = a.name
            ,     isindex                        = case when COLUMNPROPERTY( a.id, a.name, 'IsIdentity')=1 then 1 else 0 end
            ,     iskey                          = case when exists(
                                                             SELECT 1 FROM sysobjects where 
                                                                 xtype='PK' and 
                                                                 parent_obj=a.id and 
                                                                 name in (
                                                                             SELECT name FROM sysindexes WHERE indid in(
                                                                                 SELECT indid FROM sysindexkeys WHERE id = a.id AND colid=a.colid
                                                                             )
                                                                         )
                                                             ) then 1 else 0 end
            ,     [type]                         = b.name
         -- ,     typelength                     = a.length
            ,     length                         = COLUMNPROPERTY(a.id,a.name,'PRECISION')
         -- ,     digits                         = isnull(COLUMNPROPERTY(a.id,a.name,'Scale'),0)
            ,     [isnull]                       = case when a.isnullable=1 then 1 else 0 end
            ,     defaultvalue                   = isnull(e.text,'')
            ,     description                    = isnull(g.[value],'')
            ,	  biz                            = ''
            FROM  syscolumns                     a                   
            left  join  systypes                 b  on  a.xusertype=b.xusertype
            inner join  sysobjects               d  on  a.id=d.id and d.xtype='U' and d.name<>'dtproperties'
            left  join  syscomments              e  on  a.cdefault=e.id 
            left  join  sys.extended_properties  g  on  a.id=G.major_id and a.colid=g.minor_id 
            left  join  sys.extended_properties  f  on  d.id=f.major_id and f.minor_id=0
            where   d.name='{0}'
            order by  a.id,a.colorder
            ";
        #endregion

        /// <summary>
        /// 删除表字段
        /// </summary>
        public const string DeleteField = @"ALTER TABLE [{0}] DROP COLUMN {1}";

        #region 字段处理
        /// <summary>
        /// 给字段添加说明
        /// </summary>
        public const string AddFieldProperty = @" EXECUTE sp_addextendedproperty N'MS_Description', '{2}', N'user', N'dbo', N'TABLE', N'{0}', N'COLUMN', N'{1}' ";
        /// <summary>
        /// 修改字段的说明内容
        /// </summary>
        public const string ModifyFieldProperty = @" EXEC sys.sp_updateextendedproperty N'MS_Description', '{2}', 'user', dbo, 'TABLE', '{0}', 'COLUMN', {1} ";
        /// <summary>
        /// 移除字段的说明
        /// </summary>
        public const string DropFieldProperty = @" EXEC sys.sp_dropextendedproperty 'MS_Description', 'user', dbo, 'TABLE', '{0}', 'COLUMN', {1} ";

        #endregion
    }
}