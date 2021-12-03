using DBConvertSQL.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBConvertSQL.HanderStruct
{
    /// <summary>
    /// SQL转换工厂类
    /// </summary>
    public class SqlConvertFactory
    {
        readonly SqlConvert sqlConvert = null;

        /// <summary>
        /// 实例化转换类
        /// </summary>
        /// <param name="dBType"></param>
        public SqlConvertFactory(DBType dBType)
        {
            switch (dBType)
            {
                case DBType.MYSQL:
                    sqlConvert = new SqlConvertMySQL(dBType);
                    break;
                case DBType.POSTGRESQL:
                    sqlConvert = new SqlConvertPGSQL(dBType);
                    break;
                case DBType.KINGBASEESV8:
                    sqlConvert = new SqlConvertKESV8(dBType);
                    break;
                case DBType.DM8:
                    sqlConvert = new SqlConvertDM8(dBType);
                    break;
                default:
                    sqlConvert = new SqlConvert(dBType);
                    break;
            }
        }

        /// <summary>
        /// 获取转换后的结构初始化语句
        /// </summary>
        /// <param name="originalSql">原始的sql脚本</param>
        /// <param name="dbName"></param>
        /// <returns></returns>
        public String GetParseAfterSql(String originalSql, String dbName)
        {
            return sqlConvert.GetSqlParse(originalSql, dbName);
        }

        /// <summary>
        /// 获取转换后的数据初始化语句
        /// </summary>
        /// <param name="originalSql"></param>
        /// <param name="dbName"></param>
        /// <returns></returns>
        public String GetParseDataAfterSql(String originalSql, String dbName)
        {
            return sqlConvert.GetSqlDataParse(originalSql, dbName);
        }
    }
}
