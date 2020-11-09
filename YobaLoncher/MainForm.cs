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
		private Dictionary<FileInfo, Label> fileIndicators_ = new Dictionary<FileInfo, Label>();
		private LinkedListNode<FileInfo> currentFile_ = null;
		private bool ReadyToGo_ = false;

		private long lastDlstringUpdate_ = -1;

		public MainForm() {
			ThePath = Program.GamePath;

			InitializeComponent();

			int winstyle = NativeWinAPI.GetWindowLong(basePanel.Handle, NativeWinAPI.GWL_EXSTYLE);
			NativeWinAPI.SetWindowLong(basePanel.Handle, NativeWinAPI.GWL_EXSTYLE, winstyle | NativeWinAPI.WS_EX_COMPOSITED);

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

			changelogPanel.Location = new Point(0, 0);
			changelogPanel.Size = new Size(610, 330);
			modsPanel.Location = new Point(0, 0);
			modsPanel.Size = new Size(610, 330);
			linksPanel.Location = new Point(0, 0);
			linksPanel.Size = new Size(610, 330);
			statusPanel.Location = new Point(0, 0);
			statusPanel.Size = new Size(610, 330);

			if (Program.OfflineMode) {
				Controls.Remove(changelogBrowser);
			} 
			else {
				changelogBrowser.Location = new Point(0, 0);
				changelogBrowser.Size = new Size(610, 330);
				if (Program.LoncherSettings.ChangelogSite.Length > 0) {
					changelogBrowser.Url = new Uri(Program.LoncherSettings.ChangelogSite);
				}
				else {
					changelogBrowser.DocumentText = Program.LoncherSettings.ChangelogHtml;//"data:text/html;charset=UTF-8," + 
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
			string[] menuScreenKeys = new string[] {
				"BasePanel", "StatusPanel", "LinksPanel", "ModsPanel", "ChangelogPanel"
			};
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
			foreach (string menuScreenKey in menuScreenKeys) {
				if (Program.LoncherSettings.UI.ContainsKey(menuScreenKey)) {
					UIElement uiInfo = Program.LoncherSettings.UI[menuScreenKey];
					if (uiInfo != null) {
						Control[] ctrls = Controls.Find(menuScreenKey, true);
						if (ctrls.Length > 0) {
							Panel panel = (Panel)ctrls[0];
							if (uiInfo.Position != null) {
								panel.Location = new Point(uiInfo.Position.X, uiInfo.Position.Y);
							}
							if (uiInfo.Size != null) {
								panel.Size = new Size(uiInfo.Size.X, uiInfo.Size.Y);
							}
							if (YU.stringHasText(uiInfo.Color)) {
								panel.ForeColor = YU.colorFromString(uiInfo.Color, Color.White);
							}
							if (YU.stringHasText(uiInfo.BgColor)) {
								panel.BackColor = YU.colorFromString(uiInfo.BgColor, Color.DimGray);
							}
							if (uiInfo.BgImage != null && YU.stringHasText(uiInfo.BgImage.Path)) {
								if (YU.stringHasText(uiInfo.BgImage.Layout)) {
									try {
										Enum.Parse(typeof(ImageLayout), uiInfo.BgImage.Layout);
									}
									catch {
										panel.BackgroundImageLayout = ImageLayout.Stretch;
									}
								}
								else {
									panel.BackgroundImageLayout = ImageLayout.Stretch;
								}
								panel.BackgroundImage = new Bitmap(PreloaderForm.IMGPATH + uiInfo.BgImage.Path);
							}
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
					if (YU.stringHasText(lbtn.Caption)) {
						linkButton.Text = "";
						theToolTip.SetToolTip(linkButton, lbtn.Caption);
					}
					linkButton.Click += new EventHandler((object o, EventArgs a) => {
						string url = ((YobaButton)o).Url;
						if (YU.stringHasText(url)) {
							Process.Start(url);
						}
					});
					linksPanel.Controls.Add(linkButton);
				}
			}

			BackgroundImageLayout = ImageLayout.Stretch;
			BackgroundImage = Program.LoncherSettings.Background;

			switch (LauncherConfig.StartPage) {
				case StartPageEnum.Changelog:
					changelogPanel.Visible = true;
					break;
				case StartPageEnum.Mods:
					modsPanel.Visible = true;
					break;
				case StartPageEnum.Status:
					statusPanel.Visible = true;
					break;
				case StartPageEnum.Links:
					linksPanel.Visible = true;
					break;
			}

			for (int i = 0; i < Program.LoncherSettings.Files.Count; i++) {
				FileInfo fileInfo = Program.LoncherSettings.Files[i];
				Label indicator = new Label();
				Label fileDesc = new Label();
				indicator.Text = "";
				fileDesc.Text = fileInfo.Description ?? fileInfo.Path;
				fileDesc.Font = new Font("Tahoma", 11F, FontStyle.Regular, GraphicsUnit.Pixel, 204);
				fileDesc.ForeColor = Color.White;
				fileDesc.AutoSize = true;
				fileDesc.BackColor = Color.Transparent;
				indicator.BackColor = Color.Transparent;
				indicator.Size = new Size(9, 9);
				indicator.Image = fileInfo.IsOK ? Resource1.green_dot : Resource1.red_dot;
				indicator.Location = new Point(20, 18 + 22 * i);
				fileDesc.Location = new Point(34, 15 + 22 * i);

				fileIndicators_.Add(fileInfo, indicator);

				statusPanel.Controls.Add(indicator);
				statusPanel.Controls.Add(fileDesc);
			}
			Label padder = new Label();
			padder.Text = "";
			padder.Size = new Size(9, 1);
			padder.BackColor = Color.Transparent;
			padder.Location = new Point(30, 20 + 22 * Program.LoncherSettings.Files.Count);
			statusPanel.Controls.Add(padder);

			PerformLayout();
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
					ShowDownloadError(string.Format(Locale.Get("CannotDownloadFile"), fileInfo.Path) + "\r\n" + ex.Message);
				}
			}
		}

		private void OnDownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e) {
			downloadProgressTracker_.SetProgress(e.BytesReceived, e.TotalBytesToReceive);
			if (DateTime.Now.Ticks > lastDlstringUpdate_ + 500000L) {
				lastDlstringUpdate_ = DateTime.Now.Ticks;
				updateProgressBar.Value = e.ProgressPercentage;
				updateLabelText.Text = string.Format(
					Locale.Get("DLRate")
					, FormatBytes(e.BytesReceived)
					, FormatBytes(e.TotalBytesToReceive)
					, downloadProgressTracker_.GetBytesPerSecondString()
					, currentFile_.Value.Description
				);
			}
		}

		private void OnDownloadCompleted(object sender, AsyncCompletedEventArgs e) {
			downloadProgressTracker_.Reset();
			DownloadNext();
		}

		private void DownloadNext() {
			fileIndicators_[currentFile_.Value].Image = Resource1.yellow_dot;
			currentFile_ = currentFile_.Next;
			if (currentFile_ == null) {
				string filename = "";
				updateProgressBar.Value = 100;
				updateLabelText.Text = Locale.Get("StatusCopyingFiles");
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
						fileIndicators_[fileInfo].Image = Resource1.green_dot;
					}
					ReadyToGo_ = true;
					updateLabelText.Text = Locale.Get("StatusUpdatingDone");
					launchGameBtn.Text = Locale.Get("LaunchBtn");
					launchGameBtn.Enabled = true;
					if (YobaDialog.ShowDialog(Locale.Get("UpdateSuccessful"), YobaDialog.YesNoBtns) == DialogResult.Yes) {
						launch();
					}
				}
				catch (UnauthorizedAccessException ex) {
					ShowDownloadError(string.Format(Locale.Get("DirectoryAccessDenied"), filename) + ":\r\n" + ex.Message);
				}
				catch (Exception ex) {
					ShowDownloadError(string.Format(Locale.Get("CannotMoveFile"), filename) + ":\r\n" + ex.Message);
				}
			}
			else {
				DownloadFile(currentFile_.Value);
			}
		}

		private void ShowDownloadError(string error) {
			YobaDialog.ShowDialog(error);
			launchGameBtn.Enabled = true;
			updateProgressBar.Value = 0;
			updateLabelText.Text = Locale.Get("StatusDownloadError");
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
				launchGameBtn.Enabled = false;
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
				LauncherConfig.StartPage = settingsDialog.OpeningPanel;
				LauncherConfig.GameDir = settingsDialog.GamePath;
				LauncherConfig.Save();
				if (LauncherConfig.GameDir != Program.GamePath) {
					Hide();
					new PreloaderForm(this).Show();
				}
				settingsDialog.Dispose();
			}
		}

		private void changelogMenuBtn_Click(object sender, EventArgs e) {
			linksPanel.Visible = false;
			statusPanel.Visible = false;
			modsPanel.Visible = false;
			changelogPanel.Visible = true;
		}

		private void checkResultMenuBtn_Click(object sender, EventArgs e) {
			linksPanel.Visible = false;
			statusPanel.Visible = true;
			modsPanel.Visible = false;
			changelogPanel.Visible = false;
		}

		private void linksMenuBtn_Click(object sender, EventArgs e) {
			linksPanel.Visible = true;
			statusPanel.Visible = false;
			modsPanel.Visible = false;
			changelogPanel.Visible = false;
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

		private async void refreshButton_Click(object sender, EventArgs e) {
			if (!Program.OfflineMode) {
				await Program.LoncherSettings.InitChangelog();
			
				if (Program.LoncherSettings.ChangelogSite.Length > 0) {
					changelogBrowser.Url = new Uri(Program.LoncherSettings.ChangelogSite);
				}
				else {
					changelogBrowser.DocumentText = Program.LoncherSettings.ChangelogHtml;
				}
			}
		}
	}
}