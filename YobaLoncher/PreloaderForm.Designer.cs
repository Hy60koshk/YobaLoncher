namespace YobaLoncher {
	partial class PreloaderForm {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PreloaderForm));
			this.progressBar1 = new System.Windows.Forms.ProgressBar();
			this.loadingLabel = new System.Windows.Forms.Label();
			this.loadingLabelError = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// progressBar1
			// 
			this.progressBar1.Location = new System.Drawing.Point(12, 277);
			this.progressBar1.Name = "progressBar1";
			this.progressBar1.Size = new System.Drawing.Size(376, 11);
			this.progressBar1.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
			this.progressBar1.TabIndex = 0;
			// 
			// loadingLabel
			// 
			this.loadingLabel.AutoSize = true;
			this.loadingLabel.BackColor = System.Drawing.Color.Transparent;
			this.loadingLabel.ForeColor = System.Drawing.Color.White;
			this.loadingLabel.Location = new System.Drawing.Point(9, 261);
			this.loadingLabel.Name = "loadingLabel";
			this.loadingLabel.Size = new System.Drawing.Size(117, 13);
			this.loadingLabel.TabIndex = 1;
			this.loadingLabel.Text = "YobaLoncher loading...";
			// 
			// loadingLabelError
			// 
			this.loadingLabelError.AutoSize = true;
			this.loadingLabelError.BackColor = System.Drawing.Color.Transparent;
			this.loadingLabelError.ForeColor = System.Drawing.SystemColors.ButtonShadow;
			this.loadingLabelError.Location = new System.Drawing.Point(9, 9);
			this.loadingLabelError.Name = "loadingLabelError";
			this.loadingLabelError.Size = new System.Drawing.Size(0, 13);
			this.loadingLabelError.TabIndex = 2;
			// 
			// PreloaderForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackColor = System.Drawing.Color.DimGray;
			this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
			this.ClientSize = new System.Drawing.Size(400, 300);
			this.Controls.Add(this.loadingLabelError);
			this.Controls.Add(this.loadingLabel);
			this.Controls.Add(this.progressBar1);
			this.DoubleBuffered = true;
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "PreloaderForm";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Yoba Loncher";
			this.Load += new System.EventHandler(this.PreloaderForm_Load);
			this.Shown += new System.EventHandler(this.PreloaderForm_ShownAsync);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.ProgressBar progressBar1;
		private System.Windows.Forms.Label loadingLabel;
		private System.Windows.Forms.Label loadingLabelError;
	}
}

