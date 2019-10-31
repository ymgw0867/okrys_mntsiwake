using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace mntsiwake
{
    public partial class frmShiwakeAdd : Form
    {
        public frmShiwakeAdd()
        {
            InitializeComponent();

            _kingaku = 0;
            _addStatus = 0;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void frmShiwakeAdd_Load(object sender, EventArgs e)
        {
            this.Text = "仕訳パターン登録【" + global.pblComName + "】";
        }

        private void btnPath_Click(object sender, EventArgs e)
        {
            string f = string.Empty;

            // 出力ファイル
            _outFile = null;

            // ダイアログボックスの表示
            saveFileDialog1.Title = "仕訳パターン出力先";
            saveFileDialog1.Filter = "データファイル(*.csv)|*.csv";
            saveFileDialog1.InitialDirectory = global.WorkDir + global.pblComName + @"\" + global.DIR_TEMP;
            saveFileDialog1.FileName = string.Empty;
            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                f = saveFileDialog1.FileName;
                Stream str;
                str = saveFileDialog1.OpenFile();
                _outFile = new StreamWriter(str, System.Text.Encoding.GetEncoding(932));

                // 結果ステータス
                _addStatus = 1;

                // 金額ステータス
                if (radioButton1.Checked) _kingaku = 1;
                else _kingaku = 0;

                this.Close();
            }
            else return;
        }

        // フォーム結果
        public int _kingaku { get; set; }
        public int _addStatus { get; set; }
        public StreamWriter _outFile { get; set; }

    }
}
