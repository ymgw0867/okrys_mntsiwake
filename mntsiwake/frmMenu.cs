using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using mntsiwake.OCR;
using System.Data.SqlClient;

namespace mntsiwake
{
    public partial class frmMenu : Form
    {
        public frmMenu()
        {
            InitializeComponent();

            //インストールディレクトリ
            global.WorkDir = Properties.Settings.Default.instDir;

            //フォルダ作成
            System.IO.Directory.CreateDirectory(global.WorkDir + global.DIR_OK);
            System.IO.Directory.CreateDirectory(global.WorkDir + global.DIR_INCSV);
            //System.IO.Directory.CreateDirectory(global.WorkDir + global.DIR_BREAK);
            //System.IO.Directory.CreateDirectory(global.WorkDir + global.DIR_NG);
            System.IO.Directory.CreateDirectory(global.WorkDir + global.DIR_OCRREAD);
            System.IO.Directory.CreateDirectory(global.WorkDir + global.DIR_READ);
            //System.IO.Directory.CreateDirectory(global.WorkDir + global.DIR_KATSUJI);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("振替伝票のＯＣＲ処理を実施します。" + Environment.NewLine + "よろしいですか", "伝票確認", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
                return;

            // 伝票スキャン
            this.Hide();
            frmOCR frm = new frmOCR();
            frm.ShowDialog();
            this.Show();
        }

        private void frmMenu_Load(object sender, EventArgs e)
        {
            //ウィンドウズ最小サイズ
            utility.WindowsMinSize(this, this.Size.Width, this.Size.Height);

            //ウィンドウズ最大サイズ
            utility.WindowsMaxSize(this, this.Size.Width, this.Size.Height);
        }

        private void button5_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void frmMenu_FormClosing(object sender, FormClosingEventArgs e)
        {
            this.Dispose();
        }
    }
}
