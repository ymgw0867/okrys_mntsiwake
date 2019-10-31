using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace mntsiwake
{
    public partial class frmFilSelect2 : Form
    {
        string _sPath = string.Empty;

        public frmFilSelect2(string sPath)
        {
            InitializeComponent();
            _sPath = sPath;
            _getPath = string.Empty;
        }

        private void frmFilSelect2_Load(object sender, EventArgs e)
        {
            //ウィンドウズ最小サイズ
            utility.WindowsMinSize(this, this.Size.Width, this.Size.Height);

            //ウィンドウズ最大サイズ
            utility.WindowsMaxSize(this, this.Size.Width, this.Size.Height);

            //中断伝票をリスト表示
            GridViewSetting(dg1);
            GridViewShowData(dg1);

            //ボタンの表示状態
            button3.Enabled = true;
            button4.Enabled = true;

            //終了タグ初期化
            Tag = string.Empty;

            // 表示会社名
            lblComName.Text = global.pblComName;
        }

        /// <summary>
        /// データグリッドビューの定義を行います
        /// </summary>
        /// <param name="tempDGV">データグリッドビューオブジェクト</param>
        public void GridViewSetting(DataGridView tempDGV)
        {
            try
            {
                //フォームサイズ定義

                // 列スタイルを変更する

                tempDGV.EnableHeadersVisualStyles = false;

                // 列ヘッダー表示位置指定
                tempDGV.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.BottomCenter;

                // 列ヘッダーフォント指定
                tempDGV.ColumnHeadersDefaultCellStyle.Font = new Font("ＭＳ Ｐゴシック", 9, FontStyle.Regular);

                // データフォント指定
                tempDGV.DefaultCellStyle.Font = new Font("ＭＳ Ｐゴシック", (float)9.5, FontStyle.Regular);

                // 行の高さ
                tempDGV.ColumnHeadersHeight = 18;
                tempDGV.RowTemplate.Height = 18;

                // 全体の高さ
                tempDGV.Height = 180;

                // 奇数行の色
                //tempDGV.AlternatingRowsDefaultCellStyle.BackColor = Color.Lavender;

                // 各列幅指定
                tempDGV.Columns.Add("col1", "No");
                tempDGV.Columns.Add("col2", "中断時刻");
                tempDGV.Columns.Add("col3", "");

                tempDGV.Columns[2].Visible = false;

                tempDGV.Columns[0].Width = 40;
                tempDGV.Columns[1].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;

                tempDGV.Columns[0].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

                // 行ヘッダを表示しない
                tempDGV.RowHeadersVisible = false;

                // 選択モード
                tempDGV.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
                tempDGV.MultiSelect = false;

                // 編集不可とする
                tempDGV.ReadOnly = true;

                // 追加行表示しない
                tempDGV.AllowUserToAddRows = false;

                // データグリッドビューから行削除を禁止する
                tempDGV.AllowUserToDeleteRows = false;

                // 手動による列移動の禁止
                tempDGV.AllowUserToOrderColumns = false;

                // 列サイズ変更禁止
                tempDGV.AllowUserToResizeColumns = false;

                // 行サイズ変更禁止
                tempDGV.AllowUserToResizeRows = false;

                // 行ヘッダーの自動調節
                //tempDGV.RowHeadersWidthSizeMode = DataGridViewRowHeadersWidthSizeMode.AutoSizeToAllHeaders;

            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "エラーメッセージ", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// グリッドビューへ会社情報を表示する
        /// </summary>
        /// <param name="sConnect">接続文字列</param>
        /// <param name="tempDGV">DataGridViewオブジェクト名</param>
        private void GridViewShowData(DataGridView tempDGV)
        {
            try
            {
                //グリッドビューに表示する
                int iX = 0;
                tempDGV.RowCount = 0;

                // データグリッドにフォルダ名を表示する
                foreach (var iPath in System.IO.Directory.GetDirectories(_sPath))
                {                    
                    tempDGV.Rows.Add();
                    tempDGV[0, iX].Value = tempDGV.RowCount.ToString();
                    tempDGV[1, iX].Value = System.IO.Path.GetFileName(iPath);
                    tempDGV[2, iX].Value = iPath;
                    iX++;
                }

                tempDGV.CurrentCell = null;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "エラー", MessageBoxButtons.OK);
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (dg1.SelectedRows.Count == 0)
            {
                MessageBox.Show("伝票が選択されていません。", "中断伝票未選択", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

            if (MessageBox.Show("中断処理データを読み込みます。よろしいですか？", "確認", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No) return;
            
            // 中断処理パス取得
            _getPath = dg1[2, dg1.SelectedRows[0].Index].Value.ToString();

            this.Close();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        // 選択されたパス
        public string _getPath { get; set; }
    }
}
