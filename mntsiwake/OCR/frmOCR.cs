using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Leadtools;
using Leadtools.Twain;
using Leadtools.Codecs;
using Leadtools.WinForms;
using Leadtools.WinForms.CommonDialogs.File;
using Leadtools.ImageProcessing;
using IdrFormEngine;

namespace mntsiwake.OCR
{
    public partial class frmOCR : Form
    {
        public frmOCR()
        {
            InitializeComponent();
            InitClass();
        }

        // TWAINでの取得用にTwainSessionを宣言します。
        private TwainSession _twainSession;

        // ＯＣＲ変換後のＣＳＶファイルと画像を登録するフォルダパス名変数を宣言します。
        private string _ocrPath;

        // 出力ファイル名保存用変数を宣言します。
        private string _fileName;

        // RasterImageViewerコントロールを宣言します。
        private RasterImageViewer _viewer;

        // イメージロード用RasterCodecsを宣言します。
        private RasterCodecs _codecs;

        // 出力ファイルのフォーマット保存用変数を宣言します。
        private RasterImageFormat _fileFormat;
        int _bitsPerPixel = 1;

        // 取得ページ数の保存用変数を宣言します。
        private int _pageNo;

        //スキャナから出力された画像枚数
        private int _sNumber = 0;

        //スキャナから出力された画像ファイル数
        private int _sFileNumber = 0;

        // OCR変換画像枚数
        int _okCount = 0;

        // OCRファイル名連番 : WinReader
        int dNo = 0;

        // OCRファイル名（タイムスタンプ）: WinReader
        string fnm = string.Empty;
              
        /// <summary>
        /// アプリケーションの初期化処理を行います。
        /// </summary>
        private void InitClass()
        {
            // フォームのタイトルを設定します。
            this.Text = "TWAIN 取得 【振替伝票読み取り】";

            //自分自身のバージョン情報を取得する　2011/03/25
            //System.Diagnostics.FileVersionInfo ver =
            //    System.Diagnostics.FileVersionInfo.GetVersionInfo(
            //    System.Reflection.Assembly.GetExecutingAssembly().Location);

            //キャプションにバージョンを追加　2011/03/25
            //Messager.Caption += " ver " + ver.FileMajorPart.ToString() + "." + ver.FileMinorPart.ToString();

            //Text = Messager.Caption;

            // ロック解除状態を確認します。
            //Support.Unlock(false);

            // RasterImageViewerコントロールを初期化します。
            _viewer = new RasterImageViewer();
            //_viewer.Dock = DockStyle.Fill;
            _viewer.BackColor = Color.DarkGray;
            Controls.Add(_viewer);
            _viewer.BringToFront();
            _viewer.Visible = false;

            // コーデックパスを設定します。
            RasterCodecs.Startup();

            // RasterCodecsオブジェクトを初期化します。
            _codecs = new RasterCodecs();

            if (TwainSession.IsAvailable(this))
            {
                // TwainSessionオブジェクトを初期化します。
                _twainSession = new TwainSession();

                // TWAIN セッションを初期化します。
                _twainSession.Startup(this, "FKDL", "LEADTOOLS", "Ver16.5J", "OCR", TwainStartupFlags.None);
                //_twainSession.Startup2(this, "FKDL", "LEADTOOLS", "Ver16.5J", "OCR", TwainStartupFlags.None, TwainLanguage.LanguageJapanese, TwainCountry.CountryJapan);
            }
            else
            {
                //_miFileAcquire.Enabled = false;
                //_miFileSelectSource.Enabled = false;
            }

            // 各値を初期化します。
            _fileName = string.Empty;
            _fileFormat = RasterImageFormat.Tif;
            _pageNo = 1;
            _sFileNumber = 0;

            //UpdateMyControls();
            UpdateStatusBarText();
        }

        private void frmOCR_Load(object sender, EventArgs e)
        {
        }

        private void frmOCR_Shown(object sender, EventArgs e)
        {
        }

