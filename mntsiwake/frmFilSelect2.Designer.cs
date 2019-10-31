namespace mntsiwake
{
    partial class frmFilSelect2
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmFilSelect2));
            this.button3 = new System.Windows.Forms.Button();
            this.button4 = new System.Windows.Forms.Button();
            this.dg1 = new System.Windows.Forms.DataGridView();
            this.lblComName = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.dg1)).BeginInit();
            this.SuspendLayout();
            // 
            // button3
            // 
            this.button3.Font = new System.Drawing.Font("ＭＳ Ｐゴシック", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.button3.Location = new System.Drawing.Point(151, 224);
            this.button3.Margin = new System.Windows.Forms.Padding(4);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(88, 32);
            this.button3.TabIndex = 3;
            this.button3.Text = "確定(&K)";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // button4
            // 
            this.button4.Font = new System.Drawing.Font("ＭＳ Ｐゴシック", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.button4.Location = new System.Drawing.Point(246, 224);
            this.button4.Margin = new System.Windows.Forms.Padding(4);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(88, 32);
            this.button4.TabIndex = 4;
            this.button4.Text = "中止(&E)";
            this.button4.UseVisualStyleBackColor = true;
            this.button4.Click += new System.EventHandler(this.button4_Click);
            // 
            // dg1
            // 
            this.dg1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dg1.Location = new System.Drawing.Point(9, 30);
            this.dg1.Name = "dg1";
            this.dg1.RowTemplate.Height = 21;
            this.dg1.Size = new System.Drawing.Size(325, 181);
            this.dg1.TabIndex = 5;
            // 
            // lblComName
            // 
            this.lblComName.Font = new System.Drawing.Font("ＭＳ Ｐゴシック", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.lblComName.Location = new System.Drawing.Point(11, 11);
            this.lblComName.Name = "lblComName";
            this.lblComName.Size = new System.Drawing.Size(323, 15);
            this.lblComName.TabIndex = 6;
            this.lblComName.Text = "label1";
            this.lblComName.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // frmFilSelect2
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(343, 262);
            this.Controls.Add(this.lblComName);
            this.Controls.Add(this.dg1);
            this.Controls.Add(this.button4);
            this.Controls.Add(this.button3);
            this.Font = new System.Drawing.Font("ＭＳ Ｐゴシック", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(4);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmFilSelect2";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "作業中断伝票の選択";
            this.Load += new System.EventHandler(this.frmFilSelect2_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dg1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.Button button4;
        private System.Windows.Forms.DataGridView dg1;
        private System.Windows.Forms.Label lblComName;
    }
}