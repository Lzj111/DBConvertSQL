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
    /// DM8SQL转换类
    /// </summary>
    public class SqlConvertDM8 : SqlConvert
    {
        /// <summary>
        /// 调用父类构造函数
        /// </summary>
        /// <param name="dBType"></param>
        public SqlConvertDM8(DBType dBType) : base(dBType) { }

        /// <summary>
        /// 获取DM8初始化sql
        /// </summary>
        /// <param name="tableModels"></param>
        /// <returns></returns>
        public override String GetSqlParseOperation(List<TableModel> tableModels)
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
            // 1> 语句块开始
            tableSb.Append("BEGIN" + Const.LINE_FEED_SYMBOL_ORIGINAL);
            // 2> 验证表是否已经存在于当前模式(库)中
            tableSb.Append(String.Format("IF (SELECT COUNT(1) FROM DBA_TABLES WHERE TABLE_NAME = '{0}' AND DBA_TABLES.OWNER = '{1}')=0 THEN {2}",
                tableModel.tableName, tableModel.databaseName, Const.LINE_FEED_SYMBOL_ORIGINAL));
            // 3> 执行语句块开始
            tableSb.Append(String.Format("EXEC IMMEDIATE {0}' {0}", Const.LINE_FEED_SYMBOL_ORIGINAL));

            // 4> 创建表(0:双引号)
            tableSb.Append(String.Format("CREATE TABLE {0}{1}{0}.{0}{2}{0} {3}", SqlKeyword.DOUBLE_QUOTES, tableModel.databaseName,
                tableModel.tableName, Const.LINE_FEED_SYMBOL_ORIGINAL));
            tableSb.Append(String.Format("( {0}", Const.LINE_FEED_SYMBOL_ORIGINAL));
            // 4.1> 创建字段
            for (int index = 0; index < tableModel.fieldModels.Count; index++)
            {
                FieldModel fieldModel = tableModel.fieldModels[index];
                Boolean isEnd = (index == tableModel.fieldModels.Count - 1);
                tableSb.Append(GetSqlBlockField(fieldModel, isEnd));
            }

            // 5> 是否有索引
            if (tableModel.constraintModels.Count > 0)
            {
                String unique = GetSqlBlockTableConstraint(tableModel);
                if (!String.IsNullOrEmpty(unique))
                {
                    tableSb.Append(String.Format(",{0}", unique));
                }
            }

            // 6> 执行语句块结束
            tableSb.Append(String.Format(") STORAGE(ON \"MAIN\", CLUSTERBTR) ;", Const.LINE_FEED_SYMBOL_ORIGINAL));
            tableSb.Append(String.Format("{0}'; {0}END IF;{0}END; {0}", Const.LINE_FEED_SYMBOL_ORIGINAL));

            // 7> 修改字段的注释
            tableSb.Append(GetSqlFieldComment(tableModel));

            return tableSb.ToString();
        }

        /// <summary>
        /// 获取表的约束
        /// </summary>
        /// <param name="tableModel"></param>
        /// <returns></returns>
        public override String GetSqlBlockTableConstraint(TableModel tableModel)
        {
            if (tableModel.constraintModels.Count == 0)
            {
                return "";
            }

            StringBuilder sb = new StringBuilder();
            for (int index = 0; index < tableModel.constraintModels.Count; index++)
            {
                ConstraintModel constraintModel = tableModel.constraintModels[index];
                sb.Append(String.Format("CONSTRAINT \"{0}\" UNIQUE(", constraintModel.constraintName));

                // 索引字段
                List<String> columns = constraintModel.constraintColumns;
                for (int j = 0; j < columns.Count; j++)
                {
                    String splitSymbol = (j < columns.Count - 1) ? "," : "";
                    sb.Append(String.Format("\"{0}\"{1}", columns[j], splitSymbol));
                }

                // 索引语句
                String uniqueSplitSymbol = (index < tableModel.constraintModels.Count - 1) ? "," : "";
                sb.Append(String.Format("){0}", uniqueSplitSymbol));
            }

            return sb.ToString();
        }


        public override String GetSqlDataBlockInsert(InsertStatementModel insertStatementModel, bool isNewTable, int insertIndex)
        {
            // 1> 获取必要的参数
            String tableName = insertStatementModel.tableName.Replace("`", "");
            String fieldString = insertStatementModel.fieldString.Replace("`", "\"");
            String lineValue = insertStatementModel.lineValue;
            String firstField = insertStatementModel.firstField;
            Object firstValue = insertStatementModel.firstValue;

            // 2> 拼接sql语句
            StringBuilder sb = new StringBuilder();
            String dbAndTable = String.Format("\"{0}\".\"{1}\"", this.dBName, tableName);
            sb.Append(String.Format("INSERT INTO {0}({1}) ", dbAndTable, fieldString));
            sb.Append(String.Format("SELECT {0} WHERE NOT EXISTS ", lineValue));
            sb.Append(String.Format("(SELECT * FROM {0} WHERE {1} = {2}); ", dbAndTable, firstField, firstValue));

            return sb.ToString();
        }

    }
}