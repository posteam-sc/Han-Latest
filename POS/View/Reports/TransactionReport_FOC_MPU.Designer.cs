﻿namespace POS
{
    partial class TransactionReport_FOC_MPU
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TransactionReport_FOC_MPU));
            this.chkFOC = new System.Windows.Forms.CheckBox();
            this.chkMPU = new System.Windows.Forms.CheckBox();
            this.chkCredit = new System.Windows.Forms.CheckBox();
            this.chkCash = new System.Windows.Forms.CheckBox();
            this.gbPaymentType = new System.Windows.Forms.GroupBox();
            this.chkPay = new System.Windows.Forms.CheckBox();
            this.chkGlobalCard = new System.Windows.Forms.CheckBox();
            this.chkBankTransfer = new System.Windows.Forms.CheckBox();
            this.chkTester = new System.Windows.Forms.CheckBox();
            this.chkGiftCard = new System.Windows.Forms.CheckBox();
            this.chkCounter = new System.Windows.Forms.CheckBox();
            this.chkCashier = new System.Windows.Forms.CheckBox();
            this.lblCounterName = new System.Windows.Forms.Label();
            this.lblCashierName = new System.Windows.Forms.Label();
            this.cboCounter = new System.Windows.Forms.ComboBox();
            this.cboCashier = new System.Windows.Forms.ComboBox();
            this.reportViewer1 = new Microsoft.Reporting.WinForms.ReportViewer();
            this.label3 = new System.Windows.Forms.Label();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.gbTransactionList = new System.Windows.Forms.GroupBox();
            this.lblPeriod = new System.Windows.Forms.Label();
            this.dtpTo = new System.Windows.Forms.DateTimePicker();
            this.label2 = new System.Windows.Forms.Label();
            this.dtpFrom = new System.Windows.Forms.DateTimePicker();
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.rdbSummary = new System.Windows.Forms.RadioButton();
            this.rdbDebt = new System.Windows.Forms.RadioButton();
            this.rdbRefund = new System.Windows.Forms.RadioButton();
            this.rdbSale = new System.Windows.Forms.RadioButton();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.cboshoplist = new System.Windows.Forms.ComboBox();
            this.label4 = new System.Windows.Forms.Label();
            this.gbPaymentType.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.gbTransactionList.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.SuspendLayout();
            // 
            // chkFOC
            // 
            this.chkFOC.AutoSize = true;
            this.chkFOC.Checked = true;
            this.chkFOC.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkFOC.Location = new System.Drawing.Point(497, 25);
            this.chkFOC.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.chkFOC.Name = "chkFOC";
            this.chkFOC.Size = new System.Drawing.Size(62, 22);
            this.chkFOC.TabIndex = 3;
            this.chkFOC.Text = "FOC";
            this.chkFOC.UseVisualStyleBackColor = true;
            this.chkFOC.CheckedChanged += new System.EventHandler(this.chkFOC_CheckedChanged);
            // 
            // chkMPU
            // 
            this.chkMPU.AutoSize = true;
            this.chkMPU.Checked = true;
            this.chkMPU.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkMPU.Location = new System.Drawing.Point(331, 25);
            this.chkMPU.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.chkMPU.Name = "chkMPU";
            this.chkMPU.Size = new System.Drawing.Size(64, 22);
            this.chkMPU.TabIndex = 2;
            this.chkMPU.Text = "MPU";
            this.chkMPU.UseVisualStyleBackColor = true;
            this.chkMPU.CheckedChanged += new System.EventHandler(this.chkMPU_CheckedChanged);
            // 
            // chkCredit
            // 
            this.chkCredit.AutoSize = true;
            this.chkCredit.Checked = true;
            this.chkCredit.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkCredit.Location = new System.Drawing.Point(667, 25);
            this.chkCredit.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.chkCredit.Name = "chkCredit";
            this.chkCredit.Size = new System.Drawing.Size(69, 22);
            this.chkCredit.TabIndex = 4;
            this.chkCredit.Text = "Credit";
            this.chkCredit.UseVisualStyleBackColor = true;
            this.chkCredit.CheckedChanged += new System.EventHandler(this.chkCredit_CheckedChanged);
            // 
            // chkCash
            // 
            this.chkCash.AutoSize = true;
            this.chkCash.Checked = true;
            this.chkCash.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkCash.Location = new System.Drawing.Point(35, 25);
            this.chkCash.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.chkCash.Name = "chkCash";
            this.chkCash.Size = new System.Drawing.Size(65, 22);
            this.chkCash.TabIndex = 0;
            this.chkCash.Text = "Cash";
            this.chkCash.UseVisualStyleBackColor = true;
            this.chkCash.CheckedChanged += new System.EventHandler(this.chkCash_CheckedChanged);
            // 
            // gbPaymentType
            // 
            this.gbPaymentType.Controls.Add(this.chkPay);
            this.gbPaymentType.Controls.Add(this.chkGlobalCard);
            this.gbPaymentType.Controls.Add(this.chkBankTransfer);
            this.gbPaymentType.Controls.Add(this.chkTester);
            this.gbPaymentType.Controls.Add(this.chkFOC);
            this.gbPaymentType.Controls.Add(this.chkMPU);
            this.gbPaymentType.Controls.Add(this.chkCredit);
            this.gbPaymentType.Controls.Add(this.chkGiftCard);
            this.gbPaymentType.Controls.Add(this.chkCash);
            this.gbPaymentType.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.gbPaymentType.Location = new System.Drawing.Point(13, 272);
            this.gbPaymentType.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.gbPaymentType.Name = "gbPaymentType";
            this.gbPaymentType.Padding = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.gbPaymentType.Size = new System.Drawing.Size(995, 113);
            this.gbPaymentType.TabIndex = 4;
            this.gbPaymentType.TabStop = false;
            this.gbPaymentType.Text = "Report Payment Type";
            // 
            // chkPay
            // 
            this.chkPay.AutoSize = true;
            this.chkPay.Checked = true;
            this.chkPay.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkPay.Location = new System.Drawing.Point(35, 68);
            this.chkPay.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.chkPay.Name = "chkPay";
            this.chkPay.Size = new System.Drawing.Size(55, 22);
            this.chkPay.TabIndex = 8;
            this.chkPay.Text = "Pay";
            this.chkPay.UseVisualStyleBackColor = true;
            this.chkPay.CheckedChanged += new System.EventHandler(this.chkPay_CheckedChanged);
            // 
            // chkGlobalCard
            // 
            this.chkGlobalCard.AutoSize = true;
            this.chkGlobalCard.Checked = true;
            this.chkGlobalCard.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkGlobalCard.Location = new System.Drawing.Point(163, 68);
            this.chkGlobalCard.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.chkGlobalCard.Name = "chkGlobalCard";
            this.chkGlobalCard.Size = new System.Drawing.Size(109, 22);
            this.chkGlobalCard.TabIndex = 7;
            this.chkGlobalCard.Text = "Global Card";
            this.chkGlobalCard.UseVisualStyleBackColor = true;
            this.chkGlobalCard.CheckedChanged += new System.EventHandler(this.chkGlobalCard_CheckedChanged);
            // 
            // chkBankTransfer
            // 
            this.chkBankTransfer.AutoSize = true;
            this.chkBankTransfer.Checked = true;
            this.chkBankTransfer.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkBankTransfer.Location = new System.Drawing.Point(331, 68);
            this.chkBankTransfer.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.chkBankTransfer.Name = "chkBankTransfer";
            this.chkBankTransfer.Size = new System.Drawing.Size(123, 22);
            this.chkBankTransfer.TabIndex = 6;
            this.chkBankTransfer.Text = "Bank Transfer";
            this.chkBankTransfer.UseVisualStyleBackColor = true;
            this.chkBankTransfer.CheckedChanged += new System.EventHandler(this.chkBankTransfer_CheckedChanged);
            // 
            // chkTester
            // 
            this.chkTester.AutoSize = true;
            this.chkTester.Checked = true;
            this.chkTester.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkTester.Location = new System.Drawing.Point(841, 25);
            this.chkTester.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.chkTester.Name = "chkTester";
            this.chkTester.Size = new System.Drawing.Size(72, 22);
            this.chkTester.TabIndex = 5;
            this.chkTester.Text = "Tester";
            this.chkTester.UseVisualStyleBackColor = true;
            this.chkTester.CheckedChanged += new System.EventHandler(this.chkTester_CheckedChanged);
            // 
            // chkGiftCard
            // 
            this.chkGiftCard.AutoSize = true;
            this.chkGiftCard.Checked = true;
            this.chkGiftCard.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkGiftCard.Location = new System.Drawing.Point(163, 25);
            this.chkGiftCard.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.chkGiftCard.Name = "chkGiftCard";
            this.chkGiftCard.Size = new System.Drawing.Size(89, 22);
            this.chkGiftCard.TabIndex = 1;
            this.chkGiftCard.Text = "Gift Card";
            this.chkGiftCard.UseVisualStyleBackColor = true;
            this.chkGiftCard.CheckedChanged += new System.EventHandler(this.chkGiftCard_CheckedChanged);
            // 
            // chkCounter
            // 
            this.chkCounter.AutoSize = true;
            this.chkCounter.Location = new System.Drawing.Point(545, 20);
            this.chkCounter.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.chkCounter.Name = "chkCounter";
            this.chkCounter.Size = new System.Drawing.Size(100, 22);
            this.chkCounter.TabIndex = 3;
            this.chkCounter.Text = "ByCounter";
            this.chkCounter.UseVisualStyleBackColor = true;
            this.chkCounter.CheckedChanged += new System.EventHandler(this.chkCounter_CheckedChanged);
            // 
            // chkCashier
            // 
            this.chkCashier.AutoSize = true;
            this.chkCashier.Location = new System.Drawing.Point(129, 28);
            this.chkCashier.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.chkCashier.Name = "chkCashier";
            this.chkCashier.Size = new System.Drawing.Size(98, 22);
            this.chkCashier.TabIndex = 0;
            this.chkCashier.Text = "ByCashier";
            this.chkCashier.UseVisualStyleBackColor = true;
            this.chkCashier.CheckedChanged += new System.EventHandler(this.chkCashier_CheckedChanged);
            // 
            // lblCounterName
            // 
            this.lblCounterName.AutoSize = true;
            this.lblCounterName.Enabled = false;
            this.lblCounterName.Location = new System.Drawing.Point(541, 53);
            this.lblCounterName.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblCounterName.Name = "lblCounterName";
            this.lblCounterName.Size = new System.Drawing.Size(105, 18);
            this.lblCounterName.TabIndex = 4;
            this.lblCounterName.Text = "Counter Name";
            // 
            // lblCashierName
            // 
            this.lblCashierName.AutoSize = true;
            this.lblCashierName.Enabled = false;
            this.lblCashierName.Location = new System.Drawing.Point(125, 59);
            this.lblCashierName.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblCashierName.Name = "lblCashierName";
            this.lblCashierName.Size = new System.Drawing.Size(103, 18);
            this.lblCashierName.TabIndex = 1;
            this.lblCashierName.Text = "Cashier Name";
            // 
            // cboCounter
            // 
            this.cboCounter.Enabled = false;
            this.cboCounter.FormattingEnabled = true;
            this.cboCounter.Location = new System.Drawing.Point(545, 90);
            this.cboCounter.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.cboCounter.Name = "cboCounter";
            this.cboCounter.Size = new System.Drawing.Size(301, 26);
            this.cboCounter.TabIndex = 5;
            this.cboCounter.SelectedIndexChanged += new System.EventHandler(this.cboCounter_SelectedIndexChanged);
            // 
            // cboCashier
            // 
            this.cboCashier.Enabled = false;
            this.cboCashier.FormattingEnabled = true;
            this.cboCashier.Location = new System.Drawing.Point(131, 87);
            this.cboCashier.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.cboCashier.Name = "cboCashier";
            this.cboCashier.Size = new System.Drawing.Size(301, 26);
            this.cboCashier.TabIndex = 2;
            this.cboCashier.SelectedIndexChanged += new System.EventHandler(this.cboCashier_SelectedIndexChanged);
            // 
            // reportViewer1
            // 
            this.reportViewer1.LocalReport.ReportEmbeddedResource = "POS.Reports.TransactionReport.rdlc";
            this.reportViewer1.Location = new System.Drawing.Point(16, 38);
            this.reportViewer1.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.reportViewer1.Name = "reportViewer1";
            this.reportViewer1.ShowPrintButton = false;
            this.reportViewer1.ShowRefreshButton = false;
            this.reportViewer1.ShowStopButton = false;
            this.reportViewer1.ShowZoomControl = false;
            this.reportViewer1.Size = new System.Drawing.Size(989, 445);
            this.reportViewer1.TabIndex = 0;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(477, 20);
            this.label3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(49, 17);
            this.label3.TabIndex = 4;
            this.label3.Text = "Period";
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.chkCounter);
            this.groupBox3.Controls.Add(this.chkCashier);
            this.groupBox3.Controls.Add(this.lblCounterName);
            this.groupBox3.Controls.Add(this.lblCashierName);
            this.groupBox3.Controls.Add(this.cboCounter);
            this.groupBox3.Controls.Add(this.cboCashier);
            this.groupBox3.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.groupBox3.Location = new System.Drawing.Point(13, 1);
            this.groupBox3.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Padding = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.groupBox3.Size = new System.Drawing.Size(927, 133);
            this.groupBox3.TabIndex = 0;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "By Cashier or Counter";
            // 
            // gbTransactionList
            // 
            this.gbTransactionList.Controls.Add(this.reportViewer1);
            this.gbTransactionList.Controls.Add(this.label3);
            this.gbTransactionList.Controls.Add(this.lblPeriod);
            this.gbTransactionList.Location = new System.Drawing.Point(13, 393);
            this.gbTransactionList.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.gbTransactionList.Name = "gbTransactionList";
            this.gbTransactionList.Padding = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.gbTransactionList.Size = new System.Drawing.Size(1005, 491);
            this.gbTransactionList.TabIndex = 5;
            this.gbTransactionList.TabStop = false;
            // 
            // lblPeriod
            // 
            this.lblPeriod.AutoSize = true;
            this.lblPeriod.Location = new System.Drawing.Point(544, 20);
            this.lblPeriod.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblPeriod.Name = "lblPeriod";
            this.lblPeriod.Size = new System.Drawing.Size(13, 17);
            this.lblPeriod.TabIndex = 1;
            this.lblPeriod.Text = "-";
            // 
            // dtpTo
            // 
            this.dtpTo.CustomFormat = "dd-MM-yyyy";
            this.dtpTo.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtpTo.Location = new System.Drawing.Point(369, 30);
            this.dtpTo.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.dtpTo.Name = "dtpTo";
            this.dtpTo.Size = new System.Drawing.Size(156, 24);
            this.dtpTo.TabIndex = 3;
            this.dtpTo.ValueChanged += new System.EventHandler(this.dtpTo_ValueChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(291, 32);
            this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(26, 18);
            this.label2.TabIndex = 2;
            this.label2.Text = "To";
            // 
            // dtpFrom
            // 
            this.dtpFrom.CustomFormat = "dd-MM-yyyy";
            this.dtpFrom.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtpFrom.Location = new System.Drawing.Point(91, 30);
            this.dtpFrom.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.dtpFrom.Name = "dtpFrom";
            this.dtpFrom.Size = new System.Drawing.Size(156, 24);
            this.dtpFrom.TabIndex = 1;
            this.dtpFrom.ValueChanged += new System.EventHandler(this.dtpFrom_ValueChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(11, 32);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(44, 18);
            this.label1.TabIndex = 0;
            this.label1.Text = "From";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.dtpTo);
            this.groupBox2.Controls.Add(this.label2);
            this.groupBox2.Controls.Add(this.dtpFrom);
            this.groupBox2.Controls.Add(this.label1);
            this.groupBox2.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.groupBox2.Location = new System.Drawing.Point(13, 196);
            this.groupBox2.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Padding = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.groupBox2.Size = new System.Drawing.Size(557, 81);
            this.groupBox2.TabIndex = 2;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Report Period";
            // 
            // rdbSummary
            // 
            this.rdbSummary.AutoSize = true;
            this.rdbSummary.Checked = true;
            this.rdbSummary.Location = new System.Drawing.Point(77, 21);
            this.rdbSummary.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.rdbSummary.Name = "rdbSummary";
            this.rdbSummary.Size = new System.Drawing.Size(93, 22);
            this.rdbSummary.TabIndex = 0;
            this.rdbSummary.TabStop = true;
            this.rdbSummary.Text = "Summary";
            this.rdbSummary.UseVisualStyleBackColor = true;
            this.rdbSummary.CheckedChanged += new System.EventHandler(this.rdbSummary_CheckedChanged);
            // 
            // rdbDebt
            // 
            this.rdbDebt.AutoSize = true;
            this.rdbDebt.Location = new System.Drawing.Point(784, 21);
            this.rdbDebt.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.rdbDebt.Name = "rdbDebt";
            this.rdbDebt.Size = new System.Drawing.Size(103, 22);
            this.rdbDebt.TabIndex = 3;
            this.rdbDebt.Text = "Settlement ";
            this.rdbDebt.UseVisualStyleBackColor = true;
            this.rdbDebt.CheckedChanged += new System.EventHandler(this.rdbDebt_CheckedChanged);
            // 
            // rdbRefund
            // 
            this.rdbRefund.AutoSize = true;
            this.rdbRefund.Location = new System.Drawing.Point(548, 21);
            this.rdbRefund.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.rdbRefund.Name = "rdbRefund";
            this.rdbRefund.Size = new System.Drawing.Size(76, 22);
            this.rdbRefund.TabIndex = 2;
            this.rdbRefund.Text = "Refund";
            this.rdbRefund.UseVisualStyleBackColor = true;
            this.rdbRefund.CheckedChanged += new System.EventHandler(this.rdbRefund_CheckedChanged);
            // 
            // rdbSale
            // 
            this.rdbSale.AutoSize = true;
            this.rdbSale.Location = new System.Drawing.Point(319, 21);
            this.rdbSale.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.rdbSale.Name = "rdbSale";
            this.rdbSale.Size = new System.Drawing.Size(58, 22);
            this.rdbSale.TabIndex = 1;
            this.rdbSale.Text = "Sale";
            this.rdbSale.UseVisualStyleBackColor = true;
            this.rdbSale.CheckedChanged += new System.EventHandler(this.rdbSale_CheckedChanged);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.rdbSummary);
            this.groupBox1.Controls.Add(this.rdbDebt);
            this.groupBox1.Controls.Add(this.rdbRefund);
            this.groupBox1.Controls.Add(this.rdbSale);
            this.groupBox1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.groupBox1.Location = new System.Drawing.Point(13, 135);
            this.groupBox1.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Padding = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.groupBox1.Size = new System.Drawing.Size(995, 60);
            this.groupBox1.TabIndex = 1;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Report Catergory";
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.cboshoplist);
            this.groupBox4.Controls.Add(this.label4);
            this.groupBox4.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.groupBox4.Location = new System.Drawing.Point(587, 203);
            this.groupBox4.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Padding = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.groupBox4.Size = new System.Drawing.Size(421, 74);
            this.groupBox4.TabIndex = 3;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Report by shop";
            // 
            // cboshoplist
            // 
            this.cboshoplist.FormattingEnabled = true;
            this.cboshoplist.Location = new System.Drawing.Point(103, 27);
            this.cboshoplist.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.cboshoplist.Name = "cboshoplist";
            this.cboshoplist.Size = new System.Drawing.Size(265, 26);
            this.cboshoplist.TabIndex = 1;
            this.cboshoplist.SelectedIndexChanged += new System.EventHandler(this.cboshoplist_SelectedIndexChanged);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(17, 31);
            this.label4.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(43, 18);
            this.label4.TabIndex = 0;
            this.label4.Text = "Shop";
            // 
            // TransactionReport_FOC_MPU
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.ClientSize = new System.Drawing.Size(1024, 918);
            this.Controls.Add(this.groupBox4);
            this.Controls.Add(this.gbPaymentType);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.gbTransactionList);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.Name = "TransactionReport_FOC_MPU";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Transaction Report";
            this.Load += new System.EventHandler(this.TransactionReport_FOC_MPU_Load);
            this.gbPaymentType.ResumeLayout(false);
            this.gbPaymentType.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.gbTransactionList.ResumeLayout(false);
            this.gbTransactionList.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.CheckBox chkFOC;
        private System.Windows.Forms.CheckBox chkMPU;
        private System.Windows.Forms.CheckBox chkCredit;
        private System.Windows.Forms.CheckBox chkCash;
        private System.Windows.Forms.GroupBox gbPaymentType;
        private System.Windows.Forms.CheckBox chkGiftCard;
        private System.Windows.Forms.CheckBox chkCounter;
        private System.Windows.Forms.CheckBox chkCashier;
        private System.Windows.Forms.Label lblCounterName;
        private System.Windows.Forms.Label lblCashierName;
        private System.Windows.Forms.ComboBox cboCounter;
        private System.Windows.Forms.ComboBox cboCashier;
        private Microsoft.Reporting.WinForms.ReportViewer reportViewer1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.GroupBox gbTransactionList;
        private System.Windows.Forms.Label lblPeriod;
        private System.Windows.Forms.DateTimePicker dtpTo;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.DateTimePicker dtpFrom;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.RadioButton rdbSummary;
        private System.Windows.Forms.RadioButton rdbDebt;
        private System.Windows.Forms.RadioButton rdbRefund;
        private System.Windows.Forms.RadioButton rdbSale;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.CheckBox chkTester;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.ComboBox cboshoplist;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.CheckBox chkPay;
        private System.Windows.Forms.CheckBox chkGlobalCard;
        private System.Windows.Forms.CheckBox chkBankTransfer;
    }
}