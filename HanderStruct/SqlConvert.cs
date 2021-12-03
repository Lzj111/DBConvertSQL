using DBConvertSQL.Common;
using DBConvertSQL.HanderStruct.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace DBConvertSQL.HanderStruct
{
    /// <summary>
    /// SQL转换基类
    /// </summary>
    public class SqlConvert
    {
        #region 类变量

        /// <summary>
        /// 数据库类型
        /// </summary>
        public DBType dBTypeInstance;
        /// <summary>
        /// 数据映射模型
        /// </summary>
        public DBMappingModel dBMappingModelInstance;
        /// <summary>
        /// 数据库名称
        /// </summary>
        public String dBName;

        #endregion

        #region 处理结构初始化

        /// <summary>
        /// 构造函数
        /// </summary>
        public SqlConvert(DBType dBType)
        {
            GetDBMappingModelInstance(dBType);
        }

        /// <summary>
        /// 获取转换后的Sql脚本
        /// </summary>
        /// <param name="originalSql">原始的sql语句</param>
        /// <param name="dbName">模式(数据库)名称</param>
        /// <returns></returns>
        public virtual String GetSqlParse(String originalSql, String dbName)
        {
            this.dBName = dbName;
            List<TableModel> tableModels = new List<TableModel>();
            // 获取sql块脚本集合
            List<String> sqlBlockStr = Utils.GetSqlOriginalBlockList(originalSql);

            // 循环单个创建语句块
            foreach (String item in sqlBlockStr)
            {
                if (String.IsNullOrEmpty(item))
                {
                    continue;
                }
                TableModel tableModel = this.GetSqlParseTableModelItem(item);
                tableModel.databaseName = dbName;
                tableModels.Add(tableModel);
            }

            // 调用各自数据库的转换
            return GetSqlParseOperation(tableModels);
        }

        /// <summary>
        /// 获取转换操作
        /// </summary>
        /// <param name="tableModels">数据表模型</param>
        /// <returns></returns>
        public virtual String GetSqlParseOperation(List<TableModel> tableModels)
        {
            StringBuilder sbNotes = new StringBuilder();
            // 1> 添加注释说明
            sbNotes.Append(String.Format("/*{0}", Const.LINE_FEED_SYMBOL_ORIGINAL));
            sbNotes.Append(String.Format(" {0} Data Transfer{1}{1}", dBTypeInstance.ToString(), Const.LINE_FEED_SYMBOL_ORIGINAL));
            sbNotes.Append(String.Format(" Table Count  : {0}{1}", tableModels.Count, Const.LINE_FEED_SYMBOL_ORIGINAL));
            sbNotes.Append(String.Format(" Source Schema: {0}{1}", this.dBName, Const.LINE_FEED_SYMBOL_ORIGINAL));
            sbNotes.Append(String.Format(" Date         : {0}{1}", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), Const.LINE_FEED_SYMBOL_ORIGINAL));
            sbNotes.Append(String.Format("*/{0}{0}", Const.LINE_FEED_SYMBOL_ORIGINAL));

            return sbNotes.ToString();
        }

        /// <summary>
        /// 根据源数据类型获取对应关系
        /// </summary>
        /// <param name="sourceType">源数据类型</param>
        /// <returns></returns>
        public virtual MappingModelItem GetMappingModelItemBySourceType(String sourceType)
        {
            MappingModelItem mappingModelItem = null;
            if (dBMappingModelInstance == null)
            {
                throw new ArgumentException("未获取到字段类型映射关系实例！");
            }

            // 循环取相对应的映射关系
            foreach (MappingModelItem item in dBMappingModelInstance)
            {
                if (!String.IsNullOrEmpty(sourceType) && sourceType.Equals(item.sourceType))
                {
                    mappingModelItem = item;
                    break;
                }
            }

            // 如果没有找到对应的映射实例,抛出异常
            if (mappingModelItem == null)
            {
                String errMsg = String.Format("根据源类型:{0}获取映射项失败！", sourceType);
                throw new ArgumentException(errMsg);
            }
            return mappingModelItem;
        }

        /// <summary>
        /// 获取字段的注释(默认是PG内核的注释,如有不同自行从写)
        /// </summary>
        /// <param name="tableModel"></param>
        /// <returns></returns>
        public virtual String GetSqlFieldComment(TableModel tableModel)
        {
            StringBuilder commentSb = new StringBuilder();
            //commentSb.Append(String.Format("-- 增加字段注释{0}", Const.LINE_FEED_SYMBOL_ORIGINAL));

            foreach (FieldModel fieldModel in tableModel.fieldModels)
            {
                // 1> 字段注释为空跳过
                if (String.IsNullOrEmpty(fieldModel.comment))
                {
                    continue;
                }

                // 2> 字段注释不为空生成语句
                commentSb.Append("COMMENT ON COLUMN ");
                commentSb.Append(String.Format("\"{0}\".\"{1}\".\"{2}\" ", this.dBName, tableModel.tableName, fieldModel.fieldName));
                commentSb.Append(String.Format("IS '{0}';{1}", fieldModel.comment, Const.LINE_FEED_SYMBOL_ORIGINAL));
            }

            return commentSb.ToString();
        }

        /// <summary>
        /// 生成创建表的语句(暂时没有相似的初始化结构)
        /// </summary>
        /// <param name="tableModel"></param>
        /// <param name="tableIndex"></param>
        /// <returns></returns>
        public virtual String GetSqlBlockTable(TableModel tableModel, int tableIndex)
        {
            return "";
        }

        /// <summary>
        /// 生成创建字段语句
        /// </summary>
        /// <param name="fieldModel">字段模型</param>
        /// <param name="isEnd">是否是最后一个字段</param>
        /// <returns></returns>
        public virtual String GetSqlBlockField(FieldModel fieldModel, Boolean isEnd)
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
                    fieldSb.Append(String.Format("({0}) ", fieldModel.fieldLength));
                }
                // 源数据类型没有长度,使用目标数据类型的默认长度
                else
                {
                    fieldSb.Append(String.Format("({0}) ", mappingModelItem.targetLength));
                }
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

            // 4> 是否非空
            if (fieldModel.notEmpty)
            {
                fieldSb.Append("NOT NULL ");
            }

            // 5> 是否有默认值
            if (fieldModel.isDefaultValue && fieldModel.defaultValue != null)
            {
                fieldSb.Append(String.Format("{0} {1} ", MySqlKeyword.DEFAULT, fieldModel.defaultValue));
            }

            // 6> 添加字段的扩展属性
            fieldSb.Append(GetSqlBlockFieldExtend(fieldModel));

            // 7> 追加,最后一个字段不加,
            fieldSb.Append(String.Format("{0}{1}", (isEnd ? "" : ","), Const.LINE_FEED_SYMBOL_ORIGINAL));

            return fieldSb.ToString();
        }

        /// <summary>
        /// 获取字段扩展属性(排序。。。)
        /// </summary>
        /// <param name="fieldModel"></param>
        /// <returns></returns>
        public virtual String GetSqlBlockFieldExtend(FieldModel fieldModel)
        {
            StringBuilder sb = new StringBuilder();
            return sb.ToString();
        }

        /// <summary>
        /// 获取表的约束(默认是PG内核的注释,如有不同自行从写)
        /// </summary>
        /// <param name="fieldModel"></param>
        /// <returns></returns>
        public virtual String GetSqlBlockTableConstraint(TableModel tableModel)
        {
            if (tableModel.constraintModels.Count == 0)
            {
                return "";
            }

            StringBuilder sb = new StringBuilder();

            // 添加约束
            foreach (ConstraintModel constraintModel in tableModel.constraintModels)
            {
                StringBuilder constraintBlock = new StringBuilder();
                // 1> 定义
                constraintBlock.Append(String.Format("DO $$ {0}DECLARE {0}", Const.LINE_FEED_SYMBOL_ORIGINAL));
                constraintBlock.Append(String.Format("{0}UNIQUESIZE INTEGER :=(SELECT COUNT(1) FROM pg_constraint WHERE conname='{1}'); {2}",
                    Const.FOUR_SPACES, constraintModel.constraintName, Const.LINE_FEED_SYMBOL_ORIGINAL));
                constraintBlock.Append(String.Format("BEGIN {0}{1}IF UNIQUESIZE = 0 THEN {0}", Const.LINE_FEED_SYMBOL_ORIGINAL, Const.FOUR_SPACES));
                // 2> 添加索引
                constraintBlock.Append(String.Format("{0}{0}ALTER TABLE \"{1}\".\"{2}\" ADD CONSTRAINT \"{3}\" UNIQUE (",
                     Const.FOUR_SPACES, tableModel.databaseName, tableModel.tableName, constraintModel.constraintName));
                // 2.1> 添加索引字段
                List<String> columns = constraintModel.constraintColumns;
                for (int index = 0; index < columns.Count; index++)
                {
                    String splitSymbol = (index < columns.Count - 1) ? "," : "";
                    constraintBlock.Append(String.Format("\"{0}\"{1}", columns[index], splitSymbol));
                }
                constraintBlock.Append(String.Format("); {0}", Const.LINE_FEED_SYMBOL_ORIGINAL));
                // 3> 增加else的结构
                constraintBlock.Append(String.Format("{0}ELSE {1}{0}{0}raise notice '重复添加唯一键:{2}'; {1}", Const.FOUR_SPACES,
                    Const.LINE_FEED_SYMBOL_ORIGINAL, constraintModel.constraintName));
                constraintBlock.Append(String.Format("{0}END IF; {1}END; $$;{1}", Const.FOUR_SPACES, Const.LINE_FEED_SYMBOL_ORIGINAL));
                sb.Append(constraintBlock.ToString());
            }

            return sb.ToString();
        }


        /// <summary>
        /// 获取映射对象模型
        /// </summary>
        /// <param name="dBType"></param>
        /// <returns></returns>
        private DBMappingModel GetDBMappingModelInstance(DBType dBType)
        {
            // 初始化数据库类型
            dBTypeInstance = dBType;
            // 映射文件json地址
            String mappingJsonPath = String.Format(@"./HanderStruct/DBTypeMapping/{0}{1}.json", Const.DB_TYPE_MAPPING_PREFIX, dBType.ToString());
            String json = Utils.GetFileContent(mappingJsonPath);
            // 实例数据映射模型
            dBMappingModelInstance = Utils.Deserialize<DBMappingModel>(json);
            return dBMappingModelInstance;
        }

        /// <summary>
        /// 获取单个创建语句结构
        /// </summary>
        /// <param name="inputStr"></param>
        /// <returns></returns>
        public TableModel GetSqlParseTableModelItem(String inputStr)
        {
            // 分割每一行的语句
            String[] lines = inputStr.Split(Const.LINE_FEED_SYMBOL_REPLACE.ToCharArray());
            TableModel tableModel = new TableModel();
            // 主键
            String primaryKey = this.GetSqlPrimaryKey(lines);

            foreach (string lineItem in lines)
            {
                String lineItemUpper = lineItem.ToUpper();
                // 删除表标识(得到表名称)
                if (lineItemUpper.Contains(MySqlKeyword.DROP_TABLE))
                {
                    String tableName = this.GetSqlParseItemForTableName(lineItem);
                    if (!String.IsNullOrEmpty(tableName))
                    {
                        tableModel.tableName = tableName;
                    }
                }
                // 创建表标识
                else if (lineItemUpper.Contains(MySqlKeyword.CREATE_TABLE))
                {
                    // ...
                }
                // 主键标识
                else if (lineItemUpper.Contains(MySqlKeyword.PRIMARY_KEY))
                {
                    // ...
                }
                // 索引标识
                else if (lineItemUpper.Contains(MySqlKeyword.UNIQUE_INDEX))
                {
                    ConstraintModel constraintModel = this.GetConstraintModelByLine(lineItem);
                    if (null != constraintModel)
                    {
                        tableModel.constraintModels.Add(constraintModel);
                    }
                }
                // 结尾标识
                else if (lineItemUpper.Contains(MySqlKeyword.ENGINE))
                {
                    // ...
                }
                // 字段元素
                else
                {
                    FieldModel fieldModel = this.GetFieldModelByLine(lineItem, primaryKey);
                    if (null != fieldModel)
                    {
                        tableModel.fieldModels.Add(fieldModel);
                    }
                }
            }
            return tableModel;
        }

        /// <summary>
        /// 获取正则表达式内容
        /// </summary>
        /// <param name="lineItem"></param>
        /// <param name="pattern"></param>
        /// <param name="keys"></param>
        /// <returns></returns>
        private Dictionary<String, String> GetLineItemValue(String lineItem, String pattern, List<String> keys)
        {
            Dictionary<String, String> rtnObj = new Dictionary<string, string>();

            // 正则匹配值
            Regex reg = new Regex(pattern, RegexOptions.Multiline | RegexOptions.IgnoreCase);
            Match match = reg.Match(lineItem);
            if (!match.Success)
            {
                return rtnObj;
            }

            // 循环获取匹配内容(没有值存null,避免取值时报key不存在)
            keys.ForEach((key) =>
            {
                String val = match.Groups[key].Value;
                if (!String.IsNullOrEmpty(val))
                {
                    rtnObj.Add(key, val);
                }
                else
                {
                    rtnObj.Add(key, null);
                }
            });
            return rtnObj;
        }

        /// <summary>
        /// 获取正则表达式内容
        /// </summary>
        /// <param name="lineItem"></param>
        /// <param name="pattern"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        private String GetLineItemValue(String lineItem, String pattern, String key)
        {
            List<String> keys = new List<string> { key };
            Dictionary<String, String> rtnObj = this.GetLineItemValue(lineItem, pattern, keys);
            if (rtnObj.ContainsKey(key))
            {
                return rtnObj[key];
            }
            return null;
        }

        /// <summary>
        /// 获取sql块的主键
        /// </summary>
        /// <param name="lines"></param>
        /// <returns></returns>
        private String GetSqlPrimaryKey(String[] lines)
        {
            String primaryKey = "";
            foreach (string lineItem in lines)
            {
                String lineItemUpper = lineItem.ToUpper();
                // 主键
                if (lineItemUpper.Contains(MySqlKeyword.PRIMARY_KEY))
                {
                    String pattern = String.Format(@"{0}(?<primaryKey>\w+){1}", MySqlKeyword.CONTAIN_FIELD_SYMBOL, MySqlKeyword.CONTAIN_FIELD_SYMBOL);
                    primaryKey = this.GetLineItemValue(lineItem, pattern, "primaryKey");
                    break;
                }
            }
            return primaryKey;
        }

        /// <summary>
        /// 根据DROP_TABLE行得到表名称
        /// </summary>
        /// <param name="lineItem"></param>
        /// <returns></returns>
        private String GetSqlParseItemForTableName(String lineItem)
        {
            String key = "tableName";
            String pattern = String.Format(@"{0}(?<{1}>\w+){0}", MySqlKeyword.CONTAIN_FIELD_SYMBOL, key);
            return this.GetLineItemValue(lineItem, pattern, key);
        }

        /// <summary>
        /// 获取字段模型
        /// </summary>
        /// <param name="lineItem"></param>
        /// <param name="primaryKey"></param>
        /// <returns></returns>
        private FieldModel GetFieldModelByLine(String lineItem, String primaryKey)
        {
            FieldModel fieldModel = null;
            String lineItemUpper = lineItem.ToUpper();

            // 1> 获取字段名称，字段类型，字段长度
            List<String> keys = new List<string> { "fieldName", "fieldType", "fieldLength" };
            String pattern = String.Format(@"{0}(?<{1}>\w+){2}\s+(?<{3}>\w+)\((?<{4}>[\w,\s]+?)\)", MySqlKeyword.CONTAIN_FIELD_SYMBOL,
                keys[0], MySqlKeyword.CONTAIN_FIELD_SYMBOL, keys[1], keys[2]);
            Dictionary<String, String> lineObj = this.GetLineItemValue(lineItem, pattern, keys);

            // 如果lineObj为null,表示没有匹配到长度[json,text...]
            if (0 == lineObj.Count)
            {
                pattern = String.Format(@"{0}(?<{1}>\w+){2}\s+(?<{3}>\w+)", MySqlKeyword.CONTAIN_FIELD_SYMBOL,
                  keys[0], MySqlKeyword.CONTAIN_FIELD_SYMBOL, keys[1]);
                lineObj = this.GetLineItemValue(lineItem, pattern, keys);
                if (lineObj.Count == 0)
                {
                    return fieldModel;
                }
            }

            // 2> 设置字段名,字段类型,字段长度
            String fieldName = lineObj[keys[0]];
            String fieldType = lineObj[keys[1]];
            String fieldLength = lineObj[keys[2]];
            // 2.1>有一个为空都跳出去
            if (String.IsNullOrEmpty(fieldName) || String.IsNullOrEmpty(fieldType))
            {
                return fieldModel;
            }

            // 3> 设置字段模型
            fieldModel = new FieldModel();
            fieldModel.fieldName = fieldName;
            fieldModel.fieldType = fieldType;
            if (!String.IsNullOrEmpty(fieldLength))
            {
                fieldModel.fieldLength = fieldLength;
            }
            // 3.1> 是否主键
            if (fieldName == primaryKey)
            {
                fieldModel.isPrimaryKey = true;
                fieldModel.notEmpty = true;
            }
            // 3.2> 设置是否允许为空
            if (lineItemUpper.Contains(MySqlKeyword.NOT_NULL))
            {
                fieldModel.notEmpty = true;
            }
            // 3.3> 设置默认值
            if (lineItemUpper.Contains(MySqlKeyword.DEFAULT))
            {
                fieldModel.isDefaultValue = true;
                // 默认值为NULL
                if (lineItemUpper.Contains(MySqlKeyword.DEFAULT_NULL))
                {
                    fieldModel.defaultValue = null;
                }
                // 取默认值
                else
                {
                    String patternDefault = @".*?DEFAULT\s{1}(?<defaultValue>\d)";
                    String defaultVal = this.GetLineItemValue(lineItem, patternDefault, "defaultValue");
                    int numValue = Int32.Parse(defaultVal);
                    fieldModel.defaultValue = numValue;
                }
            }
            // 3.4> 设置备注
            if (lineItemUpper.Contains(MySqlKeyword.COMMENT))
            {
                String commentKey = "comment";
                String patternComment = String.Format(@".*?COMMENT\s+{0}(?<{1}>.*?){0}", SqlKeyword.SINGLE_QUOTES, commentKey);
                String commentVal = this.GetLineItemValue(lineItem, patternComment, commentKey);
                fieldModel.comment = commentVal;
            }

            return fieldModel;
        }

        /// <summary>
        /// 获取表的约束
        /// </summary>
        /// <param name="lineItem"></param>
        /// <returns></returns>
        private ConstraintModel GetConstraintModelByLine(String lineItem)
        {
            ConstraintModel constraintModel = null;
            String lineItemUpper = lineItem.ToUpper();

            // 1> 匹配约束属性
            List<String> keys = new List<string> { "constraintName", "constraintColumn", "constraintStruct" };
            String patternUniqueIndex = String.Format(@".*?`(?<{0}>\w+)`\((?<{1}>.*?)\)\sUSING\s(?<{2}>\w+)", keys[0], keys[1], keys[2]);
            Dictionary<String, String> lineObj = this.GetLineItemValue(lineItem, patternUniqueIndex, keys);
            if (lineObj.Count == 0)
            {
                return constraintModel;
            }

            // 2> 获取约束属性
            String constraintName = lineObj[keys[0]];
            String constraintColumn = lineObj[keys[1]];
            String constraintStruct = lineObj[keys[2]];
            if (String.IsNullOrEmpty(constraintName) || String.IsNullOrEmpty(constraintColumn))
            {
                return constraintModel;
            }

            // 3> 设置约束实体
            constraintModel = new ConstraintModel();
            // 3.1> 约束名称
            constraintModel.constraintName = constraintName;
            // 3.2> 约束结构
            constraintModel.constraintStruct = constraintStruct;
            // 3.3> 约束列
            String[] columns = constraintColumn.Split(',');
            foreach (String item in columns)
            {
                String afterColunm = item.Replace("`", "").Trim();
                constraintModel.constraintColumns.Add(afterColunm);
            }
            // 3.4> 约束类型(唯一索引)
            if (lineItemUpper.Contains(MySqlKeyword.UNIQUE_INDEX))
            {
                constraintModel.constraintType = MySqlKeyword.UNIQUE_INDEX;
            }

            return constraintModel;
        }

        #endregion

        #region 处理数据初始化

        /// <summary>
        /// 处理数据初始化
        /// </summary>
        /// <param name="originalSql"></param>
        /// <param name="dbName"></param>
        /// <returns></returns>
        public virtual String GetSqlDataParse(String originalSql, String dbName)
        {
            this.dBName = dbName;
            List<InsertStatementModel> insertStatementModels = new List<InsertStatementModel>();
            // 1> 获取sql块脚本集合
            List<String> sqlBlockStr = Utils.GetSqlOriginalDataBlockList(originalSql);

            // 2> 循环插入语句
            foreach (String item in sqlBlockStr)
            {
                if (String.IsNullOrEmpty(item))
                {
                    continue;
                }
                InsertStatementModel tableModel = this.GetSqlDataParseInsertStatementModelItem(item);
                if (null == tableModel)
                {
                    continue;
                }
                insertStatementModels.Add(tableModel);
            }

            // 调用各自数据库的转换
            return GetSqlDataParseOperation(insertStatementModels);
        }

        /// <summary>
        /// 默认数据转换
        /// </summary>
        /// <param name="insertStatementModels"></param>
        /// <returns></returns>
        public virtual String GetSqlDataParseOperation(List<InsertStatementModel> insertStatementModels)
        {
            StringBuilder sb = new StringBuilder();
            // 1> 添加注释说明
            sb.Append(String.Format("/*{0}", Const.LINE_FEED_SYMBOL_ORIGINAL));
            sb.Append(String.Format(" {0} Data Transfer{1}{1}", dBTypeInstance.ToString(), Const.LINE_FEED_SYMBOL_ORIGINAL));
            sb.Append(String.Format(" Statement Count  : {0}{1}", insertStatementModels.Count, Const.LINE_FEED_SYMBOL_ORIGINAL));
            sb.Append(String.Format(" Source Schema: {0}{1}", this.dBName, Const.LINE_FEED_SYMBOL_ORIGINAL));
            sb.Append(String.Format(" Date         : {0}{1}", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), Const.LINE_FEED_SYMBOL_ORIGINAL));
            sb.Append(String.Format("*/{0}{0}", Const.LINE_FEED_SYMBOL_ORIGINAL));

            // 2> 拼接前置插入语句
            sb.Append(this.GetBeforeConfigSql());

            // 3> 添加建表结构
            string beforeTableName = "";
            int index = 0;
            foreach (InsertStatementModel insertStatementModel in insertStatementModels)
            {
                // 是否是新表
                bool isNewTable = (insertStatementModel.tableName != beforeTableName);
                // 新表索引归0
                index = isNewTable ? ++index : index;

                // 插入注释
                if (isNewTable)
                {
                    sb.Append(Const.LINE_FEED_SYMBOL_ORIGINAL);
                    sb.Append(this.GetSqlDataBlockLineNotes(insertStatementModel.tableName, index));
                }

                beforeTableName = insertStatementModel.tableName;
                sb.Append(GetSqlDataBlockInsert(insertStatementModel, isNewTable, index));
                sb.Append(Const.LINE_FEED_SYMBOL_ORIGINAL);
            }
            return sb.ToString();
        }

        /// <summary>
        /// 前置插入语句(可以增加环境...)
        /// </summary>
        /// <returns></returns>
        public virtual String GetBeforeConfigSql()
        {
            return "";
        }

        /// <summary>
        /// 获取sql插入脚本数据(说明：适用于dm8,kes,pgsql)
        /// </summary>
        /// <param name="insertStatementModel"></param>
        /// <param name="isNewTable"></param>
        /// <param name="insertIndex"></param>
        /// <returns></returns>
        public virtual String GetSqlDataBlockInsert(InsertStatementModel insertStatementModel, bool isNewTable, int insertIndex)
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

        /// <summary>
        /// 获取数据插入语句的模型
        /// </summary>
        /// <param name="inputStr"></param>
        /// <returns></returns>
        private InsertStatementModel GetSqlDataParseInsertStatementModelItem(String inputStr)
        {
            InsertStatementModel insertModel = null;
            List<String> keys = new List<string> { "tableName", "fieldString", "lineValue" };

            // 1> 解析语句块
            String pattern = @"\s?INSERT INTO `?(?<tableName>\w+){1}`?\((?<fieldString>.*?)\) VALUES \((?<lineValue>.*?){1}\);";
            Dictionary<String, String> lineObj = this.GetLineItemValue(inputStr, pattern, keys);
            if (lineObj.Count == 0)
            {
                return null;
            }

            // 2> 获取插入语句的表名称,字段,行数据
            String tableName = lineObj[keys[0]];
            String fieldString = lineObj[keys[1]];
            String lineValue = lineObj[keys[2]];
            if (String.IsNullOrEmpty(tableName) || String.IsNullOrEmpty(fieldString) || String.IsNullOrEmpty(lineValue))
            {
                return null;
            }

            // 3> 获取字段数据标识
            String firstField = fieldString.Split(',')[0].Replace("`", "");
            Object firstValue = lineValue.Split(',')[0];


            // 4> 赋值模型
            insertModel = new InsertStatementModel
            {
                databaseName = dBName,
                tableName = tableName,
                fieldString = fieldString,
                lineValue = lineValue,
                firstField = firstField,
                firstValue = firstValue
            };

            return insertModel;
        }

        /// <summary>
        /// 获取每个表的插入数据注释说明
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        private String GetSqlDataBlockLineNotes(String tableName, int index)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(String.Format("-- ----------------------------{0}", Const.LINE_FEED_SYMBOL_ORIGINAL));
            sb.Append(String.Format("-- {0}> Table data for {1}; Schema {2} {3}", index, tableName, dBName, Const.LINE_FEED_SYMBOL_ORIGINAL));
            sb.Append(String.Format("-- ----------------------------{0}", Const.LINE_FEED_SYMBOL_ORIGINAL));
            return sb.ToString();
        }

        #endregion
    }
}
