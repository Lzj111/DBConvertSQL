using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

/// <summary>
/// 通用静态方法
/// </summary>
namespace DBConvertSQL.Common
{
    public static class Utils
    {
        /// <summary>
        /// 获取文件内容
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static String GetFileContent(String filePath)
        {
            using (FileStream fsRead = new FileStream(filePath, FileMode.OpenOrCreate, FileAccess.Read))
            {
                //定义二进制数组
                byte[] buffer = new byte[1024 * 1024 * 5];
                //从流中读取字节
                int r = fsRead.Read(buffer, 0, buffer.Length);
                String content = Encoding.UTF8.GetString(buffer, 0, r);
                return content;
            }
        }

        /// <summary>
        /// 获取创建结构语句块集合
        /// </summary>
        /// <param name="fileContent"></param>
        /// <returns></returns>
        public static List<String> GetSqlOriginalBlockList(String fileContent)
        {
            List<String> listContent = new List<string>();

            // 将内容的换行符回车符号替换掉,便于匹配
            fileContent = fileContent.Replace(Const.LINE_FEED_SYMBOL_ORIGINAL, Const.LINE_FEED_SYMBOL_REPLACE);
            // 定义创建语句规则匹配的正则表达式
            String pattern = @"DROP TABLE IF(.*?)= DYNAMIC;";
            Regex reg = new Regex(pattern, RegexOptions.Multiline | RegexOptions.IgnoreCase);

            // 匹配取创建语句
            // Match match = reg.Match(fileContent);
            MatchCollection matchs = reg.Matches(fileContent);
            foreach (Match item in matchs)
            {
                if (item.Success)
                {
                    // 将匹配到的项目添加到集合中
                    String value = String.IsNullOrEmpty(item.Value) ? "" : item.Value;
                    //value = value.Replace(Const.LINE_FEED_SYMBOL_REPLACE, Const.LINE_FEED_SYMBOL_ORIGINAL);
                    listContent.Add(value);
                }
            }

            return listContent;
        }

        /// <summary>
        /// 把json字符串转成对象
        /// </summary>
        /// <typeparam name="T">对象</typeparam>
        /// <param name="data">json字符串</param>
        public static T Deserialize<T>(string data)
        {
            return JsonConvert.DeserializeObject<T>(data);
        }

        /// <summary>
        /// 获取数据初始化语句块集合
        /// </summary>
        /// <param name="fileContent"></param>
        /// <returns></returns>
        public static List<String> GetSqlOriginalDataBlockList(String fileContent)
        {
            List<String> listContent = new List<string>();
            // 获取insert的多行语句
            String pattern = @"INSERT INTO.*?\);";
            Regex reg = new Regex(pattern, RegexOptions.Multiline | RegexOptions.IgnoreCase);
            MatchCollection matchs = reg.Matches(fileContent);
            foreach (Match item in matchs)
            {
                if (item.Success)
                {
                    // 将匹配到的项目添加到集合中
                    String value = String.IsNullOrEmpty(item.Value) ? "" : item.Value;
                    //value = value.Replace(Const.LINE_FEED_SYMBOL_REPLACE, Const.LINE_FEED_SYMBOL_ORIGINAL);
                    listContent.Add(value);
                }
            }
            return listContent;
        }

        /// <summary>
        /// 保存为文件
        /// </summary>
        /// <param name="fileContent"></param>
        public static void SaveToFile(String fileContent)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "SQL脚本|*.sql|文本文件|*.txt";
            sfd.FilterIndex = 1;
            sfd.RestoreDirectory = true;
            // 文件默认名
            sfd.FileName = "SQL脚本.sql";
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                String fileName = sfd.FileName.ToString();
                // 向文件中写入内容
                File.WriteAllText(fileName, fileContent);
            }
        }

    }
}
