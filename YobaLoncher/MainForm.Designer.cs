namespace YobaLoncher {
	partial class MainForm {
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing) {
			if (disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent() {
			this.components = new System.ComponentModel.Container();
			this.updateProgressBar = new System.Windows.Forms.ProgressBar();
			this.updateLabelText = new System.Windows.Forms.Label();
			this.launchGameBtn = new YobaButton();
			this.SuspendLayout();
			// 
			// updateProgressBar
			// 
			this.updateProgressBar.Location = new System.Drawing.Point(19, 399);
			this.updateProgressBar.Name = "updateProgressBar";
			this.updateProgressBar.Size = new System.Drawing.Size(602, 23);
			this.updateProgressBar.TabIndex = 0;
			// 
			// updateLabelText
			// 
			this.updateLabelText.AutoSize = true;
			this.updateLabelText.BackColor = System.Drawing.Color.Transparent;
			this.updateLabelText.ForeColor = System.Drawing.Color.White;
			this.updateLabelText.Location = new System.Drawing.Point(20, 375);
			this.updateLabelText.Name = "updateLabelText";
			this.updateLabelText.Size = new System.Drawing.Size(87, 13);
			this.updateLabelText.TabIndex = 1;
			this.updateLabelText.Text = "updateLabelText";
			// 
			// launchGameBtn
			// 
			this.launchGameBtn.Location = new System.Drawing.Point(642, 344);
			this.launchGameBtn.Name = "launchGameBtn";
			this.launchGameBtn.Size = new System.Drawing.Size(124, 79);
			this.launchGameBtn.TabIndex = 2;
			this.launchGameBtn.Text = "Launch!";
			this.launchGameBtn.UseVisualStyleBackColor = true;
			this.launchGameBtn.Click += new System.EventHandler(this.launchGameBtn_Click);
			// 
			// MainForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(62)))), ((int)(((byte)(63)))), ((int)(((byte)(64)))));
			this.ClientSize = new System.Drawing.Size(784, 441);
			this.Controls.Add(this.launchGameBtn);
			this.Controls.Add(this.updateLabelText);
			this.Controls.Add(this.updateProgressBar);
			this.Name = "MainForm";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Yoba Loncher";
			this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.MainForm_FormClosed);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.ProgressBar updateProgressBar;
		private System.Windows.Forms.Label updateLabelText;
		private YobaButton launchGameBtn;
	}
}