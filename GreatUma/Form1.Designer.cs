namespace GreatUma
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            dataGridView1 = new DataGridView();
            buttonStartUpdate = new Button();
            label1 = new Label();
            label2 = new Label();
            label3 = new Label();
            buttonSetPurchasePrice = new Button();
            numericUpDownRatio = new NumericUpDown();
            label4 = new Label();
            textBoxPriceAppliedRatio = new TextBox();
            label5 = new Label();
            numericUpDownPurchasePrice = new NumericUpDown();
            oddsDateTimeLabel = new Label();
            buttonLoginConfig = new Button();
            buttonSaveConfition = new Button();
            textBoxCurrentPrice = new TextBox();
            timer1 = new System.Windows.Forms.Timer(components);
            buttonStopAutoupdate = new Button();
            buttonStopAutoPurchase = new Button();
            buttonStartAutoPurchase = new Button();
            buttonGetPrice = new Button();
            ((System.ComponentModel.ISupportInitialize)dataGridView1).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numericUpDownRatio).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numericUpDownPurchasePrice).BeginInit();
            SuspendLayout();
            // 
            // dataGridView1
            // 
            dataGridView1.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            dataGridView1.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridView1.Location = new Point(-2, 66);
            dataGridView1.Name = "dataGridView1";
            dataGridView1.Size = new Size(976, 332);
            dataGridView1.TabIndex = 0;
            // 
            // buttonStartUpdate
            // 
            buttonStartUpdate.Location = new Point(668, 415);
            buttonStartUpdate.Name = "buttonStartUpdate";
            buttonStartUpdate.Size = new Size(75, 23);
            buttonStartUpdate.TabIndex = 1;
            buttonStartUpdate.Text = "情報更新";
            buttonStartUpdate.UseVisualStyleBackColor = true;
            buttonStartUpdate.Click += UpdateButton_Click;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(12, 9);
            label1.Name = "label1";
            label1.Size = new Size(31, 15);
            label1.TabIndex = 2;
            label1.Text = "残高";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(12, 38);
            label2.Name = "label2";
            label2.Size = new Size(108, 15);
            label2.TabIndex = 3;
            label2.Text = "現在オッズ取得時刻:";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(191, 9);
            label3.Name = "label3";
            label3.Size = new Size(67, 15);
            label3.TabIndex = 4;
            label3.Text = "投入資金率";
            // 
            // buttonSetPurchasePrice
            // 
            buttonSetPurchasePrice.Location = new Point(490, 6);
            buttonSetPurchasePrice.Name = "buttonSetPurchasePrice";
            buttonSetPurchasePrice.Size = new Size(108, 23);
            buttonSetPurchasePrice.TabIndex = 5;
            buttonSetPurchasePrice.Text = "購入金額に反映";
            buttonSetPurchasePrice.UseVisualStyleBackColor = true;
            buttonSetPurchasePrice.Click += ButtonSetPurchasePrice_Click;
            // 
            // numericUpDownRatio
            // 
            numericUpDownRatio.Location = new Point(264, 6);
            numericUpDownRatio.Name = "numericUpDownRatio";
            numericUpDownRatio.Size = new Size(69, 23);
            numericUpDownRatio.TabIndex = 7;
            numericUpDownRatio.ValueChanged += NumericUpDownRatio_ValueChanged;
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new Point(339, 10);
            label4.Name = "label4";
            label4.Size = new Size(28, 15);
            label4.TabIndex = 8;
            label4.Text = "% =";
            // 
            // textBoxPriceAppliedRatio
            // 
            textBoxPriceAppliedRatio.Enabled = false;
            textBoxPriceAppliedRatio.Location = new Point(373, 6);
            textBoxPriceAppliedRatio.Name = "textBoxPriceAppliedRatio";
            textBoxPriceAppliedRatio.Size = new Size(100, 23);
            textBoxPriceAppliedRatio.TabIndex = 9;
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Location = new Point(607, 10);
            label5.Name = "label5";
            label5.Size = new Size(55, 15);
            label5.TabIndex = 10;
            label5.Text = "購入金額";
            // 
            // numericUpDownPurchasePrice
            // 
            numericUpDownPurchasePrice.Increment = new decimal(new int[] { 100, 0, 0, 0 });
            numericUpDownPurchasePrice.Location = new Point(668, 8);
            numericUpDownPurchasePrice.Maximum = new decimal(new int[] { 1000000, 0, 0, 0 });
            numericUpDownPurchasePrice.Name = "numericUpDownPurchasePrice";
            numericUpDownPurchasePrice.Size = new Size(111, 23);
            numericUpDownPurchasePrice.TabIndex = 11;
            numericUpDownPurchasePrice.ValueChanged += NumericUpDownPurchasePrice_ValueChanged;
            // 
            // oddsDateTimeLabel
            // 
            oddsDateTimeLabel.AutoSize = true;
            oddsDateTimeLabel.Location = new Point(123, 38);
            oddsDateTimeLabel.Name = "oddsDateTimeLabel";
            oddsDateTimeLabel.Size = new Size(12, 15);
            oddsDateTimeLabel.TabIndex = 12;
            oddsDateTimeLabel.Text = "-";
            // 
            // buttonLoginConfig
            // 
            buttonLoginConfig.Location = new Point(12, 415);
            buttonLoginConfig.Name = "buttonLoginConfig";
            buttonLoginConfig.Size = new Size(75, 23);
            buttonLoginConfig.TabIndex = 13;
            buttonLoginConfig.Text = "ログイン設定";
            buttonLoginConfig.UseVisualStyleBackColor = true;
            buttonLoginConfig.Click += ButtonLoginConfig_Click;
            // 
            // buttonSaveConfition
            // 
            buttonSaveConfition.Location = new Point(887, 415);
            buttonSaveConfition.Name = "buttonSaveConfition";
            buttonSaveConfition.Size = new Size(75, 23);
            buttonSaveConfition.TabIndex = 14;
            buttonSaveConfition.Text = "設定保存";
            buttonSaveConfition.UseVisualStyleBackColor = true;
            // 
            // textBoxCurrentPrice
            // 
            textBoxCurrentPrice.Enabled = false;
            textBoxCurrentPrice.Location = new Point(49, 6);
            textBoxCurrentPrice.Name = "textBoxCurrentPrice";
            textBoxCurrentPrice.Size = new Size(100, 23);
            textBoxCurrentPrice.TabIndex = 15;
            textBoxCurrentPrice.TextChanged += TextBoxCurrentPrice_TextChanged;
            // 
            // timer1
            // 
            timer1.Enabled = true;
            timer1.Interval = 60000;
            timer1.Tick += Timer1_Tick;
            // 
            // buttonStopAutoupdate
            // 
            buttonStopAutoupdate.Location = new Point(749, 415);
            buttonStopAutoupdate.Name = "buttonStopAutoupdate";
            buttonStopAutoupdate.Size = new Size(92, 23);
            buttonStopAutoupdate.TabIndex = 16;
            buttonStopAutoupdate.Text = "更新停止";
            buttonStopAutoupdate.UseVisualStyleBackColor = true;
            buttonStopAutoupdate.Click += ButtonUpdateStatus_Click;
            // 
            // buttonStopAutoPurchase
            // 
            buttonStopAutoPurchase.Location = new Point(538, 415);
            buttonStopAutoPurchase.Name = "buttonStopAutoPurchase";
            buttonStopAutoPurchase.Size = new Size(92, 23);
            buttonStopAutoPurchase.TabIndex = 18;
            buttonStopAutoPurchase.Text = "自動購入停止";
            buttonStopAutoPurchase.UseVisualStyleBackColor = true;
            buttonStopAutoPurchase.Click += ButtonStopAutoPurchase_Click;
            // 
            // buttonStartAutoPurchase
            // 
            buttonStartAutoPurchase.Location = new Point(457, 415);
            buttonStartAutoPurchase.Name = "buttonStartAutoPurchase";
            buttonStartAutoPurchase.Size = new Size(75, 23);
            buttonStartAutoPurchase.TabIndex = 17;
            buttonStartAutoPurchase.Text = "自動購入";
            buttonStartAutoPurchase.UseVisualStyleBackColor = true;
            buttonStartAutoPurchase.Click += ButtonAutoPurchase_Click;
            // 
            // buttonGetPrice
            // 
            buttonGetPrice.Location = new Point(356, 415);
            buttonGetPrice.Name = "buttonGetPrice";
            buttonGetPrice.Size = new Size(75, 23);
            buttonGetPrice.TabIndex = 19;
            buttonGetPrice.Text = "残高反映";
            buttonGetPrice.UseVisualStyleBackColor = true;
            buttonGetPrice.Click += ButtonGetPrice_Click;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(973, 450);
            Controls.Add(buttonGetPrice);
            Controls.Add(buttonStopAutoPurchase);
            Controls.Add(buttonStartAutoPurchase);
            Controls.Add(buttonStopAutoupdate);
            Controls.Add(textBoxCurrentPrice);
            Controls.Add(buttonSaveConfition);
            Controls.Add(buttonLoginConfig);
            Controls.Add(oddsDateTimeLabel);
            Controls.Add(numericUpDownPurchasePrice);
            Controls.Add(label5);
            Controls.Add(textBoxPriceAppliedRatio);
            Controls.Add(label4);
            Controls.Add(numericUpDownRatio);
            Controls.Add(buttonSetPurchasePrice);
            Controls.Add(label3);
            Controls.Add(label2);
            Controls.Add(label1);
            Controls.Add(buttonStartUpdate);
            Controls.Add(dataGridView1);
            Name = "Form1";
            Text = "GreatUma";
            Load += Form1_Load;
            ((System.ComponentModel.ISupportInitialize)dataGridView1).EndInit();
            ((System.ComponentModel.ISupportInitialize)numericUpDownRatio).EndInit();
            ((System.ComponentModel.ISupportInitialize)numericUpDownPurchasePrice).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private DataGridView dataGridView1;
        private Button buttonStartUpdate;
        private Label label1;
        private Label label2;
        private Label label3;
        private Button buttonSetPurchasePrice;
        private NumericUpDown numericUpDownRatio;
        private Label label4;
        private TextBox textBoxPriceAppliedRatio;
        private Label label5;
        private NumericUpDown numericUpDownPurchasePrice;
        private Label oddsDateTimeLabel;
        private Button buttonLoginConfig;
        private Button buttonSaveConfition;
        private TextBox textBoxCurrentPrice;
        private System.Windows.Forms.Timer timer1;
        private Button buttonStopAutoupdate;
        private Button buttonStopAutoPurchase;
        private Button buttonStartAutoPurchase;
        private Button buttonGetPrice;
    }
}
