namespace mntsiwake
{
    partial class frmShiwakePrn
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle6 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle7 = new System.Windows.Forms.DataGridViewCellStyle();
            GrapeCity.Win.MultiRow.CellStyle cellStyle1 = new GrapeCity.Win.MultiRow.CellStyle();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmShiwakePrn));
            this.btnDltDen = new System.Windows.Forms.Button();
            this.cmdExit = new System.Windows.Forms.Button();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.button2 = new System.Windows.Forms.Button();
            this.btnOk = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.txtYear = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.txtMonth = new System.Windows.Forms.TextBox();
            this.txtDay = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.ChkKessan = new System.Windows.Forms.CheckBox();
            this.chkFukusuChk = new System.Windows.Forms.CheckBox();
            this.lblGengo = new System.Windows.Forms.Label();
            this.saveFileDialog1 = new System.Windows.Forms.SaveFileDialog();
            this.tabPage5 = new System.Windows.Forms.TabPage();
            this.fgCom = new System.Windows.Forms.DataGridView();
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.fgJigyo = new System.Windows.Forms.DataGridView();
            this.fgBumon = new System.Windows.Forms.DataGridView();
            this.fgTaxMas = new System.Windows.Forms.DataGridView();
            this.fgTax = new System.Windows.Forms.DataGridView();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.fgHojo = new System.Windows.Forms.DataGridView();
            this.fgKamoku = new System.Windows.Forms.DataGridView();
            this.tabData = new System.Windows.Forms.TabControl();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.gcMultiRow1 = new GrapeCity.Win.MultiRow.GcMultiRow();
            this.tempPrn1 = new mntsiwake.TempPrn();
            this.tabPage5.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.fgCom)).BeginInit();
            this.tabPage3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.fgJigyo)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.fgBumon)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.fgTaxMas)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.fgTax)).BeginInit();
            this.tabPage2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.fgHojo)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.fgKamoku)).BeginInit();
            this.tabData.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnDltDen
            // 
            this.btnDltDen.Font = new System.Drawing.Font("ＭＳ Ｐゴシック", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.btnDltDen.Location = new System.Drawing.Point(778, 717);
            this.btnDltDen.Name = "btnDltDen";
            this.btnDltDen.Size = new System.Drawing.Size(127, 31);
            this.btnDltDen.TabIndex = 4;
            this.btnDltDen.TabStop = false;
            this.btnDltDen.Text = "パターン登録(&D)";
            this.toolTip1.SetToolTip(this.btnDltDen, "表示中の伝票をパターン登録します");
            this.btnDltDen.UseVisualStyleBackColor = true;
            this.btnDltDen.Click += new System.EventHandler(this.btnDltDen_Click);
            // 
            // cmdExit
            // 
            this.cmdExit.Font = new System.Drawing.Font("ＭＳ Ｐゴシック", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.cmdExit.Location = new System.Drawing.Point(911, 717);
            this.cmdExit.Name = "cmdExit";
            this.cmdExit.Size = new System.Drawing.Size(99, 31);
            this.cmdExit.TabIndex = 6;
            this.cmdExit.TabStop = false;
            this.cmdExit.Text = "終了(&X)";
            this.toolTip1.SetToolTip(this.cmdExit, "処理を終了します");
            this.cmdExit.UseVisualStyleBackColor = true;
            this.cmdExit.Click += new System.EventHandler(this.cmdExit_Click);
            // 
            // button2
            // 
            this.button2.Font = new System.Drawing.Font("ＭＳ Ｐゴシック", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.button2.Location = new System.Drawing.Point(512, 717);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(127, 31);
            this.button2.TabIndex = 50;
            this.button2.TabStop = false;
            this.button2.Text = "取消(&C)";
            this.toolTip1.SetToolTip(this.button2, "表示中の伝票を消去します");
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // btnOk
            // 
            this.btnOk.Location = new System.Drawing.Point(645, 717);
            this.btnOk.Name = "btnOk";
            this.btnOk.Size = new System.Drawing.Size(127, 31);
            this.btnOk.TabIndex = 48;
            this.btnOk.Tag = "";
            this.btnOk.Text = "振替伝票印刷(&P)";
            this.toolTip1.SetToolTip(this.btnOk, "表示中の伝票を印刷します");
            this.btnOk.UseVisualStyleBackColor = true;
            this.btnOk.Click += new System.EventHandler(this.button1_Click_2);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(812, 11);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(198, 27);
            this.button1.TabIndex = 49;
            this.button1.Tag = "";
            this.button1.Text = "パターン呼び出し(&R)";
            this.toolTip1.SetToolTip(this.button1, "仕訳パターンを呼び出します");
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click_1);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("ＭＳ Ｐゴシック", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.label1.Location = new System.Drawing.Point(25, 15);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(37, 15);
            this.label1.TabIndex = 11;
            this.label1.Text = "日付";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // txtYear
            // 
            this.txtYear.Font = new System.Drawing.Font("ＭＳ Ｐゴシック", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.txtYear.ImeMode = System.Windows.Forms.ImeMode.Off;
            this.txtYear.Location = new System.Drawing.Point(116, 11);
            this.txtYear.MaxLength = 2;
            this.txtYear.Name = "txtYear";
            this.txtYear.Size = new System.Drawing.Size(33, 22);
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
            this.label2.Font = new System.Drawing.Font("ＭＳ Ｐゴシック", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.label2.Location = new System.Drawing.Point(151, 15);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(22, 15);
            this.label2.TabIndex = 13;
            this.label2.Text = "年";
            // 
            // txtMonth
            // 
            this.txtMonth.Font = new System.Drawing.Font("ＭＳ Ｐゴシック", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.txtMonth.ImeMode = System.Windows.Forms.ImeMode.Off;
            this.txtMonth.Location = new System.Drawing.Point(174, 11);
            this.txtMonth.MaxLength = 2;
            this.txtMonth.Name = "txtMonth";
            this.txtMonth.Size = new System.Drawing.Size(23, 22);
            this.txtMonth.TabIndex = 14;
            this.txtMonth.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.txtMonth.WordWrap = false;
            this.txtMonth.Enter += new System.EventHandler(this.txtYear_Enter);
            this.txtMonth.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txt_KeyPress);
            this.txtMonth.Leave += new System.EventHandler(this.txtYear_Leave);
            // 
            // txtDay
            // 
            this.txtDay.Font = new System.Drawing.Font("ＭＳ Ｐゴシック", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.txtDay.ImeMode = System.Windows.Forms.ImeMode.Off;
            this.txtDay.Location = new System.Drawing.Point(221, 11);
            this.txtDay.MaxLength = 2;
            this.txtDay.Name = "txtDay";
            this.txtDay.Size = new System.Drawing.Size(23, 22);
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
            this.label3.Font = new System.Drawing.Font("ＭＳ Ｐゴシック", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.label3.Location = new System.Drawing.Point(197, 15);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(22, 15);
            this.label3.TabIndex = 16;
            this.label3.Text = "月";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("ＭＳ Ｐゴシック", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.label4.Location = new System.Drawing.Point(245, 15);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(22, 15);
            this.label4.TabIndex = 17;
            this.label4.Text = "日";
            // 
            // ChkKessan
            // 
            this.ChkKessan.Font = new System.Drawing.Font("ＭＳ Ｐゴシック", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.ChkKessan.Location = new System.Drawing.Point(284, 8);
            this.ChkKessan.Name = "ChkKessan";
            this.ChkKessan.Size = new System.Drawing.Size(59, 29);
            this.ChkKessan.TabIndex = 20;
            this.ChkKessan.Text = "決算";
            this.ChkKessan.UseVisualStyleBackColor = true;
            // 
            // chkFukusuChk
            // 
            this.chkFukusuChk.Font = new System.Drawing.Font("ＭＳ Ｐゴシック", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.chkFukusuChk.Location = new System.Drawing.Point(349, 8);
            this.chkFukusuChk.Name = "chkFukusuChk";
            this.chkFukusuChk.Size = new System.Drawing.Size(96, 29);
            this.chkFukusuChk.TabIndex = 21;
            this.chkFukusuChk.Text = "複数枚";
            this.chkFukusuChk.UseVisualStyleBackColor = true;
            // 
            // lblGengo
            // 
            this.lblGengo.BackColor = System.Drawing.SystemColors.Control;
            this.lblGengo.Font = new System.Drawing.Font("ＭＳ Ｐゴシック", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.lblGengo.Location = new System.Drawing.Point(57, 13);
            this.lblGengo.Name = "lblGengo";
            this.lblGengo.Size = new System.Drawing.Size(53, 19);
            this.lblGengo.TabIndex = 29;
            this.lblGengo.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // tabPage5
            // 
            this.tabPage5.BackColor = System.Drawing.SystemColors.Control;
            this.tabPage5.Controls.Add(this.fgCom);
            this.tabPage5.Location = new System.Drawing.Point(4, 23);
            this.tabPage5.Name = "tabPage5";
            this.tabPage5.Size = new System.Drawing.Size(219, 706);
            this.tabPage5.TabIndex = 4;
            this.tabPage5.Text = "会社";
            // 
            // fgCom
            // 
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.Lavender;
            this.fgCom.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle1;
            this.fgCom.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.fgCom.Location = new System.Drawing.Point(3, 11);
            this.fgCom.Name = "fgCom";
            this.fgCom.RowTemplate.Height = 21;
            this.fgCom.Size = new System.Drawing.Size(208, 694);
            this.fgCom.TabIndex = 0;
            this.fgCom.TabStop = false;
            // 
            // tabPage3
            // 
            this.tabPage3.BackColor = System.Drawing.SystemColors.Control;
            this.tabPage3.Controls.Add(this.fgJigyo);
            this.tabPage3.Controls.Add(this.fgBumon);
            this.tabPage3.Controls.Add(this.fgTaxMas);
            this.tabPage3.Controls.Add(this.fgTax);
            this.tabPage3.Location = new System.Drawing.Point(4, 23);
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.Size = new System.Drawing.Size(219, 706);
            this.tabPage3.TabIndex = 2;
            this.tabPage3.Text = "部門・税・事業";
            // 
            // fgJigyo
            // 
            dataGridViewCellStyle2.BackColor = System.Drawing.Color.Lavender;
            this.fgJigyo.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle2;
            this.fgJigyo.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.fgJigyo.Location = new System.Drawing.Point(3, 589);
            this.fgJigyo.Name = "fgJigyo";
            this.fgJigyo.RowTemplate.Height = 21;
            this.fgJigyo.Size = new System.Drawing.Size(208, 110);
            this.fgJigyo.TabIndex = 3;
            this.fgJigyo.TabStop = false;
            this.fgJigyo.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.fgJigyo_CellDoubleClick);
            // 
            // fgBumon
            // 
            dataGridViewCellStyle3.BackColor = System.Drawing.Color.Lavender;
            this.fgBumon.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle3;
            this.fgBumon.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.fgBumon.Location = new System.Drawing.Point(3, 11);
            this.fgBumon.Name = "fgBumon";
            this.fgBumon.RowTemplate.Height = 21;
            this.fgBumon.Size = new System.Drawing.Size(208, 271);
            this.fgBumon.TabIndex = 2;
            this.fgBumon.TabStop = false;
            this.fgBumon.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.fgBumon_CellDoubleClick);
            // 
            // fgTaxMas
            // 
            dataGridViewCellStyle4.BackColor = System.Drawing.Color.Lavender;
            this.fgTaxMas.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle4;
            this.fgTaxMas.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.fgTaxMas.Location = new System.Drawing.Point(3, 510);
            this.fgTaxMas.Name = "fgTaxMas";
            this.fgTaxMas.RowTemplate.Height = 21;
            this.fgTaxMas.Size = new System.Drawing.Size(208, 73);
            this.fgTaxMas.TabIndex = 1;
            this.fgTaxMas.TabStop = false;
            this.fgTaxMas.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.fgTaxMas_CellDoubleClick);
            // 
            // fgTax
            // 
            dataGridViewCellStyle5.BackColor = System.Drawing.Color.Lavender;
            this.fgTax.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle5;
            this.fgTax.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.fgTax.Location = new System.Drawing.Point(3, 288);
            this.fgTax.Name = "fgTax";
            this.fgTax.RowTemplate.Height = 21;
            this.fgTax.Size = new System.Drawing.Size(208, 217);
            this.fgTax.TabIndex = 0;
            this.fgTax.TabStop = false;
            this.fgTax.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.fgTax_CellDoubleClick);
            // 
            // tabPage2
            // 
            this.tabPage2.BackColor = System.Drawing.SystemColors.Control;
            this.tabPage2.Controls.Add(this.fgHojo);
            this.tabPage2.Controls.Add(this.fgKamoku);
            this.tabPage2.Location = new System.Drawing.Point(4, 23);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(219, 706);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "科目";
            // 
            // fgHojo
            // 
            dataGridViewCellStyle6.BackColor = System.Drawing.Color.Lavender;
            this.fgHojo.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle6;
            this.fgHojo.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.fgHojo.Location = new System.Drawing.Point(3, 450);
            this.fgHojo.Name = "fgHojo";
            this.fgHojo.RowTemplate.Height = 21;
            this.fgHojo.Size = new System.Drawing.Size(208, 251);
            this.fgHojo.TabIndex = 1;
            this.fgHojo.TabStop = false;
            this.fgHojo.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.fgHojo_CellDoubleClick);
            // 
            // fgKamoku
            // 
            dataGridViewCellStyle7.BackColor = System.Drawing.Color.Lavender;
            this.fgKamoku.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle7;
            this.fgKamoku.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.fgKamoku.Location = new System.Drawing.Point(3, 11);
            this.fgKamoku.Name = "fgKamoku";
            this.fgKamoku.RowTemplate.Height = 21;
            this.fgKamoku.Size = new System.Drawing.Size(208, 433);
            this.fgKamoku.TabIndex = 0;
            this.fgKamoku.TabStop = false;
            this.fgKamoku.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.fgKamoku_CellClick);
            this.fgKamoku.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.fgKamoku_CellDoubleClick);
            this.fgKamoku.SelectionChanged += new System.EventHandler(this.fgKamoku_SelectionChanged);
            // 
            // tabData
            // 
            this.tabData.Controls.Add(this.tabPage2);
            this.tabData.Controls.Add(this.tabPage3);
            this.tabData.Controls.Add(this.tabPage5);
            this.tabData.Font = new System.Drawing.Font("ＭＳ Ｐゴシック", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.tabData.Location = new System.Drawing.Point(1016, 15);
            this.tabData.Name = "tabData";
            this.tabData.SelectedIndex = 0;
            this.tabData.Size = new System.Drawing.Size(227, 733);
            this.tabData.TabIndex = 10;
            this.tabData.TabStop = false;
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "openFileDialog1";
            // 
            // gcMultiRow1
            // 
            this.gcMultiRow1.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.gcMultiRow1.EditMode = GrapeCity.Win.MultiRow.EditMode.EditOnEnter;
            this.gcMultiRow1.Location = new System.Drawing.Point(6, 39);
            this.gcMultiRow1.Name = "gcMultiRow1";
            cellStyle1.SelectionBackColor = System.Drawing.Color.Blue;
            this.gcMultiRow1.RowsDefaultCellStyle = cellStyle1;
            this.gcMultiRow1.ScrollBars = System.Windows.Forms.ScrollBars.None;
            this.gcMultiRow1.Size = new System.Drawing.Size(1004, 646);
            this.gcMultiRow1.SplitMode = GrapeCity.Win.MultiRow.SplitMode.None;
            this.gcMultiRow1.TabIndex = 28;
            this.gcMultiRow1.TabStop = false;
            this.gcMultiRow1.Template = this.tempPrn1;
            this.gcMultiRow1.Text = "gcMultiRow1";
            this.gcMultiRow1.CellValueChanged += new System.EventHandler<GrapeCity.Win.MultiRow.CellEventArgs>(this.gcMultiRow1_CellValueChanged);
            this.gcMultiRow1.CellEnter += new System.EventHandler<GrapeCity.Win.MultiRow.CellEventArgs>(this.gcMultiRow1_CellEnter);
            this.gcMultiRow1.CellClick += new System.EventHandler<GrapeCity.Win.MultiRow.CellEventArgs>(this.gcMultiRow1_CellClick);
            this.gcMultiRow1.CellContentClick += new System.EventHandler<GrapeCity.Win.MultiRow.CellEventArgs>(this.gcMultiRow1_CellContentClick_1);
            this.gcMultiRow1.Enter += new System.EventHandler(this.gcMultiRow1_Enter);
            // 
            // frmShiwakePrn
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.ClientSize = new System.Drawing.Size(1249, 760);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.btnOk);
            this.Controls.Add(this.lblGengo);
            this.Controls.Add(this.gcMultiRow1);
            this.Controls.Add(this.chkFukusuChk);
            this.Controls.Add(this.ChkKessan);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.txtDay);
            this.Controls.Add(this.txtMonth);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.txtYear);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.tabData);
            this.Controls.Add(this.cmdExit);
            this.Controls.Add(this.btnDltDen);
            this.Font = new System.Drawing.Font("ＭＳ Ｐゴシック", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "frmShiwakePrn";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "活字振替伝票発行";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Base_FormClosing);
            this.Load += new System.EventHandler(this.Base_Load);
            this.tabPage5.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.fgCom)).EndInit();
            this.tabPage3.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.fgJigyo)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.fgBumon)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.fgTaxMas)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.fgTax)).EndInit();
            this.tabPage2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.fgHojo)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.fgKamoku)).EndInit();
            this.tabData.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.Button btnDltDen;
        private System.Windows.Forms.Button cmdExit;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtYear;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtMonth;
        private System.Windows.Forms.TextBox txtDay;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.CheckBox ChkKessan;
        private System.Windows.Forms.CheckBox chkFukusuChk;
        private System.Windows.Forms.Label lblGengo;
        private System.Windows.Forms.Button btnOk;
        private System.Windows.Forms.SaveFileDialog saveFileDialog1;
        private TempPrn tempPrn1;
        private System.Windows.Forms.TabPage tabPage5;
        private System.Windows.Forms.DataGridView fgCom;
        private System.Windows.Forms.TabPage tabPage3;
        private System.Windows.Forms.DataGridView fgJigyo;
        private System.Windows.Forms.DataGridView fgBumon;
        private System.Windows.Forms.DataGridView fgTaxMas;
        private System.Windows.Forms.DataGridView fgTax;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.DataGridView fgHojo;
        private System.Windows.Forms.DataGridView fgKamoku;
        private System.Windows.Forms.TabControl tabData;
        private GrapeCity.Win.MultiRow.GcMultiRow gcMultiRow1;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.Button button2;
    }
}