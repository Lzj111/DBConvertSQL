using DBConvertSQL.Common;
using DBConvertSQL.HanderStruct;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DBConvertSQL
{
    public partial class App : Form
    {
        public App()
        {
            InitializeComponent();
        }

        private void App_Load(object sender, EventArgs e)
        {
            // 绑定结构下拉框对象
            BindComboBoxSource(cmb_dbtype);
        }

        #region 操作区域

        /// <summary>
        /// 结构>打开文件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Btn_open_Click(object sender, EventArgs e)
        {
            SetOpenFileDialog(txt_file_path, txt_input);
        }

        /// <summary>
        /// 数据库类型下拉框选则
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Cmb_dbtype_SelectedIndexChanged(object sender, EventArgs e)
        {
            txt_parse.Text = "";
            try
            {
                String selectValue = this.cmb_dbtype.SelectedValue.ToString();
                if (!selectValue.Equals("System.Data.DataRowView"))
                {
                    DBType dBType = (DBType)Enum.Parse(typeof(DBType), selectValue);
                    // 部分数据库默认添加上模式名public
                    switch (dBType)
                    {
                        case DBType.POSTGRESQL:
                        case DBType.KINGBASEESV8:
                            txt_dbname.Text = SqlKeyword.DEFAULT_MODE;
                            break;
                        case DBType.MYSQL:
                        case DBType.DM8:
                        default:
                            txt_dbname.Text = "";
                            break;
                    }
                }
            }
            catch { }
        }

        /// <summary>
        /// 转换结构
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Btn_parse_Click(object sender, EventArgs e)
        {
            String fileContent = TransformBefore(txt_file_path, txt_input);
            if (!String.IsNullOrEmpty(fileContent))
            {
                SqlParseHandler(fileContent, ParseType.STRUCT);
            }
        }

        /// <summary>
        /// 转换数据
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Btn_parse_data_Click(object sender, EventArgs e)
        {
            String fileContent = TransformBefore(txt_file_path, txt_input);
            if (!String.IsNullOrEmpty(fileContent))
            {
                SqlParseHandler(fileContent, ParseType.DATA);
            }
        }

        /// <summary>
        /// 保存为文件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Btn_save_file_Click(object sender, EventArgs e)
        {
            String sqlStr = txt_parse.Text;
            Utils.SaveToFile(sqlStr);
        }

        /// <summary>
        /// 输入框值变化
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Txt_input_TextChanged(object sender, EventArgs e)
        {
            String value = txt_input.Text;

            // 1> 包含创建表结构语法启用结构转换
            btn_parse.Enabled = value.Contains(SqlKeyword.CREATE_STRUCT_SYNTAX);

            // 2> 包含创建数据语法启动数据转换
            btn_parse_data.Enabled = value.Contains(SqlKeyword.CREATE_DATA_SYNTAX);
        }

        #endregion

        #region 工具方法

        /// <summary>
        /// SQL转换处理
        /// </summary>
        /// <param name="filePath"></param>
        private void SqlParseHandler(String fileContent, ParseType parseType = ParseType.STRUCT)
        {
            // 1> 验证
            String dbName = txt_dbname.Text;
            if (String.IsNullOrEmpty(dbName))
            {
                MessageBox.Show("请填写模式名称(数据库名)！");
                return;
            }

            // 2> 显示输入
            ShowText(txt_input, fileContent);

            // 3> 实例化转换类工厂
            DBType dBType = (DBType)Enum.Parse(typeof(DBType), cmb_dbtype.SelectedValue.ToString());
            SqlConvertFactory cp = new SqlConvertFactory(dBType);

            // 4> 转换SQL
            string parseSql = "";
            switch (parseType)
            {
                // 4.1> 转换结构语句
                case ParseType.STRUCT:
                    parseSql = cp.GetParseAfterSql(fileContent, dbName);
                    break;
                // 4.2> 转换数据语句
                case ParseType.DATA:
                    parseSql = cp.GetParseDataAfterSql(fileContent, dbName);
                    break;
                default:
                    break;
            }

            ShowText(txt_parse, parseSql);
        }

        /// <summary>
        /// 展示文本
        /// </summary>
        /// <param name="strLog"></param>
        private void ShowText(TextBox txtBox, string txt)
        {
            if (null == txtBox)
            {
                return;
            }
            txtBox.Clear();
            txtBox.Text = txt;
            txtBox.SelectionStart = 0;
        }

        /// <summary>
        /// 绑定数据类型下拉框
        /// </summary>
        private void BindComboBoxSource(ComboBox cmb)
        {
            DataTable dtSource = new DataTable();
            dtSource.Columns.Add("id");
            dtSource.Columns.Add("value");
            dtSource.Rows.Add(DBType.MYSQL, "MySQL");
            dtSource.Rows.Add(DBType.POSTGRESQL, "PostgreSQL");
            dtSource.Rows.Add(DBType.DM8, "达梦8");
            dtSource.Rows.Add(DBType.KINGBASEESV8, "人大金仓");
            cmb.DataSource = dtSource;
            cmb.ValueMember = "id";
            cmb.DisplayMember = "value";
            // 设置默认选择项，DropDownList会默认选择第一项。
            cmb.SelectedIndex = 0;
        }

        /// <summary>
        /// 打开选择文件对话框通用
        /// </summary>
        /// <param name="filePathTxtBox"></param>
        /// <param name="contentTxtBox"></param>
        private void SetOpenFileDialog(TextBox filePathTxtBox, TextBox contentTxtBox)
        {
            // 定义一个文件打开控件
            OpenFileDialog ofd = new OpenFileDialog();
            // 设置打开对话框的初始目录，默认目录为exe运行文件所在的路径
            ofd.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            // 设置打开对话框的标题
            ofd.Title = "请选择要打开的文件";
            // 设置打开对话框可以多选
            ofd.Multiselect = true;
            // 设置对话框打开的文件类型
            ofd.Filter = "文本文件|*.txt|数据库文件|*.sql";
            // 设置文件对话框当前选定的筛选器的索引
            ofd.FilterIndex = 2;
            // 设置对话框是否记忆之前打开的目录
            ofd.RestoreDirectory = true;
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                // 获取用户选择的文件完整路径
                string filePath = ofd.FileName;
                // 获取对话框中所选文件的文件名和扩展名，文件名不包括路径
                //string fileName = ofd.SafeFileName;
                // 设置文件文本框路径
                filePathTxtBox.Text = filePath;
                String fileContent = Utils.GetFileContent(filePath);
                ShowText(contentTxtBox, fileContent);
            }
        }

        /// <summary>
        /// 切换tab标签页时重置界面
        /// </summary>
        private void ChangeTabSelected()
        {
            // 结构文本框恢复
            txt_dbname.Text = "";
            txt_file_path.Text = "";
            txt_input.Text = "";
            txt_parse.Text = "";
        }

        private String TransformBefore(TextBox txtFilePath, TextBox txtInput)
        {
            // 文件路径
            String filePath = txtFilePath.Text;
            // 文本内容
            String fileContent = txtInput.Text;

            // 文本内容为空时,验证
            if (String.IsNullOrEmpty(fileContent))
            {
                String msg = "请输入SQL脚本内容！";
                // 文件路径为空
                if (String.IsNullOrEmpty(filePath))
                {
                    msg = "请选择SQL脚本文件！";
                }
                MessageBox.Show(msg);
                return null;
            }

            // 有文件路径优先以文件路径
            if (!String.IsNullOrEmpty(filePath))
            {
                fileContent = Utils.GetFileContent(filePath);
            }

            return fileContent;
        }

        #endregion

    }
}
