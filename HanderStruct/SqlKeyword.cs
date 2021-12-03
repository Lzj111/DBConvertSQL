using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBConvertSQL.HanderStruct
{
    /// <summary>
    /// 通用的标识词
    /// </summary>
    public static class SqlKeyword
    {
        /// <summary>
        /// 单引号
        /// </summary>
        public static string SINGLE_QUOTES = "'";

        /// <summary>
        /// 双引号
        /// </summary>
        public static string DOUBLE_QUOTES = "\"";

        /// <summary>
        /// 默认模式
        /// </summary>
        public static string DEFAULT_MODE = "public";

        /// <summary>
        /// 创建表结构的语法标识
        /// </summary>
        public static string CREATE_STRUCT_SYNTAX = "CREATE TABLE";

        /// <summary>
        /// 创建表数据的语法标识
        /// </summary>
        public static string CREATE_DATA_SYNTAX = "INSERT INTO";
    }

    /// <summary>
    /// Mysql的标识词
    /// </summary>
    public static class MySqlKeyword
    {
        /// <summary>
        /// 删除表关键字
        /// </summary>
        public static string DROP_TABLE = "DROP TABLE IF EXISTS";

        /// <summary>
        /// 创建表关键字
        /// </summary>
        public static string CREATE_TABLE = "CREATE TABLE";

        /// <summary>
        /// 主键关键字
        /// </summary>
        public static string PRIMARY_KEY = "PRIMARY KEY";

        /// <summary>
        /// 引擎(验证尾部语句标识)
        /// </summary>
        public static string ENGINE = "ENGINE";

        /// <summary>
        /// 包含字段的符号
        /// </summary>
        public static string CONTAIN_FIELD_SYMBOL = "`";

        /// <summary>
        /// 字段的备注
        /// </summary>
        public static string COMMENT = "COMMENT";

        /// <summary>
        /// 不能为空
        /// </summary>
        public static string NOT_NULL = "NOT NULL";

        /// <summary>
        /// 是否有默认值
        /// </summary>
        public static string DEFAULT = "DEFAULT";

        /// <summary>
        /// 默认值是null
        /// </summary>
        public static string DEFAULT_NULL = "DEFAULT NULL";

        /// <summary>
        /// 空
        /// </summary>
        public static string NULL = "NULL";

        /// <summary>
        /// 约束(唯一索引)
        /// </summary>
        public static string UNIQUE_INDEX = "UNIQUE INDEX";

    }
}
