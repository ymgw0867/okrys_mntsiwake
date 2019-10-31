namespace mntsiwake
{
    partial class Base
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle5 = new System.Windows.Forms.DataGridViewCellStyle();
            GrapeCity.Win.MultiRow.CellStyle cellStyle1 = new GrapeCity.Win.MultiRow.CellStyle();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Base));
            this.btnDltDen = new System.Windows.Forms.Button();
            this.cmdExit = new System.Windows.Forms.Button();
            this.lblNowDen = new System.Windows.Forms.Label();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.cmdMinus = new System.Windows.Forms.Button();
            this.btnFirst = new System.Windows.Forms.Button();
            this.btnBefore = new System.Windows.Forms.Button();
            this.btnNext = new System.Windows.Forms.Button();
            this.btnEnd = new System.Windows.Forms.Button();
            this.cmdPlus = new System.Windows.Forms.Button();
            this.tabData = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.pictureBox2 = new System.Windows.Forms.PictureBox();
            this.lblErr = new System.Windows.Forms.Label();
            this.fgNg = new System.Windows.Forms.DataGridView();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.fgHojo = new System.Windows.Forms.DataGridView();
            this.fgKamoku = new System.Windows.Forms.DataGridView();
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.fgBumon = new System.Windows.Forms.DataGridView();
            this.tabPage5 = new System.Windows.Forms.TabPage();
            this.fgCom = new System.Windows.Forms.DataGridView();
            this.label1 = new System.Windows.Forms.Label();
            this.txtYear = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.txtMonth = new System.Windows.Forms.TextBox();
            this.txtDay = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.ChkKessan = new System.Windows.Forms.CheckBox();
            this.hScrollBar1 = new System.Windows.Forms.HScrollBar();
            this.ChkErrColor = new System.Windows.Forms.CheckBox();
            this.lblGengo = new System.Windows.Forms.Label();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.leadImg = new Leadtools.WinForms.RasterImageViewer();
            this.btnOk = new System.Windows.Forms.Button();
            this.saveFileDialog1 = new System.Windows.Forms.SaveFileDialog();
            this.gcMultiRow1 = new GrapeCity.Win.MultiRow.GcMultiRow();
            this.template12 = new mntsiwake.Template1();
            this.label5 = new System.Windows.Forms.Label();
            this.txtDenNo = new System.Windows.Forms.TextBox();
            this.tabData.SuspendLayout();
            this.tabPage1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.fgNg)).BeginInit();
            this.tabPage2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.fgHojo)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.fgKamoku)).BeginInit();
            this.tabPage3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.fgBumon)).BeginInit();
            this.tabPage5.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.fgCom)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // btnDltDen
            // 
            this.btnDltDen.Font = new System.Drawing.Font("ＭＳ Ｐゴシック", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.btnDltDen.Location = new System.Drawing.Point(928, 10);
            this.btnDltDen.Name = "btnDltDen";
            this.btnDltDen.Size = new System.Drawing.Size(69, 27);
            this.btnDltDen.TabIndex = 4;
            this.btnDltDen.TabStop = false;
            this.btnDltDen.Text = "削除(&D)";
            this.toolTip1.SetToolTip(this.btnDltDen, "表示中の伝票を削除します");
            this.btnDltDen.UseVisualStyleBackColor = true;
            this.btnDltDen.Click += new System.EventHandler(this.btnDltDen_Click);
            // 
            // cmdExit
            // 
            this.cmdExit.Font = new System.Drawing.Font("ＭＳ Ｐゴシック", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.cmdExit.Location = new System.Drawing.Point(1003, 10);
            this.cmdExit.Name = "cmdExit";
            this.cmdExit.Size = new System.Drawing.Size(69, 27);
            this.cmdExit.TabIndex = 6;
            this.cmdExit.TabStop = false;
            this.cmdExit.Text = "終了(&X)";
            this.toolTip1.SetToolTip(this.cmdExit, "処理を終了します");
            this.cmdExit.UseVisualStyleBackColor = true;
            this.cmdExit.Click += new System.EventHandler(this.cmdExit_Click);
            // 
            // lblNowDen
            // 
            this.lblNowDen.Font = new System.Drawing.Font("ＭＳ Ｐゴシック", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.lblNowDen.Location = new System.Drawing.Point(730, 10);
            this.lblNowDen.Name = "lblNowDen";
            this.lblNowDen.Size = new System.Drawing.Size(86, 25);
            this.lblNowDen.TabIndex = 7;
            this.lblNowDen.Text = "0000";
            this.lblNowDen.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // cmdMinus
            // 
            this.cmdMinus.Image = global::mntsiwake.Properties.Resources.tvuZoomOut;
            this.cmdMinus.Location = new System.Drawing.Point(697, 10);
            this.cmdMinus.Name = "cmdMinus";
            this.cmdMinus.Size = new System.Drawing.Size(27, 27);
            this.cmdMinus.TabIndex = 9;
            this.cmdMinus.TabStop = false;
            this.toolTip1.SetToolTip(this.cmdMinus, "画像を縮小します。");
            this.cmdMinus.UseVisualStyleBackColor = true;
            this.cmdMinus.Click += new System.EventHandler(this.cmdMinus_Click);
            // 
            // btnFirst
            // 
            this.btnFirst.Location = new System.Drawing.Point(381, 497);
            this.btnFirst.Name = "btnFirst";
            this.btnFirst.Size = new System.Drawing.Size(59, 22);
            this.btnFirst.TabIndex = 22;
            this.btnFirst.TabStop = false;
            this.btnFirst.Text = "|<< (&S)";
            this.toolTip1.SetToolTip(this.btnFirst, "一番前の伝票");
            this.btnFirst.UseVisualStyleBackColor = true;
            this.btnFirst.Click += new System.EventHandler(this.btnFirst_Click);
            // 
            // btnBefore
            // 
            this.btnBefore.Font = new System.Drawing.Font("ＭＳ Ｐゴシック", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.btnBefore.Location = new System.Drawing.Point(440, 497);
            this.btnBefore.Name = "btnBefore";
            this.btnBefore.Size = new System.Drawing.Size(59, 22);
            this.btnBefore.TabIndex = 23;
            this.btnBefore.TabStop = false;
            this.btnBefore.Text = "前伝票(&R)";
            this.toolTip1.SetToolTip(this.btnBefore, "前の伝票");
            this.btnBefore.UseVisualStyleBackColor = true;
            this.btnBefore.Click += new System.EventHandler(this.btnBefore_Click);
            // 
            // btnNext
            // 
            this.btnNext.Font = new System.Drawing.Font("ＭＳ Ｐゴシック", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.btnNext.Location = new System.Drawing.Point(499, 497);
            this.btnNext.Name = "btnNext";
            this.btnNext.Size = new System.Drawing.Size(59, 22);
            this.btnNext.TabIndex = 24;
            this.btnNext.TabStop = false;
            this.btnNext.Text = "次伝票(&N)";
            this.toolTip1.SetToolTip(this.btnNext, "次の伝票");
            this.btnNext.UseVisualStyleBackColor = true;
            this.btnNext.Click += new System.EventHandler(this.btnNext_Click);
            // 
            // btnEnd
            // 
            this.btnEnd.Location = new System.Drawing.Point(558, 497);
            this.btnEnd.Name = "btnEnd";
            this.btnEnd.Size = new System.Drawing.Size(59, 22);
            this.btnEnd.TabIndex = 25;
            this.btnEnd.TabStop = false;
            this.btnEnd.Text = ">>| (&L)";
            this.toolTip1.SetToolTip(this.btnEnd, "一番後ろの伝票");
            this.btnEnd.UseVisualStyleBackColor = true;
            this.btnEnd.Click += new System.EventHandler(this.btnEnd_Click);
            // 
            // cmdPlus
            // 
            this.cmdPlus.Image = global::mntsiwake.Properties.Resources.tvuZoomIn;
            this.cmdPlus.Location = new System.Drawing.Point(668, 10);
            this.cmdPlus.Name = "cmdPlus";
            this.cmdPlus.Size = new System.Drawing.Size(27, 27);
            this.cmdPlus.TabIndex = 8;
            this.cmdPlus.TabStop = false;
            this.toolTip1.SetToolTip(this.cmdPlus, "画像を拡大します。");
            this.cmdPlus.UseVisualStyleBackColor = true;
            this.cmdPlus.Click += new System.EventHandler(this.cmdPlus_Click);
            // 
            // tabData
            // 
            this.tabData.Controls.Add(this.tabPage1);
            this.tabData.Controls.Add(this.tabPage2);
            this.tabData.Controls.Add(this.tabPage3);
            this.tabData.Controls.Add(this.tabPage5);
            this.tabData.Font = new System.Drawing.Font("ＭＳ Ｐゴシック", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.tabData.Location = new System.Drawing.Point(668, 55);
            this.tabData.Name = "tabData";
            this.tabData.SelectedIndex = 0;
            this.tabData.Size = new System.Drawing.Size(408, 434);
            this.tabData.TabIndex = 10;
            this.tabData.TabStop = false;
            // 
            // tabPage1
            // 
            this.tabPage1.BackColor = System.Drawing.SystemColors.Control;
            this.tabPage1.Controls.Add(this.pictureBox2);
            this.tabPage1.Controls.Add(this.lblErr);
            this.tabPage1.Controls.Add(this.fgNg);
            this.tabPage1.Location = new System.Drawing.Point(4, 23);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(400, 407);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "エラー情報";
            // 
            // pictureBox2
            // 
            this.pictureBox2.Image = global::mntsiwake.Properties.Resources.mark02_s;
            this.pictureBox2.Location = new System.Drawing.Point(3, 378);
            this.pictureBox2.Name = "pictureBox2";
            this.pictureBox2.Size = new System.Drawing.Size(25, 24);
            this.pictureBox2.TabIndex = 2;
            this.pictureBox2.TabStop = false;
            // 
            // lblErr
            // 
            this.lblErr.Location = new System.Drawing.Point(27, 378);
            this.lblErr.Name = "lblErr";
            this.lblErr.Size = new System.Drawing.Size(130, 16);
            this.lblErr.TabIndex = 1;
            // 
            // fgNg
            // 
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.Lavender;
            this.fgNg.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle1;
            this.fgNg.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.fgNg.Location = new System.Drawing.Point(3, 11);
            this.fgNg.Name = "fgNg";
            this.fgNg.RowTemplate.Height = 21;
            this.fgNg.Size = new System.Drawing.Size(394, 361);
            this.fgNg.TabIndex = 0;
            this.fgNg.TabStop = false;
            this.fgNg.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.fgNg_CellDoubleClick);
            // 
            // tabPage2
            // 
            this.tabPage2.BackColor = System.Drawing.SystemColors.Control;
            this.tabPage2.Controls.Add(this.fgHojo);
            this.tabPage2.Controls.Add(this.fgKamoku);
            this.tabPage2.Location = new System.Drawing.Point(4, 23);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(400, 407);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "勘定科目・補助科目";
            // 
            // fgHojo
            // 
            dataGridViewCellStyle2.BackColor = System.Drawing.Color.Lavender;
            this.fgHojo.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle2;
            this.fgHojo.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.fgHojo.Location = new System.Drawing.Point(201, 11);
            this.fgHojo.Name = "fgHojo";
            this.fgHojo.RowTemplate.Height = 21;
            this.fgHojo.Size = new System.Drawing.Size(192, 379);
            this.fgHojo.TabIndex = 1;
            this.fgHojo.TabStop = false;
            this.fgHojo.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.fgHojo_CellDoubleClick);
            // 
            // fgKamoku
            // 
            dataGridViewCellStyle3.BackColor = System.Drawing.Color.Lavender;
            this.fgKamoku.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle3;
            this.fgKamoku.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.fgKamoku.Location = new System.Drawing.Point(3, 11);
            this.fgKamoku.Name = "fgKamoku";
            this.fgKamoku.RowTemplate.Height = 21;
            this.fgKamoku.Size = new System.Drawing.Size(192, 379);
            this.fgKamoku.TabIndex = 0;
            this.fgKamoku.TabStop = false;
            this.fgKamoku.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.fgKamoku_CellClick);
            this.fgKamoku.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.fgKamoku_CellDoubleClick);
            this.fgKamoku.SelectionChanged += new System.EventHandler(this.fgKamoku_SelectionChanged);
            // 
            // tabPage3
            // 
            this.tabPage3.BackColor = System.Drawing.SystemColors.Control;
            this.tabPage3.Controls.Add(this.fgBumon);
            this.tabPage3.Location = new System.Drawing.Point(4, 23);
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.Size = new System.Drawing.Size(400, 407);
            this.tabPage3.TabIndex = 2;
            this.tabPage3.Text = "枝番（部門）";
            // 
            // fgBumon
            // 
            dataGridViewCellStyle4.BackColor = System.Drawing.Color.Lavender;
            this.fgBumon.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle4;
            this.fgBumon.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.fgBumon.Location = new System.Drawing.Point(3, 11);
            this.fgBumon.Name = "fgBumon";
            this.fgBumon.RowTemplate.Height = 21;
            this.fgBumon.Size = new System.Drawing.Size(394, 379);
            this.fgBumon.TabIndex = 2;
            this.fgBumon.TabStop = false;
            this.fgBumon.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.fgBumon_CellDoubleClick);
            // 
            // tabPage5
            // 
            this.tabPage5.BackColor = System.Drawing.SystemColors.Control;
            this.tabPage5.Controls.Add(this.fgCom);
            this.tabPage5.Location = new System.Drawing.Point(4, 23);
            this.tabPage5.Name = "tabPage5";
            this.tabPage5.Size = new System.Drawing.Size(400, 407);
            this.tabPage5.TabIndex = 4;
            this.tabPage5.Text = "会社情報";
            // 
            // fgCom
            // 
            dataGridViewCellStyle5.BackColor = System.Drawing.Color.Lavender;
            this.fgCom.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle5;
            this.fgCom.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.fgCom.Location = new System.Drawing.Point(3, 11);
            this.fgCom.Name = "fgCom";
            this.fgCom.RowTemplate.Height = 21;
            this.fgCom.Size = new System.Drawing.Size(391, 379);
            this.fgCom.TabIndex = 0;
            this.fgCom.TabStop = false;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 502);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(29, 12);
            this.label1.TabIndex = 11;
            this.label1.Text = "日付";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // txtYear
            // 
            this.txtYear.ImeMode = System.Windows.Forms.ImeMode.Off;
            this.txtYear.Location = new System.Drawing.Point(73, 499);
            this.txtYear.MaxLength = 2;
            this.txtYear.Name = "txtYear";
            this.txtYear.Size = new System.Drawing.Size(26, 19);
            this.txtYear.TabIndex = 12;
            this.txtYear.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.txtYear.WordWrap = false;
            this.txtYear.Enter += new System.EventHandler(this.txtYear_Enter);
            this.txtYear.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txt_KeyPress);
            this.txtYear.Leave += new System.EventHandler(this.txtYear_Leave);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(101, 503);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(17, 12);
            this.label2.TabIndex = 13;
            this.label2.Text = "年";
            // 
            // txtMonth
            // 
            this.txtMonth.ImeMode = System.Windows.Forms.ImeMode.Off;
            this.txtMonth.Location = new System.Drawing.Point(118, 499);
            this.txtMonth.MaxLength = 2;
            this.txtMonth.Name = "txtMonth";
            this.txtMonth.Size = new System.Drawing.Size(23, 19);
            this.txtMonth.TabIndex = 14;
            this.txtMonth.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.txtMonth.WordWrap = false;
            this.txtMonth.Enter += new System.EventHandler(this.txtYear_Enter);
            this.txtMonth.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txt_KeyPress);
            this.txtMonth.Leave += new System.EventHandler(this.txtYear_Leave);
            // 
            // txtDay
            // 
            this.txtDay.ImeMode = System.Windows.Forms.ImeMode.Off;
            this.txtDay.Location = new System.Drawing.Point(160, 499);
            this.txtDay.MaxLength = 2;
            this.txtDay.Name = "txtDay";
            this.txtDay.Size = new System.Drawing.Size(23, 19);
            this.txtDay.TabIndex = 15;
            this.txtDay.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.txtDay.WordWrap = false;
            this.txtDay.Enter += new System.EventHandler(this.txtYear_Enter);
            this.txtDay.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txt_KeyPress);
            this.txtDay.Leave += new System.EventHandler(this.txtYear_Leave);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(141, 503);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(17, 12);
            this.label3.TabIndex = 16;
            this.label3.Text = "月";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(184, 503);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(17, 12);
            this.label4.TabIndex = 17;
            this.label4.Text = "日";
            // 
            // ChkKessan
            // 
            this.ChkKessan.Location = new System.Drawing.Point(214, 501);
            this.ChkKessan.Name = "ChkKessan";
            this.ChkKessan.Size = new System.Drawing.Size(48, 16);
            this.ChkKessan.TabIndex = 16;
            this.ChkKessan.Text = "決算";
            this.ChkKessan.UseVisualStyleBackColor = true;
            // 
            // hScrollBar1
            // 
            this.hScrollBar1.Location = new System.Drawing.Point(621, 497);
            this.hScrollBar1.Name = "hScrollBar1";
            this.hScrollBar1.Size = new System.Drawing.Size(298, 22);
            this.hScrollBar1.TabIndex = 26;
            this.hScrollBar1.Scroll += new System.Windows.Forms.ScrollEventHandler(this.hScrollBar1_Scroll);
            // 
            // ChkErrColor
            // 
            this.ChkErrColor.AutoSize = true;
            this.ChkErrColor.Font = new System.Drawing.Font("ＭＳ Ｐゴシック", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.ChkErrColor.Location = new System.Drawing.Point(922, 501);
            this.ChkErrColor.Name = "ChkErrColor";
            this.ChkErrColor.Size = new System.Drawing.Size(120, 16);
            this.ChkErrColor.TabIndex = 27;
            this.ChkErrColor.TabStop = false;
            this.ChkErrColor.Text = "NG項目カラー表示";
            this.ChkErrColor.UseVisualStyleBackColor = true;
            this.ChkErrColor.CheckedChanged += new System.EventHandler(this.ChkErrColor_CheckedChanged);
            this.ChkErrColor.Click += new System.EventHandler(this.ChkErrColor_Click);
            // 
            // lblGengo
            // 
            this.lblGengo.BackColor = System.Drawing.SystemColors.Control;
            this.lblGengo.Location = new System.Drawing.Point(40, 499);
            this.lblGengo.Name = "lblGengo";
            this.lblGengo.Size = new System.Drawing.Size(32, 19);
            this.lblGengo.TabIndex = 29;
            this.lblGengo.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // pictureBox1
            // 
            this.pictureBox1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.pictureBox1.Location = new System.Drawing.Point(2, 2);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(661, 487);
            this.pictureBox1.TabIndex = 30;
            this.pictureBox1.TabStop = false;
            // 
            // leadImg
            // 
            this.leadImg.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.leadImg.Location = new System.Drawing.Point(5, 4);
            this.leadImg.Name = "leadImg";
            this.leadImg.Size = new System.Drawing.Size(655, 481);
            this.leadImg.TabIndex = 47;
            // 
            // btnOk
            // 
            this.btnOk.Location = new System.Drawing.Point(853, 10);
            this.btnOk.Name = "btnOk";
            this.btnOk.Size = new System.Drawing.Size(69, 27);
            this.btnOk.TabIndex = 48;
            this.btnOk.Text = "作成(&E)";
            this.btnOk.UseVisualStyleBackColor = true;
            this.btnOk.Click += new System.EventHandler(this.button1_Click_2);
            // 
            // gcMultiRow1
            // 
            this.gcMultiRow1.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.gcMultiRow1.EditMode = GrapeCity.Win.MultiRow.EditMode.EditOnEnter;
            this.gcMultiRow1.Location = new System.Drawing.Point(12, 525);
            this.gcMultiRow1.Name = "gcMultiRow1";
            cellStyle1.SelectionBackColor = System.Drawing.Color.Blue;
            this.gcMultiRow1.RowsDefaultCellStyle = cellStyle1;
            this.gcMultiRow1.ScrollBars = System.Windows.Forms.ScrollBars.None;
            this.gcMultiRow1.Size = new System.Drawing.Size(1050, 315);
            this.gcMultiRow1.SplitMode = GrapeCity.Win.MultiRow.SplitMode.None;
            this.gcMultiRow1.TabIndex = 28;
            this.gcMultiRow1.TabStop = false;
            this.gcMultiRow1.Template = this.template12;
            this.gcMultiRow1.Text = "gcMultiRow1";
            this.gcMultiRow1.CellValueChanged += new System.EventHandler<GrapeCity.Win.MultiRow.CellEventArgs>(this.gcMultiRow1_CellValueChanged);
            this.gcMultiRow1.CellEnter += new System.EventHandler<GrapeCity.Win.MultiRow.CellEventArgs>(this.gcMultiRow1_CellEnter);
            this.gcMultiRow1.CellClick += new System.EventHandler<GrapeCity.Win.MultiRow.CellEventArgs>(this.gcMultiRow1_CellClick);
            this.gcMultiRow1.CurrentCellDirtyStateChanged += new System.EventHandler(this.gcMultiRow1_CurrentCellDirtyStateChanged);
            this.gcMultiRow1.Enter += new System.EventHandler(this.gcMultiRow1_Enter);
            this.gcMultiRow1.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.gcMultiRow1_KeyPress);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(271, 503);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(41, 12);
            this.label5.TabIndex = 49;
            this.label5.Text = "伝票№";
            // 
            // txtDenNo
            // 
            this.txtDenNo.ImeMode = System.Windows.Forms.ImeMode.Off;
            this.txtDenNo.Location = new System.Drawing.Point(314, 499);
            this.txtDenNo.MaxLength = 6;
            this.txtDenNo.Name = "txtDenNo";
            this.txtDenNo.Size = new System.Drawing.Size(57, 19);
            this.txtDenNo.TabIndex = 17;
            this.txtDenNo.Text = "123456";
            this.txtDenNo.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.txtDenNo.WordWrap = false;
            this.txtDenNo.Enter += new System.EventHandler(this.txtYear_Enter);
            this.txtDenNo.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txt_KeyPress);
            this.txtDenNo.Leave += new System.EventHandler(this.txtYear_Leave);
            // 
            // Base
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.ClientSize = new System.Drawing.Size(1078, 867);
            this.Controls.Add(this.txtDenNo);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.btnOk);
            this.Controls.Add(this.leadImg);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.lblGengo);
            this.Controls.Add(this.gcMultiRow1);
            this.Controls.Add(this.ChkErrColor);
            this.Controls.Add(this.hScrollBar1);
            this.Controls.Add(this.btnEnd);
            this.Controls.Add(this.btnNext);
            this.Controls.Add(this.btnBefore);
            this.Controls.Add(this.btnFirst);
            this.Controls.Add(this.ChkKessan);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.txtDay);
            this.Controls.Add(this.txtMonth);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.txtYear);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.tabData);
            this.Controls.Add(this.cmdMinus);
            this.Controls.Add(this.cmdPlus);
            this.Controls.Add(this.lblNowDen);
            this.Controls.Add(this.cmdExit);
            this.Controls.Add(this.btnDltDen);
            this.Font = new System.Drawing.Font("ＭＳ Ｐゴシック", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "Base";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Base";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Base_FormClosing);
            this.Load += new System.EventHandler(this.Base_Load);
            this.Shown += new System.EventHandler(this.Base_Shown);
            this.tabData.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.fgNg)).EndInit();
            this.tabPage2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.fgHojo)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.fgKamoku)).EndInit();
            this.tabPage3.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.fgBumon)).EndInit();
            this.tabPage5.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.fgCom)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.Button btnDltDen;
        private System.Windows.Forms.Button cmdExit;
        private System.Windows.Forms.Label lblNowDen;
        private System.Windows.Forms.Button cmdPlus;
        private System.Windows.Forms.Button cmdMinus;
        private System.Windows.Forms.TabControl tabData;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.DataGridView fgNg;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.DataGridView fgHojo;
        private System.Windows.Forms.DataGridView fgKamoku;
        private System.Windows.Forms.TabPage tabPage3;
        private System.Windows.Forms.DataGridView fgBumon;
        private System.Windows.Forms.TabPage tabPage5;
        private System.Windows.Forms.DataGridView fgCom;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtYear;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtMonth;
        private System.Windows.Forms.TextBox txtDay;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.CheckBox ChkKessan;
        private System.Windows.Forms.Button btnFirst;
        private System.Windows.Forms.Button btnBefore;
        private System.Windows.Forms.Button btnNext;
        private System.Windows.Forms.Button btnEnd;
        private System.Windows.Forms.HScrollBar hScrollBar1;
        private System.Windows.Forms.CheckBox ChkErrColor;
        private System.Windows.Forms.Label lblGengo;
        private Template1 template12;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Label lblErr;
        private System.Windows.Forms.PictureBox pictureBox2;
        private Leadtools.WinForms.RasterImageViewer leadImg;
        private System.Windows.Forms.Button btnOk;
        private System.Windows.Forms.SaveFileDialog saveFileDialog1;
        private GrapeCity.Win.MultiRow.GcMultiRow gcMultiRow1;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox txtDenNo;
    }
}