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
    /// MYSQL转换类
    /// </summary>
    public class SqlConvertMySQL : SqlConvert
    {
        /// <summary>
        /// 调用父类构造函数
        /// </summary>
        /// <param name="dBType"></param>
        public SqlConvertMySQL(DBType dBType) : base(dBType) { }

        /// <summary>
        /// 获取转换后的Sql脚本(重写父类mysql不需要结构,直接替换)
        /// </summary>
        /// <param name="originalSql"></param>
        /// <param name="dbName"></param>
        /// <returns></returns>
        public override string GetSqlParse(string originalSql, string dbName)
        {
            this.dBName = dbName;
            // 替换sql脚本为能重复执行
            originalSql = ReplaceSqlScript(originalSql);

            return originalSql;
        }

        /// <summary>
        /// 替换mysql的脚本,改为能重复执行
        /// </summary>
        /// <param name="originalSql"></param>
        /// <returns></returns>
        private string ReplaceSqlScript(string originalSql)
        {
            // 1> 在头部追加数据库导入语句
            String useStr = String.Format("SET FOREIGN_KEY_CHECKS = 0;{0}{0}use {1};", Const.LINE_FEED_SYMBOL_ORIGINAL, this.dBName);
            originalSql = originalSql.Replace("SET FOREIGN_KEY_CHECKS = 0;", useStr);

            // 2> 注释掉:DROP TABLE IF EXISTS
            originalSql = originalSql.Replace("DROP TABLE IF EXISTS", "-- DROP TABLE IF EXISTS");

            // 3> 修改创建表时验证表是否存在
            originalSql = originalSql.Replace("CREATE TABLE `", "CREATE TABLE IF NOT EXISTS `");
            return originalSql;
        }

        public override String GetBeforeConfigSql()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(String.Format("SET NAMES utf8mb4; {0}", Const.LINE_FEED_SYMBOL_ORIGINAL));
            sb.Append(String.Format("SET FOREIGN_KEY_CHECKS = 0; {0}", Const.LINE_FEED_SYMBOL_ORIGINAL));
            sb.Append(String.Format("use {0}; {1}", dBName, Const.LINE_FEED_SYMBOL_ORIGINAL));
            return sb.ToString();
        }

        public override String GetSqlDataBlockInsert(InsertStatementModel insertStatementModel, bool isNewTable, int insertIndex)
        {
            // 1> 获取必要的参数
            String tableName = insertStatementModel.tableName.Replace("`", "");
            String fieldString = insertStatementModel.fieldString;
            String lineValue = insertStatementModel.lineValue;
            String firstField = insertStatementModel.firstField;
            Object firstValue = insertStatementModel.firstValue;

            // 2> 拼接sql语句
            StringBuilder sb = new StringBuilder();
            sb.Append(String.Format("INSERT INTO {0}({1}) ", tableName, fieldString));
            sb.Append(String.Format("SELECT {0} FROM DUAL WHERE NOT EXISTS ", lineValue));
            sb.Append(String.Format("(SELECT * FROM {0} WHERE {1} = {2}); ", tableName, firstField, firstValue));

            return sb.ToString();
        }

    }
}