        private void frmOCR_FormClosing(object sender, FormClosingEventArgs e)
        {
            CleanUp();
            this.Dispose();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Hide();

            // OCRエンジン指定の判定 : 2013/08/27
            if (Properties.Settings.Default.OCR_Engine == global.OCR_PANA)
            {
                // 振替伝票スキャン処理 : PanaOCR編
                while (true)
                {
                    ScanOcr();

                    if (_viewer.Image != null)
                    {
                        string msg = "続けて読込を行いますか？" + Environment.NewLine + "読込枚数 ： " + _viewer.Image.PageCount.ToString() + "枚";
                        if (MessageBox.Show(msg, "TWAIN取得", MessageBoxButtons.YesNo, MessageBoxIcon.Information, MessageBoxDefaultButton.Button2) == DialogResult.No)
                            break;
                    }
                    else
                    {
                        MessageBox.Show("処理を中断しました", "振替伝票スキャン処理", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        break;
                    }
                }

                // ＯＣＲ処理
                this.Show();
                if (_viewer.Image != null)
                {
                    string outPath = string.Empty;

                    // ３Ｆ経理・各拠点書式定義ファイルパス取得 2013/07/01
                    string formatFileH = Properties.Settings.Default.instDir + Properties.Settings.Default.fmtHPath;

                    // ２Ｆ総務書式ファイルパス取得 2013/07/01
                    string formatFileP = Properties.Settings.Default.instDir + Properties.Settings.Default.fmtSPath;

                    //// ＯＣＲ書式定義ファイルの取得　2013/06/17
                    //if (_fmt == global.FMT_3F)　// ３Ｆ経理・各拠点書式ファイルパス取得
                    //    formatFileH = Properties.Settings.Default.instDir + Properties.Settings.Default.fmtHPath;
                    //else if (_fmt == global.FMT_2F)　// ２Ｆ総務書式ファイルパス取得
                    //    formatFileH = Properties.Settings.Default.instDir + Properties.Settings.Default.fmtSPath;

                    // ＯＣＲ出力先パス
                    outPath = Properties.Settings.Default.instDir + global.DIR_INCSV;

                    // マルチTifをページ毎に分割します
                    MultiTif(Properties.Settings.Default.instDir + global.DIR_OCRREAD);

                    // 画像数を取得します 
                    var sTif = System.IO.Directory.GetFileSystemEntries(Properties.Settings.Default.instDir + global.DIR_READ, "*.tif");

                    // ３Ｆ経理・各拠点書式ＯＣＲ処理を実施します
                    ocrMain(Properties.Settings.Default.instDir + global.DIR_READ,
                            Properties.Settings.Default.instDir + global.DIR_NG,
                            outPath, formatFileH, sTif.Length);

                    // 2F伝票の書式でOCR処理を実施します
                    sTif = System.IO.Directory.GetFileSystemEntries(Properties.Settings.Default.instDir + global.DIR_2F, "*.tif");

                    if (sTif.Length > 0)
                    {
                        // OCR起動
                        ocrMain(Properties.Settings.Default.instDir + global.DIR_2F,
                                Properties.Settings.Default.instDir + global.DIR_NG,
                                outPath, formatFileP, sTif.Length);
                    }
                }
            }
            else if (Properties.Settings.Default.OCR_Engine == global.OCR_WinReader)
            {
                // WinReader起動
                WinReaderOCR();

                // ファイル名のタイムスタンプを設定
                fnm = string.Format("{0:0000}", DateTime.Today.Year) +
                      string.Format("{0:00}", DateTime.Today.Month) +
                      string.Format("{0:00}", DateTime.Today.Day) +
                      string.Format("{0:00}", DateTime.Now.Hour) +
                      string.Format("{0:00}", DateTime.Now.Minute) +
                      string.Format("{0:00}", DateTime.Now.Second);

                // 連番を初期化
                dNo = 0;

                // ファイル分割処理
                LoadCsvDivide();
            }

            // OCR変換ファイルがあれば修正画面を表示します
            int ocrCnt = 0;
            foreach (string nm in System.IO.Directory.GetFiles(Properties.Settings.Default.instDir + global.DIR_INCSV, "*.csv"))
            {
                ocrCnt++;
            }

            // NG伝票出力先フォルダがなければ作成する
            string ngPath = Properties.Settings.Default.instDir + global.DIR_NG;
            if (!System.IO.Directory.Exists(ngPath)) System.IO.Directory.CreateDirectory(ngPath);

            // NG伝票を移動
            int ngCnt = 0;

            // エラー画像ファイル名のタイムスタンプを設定
            fnm = string.Format("{0:0000}", DateTime.Today.Year) +
                  string.Format("{0:00}", DateTime.Today.Month) +
                  string.Format("{0:00}", DateTime.Today.Day) +
                  string.Format("{0:00}", DateTime.Now.Hour) +
                  string.Format("{0:00}", DateTime.Now.Minute) +
                  string.Format("{0:00}", DateTime.Now.Second);

            foreach (string ng in System.IO.Directory.GetFiles(Properties.Settings.Default.wrHands_Path + @"\" + global.winErrPath, "*.tif"))
            {
                ngCnt++;
                System.IO.File.Move(ng, ngPath + "E" + fnm + ngCnt.ToString().PadLeft(3, '0') + ".tif");
            }

            // OCR結果表示
            MessageBox.Show("OK件数：" + ocrCnt.ToString() + Environment.NewLine + "NG件数：" + ngCnt.ToString(), "OCR認識結果", MessageBoxButtons.OK, MessageBoxIcon.Information);

            // NG確認メッセージ表示
            if (ngCnt > 0)
            {
                MessageBox.Show("「OCRの書式に一致しない」と認識された伝票が" + ocrCnt.ToString() + "件ありました" +
                    Environment.NewLine + Environment.NewLine + "以下のフォルダを確認してください" +
                    Environment.NewLine + "アンマッチ画像フォルダ：" + ngPath, "OCR認識エラー", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }

            // 認識画像があれば修正画面を表示
            if (ocrCnt > 0)
            {
                // 修正画面表示
                this.Hide();
                Base frmCorrect = new Base(global.OCRMODE);
                frmCorrect.ShowDialog();
                this.Show();
            }

            // 終了
            this.Close();
        }

        /// <summary>
        /// 2013/08/27 WinReader仕様
        /// 
        /// WinReaderを起動して振替伝票をスキャンしてOCR処理を実施する
        /// 
        /// </summary>
        private void WinReaderOCR()
        {
            // WinReaderJOB起動文字列
            string JobName = @"""" + Properties.Settings.Default.wrHands_Job + @"""" + " /H2";
            string winReader_exe = Properties.Settings.Default.wrHands_Path + @"\" +
                Properties.Settings.Default.wrHands_Prg;

            // ProcessStartInfo の新しいインスタンスを生成する
            System.Diagnostics.ProcessStartInfo p = new System.Diagnostics.ProcessStartInfo();

            // 起動するアプリケーションを設定する
            p.FileName = winReader_exe;

            // コマンドライン引数を設定する（WinReaderのJOB起動パラメーター）
            p.Arguments = JobName;

            // WinReaderを起動します
            System.Diagnostics.Process hProcess = System.Diagnostics.Process.Start(p);

            // WinReaderが終了するまで待機する
            hProcess.WaitForExit();
        }

        /// <summary>
        /// 2013/08/27 WinReader仕様
        /// 
        /// 伝票ＣＳＶデータを一枚ごとに分割する
        /// 
        /// </summary>
        private void LoadCsvDivide()
        {
            string imgName = string.Empty;      //画像ファイル名
            string firstFlg = global.FLGON;
            global.pblDenNum = 0;               //伝票枚数を0にセット
            string[] stArrayData;               //CSVファイルを１行単位で格納する配列
            string newFnm = string.Empty;       // 新ファイル名

            // 対象ファイルの存在を確認します
            if (!System.IO.File.Exists(global.WorkDir + global.DIR_READ + global.INFILE)) return;

            // StreamReader の新しいインスタンスを生成する
            //入力ファイル
            System.IO.StreamReader inFile = new System.IO.StreamReader(global.WorkDir + global.DIR_READ + global.INFILE, Encoding.Default);

            // 読み込んだ結果をすべて格納するための変数を宣言する
            string stResult = string.Empty;
            string stBuffer;

            // 行番号
            int sRow = 0;

            // 読み込みできる文字がなくなるまで繰り返す
            while (inFile.Peek() >= 0)
            {
                // ファイルを 1 行ずつ読み込む
                stBuffer = inFile.ReadLine();

                // カンマ区切りで分割して配列に格納する
                stArrayData = stBuffer.Split(',');

                //先頭に「*」か「#」があったら新たな伝票なのでCSVファイル作成
                if ((stArrayData[0] == "*"))
                {
                    //最初の伝票以外のとき
                    if (firstFlg == global.FLGOFF)
                    {
                        //ファイル書き出し
                        outFileWrite(stResult, global.WorkDir + global.DIR_READ + imgName, newFnm);
                    }

                    //伝票枚数カウント
                    global.pblDenNum++;
                    firstFlg = global.FLGOFF;

                    // 伝票連番
                    dNo++;

                    // ファイル名
                    newFnm = fnm + dNo.ToString().PadLeft(3, '0');

                    //画像ファイル名を取得
                    imgName = stArrayData[1];

                    //文字列バッファをクリア
                    stResult = string.Empty;

                    // 文字列再校正（act.1 画像ファイル名を変更する、act.2 日付を年月日に分割する）
                    stBuffer = string.Empty;
                    for (int i = 0; i < stArrayData.Length; i++)
                    {
                        if (stBuffer != string.Empty) stBuffer += ",";

                        // 画像ファイル名を変更する
                        if (i == 1) stArrayData[i] = newFnm + ".tif"; // 画像ファイル名を変更

                        // 日付（６桁）を年月日（２桁毎）に分割する
                        if (i == 3) 
                        {
                            string dt = stArrayData[i].PadLeft(6, '0');
                            stArrayData[i] = dt.Substring(0, 2) + "," + dt.Substring(2, 2) + "," + dt.Substring(4, 2);
                        }

                        // フィールド結合
                        stBuffer += stArrayData[i];
                    }

                    sRow = 0;
                }
                else sRow++;

                // 最終行は追加しない（伝票区別記号(*)のため）
                if (sRow <= global.MAXGYOU_PRN)
                {
                    // 読み込んだものを追加で格納する
                    stResult += (stBuffer + Environment.NewLine);
                }
            }

            // 後処理
            if (global.pblDenNum > 0)
            {
                //ファイル書き出し
                outFileWrite(stResult, global.WorkDir + global.DIR_READ + imgName, newFnm);

                // 入力ファイルを閉じる
                inFile.Close();

                //入力ファイル削除 : "WINOUT.csv"
                utility.FileDelete(global.WorkDir + global.DIR_READ, global.INFILE);

                //画像ファイル削除 : "WRH***.tif"
                utility.FileDelete(global.WorkDir + global.DIR_READ, "WRH*.tif");
            }
        }

        /// <summary>
        /// 2013/08/27 WinReader仕様
        /// 
        /// 分割ファイルを書き出す
        /// 
        /// </summary>
        /// <param name="tempResult">書き出す文字列</param>
        /// <param name="tempImgName">元画像ファイルパス</param>
        /// <param name="outFileName">新ファイル名</param>
        private void outFileWrite(string tempResult, string tempImgName, string outFileName)
        {
            //出力ファイル
            System.IO.StreamWriter outFile = new System.IO.StreamWriter(global.WorkDir + global.DIR_INCSV + outFileName + ".csv",
                                                    false, System.Text.Encoding.GetEncoding(932));
            // ファイル書き出し
            outFile.Write(tempResult);

            //ファイルクローズ
            outFile.Close();

            //画像ファイルをコピー
            System.IO.File.Copy(tempImgName, global.WorkDir + global.DIR_INCSV + outFileName + ".tif");
        }

        /// <summary>
        /// スキャナより勤務票をスキャンして画像を取得します
        /// </summary>
        private void ScanOcr()
        {
            //出力先パス初期化
            _ocrPath = string.Empty;

            try
            {
                RasterSaveDialogFileFormatsList saveDlgFormatList = new RasterSaveDialogFileFormatsList(RasterDialogFileFormatDataContent.User);

                string tifPath = Properties.Settings.Default.instDir + global.DIR_OCRREAD;
                _fileName = tifPath + string.Format("{0:0000}", DateTime.Today.Year) +
                                                                    string.Format("{0:00}", DateTime.Today.Month) +
                                                                    string.Format("{0:00}", DateTime.Today.Day) +
                                                                    string.Format("{0:00}", DateTime.Now.Hour) +
                                                                    string.Format("{0:00}", DateTime.Now.Minute) +
                                                                    string.Format("{0:00}", DateTime.Now.Second) + ".tif";

                /// 以下、TWAIN取得関連 //////////////////////////////////////////////////////////////////////

                _fileFormat = RasterImageFormat.CcittGroup4;
                _bitsPerPixel = 1;

                string pathName = System.IO.Path.GetDirectoryName(_fileName);
                if (System.IO.Directory.Exists(pathName))
                {
                    // ページカウンタを初期化します。
                    _pageNo = 1;

                    // 出力ファイルカウンタをインクリメントします。
                    _sFileNumber++;

                    // AcquirePageイベントハンドラを設定します。
                    _twainSession.AcquirePage += new EventHandler<TwainAcquirePageEventArgs>(_twain_AcquirePage);

                    // Acquire pages
                    _twainSession.Acquire(TwainUserInterfaceFlags.Show);

                    // AcquirePageイベントハンドラを削除します。
                    _twainSession.AcquirePage -= new EventHandler<TwainAcquirePageEventArgs>(_twain_AcquirePage);
                }
                else
                {
                    MessageBox.Show("ファイル名の書式が正しくありません。");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                _twainSession.Shutdown();
                _twainSession.Startup(this, "GrapeCity Inc.", "LEADTOOLS", "Ver.16.5J", "LEADTOOLS TWAIN", TwainStartupFlags.None);
            }
            finally
            {
                UpdateStatusBarText();
            }
        }

        /// <summary>
        /// AcquirePageイベント処理
        /// </summary>
        private void _twain_AcquirePage(object sender, TwainAcquirePageEventArgs e)
        {
            try
            {
                if (e.Image != null)
                {
                    // 選択されているファイルフォーマットがマルチページに対応しているかどうかを確認します。
                    if ((_fileFormat == RasterImageFormat.Tif) || (_fileFormat == RasterImageFormat.Ccitt) ||
                       (_fileFormat == RasterImageFormat.CcittGroup31Dim) || (_fileFormat == RasterImageFormat.CcittGroup32Dim) ||
                       (_fileFormat == RasterImageFormat.CcittGroup4) || (_fileFormat == RasterImageFormat.TifCmp) ||
                       (_fileFormat == RasterImageFormat.TifCmw) || (_fileFormat == RasterImageFormat.TifCmyk) ||
                       (_fileFormat == RasterImageFormat.TifCustom) ||
                       (_fileFormat == RasterImageFormat.TifJ2k) || (_fileFormat == RasterImageFormat.TifJbig) ||
                       (_fileFormat == RasterImageFormat.TifJpeg) || (_fileFormat == RasterImageFormat.TifJpeg411) ||
                       (_fileFormat == RasterImageFormat.TifJpeg422) || (_fileFormat == RasterImageFormat.TifLead1Bit) ||
                       (_fileFormat == RasterImageFormat.TifLzw) || (_fileFormat == RasterImageFormat.TifLzwCmyk) ||
                       (_fileFormat == RasterImageFormat.TifLzwYcc) || (_fileFormat == RasterImageFormat.TifPackBits) ||
                       (_fileFormat == RasterImageFormat.TifPackBitsCmyk) || (_fileFormat == RasterImageFormat.TifPackbitsYcc) ||
                       (_fileFormat == RasterImageFormat.TifUnknown) || (_fileFormat == RasterImageFormat.TifYcc) ||
                       (_fileFormat == RasterImageFormat.Gif))
                    { 
                        // ファイル拡張子の保存変数を初期化します。
                        string ext = string.Empty;

                        // ファイル名に拡張子を含んでいない場合、拡張子を追加します。
                        if (System.IO.Path.HasExtension(_fileName))
                            ext = System.IO.Path.GetExtension(_fileName);

                        // 保存ファイル名に連番を付加します。
                        string tmpFileName = System.IO.Path.GetFileNameWithoutExtension(_fileName);
                        string tmpDirName = System.IO.Path.GetDirectoryName(_fileName);
                        string newFileName = string.Format("{0}\\{1}{2:000}{3}", tmpDirName, tmpFileName, _sFileNumber, ext);
                        // 取得したページを保存します。
                        _codecs.Save(e.Image, newFileName, _fileFormat, _bitsPerPixel, 1, 1, 1, CodecsSavePageMode.Append);
                    }
                    else
                    {

                        // マルチページに対応していないフォーマットの場合、ファイルに番号を付加して保存します。
                        // ファイル拡張子の保存変数を初期化します。
                        string ext = string.Empty;

                        // ファイル名に拡張子を含んでいない場合、拡張子を追加します。
                        if (System.IO.Path.HasExtension(_fileName))
                            ext = System.IO.Path.GetExtension(_fileName);

                        // 保存ファイル名にページ番号を付加します。
                        string tmpFileName = System.IO.Path.GetFileNameWithoutExtension(_fileName);
                        string tmpDirName = System.IO.Path.GetDirectoryName(_fileName);
                        string newFileName = string.Format("{0}\\{1}{2:000}{3}", tmpDirName, tmpFileName, _pageNo, ext);

                        _codecs.Save(e.Image, newFileName, _fileFormat, _bitsPerPixel);

                        // ページ数のカウンタをインクリメントします。
                        _pageNo++;
                    }

                    // 取得ページをビューアに表示します。
                    if (_viewer.Image == null)
                    {
                        _viewer.Image = e.Image;
                    }
                    else
                    {
                        _viewer.Image.AddPage(e.Image);
                        _viewer.Image.Page = _viewer.Image.PageCount;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void CleanUp()
        {
            RasterCodecs.Shutdown();

            if (_twainSession != null)
            {
                try
                {
                    _twainSession.Shutdown();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
            }
        }

        /// <summary>
        /// ステータスバーのページ表示を更新します。
        /// </summary>
        private void UpdateStatusBarText()
        {
            if (_viewer.Image != null)
                this.label1.Text = string.Format("ページ {0} / {1}", _viewer.Image.Page, _viewer.Image.PageCount);
            else
                this.label1.Text = "準備済み";
        }

        /// <summary>
        /// マルチフレームの画像ファイルを頁ごとに分割する
        /// </summary>
        /// <param name="InPath">画像ファイルパス</param>
        private void MultiTif(string InPath)
        {
            //スキャン出力画像を確認
            string[] intif = System.IO.Directory.GetFiles(InPath, "*.tif");
            if (intif.Length == 0)
            {
                MessageBox.Show("ＯＣＲ変換処理対象の振替伝票画像ファイルが指定フォルダ " + InPath + " に存在しません", "スキャナ画像確認", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

            // READフォルダがなければ作成する
            string rPath = Properties.Settings.Default.instDir + global.DIR_READ;
            if (System.IO.Directory.Exists(rPath) == false)
                System.IO.Directory.CreateDirectory(rPath);

            // READフォルダ内の全てのファイルを削除する（通常ファイルは存在しないが例外処理などで残ってしまった場合に備えて念のため）
            foreach (string files in System.IO.Directory.GetFiles(rPath, "*"))
            {
                System.IO.File.Delete(files);
            }

            RasterCodecs.Startup();
            RasterCodecs cs = new RasterCodecs();
            _pageNo = 0;
            string fnm = string.Empty;

            // １．マルチTIFを分解して画像ファイルをREADフォルダへ保存する
            foreach (string files in System.IO.Directory.GetFiles(InPath, "*.tif"))
            {
                // 画像読み出す
                RasterImage leadImg = cs.Load(files, 0, CodecsLoadByteOrder.BgrOrGray, 1, -1);

                // 頁数を取得
                int _fd_count = leadImg.PageCount;

                // 頁ごとに読み出す
                for (int i = 1; i <= _fd_count; i++)
                {
                    // ファイル名（日付時間部分）
                    string fName = string.Format("{0:0000}", DateTime.Today.Year) +
                            string.Format("{0:00}", DateTime.Today.Month) +
                            string.Format("{0:00}", DateTime.Today.Day) +
                            string.Format("{0:00}", DateTime.Now.Hour) +
                            string.Format("{0:00}", DateTime.Now.Minute) +
                            string.Format("{0:00}", DateTime.Now.Second);

                    // ファイル名設定
                    _pageNo++;
                    fnm = rPath + fName + string.Format("{0:000}", _pageNo) + ".tif";

                    // 画像保存
                    cs.Save(leadImg, fnm, RasterImageFormat.CcittGroup4, 0, i, i, 1, CodecsSavePageMode.Insert);
                }
            }

            // 2．InPathフォルダの全てのtifファイルを削除する
            foreach (var files in System.IO.Directory.GetFiles(InPath, "*.tif"))
            {
                System.IO.File.Delete(files);
            }
        }

        /// <summary>
        /// OCR処理を実施します
        /// </summary>
        /// <param name="InPath">入力パス</param>
        /// <param name="NgPath">NG出力パス</param>
        /// <param name="rePath">OCR変換結果出力パス</param>
        /// <param name="FormatName">書式ファイル名</param>
        /// <param name="fCnt">書式ファイルの件数</param>
        private void ocrMain(string InPath, string NgPath, string rePath, string FormatName, int fCnt)
        {
            IEngine en = null;		            // OCRエンジンのインスタンスを保持
            string ocr_csv = string.Empty;      // OCR変換出力CSVファイル
            int _ngCount = 0;                   // フォーマットアンマッチ画像枚数
            string fnm = string.Empty;          // ファイル名
            string path2Fdir = Properties.Settings.Default.instDir + global.DIR_2F;    // ２F伝票フォルダ

            try
            {
                // 指定された出力先フォルダがなければ作成する
                if (System.IO.Directory.Exists(rePath) == false) System.IO.Directory.CreateDirectory(rePath);

                // 指定されたNGの場合の出力先フォルダがなければ作成する
                if (System.IO.Directory.Exists(NgPath) == false) System.IO.Directory.CreateDirectory(NgPath);

                // 2F伝票フォルダがなければ作成する
                if (System.IO.Directory.Exists(path2Fdir) == false) System.IO.Directory.CreateDirectory(path2Fdir);

                // OCRエンジンのインスタンスの生成・取得
                en = EngineFactory.GetEngine();
                if (en == null)
                {
                    // エンジンが他で取得されている場合は、Release() されるまで取得できない
                    System.Console.WriteLine("SDKは使用中です");
                    return;
                }

                //オーナーフォームを無効にする
                this.Enabled = false;

                //プログレスバーを表示する
                frmPrg frmP = new frmPrg();
                frmP.Owner = this;
                frmP.Show();

                IFormatList FormatList;
                IFormat Format;
                IField Field;
                int nPage;
                int ocrPage = 0;
                int fileCount = 0;

                // フォーマットのロード・設定
                FormatList = en.FormatList;
                FormatList.Add(FormatName);

                // tifファイルの認識
                foreach (string files in System.IO.Directory.GetFiles(InPath, "*.tif"))
                {
                    nPage = 1;
                    while (true)
                    {
                        try
                        {
                            // 対象画像を設定する
                            en.SetBitmap(files, nPage);

                            //プログレスバー表示
                            fileCount++;
                            frmP.Text = "OCR変換処理実行中　" + fileCount.ToString() + "/" + fCnt.ToString();
                            frmP.progressValue = fileCount * 100 / fCnt;
                            frmP.ProgressStep();
                        }
                        catch (IDRException ex)
                        {
                            // ページ読み込みエラー
                            if (ex.No == ErrorCode.IDR_ERROR_FORM_FILEREAD)
                            {
                                // ページの終了
                                break;
                            }
                            else
                            {
                                // 例外のキャッチ
                                MessageBox.Show("例外が発生しました：Error No ={0:X}", ex.No.ToString());
                            }
                        }

                        //////Console.WriteLine("-----" + strImageFile + "の" + nPage + "ページ-----");
                        // 現在ロードされている画像を自動的に傾き補正する
                        en.AutoSkew();

                        // 傾き角度の取得
                        double angle = en.GetSkewAngle();
                        //////System.Console.WriteLine("時計回りに" + angle + "度傾き補正を行いました");

                        try
                        {
                            // 現在ロードされている画像を自動回転してマッチする番号を取得する
                            Format = en.MatchFormatRotate();
                            int direct = en.GetRotateAngle();

                            //画像ロード
                            RasterCodecs.Startup();
                            RasterCodecs cs = new RasterCodecs();
                            //RasterImage img;

                            // 描画時に使用される速度、品質、およびスタイルを制御します。 
                            //RasterPaintProperties prop = new RasterPaintProperties();
                            //prop = RasterPaintProperties.Default;
                            //prop.PaintDisplayMode = RasterPaintDisplayModeFlags.Resample;
                            //leadImg.PaintProperties = prop;

                            RasterImage img = cs.Load(files, 0, CodecsLoadByteOrder.BgrOrGray, 1, 1);

                            RotateCommand rc = new RotateCommand();
                            rc.Angle = (direct) * 90 * 100;
                            rc.FillColor = new RasterColor(255, 255, 255);
                            rc.Flags = RotateCommandFlags.Resize;
                            rc.Run(img);
                            //rc.Run(leadImg.Image);

                            //cs.Save(leadImg.Image, files, RasterImageFormat.Tif, 0, 1, 1, 1, CodecsSavePageMode.Overwrite);
                            cs.Save(img, files, RasterImageFormat.CcittGroup4, 0, 1, 1, 1, CodecsSavePageMode.Overwrite);

                            // マッチしたフォーマットに登録されているフィールド数を取得
                            int fieldNum = Format.NumOfFields;
                            int matchNum = Format.FormatNo + 1;
                            //////System.Console.WriteLine(matchNum + "番目のフォーマットがマッチ");
                            int i = 1;
                            int fIndex = 0;
                            ocr_csv = "*,";
                            
                            // ファイルに画像ファイル名フィールドを付加します
                            ocr_csv += System.IO.Path.GetFileName(files);

                            // 認識されたフィールドを順次読み出します
                            Field = Format.Begin();

                            // ３F伝票処理時に２F伝票を判定する 2013/07/01
                            if (FormatName == Properties.Settings.Default.instDir + Properties.Settings.Default.fmtHPath)
                            {
                                string fldText = string.Empty;
                                while (Field != null)
                                {
                                    // 指定フィールドを認識し、テキストを取得（対象は最終フィールド）
                                    fldText = Field.ExtractFieldText();

                                    // 次のフィールドの取得
                                    Field = Format.Next();
                                }

                                // 再度認識されたフィールドを順次読み出します 2013/07/01
                                Field = Format.Begin();

                                // ２F書式伝票のとき（指定フィールドが空白である）
                                if (fldText.Trim().Length == 0)
                                {
                                    // ２F伝票フォルダへ移動する
                                    System.IO.File.Move(files, path2Fdir + System.IO.Path.GetFileName(files));

                                    // ページをカウントして次の画像のOCR処理へ
                                    ocrPage++;
                                    nPage += 1;
                                    continue;
                                }
                            }

                            // 伝票フィールド編集
                            while (Field != null)
                            {
                                //カンマ付加
                                if (ocr_csv != string.Empty) ocr_csv += ",";

                                // 指定フィールドを認識し、テキストを取得
                                string strText = Field.ExtractFieldText();

                                // 年月日のとき各々独立フィールドに分解します
                                if (fIndex == 1)
                                {
                                    string strYYMMDD = strText.PadRight(6, '0');
                                    string ymd = strYYMMDD.Substring(0, 2) + "," + strYYMMDD.Substring(2, 2) + "," + strYYMMDD.Substring(4, 2);
                                    ocr_csv += ymd;
                                }
                                else if (fIndex != 165)
                                {
                                    ocr_csv += strText;    // 他のフィールドで最終フィールド以外(2013/07/01)
                                }

                                // 摘要複写欄
                                if (fIndex == 10 || fIndex == 19 || fIndex == 28 || fIndex == 37 ||
                                    fIndex == 46 || fIndex == 55 || fIndex == 64 || fIndex == 73 || fIndex == 82 ||
                                    fIndex == 91 || fIndex == 100 || fIndex == 109 || fIndex == 118 || fIndex == 127 ||
                                    fIndex == 136 || fIndex == 145 || fIndex == 154 || fIndex == 163)
                                {
                                    ocr_csv += ",0";
                                }

                                // 改行
                                if (fIndex == 2 || fIndex == 11 || fIndex == 20 || fIndex == 29 || fIndex == 38 ||
                                    fIndex == 47 || fIndex == 56 || fIndex == 65 || fIndex == 74 || fIndex == 83 ||
                                    fIndex == 92 || fIndex == 101 || fIndex == 110 || fIndex == 119 || fIndex == 128 ||
                                    fIndex == 137 || fIndex == 146 || fIndex == 155)
                                {
                                    // ヘッダ業改行のとき明細行数を付加
                                    if (fIndex == 2)
                                    {
                                        ocr_csv += ",";
                                        ocr_csv += global.MAXGYOU_PRN.ToString();
                                    }

                                    ocr_csv += Environment.NewLine;

                                    // 取消欄
                                    ocr_csv += "0";
                                }

                                // 次のフィールドの取得
                                Field = Format.Next();
                                i += 1;

                                // フィールドインデックスインクリメント
                                fIndex++;
                            }

                            //出力ファイル
                            System.IO.StreamWriter outFile = new System.IO.StreamWriter(InPath + System.IO.Path.GetFileNameWithoutExtension(files) + ".csv", false, System.Text.Encoding.GetEncoding(932));
                            outFile.WriteLine(ocr_csv);
                            outFile.Close();

                            //OCR変換枚数カウント
                            _okCount++;
                        }
                        catch (IDRWarning ex)
                        {
                            // Engine.MatchFormatRotate() で
                            // フォーマットにマッチしなかった場合、空ファイルを出力します
                            if (ex.No == ErrorCode.IDR_WARN_FORM_NO_MATCH)
                            {
                                //////// アンマッチフォルダへ移動する
                                //////System.IO.File.Move(files, NgPath + System.IO.Path.GetFileName(files));

                                // 区切り文字
                                ocr_csv = "*,";

                                // ファイルに画像ファイル名フィールドを付加します
                                ocr_csv += System.IO.Path.GetFileName(files);

                                // ヘッダ部 （決算仕訳区分、年、月、日、伝票№（ＮＧ））
                                ocr_csv += ",0,,,,NG," + global.MAXGYOU_PRN.ToString() + Environment.NewLine; 

                                //// 明細部
                                string meisai = "0,,,,,,,,,0," + Environment.NewLine;
                                for (int i = 0; i < 18; i++)
                                {
                                    ocr_csv += meisai;
                                }

                                //出力ファイル
                                System.IO.StreamWriter outFile = new System.IO.StreamWriter(InPath + System.IO.Path.GetFileNameWithoutExtension(files) + ".csv", false, System.Text.Encoding.GetEncoding(932));
                                outFile.WriteLine(ocr_csv);
                                outFile.Close();

                                _ngCount++;　//NG枚数カウント
                            }
                        }

                        ocrPage++;
                        nPage += 1;
                    }
                }

                // いったんオーナーをアクティブにする
                this.Activate();

                // 進行状況ダイアログを閉じる
                frmP.Close();

                // オーナーのフォームを有効に戻す
                this.Enabled = true;

                // OCR変換画像とCSVデータをOCR結果出力フォルダへ移動する            
                foreach (string files in System.IO.Directory.GetFiles(InPath, "*.*"))
                {
                    System.IO.File.Move(files, rePath + System.IO.Path.GetFileName(files));
                }

                // 終了メッセージ （２F伝票画像がなければ処理終了とみなす）
                var f2Tif = System.IO.Directory.GetFileSystemEntries(Properties.Settings.Default.instDir + global.DIR_2F, "*.tif");

                if (f2Tif.Length == 0)
                {
                    string finMessage = string.Empty;
                    StringBuilder sb = new StringBuilder();
                    sb.Append("ＯＣＲ認識処理が終了しました。");
                    sb.Append("引き続き修正確認＆受け渡しデータ作成を行ってください。");
                    sb.Append(Environment.NewLine);
                    sb.Append(Environment.NewLine);
                    sb.Append("OK件数 : ");
                    sb.Append(_okCount.ToString());
                    sb.Append(Environment.NewLine);
                    sb.Append("NG件数 : ");
                    sb.Append(_ngCount.ToString());
                    sb.Append(Environment.NewLine);

                    MessageBox.Show(sb.ToString(), "処理終了", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }

                FormatList.Delete(0);
            }
            catch (System.Exception ex)
            {
                // 例外のキャッチ
                string errMessage = string.Empty;
                errMessage += "System例外が発生しました：" + Environment.NewLine;
                errMessage += "必要なDLL等が実行モジュールと同ディレクトリに存在するか確認してください。：" + Environment.NewLine;
                errMessage += ex.Message.ToString();
                MessageBox.Show(errMessage, "エラー", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            finally
            {
                en.Release();
            }
        }
    }
}
