using DBConvertSQL.Common;
using DBConvertSQL.HanderStruct.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBConvertSQL.HanderStruct
{
    /// <summary>
    /// KINGBASEESV8转换类
    /// </summary>
    public class SqlConvertKESV8 : SqlConvert
    {
        /// <summary>
        /// 调用父类构造函数
        /// </summary>
        /// <param name="dBType"></param>
        public SqlConvertKESV8(DBType dBType) : base(dBType) { }

        /// <summary>
        /// 获取POSTGRESQL初始化sql
        /// </summary>
        /// <param name="tableModels"></param>
        /// <returns></returns>
        public override string GetSqlParseOperation(List<TableModel> tableModels)
        {
            StringBuilder sqlSb = new StringBuilder();

            // 1> 添加注释
            String notes = base.GetSqlParseOperation(tableModels);
            sqlSb.Append(notes);

            // 2> 设置脚本运行环境
            sqlSb.Append(String.Format("-- ----------------------------{0}", Const.LINE_FEED_SYMBOL_ORIGINAL));
            sqlSb.Append(String.Format("-- Script environment{0}", Const.LINE_FEED_SYMBOL_ORIGINAL));
            sqlSb.Append(String.Format("-- ----------------------------{0}", Const.LINE_FEED_SYMBOL_ORIGINAL));

            sqlSb.Append(String.Format("SET statement_timeout = 0;{0}", Const.LINE_FEED_SYMBOL_ORIGINAL));
            sqlSb.Append(String.Format("SET lock_timeout = 0;{0}", Const.LINE_FEED_SYMBOL_ORIGINAL));
            sqlSb.Append(String.Format("SET idle_in_transaction_session_timeout = 0;{0}", Const.LINE_FEED_SYMBOL_ORIGINAL));
            sqlSb.Append(String.Format("SET client_encoding = 'UTF8';{0}", Const.LINE_FEED_SYMBOL_ORIGINAL));
            sqlSb.Append(String.Format("SET standard_conforming_strings = on;{0}", Const.LINE_FEED_SYMBOL_ORIGINAL));
            sqlSb.Append(String.Format("SELECT pg_catalog.set_config('search_path', 'public', false);{0}", Const.LINE_FEED_SYMBOL_ORIGINAL));
            sqlSb.Append(String.Format("SET check_function_bodies = false;{0}", Const.LINE_FEED_SYMBOL_ORIGINAL));
            sqlSb.Append(String.Format("SET xmloption = content;{0}", Const.LINE_FEED_SYMBOL_ORIGINAL));
            sqlSb.Append(String.Format("SET client_min_messages = warning;{0}", Const.LINE_FEED_SYMBOL_ORIGINAL));
            sqlSb.Append(String.Format("SET row_security = off;{0}", Const.LINE_FEED_SYMBOL_ORIGINAL));
            sqlSb.Append(String.Format("SET default_tablespace = '';{0}", Const.LINE_FEED_SYMBOL_ORIGINAL));
            sqlSb.Append(String.Format("SET default_table_access_method = heap;{0}{0}", Const.LINE_FEED_SYMBOL_ORIGINAL));

            // 3> 添加建表结构
            for (int index = 0; index < tableModels.Count; index++)
            {
                TableModel tableModel = tableModels[index];
                sqlSb.Append(GetSqlBlockTable(tableModel, index));
                sqlSb.Append(Const.LINE_FEED_SYMBOL_ORIGINAL);
            }
            return sqlSb.ToString();
        }

        /// <summary>
        /// 生成创建表的语句
        /// </summary>
        /// <param name="tableModel"></param>
        /// <returns></returns>
        public override string GetSqlBlockTable(TableModel tableModel, int tableIndex)
        {
            StringBuilder tableSb = new StringBuilder();

            // 0> 注释
            tableSb.Append(String.Format("-- ----------------------------{0}", Const.LINE_FEED_SYMBOL_ORIGINAL));
            tableSb.Append(String.Format("-- {0}:Table structure for {1}{2}", (tableIndex + 1), tableModel.tableName, Const.LINE_FEED_SYMBOL_ORIGINAL));
            tableSb.Append(String.Format("-- ----------------------------{0}", Const.LINE_FEED_SYMBOL_ORIGINAL));

            // 1> 创建表(0:双引号)
            tableSb.Append(String.Format("CREATE TABLE IF NOT EXISTS \"{0}\".\"{1}\" ({2}", dBName, tableModel.tableName, Const.LINE_FEED_SYMBOL_ORIGINAL));
            // 1.1> 创建字段
            for (int index = 0; index < tableModel.fieldModels.Count; index++)
            {
                FieldModel fieldModel = tableModel.fieldModels[index];
                Boolean isEnd = (index == tableModel.fieldModels.Count - 1);
                tableSb.Append(GetSqlBlockField(fieldModel, isEnd));
            }

            // 2> 执行语句块结束
            tableSb.Append(String.Format(");{0}", Const.LINE_FEED_SYMBOL_ORIGINAL));

            // 3> 修改字段的注释
            tableSb.Append(GetSqlFieldComment(tableModel));

            // 4> 添加表的约束
            tableSb.Append(GetSqlBlockTableConstraint(tableModel));

            return tableSb.ToString();
        }

        /// <summary>
        /// 生成创建字段语句
        /// </summary>
        /// <param name="fieldModel">字段模型</param>
        /// <param name="isEnd">是否是最后一个字段</param>
        /// <returns></returns>
        public override String GetSqlBlockField(FieldModel fieldModel, Boolean isEnd)
        {
            StringBuilder fieldSb = new StringBuilder();

            // 1> 获取类型映射项
            MappingModelItem mappingModelItem = GetMappingModelItemBySourceType(fieldModel.fieldType);
            if (mappingModelItem == null)
            {
                return fieldSb.ToString();
            }

            fieldSb.Append(String.Format("\"{0}\" ", fieldModel.fieldName));
            fieldSb.Append(mappingModelItem.targetType);
            // 2> 追加类型长度
            // 2.1> 有类型长度
            if (mappingModelItem.isTargetLength)
            {
                // 源数据类型有长度,使用源数据类型的长度
                if (mappingModelItem.isSourceLength)
                {
                    fieldSb.Append(String.Format("({0}", fieldModel.fieldLength));
                }
                // 源数据类型没有长度,使用目标数据类型的默认长度
                else
                {
                    fieldSb.Append(String.Format("({0}", mappingModelItem.targetLength));
                }

                // 判断是否是varchar，需要加长度标识
                if ("varchar".Equals(fieldModel.fieldType))
                {
                    fieldSb.Append(" char");
                }

                fieldSb.Append(") ");
            }
            // 2.2> 无类型长度
            else
            {
                // 没有类型长度需要加空格
                fieldSb.Append(" ");
            }

            // 3> 追加主键
            if (fieldModel.isPrimaryKey)
            {
                fieldSb.Append("PRIMARY KEY ");
            }

            // 4> 是否有默认值
            if (fieldModel.isDefaultValue && fieldModel.defaultValue != null)
            {
                fieldSb.Append(String.Format("{0} {1} ", MySqlKeyword.DEFAULT, fieldModel.defaultValue));
            }

            // 5> 添加字段的扩展属性
            fieldSb.Append(GetSqlBlockFieldExtend(fieldModel));

            // 6> 是否非空
            if (fieldModel.notEmpty)
            {
                fieldSb.Append("NOT NULL ");
            }

            // 7> 追加,最后一个字段不加,
            fieldSb.Append(String.Format("{0}{1}", (isEnd ? "" : ","), Const.LINE_FEED_SYMBOL_ORIGINAL));

            return fieldSb.ToString();
        }


        public override String GetBeforeConfigSql()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(String.Format("SET statement_timeout = 0; {0}", Const.LINE_FEED_SYMBOL_ORIGINAL));
            sb.Append(String.Format("SET lock_timeout = 0; {0}", Const.LINE_FEED_SYMBOL_ORIGINAL));
            sb.Append(String.Format("SET idle_in_transaction_session_timeout = 0; {0}", Const.LINE_FEED_SYMBOL_ORIGINAL));
            sb.Append(String.Format("SET client_encoding = 'UTF8'; {0}", Const.LINE_FEED_SYMBOL_ORIGINAL));
            sb.Append(String.Format("SET standard_conforming_strings = on; {0}", Const.LINE_FEED_SYMBOL_ORIGINAL));
            sb.Append(String.Format("SELECT pg_catalog.set_config('search_path', 'public', false); {0}", Const.LINE_FEED_SYMBOL_ORIGINAL));
            sb.Append(String.Format("SET check_function_bodies = false; {0}", Const.LINE_FEED_SYMBOL_ORIGINAL));
            sb.Append(String.Format("SET xmloption = content; {0}", Const.LINE_FEED_SYMBOL_ORIGINAL));
            sb.Append(String.Format("SET client_min_messages = warning; {0}", Const.LINE_FEED_SYMBOL_ORIGINAL));
            sb.Append(String.Format("SET row_security = off; {0}", Const.LINE_FEED_SYMBOL_ORIGINAL));

            return sb.ToString();
        }

    }
}
