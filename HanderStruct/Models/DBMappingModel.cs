using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBConvertSQL.HanderStruct.Models
{
    public class MappingModelItem
    {
        /// <summary>
        /// 源数据类型
        /// </summary>
        public string sourceType;

        /// <summary>
        /// 源数据类型是否有长度
        /// </summary>
        public bool isSourceLength;

        /// <summary>
        /// 对应目标数据类型
        /// </summary>
        public string targetType;

        /// <summary>
        /// 目标数据默认长度(可能有精度)
        /// </summary>
        public string targetLength;

        /// <summary>
        /// 目标数据类型是否有长度
        /// </summary>
        public bool isTargetLength;

    }

    /// <summary>
    /// 数据库映射模型
    /// </summary>
    public class DBMappingModel : List<MappingModelItem>
    {

    }
}
