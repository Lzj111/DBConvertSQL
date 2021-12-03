using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBConvertSQL.HanderStruct
{
    /// <summary>
    /// 表模型
    /// </summary>
    public class TableModel
    {
        /// <summary>
        /// 数据库名称
        /// </summary>
        public String databaseName;

        /// <summary>
        /// 表名称
        /// </summary>
        public String tableName;

        /// <summary>
        /// 字段集合
        /// </summary>
        public List<FieldModel> fieldModels = new List<FieldModel>();

        /// <summary>
        /// 约束集合
        /// </summary>
        public List<ConstraintModel> constraintModels = new List<ConstraintModel>();
    }

    /// <summary>
    /// 字段模型
    /// </summary>
    public class FieldModel
    {
        /// <summary>
        /// 是否是主键(默认false)
        /// </summary>
        public Boolean isPrimaryKey = false;

        /// <summary>
        /// 字段名称
        /// </summary>
        public String fieldName;

        /// <summary>
        /// 字段类型
        /// </summary>
        public String fieldType;

        /// <summary>
        /// 字段长度(可能有精度)
        /// </summary>
        public String fieldLength;

        /// <summary>
        /// 是否有长度
        /// </summary>
        public Boolean isLength = true;

        /// <summary>
        /// 非空
        /// </summary>
        public Boolean notEmpty = false;

        /// <summary>
        /// 是否有默认值
        /// </summary>
        public Boolean isDefaultValue;

        /// <summary>
        /// 默认值
        /// </summary>
        public Object defaultValue;

        /// <summary>
        /// 注释
        /// </summary>
        public String comment;
    }

    /// <summary>
    /// 约束
    /// </summary>
    public class ConstraintModel
    {
        /// <summary>
        /// 约束名称
        /// </summary>
        public String constraintName;

        /// <summary>
        /// 约束列
        /// </summary>
        public List<String> constraintColumns = new List<string>();

        /// <summary>
        /// 约束类型
        /// </summary>
        public String constraintType;

        /// <summary>
        /// 索引结构(BTREE,HASH)
        /// </summary>
        public String constraintStruct;
    }
}
