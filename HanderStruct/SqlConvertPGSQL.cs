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
    /// POSTGRESQL转换类
    /// </summary>
    public class SqlConvertPGSQL : SqlConvert
    {
        /// <summary>
        /// 调用父类构造函数
        /// </summary>
        /// <param name="dBType"></param>
        public SqlConvertPGSQL(DBType dBType) : base(dBType) { }

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

            // 2> 添加建表结构
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

            // 1> 删除表(删除表有需要的可以放开)
            // tableSb.Append(String.Format("-- DROP TABLE IF EXISTS \"{0}\".\"{1}\";{2}", dBName, tableModel.tableName, Const.LINE_FEED_SYMBOL_ORIGINAL));

            // 2> 创建表(0:双引号)
            tableSb.Append(String.Format("CREATE TABLE IF NOT EXISTS \"{0}\".\"{1}\" ({2}", dBName, tableModel.tableName, Const.LINE_FEED_SYMBOL_ORIGINAL));
            // 2.1> 创建字段
            for (int index = 0; index < tableModel.fieldModels.Count; index++)
            {
                FieldModel fieldModel = tableModel.fieldModels[index];
                Boolean isEnd = (index == tableModel.fieldModels.Count - 1);
                tableSb.Append(GetSqlBlockField(fieldModel, isEnd));
            }

            // 3> 执行语句块结束
            tableSb.Append(String.Format(");{0}", Const.LINE_FEED_SYMBOL_ORIGINAL));

            // 4> 修改字段的注释
            tableSb.Append(GetSqlFieldComment(tableModel));

            // 5> 添加表的约束
            tableSb.Append(GetSqlBlockTableConstraint(tableModel));

            return tableSb.ToString();
        }

        /// <summary>
        /// 部分类型需要指定排序方式
        /// </summary>
        /// <param name="fieldModel"></param>
        /// <returns></returns>
        public override String GetSqlBlockFieldExtend(FieldModel fieldModel)
        {
            StringBuilder extend = new StringBuilder();

            switch (fieldModel.fieldType)
            {
                // 字符串&text类型需要指定排序
                case "varchar":
                case "text":
                    extend.Append("COLLATE \"pg_catalog\".\"default\" ");
                    break;
                default:
                    break;
            }

            return extend.ToString();
        }



    }
}
