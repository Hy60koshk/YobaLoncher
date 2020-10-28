using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Text;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace YobaLoncher {
	public partial class MainForm : Form {
		public const int WM_NCLBUTTONDOWN = 0xA1;
		public const int HT_CAPTION = 0x2;
		[System.Runtime.InteropServices.DllImport("user32.dll")]
		public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
		[System.Runtime.InteropServices.DllImport("user32.dll")]
		public static extern bool ReleaseCapture();

		private DownloadProgressTracker downloadProgressTracker_;
		public string ThePath = "";
		private LinkedList<FileInfo> filesToUpload_;
		private LinkedListNode<FileInfo> currentFile_ = null;
		private bool ReadyToGo_ = false;
		private System.Security.Cryptography.MD5 md5_;

		public MainForm() {
			ThePath = Program.GamePath;

			InitializeComponent();

			SuspendLayout();

			int missingFilesCount = Program.GameFileCheckResult.InvalidFiles.Count;

			if (missingFilesCount > 0) {
				updateLabelText.Text = String.Format(Locale.Get("FilesMissing"), missingFilesCount);
				launchGameBtn.Text = Locale.Get("UpdateBtn");
				filesToUpload_ = Program.GameFileCheckResult.InvalidFiles;
			}
			else {
				updateLabelText.Text = Locale.Get("AllFilesIntact");
				ReadyToGo_ = true;
				launchGameBtn.Text = Locale.Get("LaunchBtn");
			}
			changelogMenuBtn.Text = Locale.Get("ChangelogBtn");
			linksMenuBtn.Text = Locale.Get("LinksBtn");
			settingsButton.Text = Locale.Get("SettingsBtn");
			checkResultMenuBtn.Text = Locale.Get("StatusBtn");
			Text = Locale.Get("MainFormTitle");

			theToolTip.SetToolTip(closeButton, Locale.Get("Close"));
			theToolTip.SetToolTip(minimizeButton, Locale.Get("Minimize"));
			theToolTip.SetToolTip(changelogMenuBtn, Locale.Get("ChangelogTooltip"));
			theToolTip.SetToolTip(checkResultMenuBtn, Locale.Get("StatusTooltip"));
			theToolTip.SetToolTip(linksMenuBtn, Locale.Get("LinksTooltip"));
			theToolTip.SetToolTip(settingsButton, Locale.Get("SettingsTooltip"));

			linksPanel.Location = new Point(153, 25);
			linksPanel.Size = new Size(610, 330);
			statusPanel.Location = new Point(153, 25);
			statusPanel.Size = new Size(610, 330);

			if (Program.OfflineMode) {
				Controls.Remove(webBrowser1);
			} 
			else {
				if (Program.LoncherSettings.ChangelogSite.Length > 0) {
					webBrowser1.Url = new Uri(Program.LoncherSettings.ChangelogSite);
				}
				else {
					webBrowser1.DocumentText = Program.LoncherSettings.ChangelogHtml;//"data:text/html;charset=UTF-8," + 
				}
			}

			if (Program.LoncherSettings.UI.ContainsKey("UpdateLabel")) {
				UIElement updateLabelInfo = Program.LoncherSettings.UI["UpdateLabel"];
				if (updateLabelInfo != null) {
					updateLabelText.BackColor = YU.colorFromString(updateLabelInfo.BgColor, Color.Transparent);
					updateLabelText.ForeColor = YU.colorFromString(updateLabelInfo.Color, Color.White);
					YU.setFont(updateLabelText, updateLabelInfo.Font, updateLabelInfo.FontSize);
				}
			}
			string[] menuBtnKeys = new string[] {
				"LaunchButton", "SettingsButton", "StatusButton", "LinksButton", "ChangelogButton", "ModsButton", "CloseButton", "MinimizeButton"
			};
			foreach (string menuBtnKey in menuBtnKeys) {
				if (Program.LoncherSettings.UI.ContainsKey(menuBtnKey)) {
					UIElement launchButtonInfo = Program.LoncherSettings.UI[menuBtnKey];
					if (launchButtonInfo != null) {
						Control[] ctrls = Controls.Find(menuBtnKey, true);
						if (ctrls.Length > 0) {
							((YobaButtonAbs)ctrls[0]).ApplyUIStyles(launchButtonInfo);
						}
					}
				}
			}
			
			for (int i = 0; i < Program.LoncherSettings.Buttons.Count; i++) {
				LinkButton lbtn = Program.LoncherSettings.Buttons[i];
				if (lbtn != null) {
					YobaButton linkButton = new YobaButton(lbtn.Url);
					linkButton.Name = "linkBtn" + (i + 1);
					linkButton.TabIndex = 10 + i;
					linkButton.UseVisualStyleBackColor = true;
					linkButton.ApplyUIStyles(lbtn);

					linkButton.Click += new EventHandler((object o, EventArgs a) => {
						string url = ((YobaButton)o).Url;
						if (YU.stringHasText(url)) {
							Process.Start(url);
						}
					});
					this.Controls.Add(linkButton);
				}
			}

			BackgroundImageLayout = ImageLayout.Stretch;
			BackgroundImage = Program.LoncherSettings.Background;

			PerformLayout();
		}

		private bool checkFileMD5(string path, string correctHash) {
			byte[] hash;
			if (md5_ == null) {
				md5_ = System.Security.Cryptography.MD5.Create();
			}
			using (FileStream stream = File.OpenRead(path)) {
				hash = md5_.ComputeHash(stream);
			}
			StringBuilder hashSB = new StringBuilder(hash.Length);
			for (int i = 0; i < hash.Length; i++) {
				hashSB.Append(hash[i].ToString("X2"));
			}
			return correctHash.ToUpper().Equals(hashSB.ToString());
		}

		private async void DownloadFile(FileInfo fileInfo) {
			if (!YU.stringHasText(fileInfo.UploadAlias)) {
				fileInfo.UploadAlias = fileInfo.Hashes.Count > 0 ? fileInfo.Hashes[0] : null;
				if (!YU.stringHasText(fileInfo.UploadAlias)) {
					int lios = Math.Max(fileInfo.Path.LastIndexOf('\\'), fileInfo.Path.LastIndexOf('/'));
					fileInfo.UploadAlias = lios > -1 ? fileInfo.Path.Substring(lios + 1, fileInfo.Path.Length) : fileInfo.Path;
				}
			}
			string uploadFilename = PreloaderForm.UPDPATH + fileInfo.UploadAlias;
			if (File.Exists(uploadFilename)) {
				if (FileChecker.CheckFileMD5(PreloaderForm.UPDPATH, fileInfo)) {
					DownloadNext();
					return;
				}
				else {
					File.Delete(uploadFilename);
				}
			}
			using (WebClient webClient = new WebClient()) {
				try {
					webClient.DownloadProgressChanged += new DownloadProgressChangedEventHandler(OnDownloadProgressChanged);
					webClient.DownloadFileCompleted += new AsyncCompletedEventHandler(OnDownloadCompleted);
					await webClient.DownloadFileTaskAsync(new Uri(fileInfo.Url), uploadFilename);
				}
				catch (Exception ex) {
					YobaDialog.ShowDialog(string.Format(Locale.Get("CannotDownloadFile"), fileInfo.Path) + "\r\n" + ex.Message);
				}
			}
		}

		private void OnDownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e) {
			downloadProgressTracker_.SetProgress(e.BytesReceived, e.TotalBytesToReceive);
			updateProgressBar.Value = e.ProgressPercentage;
			updateLabelText.Text = string.Format(
				Locale.Get("DLRate")
				, FormatBytes(e.BytesReceived)
				, FormatBytes(e.TotalBytesToReceive)
				, downloadProgressTracker_.GetBytesPerSecondString()
				, currentFile_.Value.Description
			);
		}

		private void OnDownloadCompleted(object sender, AsyncCompletedEventArgs e) {
			downloadProgressTracker_.Reset();
			DownloadNext();
		}

		private void DownloadNext() {
			currentFile_ = currentFile_.Next;
			if (currentFile_ == null) {
				string filename = "";
				try {
					foreach (FileInfo fileInfo in filesToUpload_) {
						filename = fileInfo.Path;
						String[] pathparts = filename.Split('/');
						string dirpath = ThePath;
						for (int j = 0; j < pathparts.Length - 1; j++) {
							dirpath = dirpath + pathparts[j];
							if (!Directory.Exists(dirpath)) {
								Directory.CreateDirectory(dirpath);
							}
							dirpath += "\\";
						}
						if (File.Exists(ThePath + fileInfo.Path)) {
							File.Delete(ThePath + fileInfo.Path);
						}
						File.Move(PreloaderForm.UPDPATH + fileInfo.UploadAlias, ThePath + fileInfo.Path);
					}
					ReadyToGo_ = true;
					launchGameBtn.Text = Locale.Get("LaunchBtn");
					if (YobaDialog.ShowDialog(Locale.Get("UpdateSuccessful"), YobaDialog.YesNoBtns) == DialogResult.Yes) {
						launch();
					}
				}
				catch (Exception ex) {
					YobaDialog.ShowDialog(string.Format(Locale.Get("CannotMoveFile"), filename) + "\r\n" + ex.Message);
				}
			}
			else {
				DownloadFile(currentFile_.Value);
			}
		}

		public static string FormatBytes(long byteCount) {
			string[] suf = { "B", "KB", "MB", "GB", "TB", "PB", "ЁБ" }; //Longs run out around EB
			if (byteCount == 0)
				return "0" + suf[0];
			long bytes = Math.Abs(byteCount);
			int place = Convert.ToInt32(Math.Floor(Math.Log(bytes, 1024)));
			double num = Math.Round(bytes / Math.Pow(1024, place), 1);
			return (Math.Sign(byteCount) * num).ToString() + suf[place];
		}

		private void MainForm_FormClosed(object sender, FormClosedEventArgs e) {
			Application.Exit();
		}

		private void launchGameBtn_Click(object sender, EventArgs e) {
			if (ReadyToGo_) {
				launch();
			}
			else {
				currentFile_ = filesToUpload_.First;
				downloadProgressTracker_ = new DownloadProgressTracker(50, TimeSpan.FromMilliseconds(500));
				DownloadFile(currentFile_.Value);
			}
		}

		private void launch() {
			string args = "/C \"" + ThePath + Program.LoncherSettings.ExeName + "\"";
			if (ThePath.Contains("steamapps")) {
				args = "/C explorer steam://run/" + Program.LoncherSettings.SteamID;
			}
			Process.Start(new ProcessStartInfo { Arguments = args, FileName = "cmd", WindowStyle = ProcessWindowStyle.Hidden });
			launchGameBtn.Enabled = false;
			System.Threading.Thread.Sleep(1800);
			launchGameBtn.Enabled = true;
		}

		private void settingsButton_Click(object sender, EventArgs e) {
			SettingsDialog settingsDialog = new SettingsDialog();
			settingsDialog.Icon = Program.LoncherSettings.Icon;
			if (settingsDialog.ShowDialog(this) == DialogResult.OK) {
				string path = settingsDialog.GamePath;
				if (path != Program.GamePath) {
					try {
						File.WriteAllLines(PreloaderForm.CFGFILE, new string[] {
							"path = " + path
						});
						Hide();
						new PreloaderForm(this).Show();
					}
					catch (Exception ex) {
						YobaDialog.ShowDialog(Locale.Get("CannotWriteCfg") + ":\r\n" + ex.Message);
					}
				}
				settingsDialog.Dispose();
			}
		}

		private void changelogMenuBtn_Click(object sender, EventArgs e) {
			linksPanel.Visible = false;
			statusPanel.Visible = false;
		}

		private void checkResultMenuBtn_Click(object sender, EventArgs e) {
			linksPanel.Visible = false;
			statusPanel.Visible = true;
		}

		private void linksMenuBtn_Click(object sender, EventArgs e) {
			linksPanel.Visible = true;
			statusPanel.Visible = false;
		}

		private void closeButton_Click(object sender, EventArgs e) {
			Application.Exit();
		}

		private void minimizeButton_Click(object sender, EventArgs e) {
			WindowState = FormWindowState.Minimized;
		}

		private void draggingPanel_MouseDown(object sender, MouseEventArgs e) {
			if (e.Button == MouseButtons.Left) {
				ReleaseCapture();
				SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
			}
		}
	}
}