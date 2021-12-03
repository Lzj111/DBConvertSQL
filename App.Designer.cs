namespace DBConvertSQL
{
    partial class App
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(App));
            this.txt_dbname = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.btn_save_file = new System.Windows.Forms.Button();
            this.cmb_dbtype = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.txt_file_path = new System.Windows.Forms.TextBox();
            this.txt_parse = new System.Windows.Forms.TextBox();
            this.btn_parse = new System.Windows.Forms.Button();
            this.txt_input = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.btn_open = new System.Windows.Forms.Button();
            this.btn_parse_data = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // txt_dbname
            // 
            this.txt_dbname.Location = new System.Drawing.Point(658, 14);
            this.txt_dbname.Name = "txt_dbname";
            this.txt_dbname.Size = new System.Drawing.Size(128, 21);
            this.txt_dbname.TabIndex = 34;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.BackColor = System.Drawing.Color.Transparent;
            this.label3.Location = new System.Drawing.Point(599, 18);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(65, 12);
            this.label3.TabIndex = 33;
            this.label3.Text = "数据库名：";
            // 
            // btn_save_file
            // 
            this.btn_save_file.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btn_save_file.Location = new System.Drawing.Point(954, 13);
            this.btn_save_file.Name = "btn_save_file";
            this.btn_save_file.Size = new System.Drawing.Size(75, 24);
            this.btn_save_file.TabIndex = 32;
            this.btn_save_file.Text = "保存为文件";
            this.btn_save_file.UseVisualStyleBackColor = true;
            this.btn_save_file.Click += new System.EventHandler(this.Btn_save_file_Click);
            // 
            // cmb_dbtype
            // 
            this.cmb_dbtype.Cursor = System.Windows.Forms.Cursors.Hand;
            this.cmb_dbtype.FormattingEnabled = true;
            this.cmb_dbtype.Location = new System.Drawing.Point(509, 14);
            this.cmb_dbtype.Name = "cmb_dbtype";
            this.cmb_dbtype.Size = new System.Drawing.Size(84, 20);
            this.cmb_dbtype.TabIndex = 31;
            this.cmb_dbtype.SelectedIndexChanged += new System.EventHandler(this.Cmb_dbtype_SelectedIndexChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.BackColor = System.Drawing.Color.Transparent;
            this.label2.Location = new System.Drawing.Point(437, 17);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(77, 12);
            this.label2.TabIndex = 30;
            this.label2.Text = "数据库类型：";
            // 
            // txt_file_path
            // 
            this.txt_file_path.Location = new System.Drawing.Point(115, 13);
            this.txt_file_path.Name = "txt_file_path";
            this.txt_file_path.Size = new System.Drawing.Size(217, 21);
            this.txt_file_path.TabIndex = 29;
            // 
            // txt_parse
            // 
            this.txt_parse.Location = new System.Drawing.Point(529, 47);
            this.txt_parse.Multiline = true;
            this.txt_parse.Name = "txt_parse";
            this.txt_parse.ReadOnly = true;
            this.txt_parse.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.txt_parse.Size = new System.Drawing.Size(500, 574);
            this.txt_parse.TabIndex = 28;
            // 
            // btn_parse
            // 
            this.btn_parse.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btn_parse.Location = new System.Drawing.Point(792, 13);
            this.btn_parse.Name = "btn_parse";
            this.btn_parse.Size = new System.Drawing.Size(75, 24);
            this.btn_parse.TabIndex = 27;
            this.btn_parse.Text = "转换结构";
            this.btn_parse.UseVisualStyleBackColor = true;
            this.btn_parse.Click += new System.EventHandler(this.Btn_parse_Click);
            // 
            // txt_input
            // 
            this.txt_input.Location = new System.Drawing.Point(12, 47);
            this.txt_input.Multiline = true;
            this.txt_input.Name = "txt_input";
            this.txt_input.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.txt_input.Size = new System.Drawing.Size(500, 574);
            this.txt_input.TabIndex = 26;
            this.txt_input.TextChanged += new System.EventHandler(this.Txt_input_TextChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.BackColor = System.Drawing.Color.Transparent;
            this.label1.Location = new System.Drawing.Point(12, 18);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(107, 12);
            this.label1.TabIndex = 25;
            this.label1.Text = "请选择Mysql脚本：";
            // 
            // btn_open
            // 
            this.btn_open.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btn_open.Location = new System.Drawing.Point(337, 11);
            this.btn_open.Name = "btn_open";
            this.btn_open.Size = new System.Drawing.Size(85, 25);
            this.btn_open.TabIndex = 24;
            this.btn_open.Text = "选择文件";
            this.btn_open.UseVisualStyleBackColor = true;
            this.btn_open.Click += new System.EventHandler(this.Btn_open_Click);
            // 
            // btn_parse_data
            // 
            this.btn_parse_data.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btn_parse_data.Location = new System.Drawing.Point(873, 13);
            this.btn_parse_data.Name = "btn_parse_data";
            this.btn_parse_data.Size = new System.Drawing.Size(75, 24);
            this.btn_parse_data.TabIndex = 35;
            this.btn_parse_data.Text = "转换数据";
            this.btn_parse_data.UseVisualStyleBackColor = true;
            this.btn_parse_data.Click += new System.EventHandler(this.Btn_parse_data_Click);
            // 
            // App
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1041, 633);
            this.Controls.Add(this.btn_parse_data);
            this.Controls.Add(this.txt_dbname);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.btn_save_file);
            this.Controls.Add(this.cmb_dbtype);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.txt_file_path);
            this.Controls.Add(this.txt_parse);
            this.Controls.Add(this.btn_parse);
            this.Controls.Add(this.txt_input);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btn_open);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "App";
            this.Text = "SQL脚本转换工具";
            this.Load += new System.EventHandler(this.App_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txt_dbname;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button btn_save_file;
        private System.Windows.Forms.ComboBox cmb_dbtype;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txt_file_path;
        private System.Windows.Forms.TextBox txt_parse;
        private System.Windows.Forms.Button btn_parse;
        private System.Windows.Forms.TextBox txt_input;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btn_open;
        private System.Windows.Forms.Button btn_parse_data;
    }
}

