namespace TZInfo
{
	partial class Form1
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose ( bool disposing )
		{
			if ( disposing && ( components != null ) )
			{
				components.Dispose ( );
			}
			base.Dispose ( disposing );
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent ( )
		{
			this.lblTimeZone = new System.Windows.Forms.Label();
			this.cboTimeZone = new System.Windows.Forms.ComboBox();
			this.lblDisplayName = new System.Windows.Forms.Label();
			this.txtDisplayName = new System.Windows.Forms.TextBox();
			this.lblStandardName = new System.Windows.Forms.Label();
			this.txtStandardName = new System.Windows.Forms.TextBox();
			this.lblDaylightName = new System.Windows.Forms.Label();
			this.txtDaylightName = new System.Windows.Forms.TextBox();
			this.lblBaseUTCoffset = new System.Windows.Forms.Label();
			this.txtBaseUTCoffset = new System.Windows.Forms.TextBox();
			this.lblSupportsDST = new System.Windows.Forms.Label();
			this.txtSupportsDST = new System.Windows.Forms.TextBox();
			this.textBox1 = new System.Windows.Forms.TextBox();
			this.grpDisplayOrder = new System.Windows.Forms.GroupBox();
			this.SuspendLayout();
			// 
			// lblTimeZone
			// 
			this.lblTimeZone.AutoSize = true;
			this.lblTimeZone.Location = new System.Drawing.Point(13, 22);
			this.lblTimeZone.Name = "lblTimeZone";
			this.lblTimeZone.Size = new System.Drawing.Size(100, 13);
			this.lblTimeZone.TabIndex = 0;
			this.lblTimeZone.Text = "Select a Time Zone";
			// 
			// cboTimeZone
			// 
			this.cboTimeZone.FormattingEnabled = true;
			this.cboTimeZone.Location = new System.Drawing.Point(120, 22);
			this.cboTimeZone.Name = "cboTimeZone";
			this.cboTimeZone.Size = new System.Drawing.Size(270, 21);
			this.cboTimeZone.TabIndex = 1;
			// 
			// lblDisplayName
			// 
			this.lblDisplayName.AutoSize = true;
			this.lblDisplayName.Location = new System.Drawing.Point(13, 53);
			this.lblDisplayName.Name = "lblDisplayName";
			this.lblDisplayName.Size = new System.Drawing.Size(69, 13);
			this.lblDisplayName.TabIndex = 2;
			this.lblDisplayName.Text = "DisplayName";
			// 
			// txtDisplayName
			// 
			this.txtDisplayName.Location = new System.Drawing.Point(120, 53);
			this.txtDisplayName.Name = "txtDisplayName";
			this.txtDisplayName.ReadOnly = true;
			this.txtDisplayName.Size = new System.Drawing.Size(250, 20);
			this.txtDisplayName.TabIndex = 3;
			// 
			// lblStandardName
			// 
			this.lblStandardName.AutoSize = true;
			this.lblStandardName.Location = new System.Drawing.Point(13, 84);
			this.lblStandardName.Name = "lblStandardName";
			this.lblStandardName.Size = new System.Drawing.Size(78, 13);
			this.lblStandardName.TabIndex = 4;
			this.lblStandardName.Text = "StandardName";
			// 
			// txtStandardName
			// 
			this.txtStandardName.Location = new System.Drawing.Point(120, 84);
			this.txtStandardName.Name = "txtStandardName";
			this.txtStandardName.ReadOnly = true;
			this.txtStandardName.Size = new System.Drawing.Size(250, 20);
			this.txtStandardName.TabIndex = 5;
			// 
			// lblDaylightName
			// 
			this.lblDaylightName.AutoSize = true;
			this.lblDaylightName.Location = new System.Drawing.Point(13, 115);
			this.lblDaylightName.Name = "lblDaylightName";
			this.lblDaylightName.Size = new System.Drawing.Size(73, 13);
			this.lblDaylightName.TabIndex = 6;
			this.lblDaylightName.Text = "DaylightName";
			// 
			// txtDaylightName
			// 
			this.txtDaylightName.Location = new System.Drawing.Point(120, 115);
			this.txtDaylightName.Name = "txtDaylightName";
			this.txtDaylightName.ReadOnly = true;
			this.txtDaylightName.Size = new System.Drawing.Size(250, 20);
			this.txtDaylightName.TabIndex = 7;
			// 
			// lblBaseUTCoffset
			// 
			this.lblBaseUTCoffset.AutoSize = true;
			this.lblBaseUTCoffset.Location = new System.Drawing.Point(13, 146);
			this.lblBaseUTCoffset.Name = "lblBaseUTCoffset";
			this.lblBaseUTCoffset.Size = new System.Drawing.Size(79, 13);
			this.lblBaseUTCoffset.TabIndex = 8;
			this.lblBaseUTCoffset.Text = "BaseUTCoffset";
			// 
			// txtBaseUTCoffset
			// 
			this.txtBaseUTCoffset.Location = new System.Drawing.Point(120, 146);
			this.txtBaseUTCoffset.Name = "txtBaseUTCoffset";
			this.txtBaseUTCoffset.ReadOnly = true;
			this.txtBaseUTCoffset.Size = new System.Drawing.Size(250, 20);
			this.txtBaseUTCoffset.TabIndex = 9;
			// 
			// lblSupportsDST
			// 
			this.lblSupportsDST.AutoSize = true;
			this.lblSupportsDST.Location = new System.Drawing.Point(13, 177);
			this.lblSupportsDST.Name = "lblSupportsDST";
			this.lblSupportsDST.Size = new System.Drawing.Size(71, 13);
			this.lblSupportsDST.TabIndex = 10;
			this.lblSupportsDST.Text = "SupportsDST";
			// 
			// txtSupportsDST
			// 
			this.txtSupportsDST.Location = new System.Drawing.Point(120, 177);
			this.txtSupportsDST.Name = "txtSupportsDST";
			this.txtSupportsDST.ReadOnly = true;
			this.txtSupportsDST.Size = new System.Drawing.Size(250, 20);
			this.txtSupportsDST.TabIndex = 11;
			// 
			// textBox1
			// 
			this.textBox1.Location = new System.Drawing.Point(13, 239);
			this.textBox1.Name = "textBox1";
			this.textBox1.ReadOnly = true;
			this.textBox1.Size = new System.Drawing.Size(600, 20);
			this.textBox1.TabIndex = 0;
			this.textBox1.TabStop = false;
			// 
			// grpDisplayOrder
			// 
			this.grpDisplayOrder.Location = new System.Drawing.Point(422, 22);
			this.grpDisplayOrder.Name = "grpDisplayOrder";
			this.grpDisplayOrder.Size = new System.Drawing.Size(200, 100);
			this.grpDisplayOrder.TabIndex = 12;
			this.grpDisplayOrder.TabStop = false;
			this.grpDisplayOrder.Text = "groupBox1";
			// 
			// Form1
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(644, 267);
			this.Controls.Add(this.grpDisplayOrder);
			this.Controls.Add(this.textBox1);
			this.Controls.Add(this.txtSupportsDST);
			this.Controls.Add(this.lblSupportsDST);
			this.Controls.Add(this.txtBaseUTCoffset);
			this.Controls.Add(this.lblBaseUTCoffset);
			this.Controls.Add(this.txtDaylightName);
			this.Controls.Add(this.lblDaylightName);
			this.Controls.Add(this.txtStandardName);
			this.Controls.Add(this.lblStandardName);
			this.Controls.Add(this.txtDisplayName);
			this.Controls.Add(this.lblDisplayName);
			this.Controls.Add(this.cboTimeZone);
			this.Controls.Add(this.lblTimeZone);
			this.Name = "Form1";
			this.Text = "Form1";
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Label lblTimeZone;
		private System.Windows.Forms.ComboBox cboTimeZone;
		private System.Windows.Forms.Label lblDisplayName;
		private System.Windows.Forms.TextBox txtDisplayName;
		private System.Windows.Forms.Label lblStandardName;
		private System.Windows.Forms.TextBox txtStandardName;
		private System.Windows.Forms.Label lblDaylightName;
		private System.Windows.Forms.TextBox txtDaylightName;
		private System.Windows.Forms.Label lblBaseUTCoffset;
		private System.Windows.Forms.TextBox txtBaseUTCoffset;
		private System.Windows.Forms.Label lblSupportsDST;
		private System.Windows.Forms.TextBox txtSupportsDST;
		private System.Windows.Forms.TextBox textBox1;
		private System.Windows.Forms.GroupBox grpDisplayOrder;
	}
}

