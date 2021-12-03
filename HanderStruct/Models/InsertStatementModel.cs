using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBConvertSQL.HanderStruct.Models
{
    /// <summary>
    /// 插入语句模型
    /// </summary>
    public class InsertStatementModel
    {
        /// <summary>
        /// 数据库名称||模式名称
        /// </summary>
        public String databaseName;

        /// <summary>
        /// 表名称
        /// </summary>
        public String tableName;

        /// <summary>
        /// 字段集合字符串
        /// </summary>
        public String fieldString;

        /// <summary>
        /// 行数据
        /// </summary>
        public String lineValue;

        /// <summary>
        /// 第一个字段(当作数据的唯一标识)
        /// </summary>
        public String firstField;

        /// <summary>
        /// 第一个值(当作数据的唯一标识值)
        /// </summary>
        public Object firstValue;

    }
}
