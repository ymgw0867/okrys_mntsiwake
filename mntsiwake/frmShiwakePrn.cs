using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Data.OleDb;
using System.Data.SqlClient;
using Microsoft.VisualBasic;
using Leadtools.Codecs;
using Leadtools;
using Leadtools.WinForms;
using Leadtools.ImageProcessing;
using GrapeCity.Win.MultiRow;
using Excel = Microsoft.Office.Interop.Excel;

namespace mntsiwake
{
    public partial class frmShiwakePrn : Form
    {
        Boolean bCngFlag = false;

        public frmShiwakePrn()
        {
            InitializeComponent();
        }

        private void Base_Load(object sender, EventArgs e)
        {
            //ウィンドウズ最小サイズ
            utility.WindowsMinSize(this, this.Size.Width, this.Size.Height);

            //ウィンドウズ最大サイズ
            utility.WindowsMaxSize(this, this.Size.Width, this.Size.Height);

            //インストールディレクトリを取得する
            global.WorkDir = Properties.Settings.Default.instDir;
            if (global.WorkDir == "")
            {
                MessageBox.Show("インストールディレクトリが取得できませんでした" + Environment.NewLine + "プログラムを終了します", "レジストリ未登録", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                Environment.Exit(0);
            }

            //ファイル有無チェック
            start s = new start();

            //設定データ取得
            s.InitialLoad(global.WorkDir);

            //会社選択画面
            Form frm = new frmComSelect(global.OCRMODE);
            frm.ShowDialog();

            //会社情報が存在しない場合はアプリケーションを終了する
            if (global.pblDbName == string.Empty)　return;

            // 会社フォルダ(仕訳パターン出力先）を作成します
            if (!System.IO.Directory.Exists(global.WorkDir + global.pblComName + @"\" + global.DIR_TEMP))
            {
                System.IO.Directory.CreateDirectory(global.WorkDir + global.pblComName + @"\" + global.DIR_TEMP);
            }

            //マスター情報取得
            LoadMaster();

            // 画面初期化
            DispClr();
        }

        private void DispClr()
        {
            //キャプション
            this.Text = Application.ProductName + "Ver " + Application.ProductVersion + "  【" + global.pblComName + "】";

            //フォームタグ初期化
            this.Tag = string.Empty;

            //multirow編集モード
            gcMultiRow1.EditMode = EditMode.EditProgrammatically;

            // 元号表示
            this.lblGengo.Text = company.Reki;

            // 日付
            txtYear.Text = (DateTime.Now.Year - Properties.Settings.Default.hosei).ToString();
            txtMonth.Text = DateTime.Now.Month.ToString();
            txtDay.Text = DateTime.Now.Day.ToString();

            //伝票行表示
            this.gcMultiRow1.AllowUserToAddRows = false;                    // 手動による行追加を禁止する
            this.gcMultiRow1.AllowUserToDeleteRows = false;                 // 手動による行削除を禁止する
            this.gcMultiRow1.Rows.Clear();                                  // 行数をクリア
            this.gcMultiRow1.RowCount = global.MAXGYOU_PRN;                 // 行数を設定
            this.gcMultiRow1.RowsDefaultCellStyle.ForeColor = Color.Blue;   // テキストカラーの設定
            this.gcMultiRow1.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(200, 249, 196);  //Alternating表示
            //this.gcMultiRow1.ScrollBars = ScrollBars.Both;
            //this.gcMultiRow1.ScrollMode = ScrollMode.Row;
        }

        /// <summary>
        /// 各種マスターをロードする
        /// </summary>
        private void LoadMaster()
        {
            //ステータス
            global.MASTERLOAD_STATUS = 1;

            //オーナーフォームを無効にする
            this.Enabled = false;

            //プログレスバーを表示する
            frmPrg frmP = new frmPrg();
            frmP.Owner = this;
            frmP.Show();

            //会社データ
            frmP.Text = "会社データロード中";
            frmP.progressValue = 10;
            frmP.ProgressStep();

            GridViewSetting_Company(fgCom);     //グリッドビュー設定
            GridViewShow_company(fgCom);        //グリッドにデータ表示

            //勘定科目
            frmP.Text = "勘定科目をロード中";
            frmP.progressValue = 40;
            frmP.ProgressStep();

            GridViewSetting_Kamoku(fgKamoku);   //グリッドビュー設定
            GridViewShow_Kamoku(fgKamoku);      //グリッドにデータ表示

            //補助科目
            GridViewSetting_Hojo(fgHojo);       //グリッドビュー設定

            //部門
            frmP.Text = "部門データをロード中";
            frmP.progressValue = 50;
            frmP.ProgressStep();

            GridViewSetting_Bumon(fgBumon);     //グリッドビュー設定
            GridViewShow_Bumon(fgBumon);        //グリッドにデータ表示

            //税区分
            frmP.Text = "税区分をロード中";
            frmP.progressValue = 70;
            frmP.ProgressStep();

            GridViewSetting_Tax(fgTax);         //グリッドビュー設定
            GridViewShow_Tax(fgTax);            //グリッドにデータ表示

            //税処理
            frmP.Text = "税処理をロード中";
            frmP.progressValue = 90;
            frmP.ProgressStep();

            GridViewSetting_TaxMas(fgTaxMas);   //グリッドビュー設定
            GridViewShow_TaxMas(fgTaxMas);      //グリッドにデータ表示

            //事業区分
            frmP.Text = "事業区分をロード中";
            frmP.progressValue = 95;
            frmP.ProgressStep();

            //グリッドビュー設定
            GridViewSetting_Jigyo(fgJigyo);

            //グリッドにデータ表示
            GridViewShow_Jigyo(fgJigyo);

            // いったんオーナーをアクティブにする
            this.Activate();

            // 進行状況ダイアログを閉じる
            frmP.Close();

            // オーナーのフォームを有効に戻す
            this.Enabled = true;

            //ステータスオフ
            global.MASTERLOAD_STATUS = 0;
        }

        /// <summary>
        /// データグリッドビューの定義を行います
        /// </summary>
        /// <param name="tempDGV">データグリッドビューオブジェクト</param>
        private void GridViewSetting_Company(DataGridView tempDGV)
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
                //tempDGV.Height = 180;

                // 奇数行の色
                //tempDGV.AlternatingRowsDefaultCellStyle.BackColor = Color.Lavender;

                // 各列幅指定
                tempDGV.Columns.Add("col1", "項目名");
                tempDGV.Columns.Add("col2", "摘要");

                //tempDGV.Columns[4].Visible = false; //データベース名は非表示
                //tempDGV.Columns[5].Visible = false; //税区分は非表示

                tempDGV.Columns[0].Width = 100;
                //tempDGV.Columns[1].Width = 100;
                tempDGV.Columns[1].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;

                tempDGV.Columns[0].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
                tempDGV.Columns[1].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

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
        private void GridViewShow_company(DataGridView tempDGV)
        {

            string wrkGengou;
            //string wrkKikan;
            string wrkFromYear;
            string wrkFromMonth;
            string wrkFromDay;
            string wrkToYear;
            string wrkToMonth;
            string wrkToDay;
            string wrkKaisi;

            ////////勘定奉行データベースより会社情報を取得する
            //////company cp = new company();
            //////cp.CompDataLoad();

            //会計期間のフォーマット
            //if (company.Hosei != "0")
            if (global.pblReki == global.RWAREKI)  //和暦ならば
            {
                wrkGengou = company.Gengou;
                wrkFromYear = (int.Parse(company.FromYear) - int.Parse(company.Hosei)).ToString();
                wrkFromYear = String.Format(string.Format("{0,2}", int.Parse(wrkFromYear)));
                wrkToYear = (int.Parse(company.ToYear) - int.Parse(company.Hosei)).ToString();
                wrkToYear = String.Format("{0,2}", int.Parse(wrkToYear));
            }
            else
            {
                wrkGengou = "  ";
                wrkFromYear = company.FromYear;
                wrkToYear = company.ToYear;
            }

            wrkFromMonth = String.Format("{0,2}", int.Parse(company.FromMonth));
            wrkFromDay = String.Format("{0,2}", int.Parse(company.FromDay));
            wrkToMonth = String.Format("{0,2}", int.Parse(company.ToMonth));
            wrkToDay = String.Format("{0,2}", int.Parse(company.ToDay));

            //入力開始月フォーマット
            wrkKaisi = int.Parse(company.FromMonth).ToString();
            if (int.Parse(wrkKaisi) > 12) wrkKaisi = (int.Parse(wrkKaisi) - 12).ToString();

            //取得方法追加「税処理を取得」 (v6.0対応)--
            if (global.gsTaxMas.Trim() == "2")
            {
                company.TaxMas = "1";
            }
            else
            {
                company.TaxMas = "0";
            }

            try
            {
                //グリッドビューに表示する
                tempDGV.RowCount = 6;

                //会社名
                tempDGV[0, 0].Value = "会社名";
                tempDGV[1, 0].Value = company.Name;

                //会計期間期首
                tempDGV[0, 1].Value = "会計期間・期首";
                tempDGV[1, 1].Value = wrkFromYear + "年" + wrkFromMonth + "月" + wrkFromDay + "日";

                //会計期間期末
                tempDGV[0, 2].Value = "会計期間・期末";
                tempDGV[1, 2].Value = wrkToYear + "年" + wrkToMonth + "月" + wrkToDay + "日";

                //入力開始月
                tempDGV[0, 3].Value = "入力開始月";
                tempDGV[1, 3].Value = string.Format("{0,2}", int.Parse(wrkKaisi)) + "月";

                //中間期決算
                tempDGV[0, 4].Value = "決算回数";
                if (company.Middle == global.FLGON)
                {
                    tempDGV[1, 4].Value = "する";
                }
                else
                {
                    tempDGV[1, 4].Value = "しない";
                }

                //決算回数
                switch (company.Middle)
                {
                    case "0":
                        tempDGV[1, 4].Value = "年1回";
                        break;

                    case "1":
                        tempDGV[1, 4].Value = "年2回（中間決算）";
                        break;

                    case "2":
                        tempDGV[1, 4].Value = "年4回（四半期決算）";
                        break;

                    default:
                        //////tempDGV[1, 4].Value = "不明";
                        tempDGV[1, 4].Value = "年12回（月次決算）";
                        break;
                }

                //税処理
                tempDGV[0, 5].Value = "税処理";

                if (global.gsTaxMas == "0")
                {
                    tempDGV[1, 5].Value = "税抜別段";
                }
                else
                {
                    tempDGV[1, 5].Value = "税込自動";
                }

                tabData.SelectedIndex = global.TAB_COM_PRN;
                tempDGV.CurrentCell = null;

            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "エラー", MessageBoxButtons.OK);
            }

        }

        /// <summary>
        /// 日付入力範囲の設定
        /// </summary>
        private void SetLimit()
        {
            int wrkLock = int.Parse(company.LmLock);
            int wrkSt = int.Parse(company.LmStSoeji);
            int wrkEd = int.Parse(company.LmEdSoeji);
            int wrkKaisi = int.Parse(company.Kaisi);

            //通常仕訳の入力期間　とりあえずマスターの指定期間を入れておく
            Limit.LimitKikan s = new Limit.LimitKikan();
            s.initialDataSet();

            //最初の四半期決算期間
            Limit.BfQuaKessanDate1 kessan1 = new Limit.BfQuaKessanDate1();
            kessan1.GetKessanDate();

            //2度目の四半期決算期間
            Limit.BfQuaKessanDate1 kessan2 = new Limit.BfQuaKessanDate1();
            kessan2.GetKessanDate();

            //3度目の四半期決算期間
            Limit.BfQuaKessanDate1 kessan3 = new Limit.BfQuaKessanDate1();
            kessan3.GetKessanDate();

            //中間期決算期間
            Limit.MidKessanDate midKessan = new Limit.MidKessanDate();
            midKessan.GetKessanDate();

            //元の中間期決算期間
            Limit.BfMidKessan bfmidKessan = new Limit.BfMidKessan();
            bfmidKessan.GetKessanDate();

            //決算期間の取得
            Limit.KessanDate kessan = new Limit.KessanDate();
            kessan.GetKessanDate();

            //元の決算期間の取得
            Limit.BfKessan bfkessan = new Limit.BfKessan();
            bfkessan.GetKessanDate();

            //使用可のフラグON
            company.LmFlag = true;
            Limit.LimitKikan.Flag = true;
            Limit.MidKessanDate.Flag = true;
            Limit.KessanDate.Flag = true;

            DateTime sDate;

            switch (wrkLock)
            {
                //入力制限なしの場合
                case 0:
                    //入力開始月が中間期決算月以降の場合
                    if (wrkKaisi > 5) Limit.MidKessanDate.Flag = false; //中間期決算期間の入力を禁止

                    //入力期間表示
                    //Call ShowLimit(pblLimitKikan, 0, pblLimitKikan, 0)
                    break;

                //指定期間を入力禁止
                case 1:
                    if ((0 <= wrkEd) && (wrkEd <= 5))
                    {
                        //通常仕訳　指定期間の翌日から期末まで
                        sDate = DateTime.Parse(company.LmToYear + "/" + company.LmToMonth + "/" + company.LmToDay);
                        Limit.LimitKikan.FromYear = new Limit.GetNextDay(sDate).GetYear();
                        Limit.LimitKikan.FromMonth = new Limit.GetNextDay(sDate).GetMonth();
                        Limit.LimitKikan.FromDay = new Limit.GetNextDay(sDate).GetDay();
                        Limit.LimitKikan.ToYear = company.ToYear;
                        Limit.LimitKikan.ToMonth = company.ToMonth;
                        Limit.LimitKikan.ToDay = company.ToDay;

                        ////入力期間表示
                        //Call ShowLimit(pblLimitKikan, 0, pblKessanDate, 2)
                    }
                    else
                    {
                        if (wrkEd == 6)
                        {
                            //指定期間終了日が、中間期決算期間の終了日より後でなければ通常処理
                            if (JudgeDate(DateTime.Parse(company.LmToYear + "/" + company.LmToMonth + "/" + company.LmToDay), DateTime.Parse(Limit.MidKessanDate.ToYear + "/" + Limit.MidKessanDate.ToMonth + "/" + Limit.MidKessanDate.ToDay)))
                            {
                                //通常仕訳　中間期決算期間の翌日から期末まで
                                sDate = DateTime.Parse(Limit.MidKessanDate.ToYear + "/" + Limit.MidKessanDate.ToMonth + "/" + Limit.MidKessanDate.ToDay);
                                Limit.LimitKikan.FromYear = new Limit.GetNextDay(sDate).GetYear();
                                Limit.LimitKikan.FromMonth = new Limit.GetNextDay(sDate).GetMonth();
                                Limit.LimitKikan.FromDay = new Limit.GetNextDay(sDate).GetDay();
                                Limit.LimitKikan.ToYear = company.ToYear;
                                Limit.LimitKikan.ToMonth = company.ToMonth;
                                Limit.LimitKikan.ToDay = company.ToDay;

                                //中間期決算期間　指定期間の翌日から
                                sDate = DateTime.Parse(company.LmToYear + "/" + company.LmToMonth + "/" + company.LmToDay);
                                Limit.MidKessanDate.FromYear = new Limit.GetNextDay(sDate).GetYear();
                                Limit.MidKessanDate.FromMonth = new Limit.GetNextDay(sDate).GetMonth();
                                Limit.MidKessanDate.FromDay = new Limit.GetNextDay(sDate).GetDay();

                                //' 入力期間表示
                                //Call ShowLimit(pblMidKessanDate, 1, pblKessanDate, 2)
                            }
                            else
                            {
                                //通常仕訳　指定期間の翌日から期末まで
                                sDate = DateTime.Parse(company.LmToYear + "/" + company.LmToMonth + "/" + company.LmToDay);
                                Limit.LimitKikan.FromYear = new Limit.GetNextDay(sDate).GetYear();
                                Limit.LimitKikan.FromMonth = new Limit.GetNextDay(sDate).GetMonth();
                                Limit.LimitKikan.FromDay = new Limit.GetNextDay(sDate).GetDay();
                                Limit.LimitKikan.ToYear = company.ToYear;
                                Limit.LimitKikan.ToMonth = company.ToMonth;
                                Limit.LimitKikan.ToDay = company.ToDay;

                                //中間期決算を使用禁止
                                Limit.MidKessanDate.Flag = false;

                                ////入力期間表示
                                //Call ShowLimit(wrkNextDay, 0, pblKessanDate, 2)

                            }
                        }
                        else
                        {
                            if (7 <= wrkEd && wrkEd <= 12)
                            {
                                //通常仕訳　指定期間の翌日から期末まで
                                sDate = DateTime.Parse(company.LmToYear + "/" + company.LmToMonth + "/" + company.LmToDay);
                                Limit.LimitKikan.FromYear = new Limit.GetNextDay(sDate).GetYear();
                                Limit.LimitKikan.FromMonth = new Limit.GetNextDay(sDate).GetMonth();
                                Limit.LimitKikan.FromDay = new Limit.GetNextDay(sDate).GetDay();
                                Limit.LimitKikan.ToYear = company.ToYear;
                                Limit.LimitKikan.ToMonth = company.ToMonth;
                                Limit.LimitKikan.ToDay = company.ToDay;

                                //中間期決算を使用禁止
                                Limit.MidKessanDate.Flag = false;

                                ////入力期間表示
                                //Call ShowLimit(pblLimitKikan, 0, pblKessanDate, 2)
                            }
                            else
                            {
                                if (wrkEd == 13)
                                {
                                    //通常仕訳の使用禁止
                                    Limit.LimitKikan.Flag = false;

                                    //中間期決算の使用禁止
                                    Limit.MidKessanDate.Flag = false;

                                    //指定範囲末と期末が同じ場合
                                    if (company.LmToDay == company.ToDay)
                                    {
                                        //決算の使用禁止
                                        Limit.KessanDate.Flag = false;
                                    }
                                    else
                                    {
                                        //決算期間　指定期間の翌日から
                                        sDate = DateTime.Parse(company.LmToYear + "/" + company.LmToMonth + "/" + company.LmToDay);
                                        Limit.KessanDate.FromYear = new Limit.GetNextDay(sDate).GetYear();
                                        Limit.KessanDate.FromMonth = new Limit.GetNextDay(sDate).GetMonth();
                                        Limit.KessanDate.FromDay = new Limit.GetNextDay(sDate).GetDay();
                                    }

                                    ////入力期間表示
                                    //Call ShowLimit(pblKessanDate, 2, pblKessanDate, 2)
                                }
                            }
                        }
                    }
                    break;

                //指定期間のみ入力許可
                case 2:
                    if (0 <= wrkSt && wrkSt <= 5)
                    {
                        if (0 <= wrkEd && wrkEd <= 5)
                        {
                            //中間期決算の使用禁止
                            Limit.MidKessanDate.Flag = false;

                            //決算の使用禁止
                            Limit.KessanDate.Flag = false;

                            ////入力期間表示
                            //Call ShowLimit(pblLimitKikan, 0, pblLimitKikan, 0)
                        }
                        else
                        {
                            if (wrkEd == 6)
                            {
                                //通常仕訳　現時点の中間期決算末まで
                                Limit.LimitKikan.ToYear = Limit.MidKessanDate.ToYear;
                                Limit.LimitKikan.ToMonth = Limit.MidKessanDate.ToMonth;
                                Limit.LimitKikan.ToDay = Limit.MidKessanDate.ToDay;

                                //指定期間終了日が、中間期決算期間の終了日より後でなければ通常処理
                                if (JudgeDate(DateTime.Parse(company.LmToYear + "/" + company.LmToMonth + "/" + company.LmToDay), DateTime.Parse(Limit.MidKessanDate.FromYear + "/" + Limit.MidKessanDate.FromMonth + "/" + Limit.MidKessanDate.FromDay)))
                                {
                                    //中間期決算期間　指定期間まで
                                    Limit.MidKessanDate.ToYear = company.LmToYear;
                                    Limit.MidKessanDate.ToMonth = company.LmToMonth;
                                    Limit.MidKessanDate.ToDay = company.LmToDay;
                                }

                                //決算の使用禁止
                                Limit.KessanDate.Flag = false;

                                ////入力期間表示
                                //Call ShowLimit(pblLimitKikan, 0, pblMidKessanDate, 1)
                            }
                            else
                            {
                                if (7 <= wrkEd && wrkEd <= 12)
                                {
                                    //決算の使用禁止
                                    Limit.KessanDate.Flag = false;

                                    ////入力期間表示
                                    //Call ShowLimit(pblLimitKikan, 0, pblLimitKikan, 0)
                                }
                                else
                                {
                                    if (wrkEd == 13)
                                    {
                                        //通常仕訳　期末まで
                                        Limit.LimitKikan.ToYear = company.ToYear;
                                        Limit.LimitKikan.ToMonth = company.ToMonth;
                                        Limit.LimitKikan.ToDay = company.ToDay;

                                        //決算期間　指定期間まで
                                        Limit.KessanDate.ToYear = company.LmToYear;
                                        Limit.KessanDate.ToMonth = company.LmToMonth;
                                        Limit.KessanDate.ToDay = company.LmToDay;

                                        ////入力期間表示
                                        //Call ShowLimit(pblLimitKikan, 0, pblKessanDate, 2)
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        if (wrkSt == 6)
                        {
                            if (wrkEd == 6)
                            {
                                //通常仕訳の使用禁止
                                Limit.LimitKikan.Flag = false;

                                //指定期間開始日が、中間期決算期間の開始日より前でなければ通常処理
                                if (JudgeDate(DateTime.Parse(Limit.MidKessanDate.FromYear + "/" +
                                                             Limit.MidKessanDate.FromMonth + "/" +
                                                             Limit.MidKessanDate.FromDay),
                                              DateTime.Parse(company.LmFromYear + "/" +
                                                             company.LmFromMonth + "/" +
                                                             company.LmFromDay)))
                                {
                                    //中間期決算期間の開始日 = 指定期間開始日
                                    Limit.MidKessanDate.FromYear = company.LmFromYear;
                                    Limit.MidKessanDate.FromMonth = company.LmFromMonth;
                                    Limit.MidKessanDate.FromDay = company.LmFromDay;
                                }

                                //指定期間終了日が、中間期決算期間の終了日より後でなければ通常処理
                                if (JudgeDate(DateTime.Parse(company.LmToYear + "/" +
                                                             company.LmToMonth + "/" +
                                                             company.LmToDay),
                                              DateTime.Parse(Limit.MidKessanDate.ToYear + "/" +
                                                             Limit.MidKessanDate.ToMonth + "/" +
                                                             Limit.MidKessanDate.ToDay)))
                                {

                                    //中間期決算期間の終了日 = 指定期間終了日
                                    Limit.MidKessanDate.ToYear = company.LmToYear;
                                    Limit.MidKessanDate.ToMonth = company.LmToMonth;
                                    Limit.MidKessanDate.ToDay = company.LmToDay;

                                }

                                //決算の使用禁止
                                Limit.KessanDate.Flag = false;

                                ////入力期間表示
                                //Call ShowLimit(pblMidKessanDate, 1, pblMidKessanDate, 1)
                            }
                        }
                        else
                        {
                            if (7 <= wrkEd && wrkEd <= 12)
                            {
                                //通常仕訳　中間期決算期間の翌日から指定期間末まで
                                sDate = DateTime.Parse(company.LmToYear + "/" + company.LmToMonth + "/" + company.LmToDay);
                                Limit.LimitKikan.FromYear = new Limit.GetNextDay(sDate).GetYear();
                                Limit.LimitKikan.FromMonth = new Limit.GetNextDay(sDate).GetMonth();
                                Limit.LimitKikan.FromDay = new Limit.GetNextDay(sDate).GetDay();

                                //指定期間開始日が、中間期決算期間の開始日より前でなければ通常処理
                                if (JudgeDate(DateTime.Parse(Limit.MidKessanDate.FromYear + "/" +
                                                             Limit.MidKessanDate.FromMonth + "/" +
                                                             Limit.MidKessanDate.FromDay),
                                              DateTime.Parse(company.LmFromYear + "/" +
                                                             company.LmFromMonth + "/" +
                                                             company.LmFromDay)))
                                {
                                    //中間期決算期間の開始日 = 指定期間開始日
                                    Limit.MidKessanDate.FromYear = company.LmFromYear;
                                    Limit.MidKessanDate.FromMonth = company.LmFromMonth;
                                    Limit.MidKessanDate.FromDay = company.LmFromDay;
                                }

                                //決算の使用禁止
                                Limit.KessanDate.Flag = false;

                                ////入力期間表示
                                //Call ShowLimit(pblMidKessanDate, 1, pblLimitKikan, 0)
                            }
                            else
                            {
                                if (wrkEd == 13)
                                {
                                    //通常仕訳　中間期決算期間の翌日から期末まで
                                    sDate = DateTime.Parse(Limit.MidKessanDate.ToYear + "/" +
                                                           Limit.MidKessanDate.ToMonth + "/" +
                                                           Limit.MidKessanDate.ToDay);

                                    Limit.LimitKikan.FromYear = new Limit.GetNextDay(sDate).GetYear();
                                    Limit.LimitKikan.FromMonth = new Limit.GetNextDay(sDate).GetMonth();
                                    Limit.LimitKikan.FromDay = new Limit.GetNextDay(sDate).GetDay();
                                    Limit.LimitKikan.ToYear = company.ToYear;
                                    Limit.LimitKikan.ToMonth = company.ToMonth;
                                    Limit.LimitKikan.ToDay = company.ToDay;

                                    //指定期間開始日が、中間期決算期間の開始日より前でなければ通常処理
                                    if (JudgeDate(DateTime.Parse(Limit.MidKessanDate.FromYear + "/" +
                                                                 Limit.MidKessanDate.FromMonth + "/" +
                                                                 Limit.MidKessanDate.FromDay),
                                                  DateTime.Parse(company.LmFromYear + "/" +
                                                                 company.LmFromMonth + "/" +
                                                                 company.LmFromDay)))
                                    {
                                        //中間期決算期間　指定期間から
                                        Limit.MidKessanDate.FromYear = company.LmFromYear;
                                        Limit.MidKessanDate.FromMonth = company.LmFromMonth;
                                        Limit.MidKessanDate.FromDay = company.LmFromDay;
                                    }

                                    //決算期間　指定期間まで
                                    Limit.KessanDate.ToYear = company.LmToYear;
                                    Limit.KessanDate.ToMonth = company.LmToMonth;
                                    Limit.KessanDate.ToDay = company.LmToDay;

                                    ////入力期間表示
                                    //Call ShowLimit(pblMidKessanDate, 1, pblKessanDate, 2)

                                }
                                else
                                {
                                    if (7 <= wrkSt && wrkSt <= 12)
                                    {
                                        if (7 <= wrkEd && wrkEd <= 12)
                                        {
                                            //中間期決算の使用禁止
                                            Limit.MidKessanDate.Flag = false;

                                            //決算の使用禁止
                                            Limit.KessanDate.Flag = false;

                                            ////入力期間表示
                                            //Call ShowLimit(pblLimitKikan, 0, pblLimitKikan, 0)
                                        }
                                        else
                                        {
                                            if (wrkEd == 13)
                                            {
                                                //通常仕訳　指定期間開始から期末まで
                                                Limit.LimitKikan.ToYear = company.ToYear;
                                                Limit.LimitKikan.ToMonth = company.ToMonth;
                                                Limit.LimitKikan.ToDay = company.ToDay;

                                                //中間期決算の使用禁止
                                                Limit.MidKessanDate.Flag = false;

                                                //決算期間　指定期間まで
                                                Limit.KessanDate.ToYear = company.LmToYear;
                                                Limit.KessanDate.ToMonth = company.LmToMonth;
                                                Limit.KessanDate.ToDay = company.LmToDay;

                                                ////入力期間表示
                                                //Call ShowLimit(pblLimitKikan, 0, pblKessanDate, 2)
                                            }
                                            else
                                            {
                                                if (wrkSt == 13 && wrkEd == 13)
                                                {
                                                    //通常仕訳の使用禁止
                                                    Limit.LimitKikan.Flag = false;

                                                    //中間期決算の使用禁止
                                                    Limit.MidKessanDate.Flag = false;

                                                    //指定期間開始日が、中間期決算期間の開始日より前でなければ通常処理
                                                    if (JudgeDate(DateTime.Parse(Limit.KessanDate.FromYear + "/" +
                                                                                 Limit.KessanDate.FromMonth + "/" +
                                                                                 Limit.KessanDate.FromDay),
                                                                  DateTime.Parse(company.LmFromYear + "/" +
                                                                                 company.LmFromMonth + "/" +
                                                                                 company.LmFromDay)))
                                                    {
                                                        //決算期間 = 指定期間
                                                        Limit.KessanDate.FromYear = company.LmFromYear;
                                                        Limit.KessanDate.FromMonth = company.LmFromMonth;
                                                        Limit.KessanDate.FromDay = company.LmFromDay;
                                                        Limit.KessanDate.StSoeji = company.LmStSoeji;
                                                        Limit.KessanDate.ToYear = company.LmToYear;
                                                        Limit.KessanDate.ToMonth = company.LmToMonth;
                                                        Limit.KessanDate.ToDay = company.LmToDay;
                                                        Limit.KessanDate.EdSoeji = company.LmEdSoeji;
                                                        Limit.KessanDate.Flag = company.LmFlag;
                                                        Limit.KessanDate.Lock = company.LmLock;

                                                    }
                                                    ////入力期間表示
                                                    //Call ShowLimit(pblKessanDate, 2, pblKessanDate, 2)
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }

                    break;


                default:
                    break;
            }
        }

        /// <summary>
        /// 勘定科目データグリッドビューの定義を行います
        /// </summary>
        /// <param name="tempDGV">科目データグリッドビューオブジェクト</param>
        private void GridViewSetting_Kamoku(DataGridView tempDGV)
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
                //tempDGV.Height = 180;

                // 奇数行の色
                //tempDGV.AlternatingRowsDefaultCellStyle.BackColor = Color.Lavender;

                // 各列幅指定
                tempDGV.Columns.Add("col1", "コード");
                tempDGV.Columns.Add("col2", "勘定科目名");
                tempDGV.Columns.Add("col3", "勘定科目内部コード");
                tempDGV.Columns.Add("col4", "");
                tempDGV.Columns.Add("col5", "");

                tempDGV.Columns[2].Visible = false; //データベース名は非表示
                tempDGV.Columns[3].Visible = false; //データベース名は非表示
                tempDGV.Columns[4].Visible = false; //税区分は非表示

                tempDGV.Columns[0].Width = 60;
                //tempDGV.Columns[1].Width = 100;
                tempDGV.Columns[1].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;

                tempDGV.Columns[0].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                tempDGV.Columns[1].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

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
        /// グリッドビューへ勘定科目を表示する
        /// </summary>
        /// <param name="tempDGV">DataGridViewオブジェクト名</param>
        private void GridViewShow_Kamoku(DataGridView tempDGV)
        {
            //勘定奉行データベース接続文字列を取得する
            string sc = utility.GetDBConnect(global.pblDbName);

            //勘定奉行データベースへ接続する
            SqlControl.DataControl dCon = new SqlControl.DataControl(sc);

            //科目データ取得
            //データリーダーを取得する
            SqlDataReader dR;
            
            //string sqlSTRING = "SELECT sUcd,sNcd,sNm,tiIsTrk,tiIsZei FROM wkskm01 WHERE tiIsTrk = 1 ORDER BY sUcd";

            string sqlSTRING = string.Empty;
            sqlSTRING += "SELECT tbAccountItem.AccountItemID, tbAccountItem.AccountItemCode, tbAccountItem.AccountItemName, ";
            sqlSTRING += "tbAccountItem.IsUse, tbAccountItemAndConsumptionTaxDivisionRelation.AutomaticCalculationTax ";
            sqlSTRING += "FROM tbAccountItem inner join tbAccountItemAndConsumptionTaxDivisionRelation ";
            sqlSTRING += "on tbAccountItem.AccountItemID = tbAccountItemAndConsumptionTaxDivisionRelation.AccountItemID ";
            sqlSTRING += "WHERE (tbAccountItem.IsUse = 1) and (tbAccountItem.AccountingPeriodID = " + global.pblAccPID + ") ";
            sqlSTRING += "ORDER BY tbAccountItem.AccountItemCode";

            dR = dCon.free_dsReader(sqlSTRING);

            try
            {
                int iX = 0;

                //グリッドビューに表示する
                tempDGV.RowCount = 0;

                while (dR.Read())
                {
                    tempDGV.Rows.Add();

                    //コード
                    if (dR["AccountItemCode"].ToString().Trim().Length > global.LEN_KAMOKU)
                    {
                        tempDGV[0, iX].Value = dR["AccountItemCode"].ToString().Trim().Substring(dR["AccountItemCode"].ToString().Trim().Length - global.LEN_KAMOKU, global.LEN_KAMOKU);
                    }
                    else
                    {
                        tempDGV[0, iX].Value = dR["AccountItemCode"].ToString().Trim();
                    }

                    tempDGV[1, iX].Value = dR["AccountItemName"].ToString().Trim();     //名称
                    tempDGV[2, iX].Value = dR["AccountItemID"].ToString().Trim();       //勘定科目内部コード
                    tempDGV[3, iX].Value = dR["IsUse"].ToString();                      //
                    tempDGV[4, iX].Value = dR["AutomaticCalculationTax"].ToString();    //税

                    iX++;
                }

                dR.Close();
                dCon.Close();

                //tabData.SelectedIndex = global.TAB_KAMOKU_PRN;
                tempDGV.CurrentCell = null;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "エラー", MessageBoxButtons.OK);
            }

        }

        /// <summary>
        /// 補助科目データグリッドビューの定義を行います
        /// </summary>
        /// <param name="tempDGV">補助科目データグリッドビューオブジェクト</param>
        private void GridViewSetting_Hojo(DataGridView tempDGV)
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
                //tempDGV.Height = 180;

                // 奇数行の色
                //tempDGV.AlternatingRowsDefaultCellStyle.BackColor = Color.Lavender;

                // 各列幅指定
                tempDGV.Columns.Add("col1", "コード");
                tempDGV.Columns.Add("col2", "補助科目名");

                //tempDGV.Columns[3].Visible = false; //データベース名は非表示
                //tempDGV.Columns[4].Visible = false; //データベース名は非表示
                //tempDGV.Columns[5].Visible = false; //税区分は非表示

                tempDGV.Columns[0].Width = 60;
                //tempDGV.Columns[1].Width = 100;
                tempDGV.Columns[1].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;

                tempDGV.Columns[0].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                tempDGV.Columns[1].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

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
        /// グリッドビューへ補助科目を表示する
        /// </summary>
        /// <param name="tempDGV">DataGridViewオブジェクト名</param>
        private void GridViewShow_Hojo(DataGridView tempDGV, string tempNcd)
        {
            string KanjoCode = string.Empty;
            string sonotaName = string.Empty;

            //勘定奉行データベース接続文字列を取得する
            string sc = utility.GetDBConnect(global.pblDbName);

            //勘定奉行データベースへ接続する
            SqlControl.DataControl dCon = new SqlControl.DataControl(sc);

            //補助データ取得
            //データリーダーを取得する
            SqlDataReader dR;

            //勘定科目取得
            if (utility.NumericCheck(tempNcd))
            {
                KanjoCode = string.Format("{0:D10}", int.Parse(tempNcd));
            }
            else
            {
                KanjoCode = tempNcd;
            }

            string sqlSTRING = string.Empty;

            //補助コードがあるか？
            //////sqlSTRING += "select sNcd,sUcd,sHjoUcd,wkhjm01.sNm ";
            //////sqlSTRING += "from wkskm01 inner join wkhjm01 ";
            //////sqlSTRING += "on wkskm01.sNcd = wkhjm01.sSknNcd ";
            //////sqlSTRING += "where sUcd = '" + string.Format("{0,6}", tempNcd) + "'";

            sqlSTRING += "select tbAccountItem.AccountItemID,tbAccountItem.AccountItemCode,";
            sqlSTRING += "tbAccountItem.AccountItemName,tbSubAccountItem.SubAccountItemCode,";
            sqlSTRING += "tbSubAccountItem.SubAccountItemName ";
            sqlSTRING += "from tbAccountItem inner join tbSubAccountItem ";
            sqlSTRING += "on tbAccountItem.AccountItemID = tbSubAccountItem.AccountItemID ";
            sqlSTRING += "where tbAccountItem.AccountingPeriodID = " + global.pblAccPID + " and ";
            sqlSTRING += "tbAccountItem.AccountItemCode = '" + KanjoCode + "'";

            dR = dCon.free_dsReader(sqlSTRING);

            try
            {
                int iX = 0;

                //グリッドビューに表示する
                tempDGV.RowCount = 0;

                while (dR.Read())
                {
                    //最初のデータがコード「0」のときスキップする
                    if (iX == 0 && dR["SubAccountItemCode"].ToString().Trim() == "0000000000")
                    {
                        sonotaName = dR["SubAccountItemName"].ToString().Trim();
                    }
                    else
                    {
                        tempDGV.Rows.Add();
                        
                        //コード
                        if (dR["SubAccountItemCode"].ToString().Trim().Length > global.LEN_HOJO)
                        {
                            tempDGV[0, iX].Value = dR["SubAccountItemCode"].ToString().Substring(dR["SubAccountItemCode"].ToString().Length - global.LEN_HOJO, global.LEN_HOJO);
                        }
                        else
                        {
                            tempDGV[0, iX].Value = dR["SubAccountItemCode"].ToString().Trim();
                        }

                        tempDGV[1, iX].Value = dR["SubAccountItemName"].ToString().Trim();  //名称

                        iX++;
                    }
                }

                dR.Close();
                dCon.Close();

                //補助科目があれば"その他"を追加する
                if (iX > 0)
                {
                    tempDGV.Rows.Add();
                    tempDGV[0, iX].Value = "0";
                    tempDGV[1, iX].Value = sonotaName;
                }

                tempDGV.CurrentCell = null;

            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "エラー", MessageBoxButtons.OK);
            }
        }

        /// <summary>
        /// 部門データグリッドビューの定義を行います
        /// </summary>
        /// <param name="tempDGV">部門データグリッドビューオブジェクト</param>
        private void GridViewSetting_Bumon(DataGridView tempDGV)
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
                //tempDGV.Height = 180;

                // 奇数行の色
                //tempDGV.AlternatingRowsDefaultCellStyle.BackColor = Color.Lavender;

                // 各列幅指定
                tempDGV.Columns.Add("col1", "コード");
                tempDGV.Columns.Add("col2", "部門名");

                //tempDGV.Columns[3].Visible = false; //データベース名は非表示
                //tempDGV.Columns[4].Visible = false; //データベース名は非表示
                //tempDGV.Columns[5].Visible = false; //税区分は非表示

                tempDGV.Columns[0].Width = 60;
                //tempDGV.Columns[1].Width = 100;
                tempDGV.Columns[1].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;

                tempDGV.Columns[0].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                tempDGV.Columns[1].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

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
        /// グリッドビューへ部門を表示する
        /// </summary>
        /// <param name="tempDGV">DataGridViewオブジェクト名</param>
        private void GridViewShow_Bumon(DataGridView tempDGV)
        {
            //勘定奉行データベース接続文字列を取得する
            string sc = utility.GetDBConnect(global.pblDbName);

            //勘定奉行データベースへ接続する
            SqlControl.DataControl dCon = new SqlControl.DataControl(sc);

            //部門データ取得
            //データリーダーを取得する
            OleDbDataReader dR;

            //////string sqlSTRING = "SELECT sUcd,sNm FROM wkbnm01 ORDER BY sUcd";
            string sqlSTRING = string.Empty;
            sqlSTRING += "select DepartmentID, DepartmentCode, DepartmentName from tbDepartment ";
            sqlSTRING += "order by DepartmentCode";

            dR = dCon.free_dsReader(sqlSTRING);

            try
            {
                int iX = 0;
                string sSonota = string.Empty;
                global.pblBumonFlg = false;

                //グリッドビューに表示する
                tempDGV.RowCount = 0;

                while (dR.Read())
                {
                    if (dR["DepartmentCode"].ToString() != "000000000000000")    //その他以外
                    {
                        tempDGV.Rows.Add();
                        //コード
                        if (dR["DepartmentCode"].ToString().Trim().Length > global.LEN_BUMON)
                        {
                            tempDGV[0, iX].Value = dR["DepartmentCode"].ToString().Trim().Substring(dR["DepartmentCode"].ToString().Trim().Length - global.LEN_BUMON, global.LEN_BUMON);
                        }
                        else
                        {
                            tempDGV[0, iX].Value = dR["DepartmentCode"].ToString().Trim();
                        }

                        tempDGV[1, iX].Value = dR["DepartmentName"].ToString().Trim();      //名称

                        iX++;

                        if (global.pblBumonFlg == false) global.pblBumonFlg = true;
                    }
                    else
                    {
                        sSonota = dR["DepartmentName"].ToString().Trim();     //名称
                    }
                }

                dR.Close();

                //その他取得
                sqlSTRING = string.Empty;
                sqlSTRING += "select DepartmentID,DepartmentCode,DepartmentName from tbDepartment ";
                sqlSTRING += "where DepartmentCode = '000000000000000' ";
                sqlSTRING += "order by DepartmentCode";

                dR = dCon.free_dsReader(sqlSTRING);

                while (dR.Read())
                {
                    sSonota = dR["DepartmentName"].ToString().Trim();     //名称
                }

                dR.Close();
                dCon.Close();

                //部門データありなら最終行に「その他」追加
                if (global.pblBumonFlg == true)
                {
                    tempDGV.Rows.Add();
                    tempDGV[0, iX].Value = "0";         //コード
                    tempDGV[1, iX].Value = sSonota;     //名称
                }

                tabData.SelectedIndex = global.TAB_TAXBUMON_PRN;
                tempDGV.CurrentCell = null;

            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "エラー", MessageBoxButtons.OK);
            }

        }

        /// <summary>
        /// 税区分データグリッドビューの定義を行います
        /// </summary>
        /// <param name="tempDGV">税区分データグリッドビューオブジェクト</param>
        private void GridViewSetting_Tax(DataGridView tempDGV)
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
                //tempDGV.Height = 180;

                // 奇数行の色
                //tempDGV.AlternatingRowsDefaultCellStyle.BackColor = Color.Lavender;

                // 各列幅指定
                tempDGV.Columns.Add("col1", "コード");
                tempDGV.Columns.Add("col2", "税区分名");

                //tempDGV.Columns[3].Visible = false; //データベース名は非表示
                //tempDGV.Columns[4].Visible = false; //データベース名は非表示
                //tempDGV.Columns[5].Visible = false; //税区分は非表示

                tempDGV.Columns[0].Width = 60;
                //tempDGV.Columns[1].Width = 100;
                tempDGV.Columns[1].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;

                tempDGV.Columns[0].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                tempDGV.Columns[1].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

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
        /// グリッドビューへ税区分を表示する
        /// </summary>
        /// <param name="tempDGV">DataGridViewオブジェクト名</param>
        private void GridViewShow_Tax(DataGridView tempDGV)
        {
            //勘定奉行データベース接続文字列を取得する
            string sc = utility.GetDBConnect(global.pblDbName);

            //勘定奉行データベースへ接続する
            SqlControl.DataControl dCon = new SqlControl.DataControl(sc);

            //税区分データ取得
            //データリーダーを取得する
            OleDbDataReader dR;
            //////string sqlSTRING = "SELECT tiZeiCd,sZeiNm FROM wktax01 ORDER BY tiZeiCd";
            string sqlSTRING = string.Empty;
            sqlSTRING += "select TaxDivisionCode,TaxDivisionName from tbTaxDivision ";
            sqlSTRING += "WHERE AccountingPeriodID = " + global.pblAccPID + " ";
            sqlSTRING += "ORDER BY TaxDivisionCode";

            dR = dCon.free_dsReader(sqlSTRING);

            try
            {
                int iX = 0;

                //グリッドビューに表示する
                tempDGV.RowCount = 0;

                while (dR.Read())
                {
                    tempDGV.Rows.Add();
                    tempDGV[0, iX].Value = dR["TaxDivisionCode"].ToString().Trim();    //コード
                    tempDGV[1, iX].Value = dR["TaxDivisionName"].ToString().Trim();     //名称

                    iX++;
                }

                dR.Close();
                dCon.Close();

                tabData.SelectedIndex = global.TAB_TAXBUMON_PRN;
                tempDGV.CurrentCell = null;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "エラー", MessageBoxButtons.OK);
            }
        }

        /// <summary>
        /// 税処理データグリッドビューの定義を行います
        /// </summary>
        /// <param name="tempDGV">税処理データグリッドビューオブジェクト</param>
        private void GridViewSetting_TaxMas(DataGridView tempDGV)
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
                //tempDGV.Height = 180;

                // 奇数行の色
                //tempDGV.AlternatingRowsDefaultCellStyle.BackColor = Color.Lavender;

                // 各列幅指定
                tempDGV.Columns.Add("col1", "コード");
                tempDGV.Columns.Add("col2", "税処理名");

                //tempDGV.Columns[3].Visible = false; //データベース名は非表示
                //tempDGV.Columns[4].Visible = false; //データベース名は非表示
                //tempDGV.Columns[5].Visible = false; //税区分は非表示

                tempDGV.Columns[0].Width = 60;
                //tempDGV.Columns[1].Width = 100;
                tempDGV.Columns[1].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;

                tempDGV.Columns[0].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                tempDGV.Columns[1].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

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
        /// グリッドビューへ税処理を表示する
        /// </summary>
        /// <param name="tempDGV">DataGridViewオブジェクト名</param>
        private void GridViewShow_TaxMas(DataGridView tempDGV)
        {
            try
            {
                //グリッドビューに表示する
                tempDGV.RowCount = 3;

                //消費税計算区分をセット
                tempDGV[0, 0].Value = "0";
                tempDGV[1, 0].Value = "しない";
                tempDGV[0, 1].Value = "1";
                tempDGV[1, 1].Value = "税抜金額からの計算";
                tempDGV[0, 2].Value = "2";
                tempDGV[1, 2].Value = "税込金額からの計算";

                tabData.SelectedIndex = global.TAB_TAXBUMON_PRN;
                tempDGV.CurrentCell = null;

            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "エラー", MessageBoxButtons.OK);
            }

        }

        /// <summary>
        /// 事業区分データグリッドビューの定義を行います
        /// </summary>
        /// <param name="tempDGV">事業区分データグリッドビューオブジェクト</param>
        private void GridViewSetting_Jigyo(DataGridView tempDGV)
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
                //tempDGV.Height = 180;

                // 奇数行の色
                //tempDGV.AlternatingRowsDefaultCellStyle.BackColor = Color.Lavender;

                // 各列幅指定
                tempDGV.Columns.Add("col1", "コード");
                tempDGV.Columns.Add("col2", "事業区分名");

                //tempDGV.Columns[3].Visible = false; //データベース名は非表示
                //tempDGV.Columns[4].Visible = false; //データベース名は非表示
                //tempDGV.Columns[5].Visible = false; //税区分は非表示

                tempDGV.Columns[0].Width = 60;
                //tempDGV.Columns[1].Width = 100;
                tempDGV.Columns[1].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;

                tempDGV.Columns[0].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                tempDGV.Columns[1].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

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
        /// グリッドビューへ事業区分名を表示する
        /// </summary>
        /// <param name="tempDGV">DataGridViewオブジェクト名</param>
        private void GridViewShow_Jigyo(DataGridView tempDGV)
        {
            try
            {
                //グリッドビューに表示する
                tempDGV.RowCount = 5;

                //消費税計算区分をセット
                tempDGV[0, 0].Value = "1";
                tempDGV[1, 0].Value = "第１種（卸売業）";
                tempDGV[0, 1].Value = "2";
                tempDGV[1, 1].Value = "第２種（小売業）";
                tempDGV[0, 2].Value = "3";
                tempDGV[1, 2].Value = "第３種（製造業）";
                tempDGV[0, 3].Value = "4";
                tempDGV[1, 3].Value = "第４種（その他）";
                tempDGV[0, 4].Value = "5";
                tempDGV[1, 4].Value = "第５種（サービス業）";

                tabData.SelectedIndex = global.TAB_TAXBUMON_PRN;
                tempDGV.CurrentCell = null;

            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "エラー", MessageBoxButtons.OK);
            }

        }

        /// <summary>
        /// 摘要データグリッドビューの定義を行います
        /// </summary>
        /// <param name="tempDGV">摘要データグリッドビューオブジェクト</param>
        private void GridViewSetting_Tekiyo(DataGridView tempDGV)
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
                //tempDGV.Height = 180;

                // 奇数行の色
                //tempDGV.AlternatingRowsDefaultCellStyle.BackColor = Color.Lavender;

                // 各列幅指定
                tempDGV.Columns.Add("col1", "摘要名");

                //tempDGV.Columns[0].Width = 200;
                tempDGV.Columns[0].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;

                tempDGV.Columns[0].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

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
        /// 決算日付と中間決算日付の比較
        /// </summary>
        /// <returns></returns>
        private Boolean JudgeDate(DateTime Date1, DateTime Date2)
        {
            //Date1の日付が後の場合、NG
            if (Date1 >= Date2)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        //伝票ヘッダ部格納
        private void DataGetHead(int iX, string readbuf, string csvf, Entity.InputRecord[] sDenpyo)
        {
            // カンマ区切りで分割して配列に格納する
            string[] stArrayData = readbuf.Split(',');

            //画像ファイル名                   
            sDenpyo[iX].Head.image = stArrayData[1].Trim();                
            //sDenpyo[iX].Head.image = csvf.Replace(".csv", ".tif").Trim();

            //CSVファイル名
            sDenpyo[iX].Head.CsvFile = csvf;

            //年
            sDenpyo[iX].Head.Year = stArrayData[2].Replace("-", string.Empty).Trim();

            //月
            sDenpyo[iX].Head.Month = stArrayData[3].Replace("-", string.Empty).Trim();

            //日
            sDenpyo[iX].Head.Day = stArrayData[4].Replace("-", string.Empty).Trim();

            ////////伝票No.
            //////sDenpyo[iX].Head.DenNo = stArrayData[5].Replace("-", string.Empty).Trim();

            //決算処理フラグ
            sDenpyo[iX].Head.Kessan = stArrayData[5].Trim();

            //複数枚チェック
            sDenpyo[iX].Head.FukusuChk = stArrayData[6].ToString().Trim();

        }

        /// <summary>
        /// 出力用データ初期化
        /// </summary>
        /// <param name="OutData">出力用データ</param>
        private void InitOutRec(Entity.OutputRecord OutData)
        {
            OutData.Kugiri = string.Empty;
            OutData.DenBumon = string.Empty;
            OutData.Date = string.Empty;
            
            OutData.Kari.Bumon = string.Empty;
            OutData.Kari.Kamoku = string.Empty;
            OutData.Kari.Hojo = string.Empty;
            OutData.Kari.TaxKbn = string.Empty;
            OutData.Kari.TaxMas = string.Empty;
            OutData.Kari.Kin = string.Empty;
            OutData.Kari.ProjectCode = string.Empty;
            OutData.Kari.SubProjectCode = string.Empty;
            OutData.Kari.JigyoKbn = string.Empty;

            OutData.Kashi.Bumon = string.Empty;
            OutData.Kashi.Kamoku = string.Empty;
            OutData.Kashi.Hojo = string.Empty;
            OutData.Kashi.TaxKbn = string.Empty;
            OutData.Kashi.TaxMas = string.Empty;
            OutData.Kashi.Kin = string.Empty;
            OutData.Kashi.ProjectCode = string.Empty;
            OutData.Kashi.SubProjectCode = string.Empty;
            OutData.Kashi.JigyoKbn = string.Empty;

            OutData.Tekiyou = string.Empty;
        }

        /// <summary>
        /// 出力データ作成
        /// </summary>
        /// <param name="iX">伝票添え字</param>
        /// <param name="i">行添え字</param>
        /// <param name="st">伝票配列データ</param>
        /// <param name="fFlg">最初データフラグ</param>
        /// <param name="OutData">出力データ</param>
        /// <returns>出力データ文字列</returns>
        private string SetData(int iX, int i, Entity.InputRecord[] st, Boolean fFlg, Entity.OutputRecord OutData)
        {
            //伝票区切
            //複数チェックなし　かつ　伝票最初の行のみ
            if (st[iX].Head.FukusuChk == "0" && fFlg == true)
            {
                OutData.Kugiri = "*";
            }
            else
            {
                OutData.Kugiri = string.Empty;
            }
        
            //伝票部門コード
            OutData.DenBumon = string.Empty;

            //日付
            int sYear = int.Parse(st[iX].Head.Year);

            //西暦を求める
            if (global.pblReki == global.RWAREKI) //和暦のとき
            {
                sYear = sYear + int.Parse(company.Hosei);
            }
            else
            {
                sYear = sYear + 2000;
            }

            OutData.Date = sYear.ToString() + "/" + st[iX].Head.Month.PadLeft(2,'0') + "/" + st[iX].Head.Day.PadLeft(2,'0');

            //整理区分　2011/06/07
            //決算チェックありで勘定奉行の整理仕訳区分が"0"のとき：１、それ以外は０
            if (st[iX].Head.Kessan == global.FLGON && company.Arrange == global.FLGON)
            {
                OutData.Arrange = global.FLGON;
            }
            else
            {
                OutData.Arrange = global.FLGOFF;
            }

            //借方部門
            OutData.Kari.Bumon = CodeFormat(st[iX].Gyou[i].Kari.Bumon, global.LEN_BUMON);
    
            //借方科目
            OutData.Kari.Kamoku = CodeFormat(st[iX].Gyou[i].Kari.Kamoku, global.LEN_KAMOKU);
    
            //借方補助
            OutData.Kari.Hojo = CodeFormat(st[iX].Gyou[i].Kari.Hojo, global.LEN_HOJO);

            //借方消費税区分
            if (st[iX].Gyou[i].Kari.TaxKbn == string.Empty)
            {
                OutData.Kari.TaxKbn = string.Empty;
            }
            else
            {
                OutData.Kari.TaxKbn = st[iX].Gyou[i].Kari.TaxKbn;
            }

            //借方消費税額計算区分
            if (st[iX].Gyou[i].Kari.TaxMas == string.Empty)
            {
                OutData.Kari.TaxMas = fncGetZeiFlag(OutData.Kari.Kamoku.PadLeft(global.LEN_KAMOKU,'0'));
            }
            else if (st[iX].Gyou[i].Kari.TaxMas == "1")
            {
                OutData.Kari.TaxMas = string.Empty;
            }
            else
            {
                OutData.Kari.TaxMas = st[iX].Gyou[i].Kari.TaxMas;
            }

            ////借方プロジェクトコード
            //OutData.Kari.ProjectCode = CodeFormat(st[iX].Gyou[i].ProjectCode, global.LEN_PROJECT);

            ////借方サブプロジェクトコード
            //OutData.Kari.SubProjectCode = CodeFormat(st[iX].Gyou[i].SubProjectCode, global.LEN_SUBPROJECT);

            //借方事業区分
            OutData.Kari.JigyoKbn = CodeFormat(st[iX].Gyou[i].Kari.JigyoKbn, global.LEN_JIGYO);

            //借方金額
            OutData.Kari.Kin = st[iX].Gyou[i].Kari.Kin;
    
            //貸方部門
            OutData.Kashi.Bumon = CodeFormat(st[iX].Gyou[i].Kashi.Bumon, global.LEN_BUMON);

            //貸方科目
            OutData.Kashi.Kamoku = CodeFormat(st[iX].Gyou[i].Kashi.Kamoku, global.LEN_KAMOKU);

            //貸方補助
            OutData.Kashi.Hojo = CodeFormat(st[iX].Gyou[i].Kashi.Hojo, global.LEN_HOJO);

            //貸方消費税区分
            if (st[iX].Gyou[i].Kashi.TaxKbn == string.Empty)
            {
                OutData.Kashi.TaxKbn = string.Empty;
            }
            else
            {
                OutData.Kashi.TaxKbn = st[iX].Gyou[i].Kashi.TaxKbn;
            }

            //借方消費税額計算区分
            if (st[iX].Gyou[i].Kashi.TaxMas == string.Empty)
            {
                OutData.Kashi.TaxMas = fncGetZeiFlag(OutData.Kashi.Kamoku.PadLeft(global.LEN_KAMOKU, '0'));
            }
            else if (st[iX].Gyou[i].Kashi.TaxMas == "1")
            {
                OutData.Kashi.TaxMas = string.Empty;
            }
            else
            {
                OutData.Kashi.TaxMas = st[iX].Gyou[i].Kashi.TaxMas;
            }

            ////借方プロジェクトコード
            //OutData.Kashi.ProjectCode = CodeFormat(st[iX].Gyou[i].ProjectCode, global.LEN_PROJECT);

            ////借方サブプロジェクトコード
            //OutData.Kashi.SubProjectCode = CodeFormat(st[iX].Gyou[i].SubProjectCode, global.LEN_SUBPROJECT);

            //貸方事業区分
            OutData.Kashi.JigyoKbn = CodeFormat(st[iX].Gyou[i].Kashi.JigyoKbn, global.LEN_JIGYO);

            //貸方金額
            OutData.Kashi.Kin = st[iX].Gyou[i].Kashi.Kin;
        
            //摘要 
            OutData.Tekiyou = st[iX].Gyou[i].Tekiyou.TrimEnd();

            //出力文字列作成
            StringBuilder sb = new StringBuilder();
            sb.Append(OutData.Kugiri).Append(",");
            sb.Append(OutData.DenBumon).Append(",");
            sb.Append(OutData.Date).Append(",");
            sb.Append(OutData.Arrange).Append(",");     //整理区分 2011/06/07
            sb.Append(OutData.Kari.Bumon).Append(",");
            sb.Append(OutData.Kari.Kamoku).Append(",");
            sb.Append(OutData.Kari.Hojo).Append(",");
            sb.Append(OutData.Kari.TaxKbn).Append(",");
            sb.Append(OutData.Kari.JigyoKbn).Append(",");
            sb.Append(OutData.Kari.TaxMas).Append(",");
            //sb.Append(OutData.Kari.ProjectCode).Append(",");
            //sb.Append(OutData.Kari.SubProjectCode).Append(",");
            sb.Append(OutData.Kari.Kin).Append(",");
            sb.Append(OutData.Kashi.Bumon).Append(",");
            sb.Append(OutData.Kashi.Kamoku).Append(",");
            sb.Append(OutData.Kashi.Hojo).Append(",");
            sb.Append(OutData.Kashi.TaxKbn).Append(",");
            sb.Append(OutData.Kashi.JigyoKbn).Append(",");
            sb.Append(OutData.Kashi.TaxMas).Append(",");
            //sb.Append(OutData.Kashi.ProjectCode).Append(",");
            //sb.Append(OutData.Kashi.SubProjectCode).Append(",");
            sb.Append(OutData.Kashi.Kin).Append(",");
            sb.Append(OutData.Tekiyou);

            return sb.ToString();
        }

        /// <summary>
        /// 総勘定科目税処理区分取得
        /// </summary>
        /// <param name="kCode">勘定科目コード</param>
        /// <returns>税処理フラグ</returns>
        private string fncGetZeiFlag(string kCode)
        {
            string sRet = string.Empty;

            for (int i = 0; i < fgKamoku.Rows.Count; i++)
            {
                if (fgKamoku[0, i].Value.ToString() == kCode)
                {
                    sRet = fgKamoku[4, i].Value.ToString();
                    break;
                }
            }

            return sRet;
        }

        private void Base_FormClosing(object sender, FormClosingEventArgs e)
        {
            this.Dispose();
        }

        private void gcMultiRow1_CellValueChanged(object sender, GrapeCity.Win.MultiRow.CellEventArgs e)
        {
            string sWorkB;
            string sWorkA1;
            string sWorkA2;
            string CngVal;
    
            if (bCngFlag == true) return; //多重処理を避ける
            
            switch (e.CellName)
	        {
                case "txtTekiyou":  //摘要
                    bCngFlag = true;
                    if (gcMultiRow1.GetValue(e.RowIndex, e.CellName) != null)
                    {
                        sWorkB = gcMultiRow1.GetValue(e.RowIndex, e.CellName).ToString();
                    }
                    else
                    {
                        sWorkB = string.Empty;
                    }

                    sWorkA1 = string.Empty;
                    sWorkA2 = string.Empty;

                    for (int i = 0; i < sWorkB.Length ; i++)
			        {
			            sWorkA1 += sWorkB.Substring(i, 1);

                        if (System.Text.Encoding.GetEncoding("Shift_JIS").GetByteCount(sWorkA1) <= 80)
                        {
                            sWorkA2 += sWorkB.Substring(i, 1);
                        }
			        }

                    gcMultiRow1.SetValue(e.RowIndex, "txtTekiyou",sWorkA2);
                    bCngFlag = false;
                    break;

                case "txtKin_K":    //--借方金額カンマ編集・変換データ適切チェック処理 (金額)
                    bCngFlag = true;
                    KinCellValueChange(e);
                    KinSum();   // 合計計算
                    bCngFlag = false;
                    break;

                case "txtKin_S":    //--貸方金額カンマ編集・変換データ適切チェック処理 (金額)
                    bCngFlag = true;
                    KinCellValueChange(e);
                    KinSum();   // 合計計算
                    bCngFlag = false;
                    break;

                case "txtKCode_K":  //借方勘定科目
                
                    bCngFlag = true;
                    if (gcMultiRow1.GetValue(e.RowIndex, e.CellName) != null)
                    {
                        CngVal = gcMultiRow1.GetValue(e.RowIndex, e.CellName).ToString().Trim();          //両端空白削除
                        CngVal = CngVal.Replace(" ", string.Empty);                                         //文中空白削除
                        gcMultiRow1.SetValue(e.RowIndex, e.CellName, CngVal.Replace("-", string.Empty));    //"-"ハイフン削除

                        if (utility.NumericCheck(gcMultiRow1.GetValue(e.RowIndex, e.CellName).ToString()))
                        {
                            //gcMultiRow1.SetValue(e.RowIndex, e.CellName, string.Format("{0:##0}", int.Parse(gcMultiRow1.GetValue(e.RowIndex, e.CellName).ToString())));
                            gcMultiRow1.SetValue(e.RowIndex, e.CellName, gcMultiRow1.GetValue(e.RowIndex, e.CellName).ToString().PadLeft(global.LEN_KAMOKU, '0'));
                        }

                        //勘定科目名表示
                        if (gcMultiRow1.GetValue(e.RowIndex, e.CellName).ToString() != string.Empty)
                        {
                            GetKamokeName(gcMultiRow1, e.RowIndex, "txtKName_K", "txtKCode_K");
                        }
                    }
                    else
                    {
                        gcMultiRow1.SetValue(e.RowIndex, e.CellName, string.Empty);
                        gcMultiRow1.SetValue(e.RowIndex, "txtKName_K", string.Empty);
                    }

                    //MessageBox.Show(e.RowIndex.ToString() + " : " + gcMultiRow1.GetValue(e.RowIndex, e.CellName).ToString());

                    bCngFlag = false;
                
                    break;
                                    
                case "txtHojo_K":   //借方補助コード
                 
                    bCngFlag = true;
                    if (gcMultiRow1.GetValue(e.RowIndex, e.CellName) != null)
                    {
                        if (gcMultiRow1.GetValue(e.RowIndex, "txtKCode_K") != null)
                        {
                            CngVal = gcMultiRow1.GetValue(e.RowIndex, e.CellName).ToString().Trim();                    //両端空白削除
                            CngVal = CngVal.Replace(" ", string.Empty);                                                 //文中空白削除
                            gcMultiRow1.SetValue(e.RowIndex, e.CellName, CngVal.Replace("-", string.Empty));            //"-"ハイフン削除

                            if (utility.NumericCheck(gcMultiRow1.GetValue(e.RowIndex, e.CellName).ToString()))
                            {
                                //gcMultiRow1.SetValue(e.RowIndex, e.CellName, string.Format("{0:###0}", int.Parse(gcMultiRow1.GetValue(e.RowIndex, e.CellName).ToString())));
                                gcMultiRow1.SetValue(e.RowIndex, e.CellName, gcMultiRow1.GetValue(e.RowIndex, e.CellName).ToString().PadLeft(global.LEN_HOJO, '0'));
                            }

                            //補助科目名表示
                            GetHojoName(gcMultiRow1, e.RowIndex, "txtKName_K", "txtKCode_K", "txtHojoName_K", "txtHojo_K");
                        }
                        else
                        {
                            gcMultiRow1.SetValue(e.RowIndex, e.CellName, string.Empty);
                            gcMultiRow1.SetValue(e.RowIndex, "txtHojoName_K", string.Empty);
                        }
                    }
                    else
                    {
                        gcMultiRow1.SetValue(e.RowIndex, e.CellName, string.Empty);
                        gcMultiRow1.SetValue(e.RowIndex, "txtHojoName_K", string.Empty);
                    }
                    
                    bCngFlag = false;           
                    break;

                case "txtBCode_K":  //借方部門コード
                
                    bCngFlag = true;
                    if (gcMultiRow1.GetValue(e.RowIndex, e.CellName) != null)
                    {
                        CngVal = gcMultiRow1.GetValue(e.RowIndex, e.CellName).ToString().Trim();                      //両端空白削除
                        CngVal = CngVal.Replace(" ", string.Empty);  //文中空白削除
                        gcMultiRow1.SetValue(e.RowIndex, e.CellName, CngVal.Replace("-", string.Empty));  //"-"ハイフン削除

                        if (utility.NumericCheck(gcMultiRow1.GetValue(e.RowIndex, e.CellName).ToString()))
                        {
                            //gcMultiRow1.SetValue(e.RowIndex, e.CellName, string.Format("{0:###0}", int.Parse(gcMultiRow1.GetValue(e.RowIndex, e.CellName).ToString())));
                            gcMultiRow1.SetValue(e.RowIndex, e.CellName, gcMultiRow1.GetValue(e.RowIndex, e.CellName).ToString().PadLeft(global.LEN_BUMON, '0'));
                        }

                        //部門名表示
                        if (gcMultiRow1.GetValue(e.RowIndex, e.CellName).ToString() != string.Empty)
                        {
                            GetBumonName(gcMultiRow1, e.RowIndex, "txtBName_K", "txtBCode_K");
                        }
                    }
                    else
                    {
                        gcMultiRow1.SetValue(e.RowIndex, e.CellName, string.Empty);
                        gcMultiRow1.SetValue(e.RowIndex, "txtBName_K", string.Empty);
                    }
                    
                    bCngFlag = false;  

                    break;
                    
                case "txtZeik_K":   //借方税区分コード
                
                    bCngFlag = true;
                    if (gcMultiRow1.GetValue(e.RowIndex, e.CellName) != null)
                    {
                        CngVal = gcMultiRow1.GetValue(e.RowIndex, e.CellName).ToString().Trim();        //両端空白削除
                        CngVal = CngVal.Replace(" ", string.Empty);                                     //文中空白削除
                        gcMultiRow1.SetValue(e.RowIndex, e.CellName, CngVal.Replace("-", string.Empty)); //"-"ハイフン削除

                        if (utility.NumericCheck(gcMultiRow1.GetValue(e.RowIndex, e.CellName).ToString()))
                        {
                            gcMultiRow1.SetValue(e.RowIndex, e.CellName, string.Format("{0:#0}",int.Parse(gcMultiRow1.GetValue(e.RowIndex, e.CellName).ToString())));
                        }
                    }
                    else
                    {
                        gcMultiRow1.SetValue(e.RowIndex, e.CellName, string.Empty);
                    }

                    bCngFlag = false;
                    break;

                case "txtZeis_K":   //税処理

                    bCngFlag = true;

                    if (gcMultiRow1.GetValue(e.RowIndex, e.CellName) == null)
                    {
                        gcMultiRow1.SetValue(e.RowIndex, e.CellName, string.Empty);
                    }
                    bCngFlag = false;
                    break;

                case "txtZig_K":   //事業区分

                    bCngFlag = true;

                    if (gcMultiRow1.GetValue(e.RowIndex, e.CellName) == null)
                    {
                        gcMultiRow1.SetValue(e.RowIndex, e.CellName, string.Empty);
                    }
                    else
                    {
                        if (utility.NumericCheck(gcMultiRow1.GetValue(e.RowIndex, e.CellName).ToString()))
                        {
                            gcMultiRow1.SetValue(e.RowIndex, e.CellName, gcMultiRow1.GetValue(e.RowIndex, e.CellName).ToString().PadLeft(4, '0'));
                        }
                    }

                    bCngFlag = false;
                    break;

                case "txtKCode_S":  //貸方勘定科目
                
                    bCngFlag = true;
                    if (gcMultiRow1.GetValue(e.RowIndex, e.CellName) != null)
                    {
                        CngVal = gcMultiRow1.GetValue(e.RowIndex, e.CellName).ToString().Trim();          //両端空白削除
                        CngVal = CngVal.Replace(" ", string.Empty);                                         //文中空白削除
                        gcMultiRow1.SetValue(e.RowIndex, e.CellName, CngVal.Replace("-", string.Empty));    //"-"ハイフン削除

                        if (utility.NumericCheck(gcMultiRow1.GetValue(e.RowIndex, e.CellName).ToString()))
                        {
                            //gcMultiRow1.SetValue(e.RowIndex, e.CellName, string.Format("{0:##0}", int.Parse(gcMultiRow1.GetValue(e.RowIndex, e.CellName).ToString())));
                            gcMultiRow1.SetValue(e.RowIndex, e.CellName, gcMultiRow1.GetValue(e.RowIndex, e.CellName).ToString().PadLeft(global.LEN_KAMOKU, '0'));
                        }
                        
                        //勘定科目名表示
                        if (gcMultiRow1.GetValue(e.RowIndex, e.CellName).ToString() != string.Empty)
                        {
                            GetKamokeName(gcMultiRow1, e.RowIndex, "txtKName_S", "txtKCode_S");
                        }
                    }
                    else
                    {
                        gcMultiRow1.SetValue(e.RowIndex, e.CellName, string.Empty);
                        gcMultiRow1.SetValue(e.RowIndex, "txtKName_S", string.Empty);
                    }
                    
                    bCngFlag = false;

                    break;
                                    
                case "txtHojo_S":   //貸方補助コード
                 
                    bCngFlag = true;
                    if (gcMultiRow1.GetValue(e.RowIndex, e.CellName) != null)
                    {
                        if (gcMultiRow1.GetValue(e.RowIndex, "txtKCode_S") != null)
                        {
                            CngVal = gcMultiRow1.GetValue(e.RowIndex, e.CellName).ToString().Trim();                   //両端空白削除
                            CngVal = CngVal.Replace(" ", string.Empty);                                                 //文中空白削除
                            gcMultiRow1.SetValue(e.RowIndex, e.CellName, CngVal.Replace("-", string.Empty));   //"-"ハイフン削除

                            if (utility.NumericCheck(gcMultiRow1.GetValue(e.RowIndex, e.CellName).ToString()))
                            {
                                //gcMultiRow1.SetValue(e.RowIndex, e.CellName, string.Format("{0:###0}", int.Parse(gcMultiRow1.GetValue(e.RowIndex, e.CellName).ToString())));
                                gcMultiRow1.SetValue(e.RowIndex, e.CellName, gcMultiRow1.GetValue(e.RowIndex, e.CellName).ToString().PadLeft(global.LEN_HOJO, '0'));
                            }

                            //補助科目名表示
                            GetHojoName(gcMultiRow1, e.RowIndex, "txtKName_S", "txtKCode_S", "txtHojoName_S", "txtHojo_S");
                        }
                        else
                        {
                            gcMultiRow1.SetValue(e.RowIndex, e.CellName, string.Empty);
                            gcMultiRow1.SetValue(e.RowIndex, "txtHojoName_S", string.Empty);
                        }
                    }
                    else
                    {
                        gcMultiRow1.SetValue(e.RowIndex, e.CellName, string.Empty);
                        gcMultiRow1.SetValue(e.RowIndex, "txtHojoName_S", string.Empty);
                    }
                    
                    bCngFlag = false;           
                    break;

                case "txtBCode_S":  //貸方部門コード
                
                    bCngFlag = true;
                    if (gcMultiRow1.GetValue(e.RowIndex, e.CellName) != null)
                    {
                        CngVal = gcMultiRow1.GetValue(e.RowIndex, e.CellName).ToString().Trim();                      //両端空白削除
                        CngVal = CngVal.Replace(" ", string.Empty);  //文中空白削除
                        gcMultiRow1.SetValue(e.RowIndex, e.CellName, CngVal.Replace("-", string.Empty));  //"-"ハイフン削除

                        if (utility.NumericCheck(gcMultiRow1.GetValue(e.RowIndex, e.CellName).ToString()))
                        {
                            //gcMultiRow1.SetValue(e.RowIndex, e.CellName, string.Format("{0:###0}", int.Parse(gcMultiRow1.GetValue(e.RowIndex, e.CellName).ToString())));
                            gcMultiRow1.SetValue(e.RowIndex, e.CellName, gcMultiRow1.GetValue(e.RowIndex, e.CellName).ToString().PadLeft(global.LEN_BUMON, '0'));
                        }

                        //部門名表示
                        if (gcMultiRow1.GetValue(e.RowIndex, e.CellName).ToString() != string.Empty)
                        {
                            GetBumonName(gcMultiRow1, e.RowIndex, "txtBName_S", "txtBCode_S");
                        }
                    }
                    else
                    {
                        gcMultiRow1.SetValue(e.RowIndex, e.CellName, string.Empty);
                        gcMultiRow1.SetValue(e.RowIndex, "txtBName_S", string.Empty);
                    }
                    
                    bCngFlag = false;  

                    break;
                    
                case "txtZeik_S":   //貸方税区分コード
                
                    bCngFlag = true;
                    if (gcMultiRow1.GetValue(e.RowIndex, e.CellName) != null)
                    {
                        CngVal = gcMultiRow1.GetValue(e.RowIndex, e.CellName).ToString().Trim();        //両端空白削除
                        CngVal = CngVal.Replace(" ", string.Empty);                                     //文中空白削除
                        gcMultiRow1.SetValue(e.RowIndex, e.CellName, CngVal.Replace("-", string.Empty)); //"-"ハイフン削除

                        if (utility.NumericCheck(gcMultiRow1.GetValue(e.RowIndex, e.CellName).ToString()))
                        {
                            gcMultiRow1.SetValue(e.RowIndex, e.CellName, string.Format("{0:#0}",int.Parse(gcMultiRow1.GetValue(e.RowIndex, e.CellName).ToString())));
                        }
                    }
                    else
                    {
                        gcMultiRow1.SetValue(e.RowIndex, e.CellName, string.Empty);
                    }

                    bCngFlag = false;
                    break;

                case "txtZeis_S":   //貸方税処理

                    bCngFlag = true;

                    if (gcMultiRow1.GetValue(e.RowIndex, e.CellName) == null)
                    {
                        gcMultiRow1.SetValue(e.RowIndex, e.CellName, string.Empty);
                    }
                    bCngFlag = false;
                    break;

                case "txtZig_S":   //事業区分

                    bCngFlag = true;

                    if (gcMultiRow1.GetValue(e.RowIndex, e.CellName) == null)
                    {
                        gcMultiRow1.SetValue(e.RowIndex, e.CellName, string.Empty);
                    }
                    else
                    {
                        if (utility.NumericCheck(gcMultiRow1.GetValue(e.RowIndex, e.CellName).ToString()))
                        {
                            gcMultiRow1.SetValue(e.RowIndex, e.CellName, gcMultiRow1.GetValue(e.RowIndex, e.CellName).ToString().PadLeft(4, '0'));
                        }
                    }

                    bCngFlag = false;
                    break;

                //case "txtProjectCode":  //プロジェクトコード

                //    bCngFlag = true;
                //    if (gcMultiRow1.GetValue(e.RowIndex, e.CellName) != null)
                //    {
                //        CngVal = gcMultiRow1.GetValue(e.RowIndex, e.CellName).ToString().Trim();                      //両端空白削除
                //        CngVal = CngVal.Replace(" ", string.Empty);  //文中空白削除

                //        //if (utility.NumericCheck(gcMultiRow1.GetValue(e.RowIndex, e.CellName).ToString()))
                //        //{
                //        //    gcMultiRow1.SetValue(e.RowIndex, e.CellName, string.Format("{0:#######0}", int.Parse(gcMultiRow1.GetValue(e.RowIndex, e.CellName).ToString())));
                //        //}


                //        ////左側ゼロ埋め
                //        //gcMultiRow1.SetValue(e.RowIndex, e.CellName, gcMultiRow1.GetValue(e.RowIndex, e.CellName).ToString().PadLeft(global.LEN_PROJECT,'0'));

                //        ////プロジェクト名表示
                //        //if (gcMultiRow1.GetValue(e.RowIndex, e.CellName).ToString() != string.Empty)
                //        //{
                //        //    GetProjectName(gcMultiRow1, e.RowIndex, MultiRow.DP_NAMEP, MultiRow.DP_CODEP);
                //        //}
                //    }
                //    else
                //    {
                //        gcMultiRow1.SetValue(e.RowIndex, e.CellName, string.Empty);
                //        gcMultiRow1.SetValue(e.RowIndex, MultiRow.DP_NAMEP, string.Empty);
                //    }

                //    bCngFlag = false;

                //    break;

                //case "txtSubProjectCode":  //サブプロジェクトコード

                //    bCngFlag = true;

                //    if (gcMultiRow1.GetValue(e.RowIndex, e.CellName) != null)
                //    {
                //        CngVal = gcMultiRow1.GetValue(e.RowIndex, e.CellName).ToString().Trim();    //両端空白削除
                //        CngVal = CngVal.Replace(" ", string.Empty);  //文中空白削除

                //        if (utility.NumericCheck(gcMultiRow1.GetValue(e.RowIndex, e.CellName).ToString()))
                //        {
                //            gcMultiRow1.SetValue(e.RowIndex, e.CellName, string.Format("{0:D2}", int.Parse(gcMultiRow1.GetValue(e.RowIndex, e.CellName).ToString())));
                //        }

                //        //サブプロジェクト名表示
                //        if (gcMultiRow1.GetValue(e.RowIndex, e.CellName).ToString() != string.Empty)
                //        {
                //            GetSubProjectName(gcMultiRow1, e.RowIndex, MultiRow.DP_NAMESP, MultiRow.DP_CODESP);
                //        }
                //    }
                //    else
                //    {
                //        gcMultiRow1.SetValue(e.RowIndex, e.CellName, string.Empty);
                //        gcMultiRow1.SetValue(e.RowIndex, MultiRow.DP_NAMESP, string.Empty);
                //    }

                //    bCngFlag = false;

                //    break;

		        default:
                    break;
	        }
        }

        /// <summary>
        /// 金額セル値変更時の処理
        /// </summary>
        /// <param name="e">CellEventArgs</param>
        private void KinCellValueChange(GrapeCity.Win.MultiRow.CellEventArgs e)
        {
            if (gcMultiRow1.GetValue(e.RowIndex, e.CellName) != null)
            {
                if (errCheck.ChkKinIndi(gcMultiRow1.GetValue(e.RowIndex, e.CellName).ToString()))
                {
                    if (gcMultiRow1.GetValue(e.RowIndex, e.CellName).ToString() != string.Empty)
                    {
                        gcMultiRow1.SetValue(e.RowIndex, e.CellName, string.Format("{0:#,###}", int.Parse(gcMultiRow1.GetValue(e.RowIndex, e.CellName).ToString().Replace(",", string.Empty))));
                    }
                }
            }
            else
            {
                gcMultiRow1.SetValue(e.RowIndex, e.CellName, string.Empty);
            }
        }

        private void cmdExit_Click(object sender, EventArgs e)
        {
            //確認
            if (MessageBox.Show("終了しますか？", "確認", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No) return;
            
            //処理終了
            this.Close();
        }

        private void KinSum()
        {
            int kt = 0;
            int st = 0;

            for (int i = 0; i < global.MAXGYOU_PRN; i++)
            {
                kt += utility.StrToZero(utility.NulltoStr(gcMultiRow1[i, MultiRow.DP_KARI_KIN].Value));
                st += utility.StrToZero(utility.NulltoStr(gcMultiRow1[i, MultiRow.DP_KASHI_KIN].Value));
            }

            //頁合計
            gcMultiRow1.ColumnFooters[0].Cells[MultiRow.DP_KARI_P].Value = kt;              // 借方合計
            gcMultiRow1.ColumnFooters[0].Cells[MultiRow.DP_KASHI_P].Value = st;             // 貸方合計
            gcMultiRow1.ColumnFooters[0].Cells[MultiRow.DP_SAGAKU_P].Value = kt - st;       // 差額合計

            //差額があれば赤表示
            if (gcMultiRow1.ColumnFooters[0].Cells[MultiRow.DP_SAGAKU_P].Value.ToString() != "0")
            {
                gcMultiRow1.ColumnFooters[0].Cells[MultiRow.DP_SAGAKU_P].Style.ForeColor = Color.Red;
            }
            else
            {
                gcMultiRow1.ColumnFooters[0].Cells[MultiRow.DP_SAGAKU_P].Style.ForeColor = Color.Black;
            }
        }

        private void btnDltDen_Click(object sender, EventArgs e)
        {
            //確認
            if (MessageBox.Show("表示中の伝票をパターン登録しますか？", "確認", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No) return;
            
            //現在の伝票データを取得
            //DlgDataGet();

            // 登録画面表示
            frmShiwakeAdd frm = new frmShiwakeAdd();
            frm.ShowDialog();

            // 中止なら何もしない
            if (frm._addStatus == 0)
            {
                frm.Dispose();
                return;
            }

            //ファイルへ出力
            patternWrite(frm._outFile, frm._kingaku);

            frm.Dispose();
        }

        private void patternWrite(StreamWriter outFile, int sKin)
        {
            // ヘッダ出力文字列作成
            StringBuilder sb = new StringBuilder();
            sb.Append("*").Append(",");
            sb.Append(txtYear.Text.PadLeft(2, '0')).Append(",");
            sb.Append(txtMonth.Text.PadLeft(2, '0')).Append(",");
            sb.Append(txtDay.Text.PadLeft(2, '0')).Append(",");

            if (ChkKessan.CheckState == CheckState.Checked) sb.Append("1").Append(",");
            else sb.Append("0").Append(",");

            if (chkFukusuChk.CheckState == CheckState.Checked) sb.Append("1");
            else sb.Append("0");

            //ファイルへ出力            
            outFile.WriteLine(sb.ToString());

            // 行明細
            for (int i = 0; i < global.MAXGYOU_PRN; i++)
            {
                sb.Clear();
                sb.Append(utility.NulltoStr(gcMultiRow1.GetValue(i, MultiRow.DP_KARI_CODEB))).Append(",");              // 借方部門コード
                sb.Append(utility.NulltoStr(gcMultiRow1.GetValue(i, MultiRow.DP_KARI_CODE))).Append(",");               // 借方勘定科目コード
                sb.Append(utility.NulltoStr(gcMultiRow1.GetValue(i, MultiRow.DP_KARI_NAME))).Append(",");               // 借方勘定科目名
                sb.Append(utility.NulltoStr(gcMultiRow1.GetValue(i, MultiRow.DP_KARI_CODEH))).Append(",");              // 借方補助科目コード
                sb.Append(utility.NulltoStr(gcMultiRow1.GetValue(i, MultiRow.DP_KARI_NAMEH))).Append(",");              // 借方補助科目名
                if (sKin == 1) sb.Append(utility.NulltoStr(gcMultiRow1.GetValue(i, MultiRow.DP_KARI_KIN)).Replace(",", string.Empty));   // 借方金額
                sb.Append(","); 
                sb.Append(utility.NulltoStr(gcMultiRow1.GetValue(i, MultiRow.DP_KARI_ZEI))).Append(",");                // 借方税区分
                sb.Append(utility.NulltoStr(gcMultiRow1.GetValue(i, MultiRow.DP_KARI_ZEI_S))).Append(",");              // 借方税処理
                sb.Append(utility.NulltoStr(gcMultiRow1.GetValue(i, MultiRow.DP_KARI_ZIGYO))).Append(",");              // 借方事業区分

                sb.Append(utility.NulltoStr(gcMultiRow1.GetValue(i, MultiRow.DP_KASHI_CODEB))).Append(",");             // 貸方部門コード
                sb.Append(utility.NulltoStr(gcMultiRow1.GetValue(i, MultiRow.DP_KASHI_CODE))).Append(",");              // 貸方勘定科目コード
                sb.Append(utility.NulltoStr(gcMultiRow1.GetValue(i, MultiRow.DP_KASHI_NAME))).Append(",");              // 貸方勘定科目名
                sb.Append(utility.NulltoStr(gcMultiRow1.GetValue(i, MultiRow.DP_KASHI_CODEH))).Append(",");             // 貸方補助科目コード
                sb.Append(utility.NulltoStr(gcMultiRow1.GetValue(i, MultiRow.DP_KASHI_NAMEH))).Append(",");             // 貸方補助科目名
                if (sKin == 1) sb.Append(utility.NulltoStr(gcMultiRow1.GetValue(i, MultiRow.DP_KASHI_KIN)).Replace(",", string.Empty));   // 貸方金額
                sb.Append(",");
                sb.Append(utility.NulltoStr(gcMultiRow1.GetValue(i, MultiRow.DP_KASHI_ZEI))).Append(",");               // 貸方税区分
                sb.Append(utility.NulltoStr(gcMultiRow1.GetValue(i, MultiRow.DP_KASHI_ZEI_S))).Append(",");             // 貸方税処理
                sb.Append(utility.NulltoStr(gcMultiRow1.GetValue(i, MultiRow.DP_KASHI_ZIGYO))).Append(",");             // 貸方事業区分

                sb.Append(utility.NulltoStr(gcMultiRow1.GetValue(i, MultiRow.DP_TEKIYOU)));                             // 摘要

                //ファイルへ出力            
                outFile.WriteLine(sb.ToString());
            }

            // ファイルを閉じる
            outFile.Close();
        }

        /// <summary>
        /// MultiRow勘定科目名表示
        /// </summary>
        /// <param name="gmr">MultiRowoオブジェクト名</param>
        /// <param name="i">rowIndex</param>
        /// <param name="cName">科目名セル名</param>
        /// <param name="cCode">科目コードセル名</param>
        private void GetKamokeName(GcMultiRow gmr, int i, string cName, string cCode)
        {
            string KanjoCode = string.Empty;

            string sc = utility.GetDBConnect(global.pblDbName);             //SQLServer接続文字列取得
            SqlControl.DataControl sCon = new SqlControl.DataControl(sc);   //コントロールインスタンス生成
            string mySql = string.Empty;
            OleDbDataReader dR;                                             //データリーダー

            //勘定科目取得
            if (utility.NumericCheck(gmr.GetValue(i, cCode).ToString().Trim()))
            {
                KanjoCode = string.Format("{0:D10}", int.Parse(gmr.GetValue(i, cCode).ToString().Trim()));
            }
            else
            {
                KanjoCode = gmr.GetValue(i, cCode).ToString().Trim();
            }

            //科目名表示
            //mySql += "select sUcd,sNm from wkskm01 ";
            //mySql += "where tiIsTrk = 1 ";
            //mySql += "and sUcd = '" + 
            //            string.Format("{0,6}", gmr.GetValue(i, cCode).ToString().Trim()) + "'";

            mySql += "SELECT AccountItemCode, AccountItemName FROM tbAccountItem ";
            mySql += "WHERE (tbAccountItem.IsUse = 1) and ";
            mySql += "(tbAccountItem.AccountingPeriodID = " + global.pblAccPID + ") and ";
            mySql += "(AccountItemCode = '" + KanjoCode + "')";
            
            //データリーダーを取得する
            dR = sCon.free_dsReader(mySql);

            if (dR.HasRows)
            {
                dR.Read();
                gmr.SetValue(i, cName, dR["AccountItemName"].ToString().Trim());
                gmr[i, cName].Style.ForeColor = Color.Blue;
            }
            else
            {
                gmr.SetValue(i, cName, "存在しない勘定科目コードです");
                gmr[i, cName].Style.ForeColor = Color.Red;
            }

            dR.Close();
            sCon.Close();
        }

        /// <summary>
        /// MultiRow補助科目名表示
        /// </summary>
        /// <param name="gmr">MultiRowoオブジェクト名</param>
        /// <param name="i">rowindex</param>
        /// <param name="cName">勘定科目名セル名</param>
        /// <param name="cCode">勘定科目コードセル名</param>
        /// <param name="hName">補助科目名セル名</param>
        /// <param name="hCode">補助科目コードセル名</param>
        private void GetHojoName(GcMultiRow gmr, int i, string cName, string cCode, string hName, string hCode)
        {
            string KanjoCode = string.Empty;
            string hojoCode = string.Empty;
            Boolean hCodestatus = false;                                    //補助コードの有無ステータス
            int hCodeCount = 0;                                             //補助コードの該当有無
            string sc = utility.GetDBConnect(global.pblDbName);             //SQLServer接続文字列取得
            SqlControl.DataControl sCon = new SqlControl.DataControl(sc);   //コントロールインスタンス生成
            string mySql = string.Empty;
            OleDbDataReader dR;                                             //データリーダー

            //勘定科目取得
            if (utility.NumericCheck(gmr.GetValue(i, cCode).ToString().Trim()))
            {
                KanjoCode = string.Format("{0:D10}", int.Parse(gmr.GetValue(i, cCode).ToString().Trim()));
            }
            else
            {
                KanjoCode = gmr.GetValue(i, cCode).ToString().Trim();
            }

            //補助科目取得
            if (utility.NumericCheck(gmr.GetValue(i, hCode).ToString().Trim()))
            {
                hojoCode = string.Format("{0:D10}", int.Parse(gmr.GetValue(i, hCode).ToString().Trim()));
            }
            else
            {
                hojoCode = gmr.GetValue(i, hCode).ToString().Trim();
            }

            //補助コードがあるか？
            ////mySql += "select sNcd,sUcd,sHjoUcd,wkhjm01.sNm ";
            ////mySql += "from wkskm01 inner join wkhjm01 ";
            ////mySql += "on wkskm01.sNcd = wkhjm01.sSknNcd ";
            ////mySql += "where sHjoUcd <> '000000' and sUcd = '" + string.Format("{0,6}", gmr.GetValue(i, cCode).ToString().Trim()) + "' ";
            ////mySql += "order by sSknNcd,sHjoUcd";

            //補助コードがあるか？
            mySql += "select tbAccountItem.AccountItemID,tbAccountItem.AccountItemCode,";
            mySql += "tbAccountItem.AccountItemName,tbSubAccountItem.SubAccountItemCode,";
            mySql += "tbSubAccountItem.SubAccountItemName ";
            mySql += "from tbAccountItem inner join tbSubAccountItem ";
            mySql += "on tbAccountItem.AccountItemID = tbSubAccountItem.AccountItemID ";
            mySql += "where tbAccountItem.AccountingPeriodID = " + global.pblAccPID + " and ";
            mySql += "SubAccountItemCode <> '0000000000' and ";
            mySql += "tbAccountItem.AccountItemCode = '" + KanjoCode + "'";

            //データリーダーを取得し勘定科目に補助科目が設定されているか調べる
            dR = sCon.free_dsReader(mySql);
            if (dR.HasRows) hCodestatus = true;
            dR.Close();

            //勘定科目に補助コードが登録されているとき
            if (hCodestatus == true)
            {
                if (KanjoCode == string.Empty)
                {
                    gmr.SetValue(i, hName, "補助コード未登録です");
                    gmr[i, hName].Style.ForeColor = Color.Red;
                }
                else
                {
                    //その他を含めた補助科目のデータリーダーを取得する
                    //////mySql = string.Empty;
                    //////mySql += "select sNcd,sUcd,sHjoUcd,wkhjm01.sNm ";
                    //////mySql += "from wkskm01 inner join wkhjm01 ";
                    //////mySql += "on wkskm01.sNcd = wkhjm01.sSknNcd ";
                    //////mySql += "where sUcd = '" + string.Format("{0,6}", gmr.GetValue(i, cCode).ToString().Trim()) + "' ";
                    //////mySql += "order by sSknNcd,sHjoUcd";

                    //その他を含めた補助科目のデータリーダーを取得する
                    mySql = string.Empty;
                    mySql += "select tbAccountItem.AccountItemID,tbAccountItem.AccountItemCode,";
                    mySql += "tbAccountItem.AccountItemName,tbSubAccountItem.SubAccountItemCode,";
                    mySql += "tbSubAccountItem.SubAccountItemName ";
                    mySql += "from tbAccountItem inner join tbSubAccountItem ";
                    mySql += "on tbAccountItem.AccountItemID = tbSubAccountItem.AccountItemID ";
                    mySql += "where tbAccountItem.AccountingPeriodID = " + global.pblAccPID + " and ";
                    mySql += "tbAccountItem.AccountItemCode = '" + KanjoCode + "'";

                    dR = sCon.free_dsReader(mySql);

                    while (dR.Read())
                    {
                        if (dR["SubAccountItemCode"].ToString().Trim() == hojoCode)
                        {
                            gmr.SetValue(i, hName, dR["SubAccountItemName"].ToString().Trim());
                            gmr[i, hName].Style.ForeColor = Color.Blue;
                            hCodeCount = 1;
                            break;
                        }
                    }

                    if (hCodeCount == 0)
                    {
                        gmr.SetValue(i, hName, "存在しないコードです");
                        gmr[i, hName].Style.ForeColor = Color.Red;
                    }

                    dR.Close();
                }
            }
            else if (hojoCode == string.Empty)
            {
                gmr.SetValue(i, hName, string.Empty);
                gmr[i, hName].Style.ForeColor = Color.Blue;
            }
            else
            {
                gmr.SetValue(i, hName, "存在しないコードです");
                gmr[i, hName].Style.ForeColor = Color.Red;
            }
            
            sCon.Close();
        }

        /// <summary>
        /// MultiRow部門名表示
        /// </summary>
        /// <param name="gmr">MultiRowoオブジェクト名</param>
        /// <param name="i">rowindex</param>
        /// <param name="cName">部門名セル名</param>
        /// <param name="cCode">部門コードセル名</param>
        private void GetBumonName(GcMultiRow gmr, int i, string cName, string cCode)
        {
            string sc = utility.GetDBConnect(global.pblDbName);             //SQLServer接続文字列取得
            SqlControl.DataControl sCon = new SqlControl.DataControl(sc);   //コントロールインスタンス生成
            string mySql = string.Empty;
            OleDbDataReader dR;                                             //データリーダー
            string CodeB;

            //部門コード編集
            if (utility.NumericCheck(gmr.GetValue(i, cCode).ToString().Trim()))
            {
                CodeB = string.Format("{0:D15}", int.Parse(gmr.GetValue(i, cCode).ToString().Trim()));
            }
            else
            {
                CodeB = gmr.GetValue(i, cCode).ToString().Trim();
            }
                       
            //勘定奉行データベースへ接続する
            mySql = string.Empty;

            //mySql += "SELECT sUcd,sNm from wkbnm01 ";
            //mySql += "where sUcd = '" + CodeB + "'";

            mySql += "select DepartmentID,DepartmentCode,DepartmentName ";
            mySql += "from tbDepartment ";
            mySql += "where tbDepartment.DepartmentCode = '" + CodeB + "'";

            //データリーダーを取得する
            dR = sCon.free_dsReader(mySql);
            if (dR.HasRows)
            {
                dR.Read();
                gmr.SetValue(i, cName, dR["DepartmentName"].ToString().Trim());
                gmr[i, cName].Style.ForeColor = Color.Blue;
            }
            else
            {
                gmr.SetValue(i, cName, "存在しないコードです");
                gmr[i, cName].Style.ForeColor = Color.Red;
            }

            dR.Close();
            sCon.Close();
        }

        /// <summary>
        /// MultiRowプロジェクト名表示
        /// </summary>
        /// <param name="gmr">MultiRowoオブジェクト名</param>
        /// <param name="i">rowindex</param>
        /// <param name="cName">プロジェクト名セル名</param>
        /// <param name="cCode">プロジェクトコードセル名</param>
        private void GetProjectName(GcMultiRow gmr, int i, string cName, string cCode)
        {
            string sc = utility.GetDBConnect(global.pblDbName);             //SQLServer接続文字列取得
            SqlControl.DataControl sCon = new SqlControl.DataControl(sc);   //コントロールインスタンス生成
            string mySql = string.Empty;
            OleDbDataReader dR;                                             //データリーダー
            string CodeB;

            //プロジェクトコード編集
            if (utility.NumericCheck(gmr.GetValue(i, cCode).ToString().Trim()))
            {
                CodeB = string.Format("{0:D20}", int.Parse(gmr.GetValue(i, cCode).ToString().Trim()));
            }
            else
            {
                CodeB = gmr.GetValue(i, cCode).ToString().Trim().PadRight(20);
            }

            //勘定奉行データベースへ接続する
            mySql = string.Empty;

            mySql += "SELECT ProjectCode,ProjectName from tbProject ";
            mySql += "where ProjectCode = '" + CodeB + "'";

            //データリーダーを取得する
            dR = sCon.free_dsReader(mySql);
            if (dR.HasRows)
            {
                dR.Read();
                gmr.SetValue(i, cName, dR["ProjectName"].ToString().Trim());
                gmr[i, cName].Style.ForeColor = Color.Blue;
            }
            else
            {
                gmr.SetValue(i, cName, "存在しないコードです");
                gmr[i, cName].Style.ForeColor = Color.Red;
            }

            dR.Close();
            sCon.Close();
        }

        /// <summary>
        /// MultiRowサブプロジェクト名表示
        /// </summary>
        /// <param name="gmr">MultiRowoオブジェクト名</param>
        /// <param name="i">rowindex</param>
        /// <param name="cName">サブプロジェクト名セル名</param>
        /// <param name="cCode">サブプロジェクトコードセル名</param>
        private void GetSubProjectName(GcMultiRow gmr, int i, string cName, string cCode)
        {
            string sc = utility.GetDBConnect(global.pblDbName);             //SQLServer接続文字列取得
            SqlControl.DataControl sCon = new SqlControl.DataControl(sc);   //コントロールインスタンス生成
            string mySql = string.Empty;
            OleDbDataReader dR;                                             //データリーダー
            string CodeB;

            //プロジェクトコード編集
            if (utility.NumericCheck(gmr.GetValue(i, cCode).ToString().Trim()))
            {
                CodeB = string.Format("{0:D20}", int.Parse(gmr.GetValue(i, cCode).ToString().Trim()));
            }
            else
            {
                CodeB = gmr.GetValue(i, cCode).ToString().Trim().PadRight(20);
            }

            //勘定奉行データベースへ接続する
            mySql = string.Empty;

            mySql += "SELECT SubProjectCode,SubProjectName from tbSubProject ";
            mySql += "where SubProjectCode = '" + CodeB + "'";

            //データリーダーを取得する
            dR = sCon.free_dsReader(mySql);
            if (dR.HasRows)
            {
                dR.Read();
                gmr.SetValue(i, cName, dR["SubProjectName"].ToString().Trim());
                gmr[i, cName].Style.ForeColor = Color.Blue;
            }
            else
            {
                gmr.SetValue(i, cName, "存在しないコードです");
                gmr[i, cName].Style.ForeColor = Color.Red;
            }

            dR.Close();
            sCon.Close();
        }

        private void txt_KeyPress(object sender, KeyPressEventArgs e)
        {
            //数字またはバックスペースキーのみ許可する
            if (sender == txtYear || sender == txtMonth || sender == txtDay)
            {
                 if ((e.KeyChar < '0' || e.KeyChar > '9') && e.KeyChar != '\b')
                 {
                     e.Handled = true;
                 }
            }
        }

        /// <summary>
        /// cellによってマスター表示タブを切り替える
        /// </summary>
        /// <param name="e">セル関連イベント</param>
        private void Cell_EnterClick(CellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            //科目欄クリック時
            if (e.CellName == MultiRow.DP_KARI_CODE || e.CellName == MultiRow.DP_KARI_NAME ||
                e.CellName == MultiRow.DP_KASHI_CODE || e.CellName == MultiRow.DP_KASHI_NAME)
            {
                tabData.SelectedIndex = global.TAB_KAMOKU_PRN;
                return;
            }

            //借方補助欄クリック時
            if (e.CellName == MultiRow.DP_KARI_CODEH || e.CellName == MultiRow.DP_KARI_NAMEH)
            {
                if (gcMultiRow1.GetValue(e.RowIndex, MultiRow.DP_KARI_CODE) != null)
                {
                    tabData.SelectedIndex = global.TAB_KAMOKU_PRN;
                    this.fgHojo.RowCount = 0;

                    //選択された科目の補助設定がある場合、補助リストを表示
                    GridViewShow_Hojo(this.fgHojo, gcMultiRow1.GetValue(e.RowIndex, MultiRow.DP_KARI_CODE).ToString());
                    return;
                }
            }

            //貸方補助欄クリック時
            if (e.CellName == MultiRow.DP_KASHI_CODEH || e.CellName == MultiRow.DP_KASHI_NAMEH)
            {
                if (gcMultiRow1.GetValue(e.RowIndex, MultiRow.DP_KASHI_CODE) != null)
                {
                    tabData.SelectedIndex = global.TAB_KAMOKU_PRN;
                    this.fgHojo.RowCount = 0;

                    //選択された科目の補助設定がある場合、補助リストを表示
                    GridViewShow_Hojo(this.fgHojo, gcMultiRow1.GetValue(e.RowIndex, MultiRow.DP_KASHI_CODE).ToString());
                    return;
                }
            }

            //税処理欄クリック時
            if (e.CellName == MultiRow.DP_KARI_ZEI_S || e.CellName == MultiRow.DP_KASHI_ZEI_S)
            {
                tabData.SelectedIndex = global.TAB_TAXBUMON_PRN;
                return;
            }

            //税区分欄クリック時
            if (e.CellName == MultiRow.DP_KARI_ZEI || e.CellName == MultiRow.DP_KASHI_ZEI)
            {
                tabData.SelectedIndex = global.TAB_TAXBUMON_PRN;
                return;
            }

            //部門欄クリック時
            if (e.CellName == MultiRow.DP_KARI_CODEB || e.CellName == MultiRow.DP_KARI_NAMEB ||
                e.CellName == MultiRow.DP_KASHI_CODEB || e.CellName == MultiRow.DP_KASHI_NAMEB)
            {
                tabData.SelectedIndex = global.TAB_TAXBUMON_PRN;
                return;
            }

            //摘要欄クリック時
            //if (e.CellName == MultiRow.DP_TEKIYOU)
            //{
            //    //tabData.SelectedIndex = global.TAB_TEKIYOU;
            //    //return;
            //}

            //事業区分欄クリック時
            if (e.CellName == MultiRow.DP_KARI_ZIGYO || e.CellName == MultiRow.DP_KASHI_ZIGYO)
            {
                tabData.SelectedIndex = global.TAB_TAXBUMON_PRN;
                return;
            }

            ////プロジェクトコード,サブプロジェクトコードクリック時
            //if (e.CellName == MultiRow.DP_CODEP)
            //{
            //    tabData.SelectedIndex = global.TAB_PROJECT;
            //    return;
            //}
        }

        private void gcMultiRow1_CellClick(object sender, CellEventArgs e)
        {
            Cell_EnterClick(e);
            gcMultiRow1.BeginEdit(true);
        }

        private void ChkErrColor_Click(object sender, EventArgs e)
        {
        }

        private void fgBumon_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            //伝票データへコード、名称をセットする
            fgDataSet(fgBumon, MultiRow.DP_KARI_CODEB, MultiRow.DP_KARI_NAMEB, MultiRow.DP_KASHI_CODEB, MultiRow.DP_KASHI_NAMEB);
        }

        /// <summary>
        /// マスター表示グリッドから伝票へデータをセットする
        /// </summary>
        /// <param name="Dgv">マスター表示用 DataGridViewオブジェクト名</param>
        /// <param name="cuCellCode_Kari">借方コードセル名</param>
        /// <param name="cuCellName_Kari">借方名称セル名</param>
        /// <param name="cuCellCode_Kashi">貸方コードセル名</param>
        /// <param name="cuCellName_Kashi">貸方名称セル名</param>
        private void fgDataSet(DataGridView Dgv, string cuCellCode_Kari, string cuCellName_Kari, string cuCellCode_Kashi, string cuCellName_Kashi)
        {
            string sKmkCode;    //コード
            string sKmkName;    //名称

            if (Dgv.Rows.Count == 0) return;

            sKmkCode = Dgv.SelectedRows[0].Cells[0].Value.ToString().Trim();
            sKmkName = Dgv.SelectedRows[0].Cells[1].Value.ToString().Trim();
            
                //借方科目にフォーカスがある時
                if (gcMultiRow1.CurrentCellPosition.CellName == cuCellCode_Kari ||
                    gcMultiRow1.CurrentCellPosition.CellName == cuCellName_Kari)
                {
                    gcMultiRow1.SetValue(gcMultiRow1.CurrentCellPosition.RowIndex, cuCellCode_Kari, sKmkCode);
                    gcMultiRow1.SetValue(gcMultiRow1.CurrentCellPosition.RowIndex, cuCellName_Kari, sKmkName);

                    //テキストカラーを戻す
                    gcMultiRow1[gcMultiRow1.CurrentCellPosition.RowIndex, gcMultiRow1.CurrentCellPosition.CellName].Style.ForeColor = Color.Blue;
                    gcMultiRow1.Focus();
                }

                //貸方科目にフォーカスがある時
                if (gcMultiRow1.CurrentCellPosition.CellName == cuCellCode_Kashi ||
                    gcMultiRow1.CurrentCellPosition.CellName == cuCellName_Kashi)
                {
                    gcMultiRow1.SetValue(gcMultiRow1.CurrentCellPosition.RowIndex, cuCellCode_Kashi, sKmkCode);
                    gcMultiRow1.SetValue(gcMultiRow1.CurrentCellPosition.RowIndex, cuCellName_Kashi, sKmkName);

                    //テキストカラーを戻す
                    gcMultiRow1[gcMultiRow1.CurrentCellPosition.RowIndex, gcMultiRow1.CurrentCellPosition.CellName].Style.ForeColor = Color.Blue;
                    gcMultiRow1.Focus();
                }
        }

        private void fgKamoku_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            //伝票データへコード、名称をセットする
            fgDataSet(fgKamoku, MultiRow.DP_KARI_CODE, MultiRow.DP_KARI_NAME, MultiRow.DP_KASHI_CODE, MultiRow.DP_KASHI_NAME);
        }

        private void fgHojo_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            //伝票データへコード、名称をセットする
            fgDataSet(fgHojo, MultiRow.DP_KARI_CODEH, MultiRow.DP_KARI_NAMEH, MultiRow.DP_KASHI_CODEH, MultiRow.DP_KASHI_NAMEH);
        }

        /// <summary>
        /// 税処理、税区分表示グリッドから伝票へデータをセットする
        /// </summary>
        /// <param name="Dgv">マスター表示用 DataGridViewオブジェクト名</param>
        /// <param name="cuCellCode_Kari">借方コードセル名</param>
        /// <param name="cuCellCode_Kashi">貸方コードセル名</param>
        private void fgTaxDataSet(DataGridView Dgv, string cuCellCode_Kari,string cuCellCode_Kashi)
        {
            string sKmkCode;    //コード

            if (Dgv.Rows.Count == 0) return;

            sKmkCode = Dgv.SelectedRows[0].Cells[0].Value.ToString().Trim();

            //if (gcMultiRow1.Rows[gcMultiRow1.CurrentCellPosition.RowIndex].Cells[MultiRow.DP_DELCHK].Value.ToString() == "No")
            
                //借方科目にフォーカスがある時
                if (gcMultiRow1.CurrentCellPosition.CellName == cuCellCode_Kari)
                {
                    gcMultiRow1.SetValue(gcMultiRow1.CurrentCellPosition.RowIndex, cuCellCode_Kari, sKmkCode);

                    //テキストカラーを戻す
                    gcMultiRow1[gcMultiRow1.CurrentCellPosition.RowIndex, gcMultiRow1.CurrentCellPosition.CellName].Style.ForeColor = Color.Blue;
                    gcMultiRow1.Focus();
                }

                //貸方科目にフォーカスがある時
                if (gcMultiRow1.CurrentCellPosition.CellName == cuCellCode_Kashi)
                {
                    gcMultiRow1.SetValue(gcMultiRow1.CurrentCellPosition.RowIndex, cuCellCode_Kashi, sKmkCode);

                    //テキストカラーを戻す
                    gcMultiRow1[gcMultiRow1.CurrentCellPosition.RowIndex, gcMultiRow1.CurrentCellPosition.CellName].Style.ForeColor = Color.Blue;
                    gcMultiRow1.Focus();
                }
        }

        /// <summary>
        /// 摘要表示グリッドから伝票へデータをセットする
        /// </summary>
        /// <param name="Dgv">マスター表示用 DataGridViewオブジェクト名</param>
        /// <param name="cuCellName">セル名</param>
        private void fgTekiyoDataSet(DataGridView Dgv, string cuCellName)
        {
            string sKmkName;    //コード

            if (Dgv.Rows.Count == 0) return;

            sKmkName = Dgv.SelectedRows[0].Cells[0].Value.ToString().Trim();

            //if (gcMultiRow1.Rows[gcMultiRow1.CurrentCellPosition.RowIndex].Cells[MultiRow.DP_DELCHK].Value.ToString() == "No")
           
                if (gcMultiRow1.CurrentCellPosition.CellName == cuCellName)
                {
                    gcMultiRow1.SetValue(gcMultiRow1.CurrentCellPosition.RowIndex, cuCellName, sKmkName);

                    //テキストカラーを戻す
                    gcMultiRow1[gcMultiRow1.CurrentCellPosition.RowIndex, gcMultiRow1.CurrentCellPosition.CellName].Style.ForeColor = Color.Blue;
                    gcMultiRow1.Focus();
                }
        }

        /// <summary>
        /// プロジェクトグリッドから伝票へデータをセットする
        /// </summary>
        /// <param name="Dgv">マスター表示用 DataGridViewオブジェクト名</param>
        /// <param name="cuCellName">セル名</param>
        private void fgProjectDataSet(DataGridView Dgv, string cuCellCode, string cuCellName)
        {
            if (Dgv.Rows.Count == 0) return;

            //コード
            string sKmkCode = Dgv.SelectedRows[0].Cells[0].Value.ToString().Trim();

            //名称
            string sKmkName = Dgv.SelectedRows[0].Cells[1].Value.ToString().Trim();

            if (gcMultiRow1.CurrentCellPosition.CellName == cuCellCode ||
                gcMultiRow1.CurrentCellPosition.CellName == cuCellName)
            {
                gcMultiRow1.SetValue(gcMultiRow1.CurrentCellPosition.RowIndex, cuCellCode, sKmkCode);
                gcMultiRow1.SetValue(gcMultiRow1.CurrentCellPosition.RowIndex, cuCellName, sKmkName);

                //テキストカラーを戻す
                gcMultiRow1[gcMultiRow1.CurrentCellPosition.RowIndex, gcMultiRow1.CurrentCellPosition.CellName].Style.ForeColor = Color.Blue;
                gcMultiRow1.Focus();
            }
        }

        private void fgTax_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            //伝票データへコードをセットする
            fgTaxDataSet(fgTax, MultiRow.DP_KARI_ZEI, MultiRow.DP_KASHI_ZEI);
        }

        private void fgTaxMas_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            //伝票データへコードをセットする
            fgTaxDataSet(fgTaxMas, MultiRow.DP_KARI_ZEI_S, MultiRow.DP_KASHI_ZEI_S);
        }

        private void fgTekiyo_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            //伝票データへ摘要をセットする
            //fgTekiyoDataSet(fgTekiyo, MultiRow.DP_TEKIYOU);
        }

        private void fgKamoku_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            //選択された勘定科目の補助科目を表示する
            GridViewShow_Hojo(fgHojo, fgKamoku.SelectedRows[0].Cells[0].Value.ToString());
        }

        private void fgKamoku_SelectionChanged(object sender, EventArgs e)
        {
            if (global.MASTERLOAD_STATUS == 1) return;
 
            //選択された勘定科目の補助科目を表示する
            GridViewShow_Hojo(fgHojo, fgKamoku.SelectedRows[0].Cells[0].Value.ToString());
        }


        private void button1_Click(object sender, EventArgs e)
        {
            fgKamoku.CurrentCell = null;
        }

        //private void btnPrint_Click(object sender, EventArgs e)
        //{
        //    //確認
        //    if (MessageBox.Show("表示中の伝票を印刷しますか？", "確認", MessageBoxButtons.YesNo, MessageBoxIcon.Question,MessageBoxDefaultButton.Button1) == DialogResult.No) return;

        //    cPrint Prn = new cPrint();
        //    Prn.Denpyo(DenData, DenIndex, global.PRINTMODEALL);
        //}

        //private void TestPrint(object sender, System.Drawing.Printing.PrintPageEventArgs e)
        //{
        //    for (float x  = 1; x < 120; x++)
        //    {
        //        for (float y = 1; y < 50; y++)
        //        {
        //            SetXY(x, y, e);
        //            e.Graphics.DrawString(x.ToString().Substring(0,1), new Font("ＭＳ ゴシック", PRINTFONTSIZE), Brushes.Black, PrnX, PrnY);
        //        }
        //    }
        //}


        private void txtYear_Enter(object sender, EventArgs e)
        {
            TextBox  Obj = new TextBox();

            if (sender == txtYear) Obj = txtYear;
            if (sender == txtMonth) Obj = txtMonth;
            if (sender == txtDay) Obj = txtDay;
            //if (sender == txtDenNo) Obj = txtDenNo;

            Obj.BackColor = Color.LightGray;
            Obj.SelectAll();
        }

        private void txtYear_Leave(object sender, EventArgs e)
        {
            TextBox Obj = new TextBox();

            if (sender == txtYear) Obj = txtYear;
            if (sender == txtMonth) Obj = txtMonth;
            if (sender == txtDay) Obj = txtDay;
            //if (sender == txtDenNo) Obj = txtDenNo;
            
            Obj.BackColor = Color.White;

            if (utility.NumericCheck(Obj.Text) == false)
            {
                Obj.Text = "0";
            }
        }

        private void Base_KeyPress(object sender, KeyPressEventArgs e)
        {
        }

        private void gcMultiRow1_CellContentClick(object sender, CellEventArgs e)
        {

        }

        private void gcMultiRow1_CellEnter(object sender, CellEventArgs e)
        {
            if (gcMultiRow1.EditMode == EditMode.EditProgrammatically)
            {
                Cell_EnterClick(e);
                gcMultiRow1.BeginEdit(true);
            }
        }

        private void gcMultiRow1_Enter(object sender, EventArgs e)
        {
        }

        private void fgProject_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            ////伝票データへプロジェクトコード、名称をセットする
            //fgProjectDataSet(fgProject, MultiRow.DP_CODEP, MultiRow.DP_NAMEP);
        }

        //private void fgSubProject_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        //{
        //    //伝票データへサブプロジェクトコード、名称をセットする
        //    fgProjectDataSet(fgSubProject, MultiRow.DP_CODESP, MultiRow.DP_NAMESP);
        //}

        /// <summary>
        /// コードの桁数数分左にゼロ埋めした文字列を返す
        /// </summary>
        /// <param name="sCode">コードの値</param>
        /// <param name="sLength">桁長</param>
        /// <returns>ゼロ埋めした文字列</returns>
        private string CodeFormat(string sCode, int sLength)
        {
            //戻り値
            string rtnValue;

            if (utility.NumericCheck(sCode))
            {
                rtnValue = sCode.PadLeft(sLength, '0');
            }
            else
            {
                rtnValue = sCode;
            }

            return rtnValue;
        }

        private void button1_Click_2(object sender, EventArgs e)
        {
            //確認
            //キャンセル
            if (MessageBox.Show("伝票を印刷しますか？", "確認", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No) return;

            //編集モード終了
            this.gcMultiRow1.EndEdit();

            // 活字振替伝票発行
            sReportStaff(global.WorkDir + Properties.Settings.Default.xlsPath);

        }

        private void fgJigyo_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            //伝票データへコードをセットする
            fgTaxDataSet(fgJigyo, MultiRow.DP_KARI_ZIGYO, MultiRow.DP_KASHI_ZIGYO);
        }

        private void gcMultiRow1_CellContentClick_1(object sender, CellEventArgs e)
        {

        }

        /// <summary>
        /// 振替伝票印刷
        /// </summary>
        /// <param name="xlsPath">勤務票エクセルシートパス</param>
        private void sReportStaff(string xlsPath)
        {
            string sID = string.Empty;

            try
            {
                //マウスポインタを待機にする
                this.Cursor = Cursors.WaitCursor;
                Excel.Application oXls = new Excel.Application();
                Excel.Workbook oXlsBook = (Excel.Workbook)(oXls.Workbooks.Open(xlsPath, Type.Missing, Type.Missing, Type.Missing, Type.Missing,
                                                   Type.Missing, Type.Missing, Type.Missing, Type.Missing,
                                                   Type.Missing, Type.Missing, Type.Missing, Type.Missing,
                                                   Type.Missing, Type.Missing));

                //Excel.Worksheet oxlsSheet = new Excel.Worksheet();

                Excel.Worksheet oxlsSheet = (Excel.Worksheet)oXlsBook.Sheets[1];
                Excel.Range[] rng = new Microsoft.Office.Interop.Excel.Range[2];

                try
                {
                    ////印刷2件目以降はシートを追加する
                    //pCnt++;

                    //if (pCnt > 1)
                    //{
                    //    oxlsSheet.Copy(Type.Missing, oXlsBook.Sheets[pCnt - 1]);
                    //    oxlsSheet = (Excel.Worksheet)oXlsBook.Sheets[pCnt];
                    //}

                    // シートを初期化します
                    oxlsSheet.Cells[2, 1] = string.Empty;
                    oxlsSheet.Cells[2, 4] = string.Empty;
                    oxlsSheet.Cells[2, 7] = string.Empty;
                    oxlsSheet.Cells[2, 10] = string.Empty;

                    for (int ix = 5; ix <= 38; ix+=2)
                    {
                        oxlsSheet.Cells[ix, 1] = string.Empty;   // 借方金額
                        oxlsSheet.Cells[ix, 2] = string.Empty;   // 借方部門コード
                        oxlsSheet.Cells[ix, 3] = string.Empty;   // 借方勘定科目コード
                        oxlsSheet.Cells[ix, 5] = string.Empty;   // 借方勘定科目名
                        oxlsSheet.Cells[ix, 6] = string.Empty;   // 借方補助科目コード
                        oxlsSheet.Cells[ix, 7] = string.Empty;   // 借方補助科目名
                        oxlsSheet.Cells[ix, 9] = string.Empty;   // 貸方勘定科目コード
                        oxlsSheet.Cells[ix, 10] = string.Empty;  // 貸方勘定科目名
                        oxlsSheet.Cells[ix, 12] = string.Empty;  // 貸方補助科目コード
                        oxlsSheet.Cells[ix, 13] = string.Empty;  // 貸方補助科目名
                        oxlsSheet.Cells[ix, 16] = string.Empty;  // 貸方部門コード
                        oxlsSheet.Cells[ix, 17] = string.Empty;  // 貸方金額

                        oxlsSheet.Cells[ix + 1, 2] = string.Empty;   // 税処理
                        oxlsSheet.Cells[ix + 1, 3] = string.Empty;   // 税区分
                        oxlsSheet.Cells[ix + 1, 4] = string.Empty;   // 事業区分
                        oxlsSheet.Cells[ix + 1, 5] = string.Empty;   // 摘要
                        oxlsSheet.Cells[ix + 1, 14] = string.Empty;  // 税処理
                        oxlsSheet.Cells[ix + 1, 15] = string.Empty;  // 税区分
                        oxlsSheet.Cells[ix + 1, 16] = string.Empty;  // 事業区分
                    }

                    oxlsSheet.Cells[2, 1] = txtYear.Text.PadLeft(2, '0') + txtMonth.Text.PadLeft(2, '0') + txtDay.Text.PadLeft(2, '0');
                    if (ChkKessan.CheckState == CheckState.Checked) oxlsSheet.Cells[2, 7] = "○";
                    if (chkFukusuChk.CheckState == CheckState.Checked) oxlsSheet.Cells[2, 10] = "○";

                    // 行明細
                    for (int i = 0; i < global.MAXGYOU_PRN; i++)
                    {
                        oxlsSheet.Cells[i * 2 + 5, 1] = utility.NulltoStr(gcMultiRow1.GetValue(i, MultiRow.DP_KARI_KIN)).Replace(",",string.Empty);   // 借方金額
                        oxlsSheet.Cells[i * 2 + 5, 2] = utility.NulltoStr(gcMultiRow1.GetValue(i, MultiRow.DP_KARI_CODEB));   // 借方部門コード
                        oxlsSheet.Cells[i * 2 + 5, 3] = utility.NulltoStr(gcMultiRow1.GetValue(i, MultiRow.DP_KARI_CODE));   // 借方勘定科目コード
                        oxlsSheet.Cells[i * 2 + 5, 5] = utility.NulltoStr(gcMultiRow1.GetValue(i, MultiRow.DP_KARI_NAME));   // 借方勘定科目名
                        oxlsSheet.Cells[i * 2 + 5, 6] = utility.NulltoStr(gcMultiRow1.GetValue(i, MultiRow.DP_KARI_CODEH));   // 借方補助科目コード
                        oxlsSheet.Cells[i * 2 + 5, 7] = utility.NulltoStr(gcMultiRow1.GetValue(i, MultiRow.DP_KARI_NAMEH));   // 借方補助科目名
                        oxlsSheet.Cells[i * 2 + 5, 9] = utility.NulltoStr(gcMultiRow1.GetValue(i, MultiRow.DP_KASHI_CODE));   // 貸方勘定科目コード
                        oxlsSheet.Cells[i * 2 + 5, 10] = utility.NulltoStr(gcMultiRow1.GetValue(i, MultiRow.DP_KASHI_NAME));  // 貸方勘定科目名
                        oxlsSheet.Cells[i * 2 + 5, 12] = utility.NulltoStr(gcMultiRow1.GetValue(i, MultiRow.DP_KASHI_CODEH));  // 貸方補助科目コード
                        oxlsSheet.Cells[i * 2 + 5, 13] = utility.NulltoStr(gcMultiRow1.GetValue(i, MultiRow.DP_KASHI_NAMEH));  // 貸方補助科目名
                        oxlsSheet.Cells[i * 2 + 5, 16] = utility.NulltoStr(gcMultiRow1.GetValue(i, MultiRow.DP_KASHI_CODEB));  // 貸方部門コード
                        oxlsSheet.Cells[i * 2 + 5, 17] = utility.NulltoStr(gcMultiRow1.GetValue(i, MultiRow.DP_KASHI_KIN)).Replace(",", string.Empty);  // 貸方金額

                        oxlsSheet.Cells[i * 2 + 6, 2] = utility.NulltoStr(gcMultiRow1.GetValue(i, MultiRow.DP_KARI_ZEI_S));    // 借方税処理
                        oxlsSheet.Cells[i * 2 + 6, 3] = utility.NulltoStr(gcMultiRow1.GetValue(i, MultiRow.DP_KARI_ZEI));      // 借方税区分
                        oxlsSheet.Cells[i * 2 + 6, 4] = utility.NulltoStr(gcMultiRow1.GetValue(i, MultiRow.DP_KARI_ZIGYO));    // 借方事業区分
                        oxlsSheet.Cells[i * 2 + 6, 5] = utility.NulltoStr(gcMultiRow1.GetValue(i, MultiRow.DP_TEKIYOU));       // 摘要
                        oxlsSheet.Cells[i * 2 + 6, 14] = utility.NulltoStr(gcMultiRow1.GetValue(i, MultiRow.DP_KASHI_ZEI_S));  // 貸方税処理
                        oxlsSheet.Cells[i * 2 + 6, 15] = utility.NulltoStr(gcMultiRow1.GetValue(i, MultiRow.DP_KASHI_ZEI));  // 貸方税区分
                        oxlsSheet.Cells[i * 2 + 6, 16] = utility.NulltoStr(gcMultiRow1.GetValue(i, MultiRow.DP_KASHI_ZIGYO));  // 貸方事業区分
                    }

                    // ウィンドウを非表示にする
                    //oXls.Visible = false;
                    // 印刷
                    //oxlsSheet.PrintPreview(false);
                    oxlsSheet.PrintOut(1, Type.Missing, 1, false, oXls.ActivePrinter, Type.Missing, Type.Missing, Type.Missing);
                    //oXlsBook.PrintOut();

                    // マウスポインタを元に戻す
                    this.Cursor = Cursors.Default;

                    // 終了メッセージ
                    MessageBox.Show("印刷が終了しました", "活字振替伝票印刷", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.Message, "印刷処理", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }

                finally
                {
                    // ウィンドウを非表示にする
                    oXls.Visible = false;

                    // 保存処理
                    oXls.DisplayAlerts = false;

                    // Bookをクローズ
                    oXlsBook.Close(Type.Missing, Type.Missing, Type.Missing);

                    // Excelを終了
                    oXls.Quit();

                    // COMオブジェクトの参照カウントを解放する 
                    System.Runtime.InteropServices.Marshal.ReleaseComObject(oxlsSheet);
                    System.Runtime.InteropServices.Marshal.ReleaseComObject(oXlsBook);
                    System.Runtime.InteropServices.Marshal.ReleaseComObject(oXls);

                    // マウスポインタを元に戻す
                    this.Cursor = Cursors.Default;
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "印刷処理", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }

            //マウスポインタを元に戻す
            this.Cursor = Cursors.Default;
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            string f = string.Empty;
            StreamReader inFile = null;

            // ダイアログボックスの表示
            openFileDialog1.Title = "汎用データ出力先ファイル";
            openFileDialog1.Filter = "データファイル(*.csv)|*.csv";
            openFileDialog1.InitialDirectory = global.WorkDir + global.pblComName + @"\" + global.DIR_TEMP;
            openFileDialog1.FileName = string.Empty;
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                f = openFileDialog1.FileName;
                inFile = new StreamReader(f, Encoding.Default);
            }
            else return;

            // パターン表示
            patternPreview(inFile);

            // StreamReader 閉じる
            inFile.Close();
        }

        private void patternPreview(StreamReader inFile)
        {
            // 明細行カウント
            int rInt = 0;

            // 読み込んだ結果をすべて格納するための変数を宣言する
            string stResult = string.Empty;
            string stBuffer;

            // 読み込みできる文字がなくなるまで繰り返す
            while (inFile.Peek() >= 0)
            {
                // ファイルを 1 行ずつ読み込む
                stBuffer = inFile.ReadLine();

                if (stBuffer != string.Empty)
                {
                    // カンマ区切りで分割して配列に格納する
                    string[] stArrayData = stBuffer.Split(',');

                    //先頭に「*」があったらヘッダ情報
                    if (stArrayData[0].Trim() == "*")
                    {
                        // 決算
                        if (stArrayData[4].Trim() == "1") ChkKessan.Checked = true;
                        else ChkKessan.Checked = false;

                        // 複数枚
                        if (stArrayData[5].Trim() == "1") chkFukusuChk.Checked = true;
                        else chkFukusuChk.Checked = false;
                    }
                    else
                    {
                        //行データ格納表示
                        gcMultiRow1.SetValue(rInt, MultiRow.DP_KARI_CODEB, stArrayData[0]);     // 借方部門
                        gcMultiRow1.SetValue(rInt, MultiRow.DP_KARI_CODE, stArrayData[1]);      // 借方勘定科目
                        gcMultiRow1.SetValue(rInt, MultiRow.DP_KARI_CODEH, stArrayData[3]);     // 借方補助科目
                        gcMultiRow1.SetValue(rInt, MultiRow.DP_KARI_KIN, stArrayData[5]);       // 借方金額
                        gcMultiRow1.SetValue(rInt, MultiRow.DP_KARI_ZEI, stArrayData[6]);       // 借方税区分
                        gcMultiRow1.SetValue(rInt, MultiRow.DP_KARI_ZEI, stArrayData[7]);       // 借方税処理
                        gcMultiRow1.SetValue(rInt, MultiRow.DP_KARI_ZIGYO, stArrayData[8]);     // 借方事業区分

                        gcMultiRow1.SetValue(rInt, MultiRow.DP_KASHI_CODEB, stArrayData[9]);    // 貸方部門
                        gcMultiRow1.SetValue(rInt, MultiRow.DP_KASHI_CODE, stArrayData[10]);    // 貸方勘定科目
                        gcMultiRow1.SetValue(rInt, MultiRow.DP_KASHI_CODEH, stArrayData[12]);   // 貸方補助科目
                        gcMultiRow1.SetValue(rInt, MultiRow.DP_KASHI_KIN, stArrayData[14]);     // 貸方金額
                        gcMultiRow1.SetValue(rInt, MultiRow.DP_KASHI_ZEI, stArrayData[15]);     // 貸方税区分
                        gcMultiRow1.SetValue(rInt, MultiRow.DP_KASHI_ZEI, stArrayData[16]);     // 貸方税処理
                        gcMultiRow1.SetValue(rInt, MultiRow.DP_KASHI_ZIGYO, stArrayData[17]);   // 貸方事業区分

                        gcMultiRow1.SetValue(rInt, MultiRow.DP_TEKIYOU, stArrayData[18]);       // 貸方事業区分

                        rInt++;
                    }
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            //確認
            if (MessageBox.Show("表示中の仕訳伝票を取り消しますか？", "確認", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No) return;

            ChkKessan.Checked = false;  // 決算
            chkFukusuChk.Checked = false;// 複数枚
            gcMultiRow1.RowCount = 0;
            gcMultiRow1.RowCount = global.MAXGYOU_PRN;
        }
    }
}

