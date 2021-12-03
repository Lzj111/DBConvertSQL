using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBConvertSQL.Common
{
    /// <summary>
    /// 一些公用到的常量
    /// </summary>
    public static class Const
    {
        /// <summary>
        /// 换行符原始
        /// </summary>
        public static String LINE_FEED_SYMBOL_ORIGINAL = "\r\n";

        /// <summary>
        /// 换行符替换文本
        /// </summary>
        public static String LINE_FEED_SYMBOL_REPLACE = "$";

        /// <summary>
        /// 映射JSON文件前缀
        /// </summary>
        public static String DB_TYPE_MAPPING_PREFIX = "MappingFor";

        /// <summary>
        /// 4个空格
        /// </summary>
        public static String FOUR_SPACES = "    ";

        /// <summary>
        /// 2个空格
        /// </summary>
        public static String TWO_SPACES = "  ";
    }

    /// <summary>
    /// 数据库类型
    /// </summary>
    public enum DBType
    {
        /// <summary>
        /// mysql数据源
        /// </summary>
        MYSQL = 0,
        /// <summary>
        /// postgresql数据源
        /// </summary>
        POSTGRESQL = 1,
        /// <summary>
        /// 人大金仓
        /// </summary>
        KINGBASEESV8 = 2,
        /// <summary>
        /// 达梦8
        /// </summary>
        DM8 = 3,
    }

    /// <summary>
    /// 转换类型
    /// </summary>
    public enum ParseType
    {
        /// <summary>
        /// 结构
        /// </summary>
        STRUCT = 0,
        /// <summary>
        /// 数据
        /// </summary>
        DATA = 1,
    }
}
