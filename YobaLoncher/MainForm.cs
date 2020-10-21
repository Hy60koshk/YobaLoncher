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
		private DownloadProgressTracker downloadProgressTracker_;
		public string ThePath = "";
		private LinkedList<FileInfo> filesToUpload_ = new LinkedList<FileInfo>();
		private LinkedListNode<FileInfo> currentFile_ = null;
		private bool ReadyToGo_ = false;
		private System.Security.Cryptography.MD5 md5_;

		public MainForm() {
			InitializeComponent();

			string missingFiles = "";
			int missingFilesCount = 0;

			foreach (FileInfo fi in Program.launcherData.Files) {
				if (!fi.IsOK) {
					filesToUpload_.AddLast(fi);
					missingFilesCount++;
					missingFiles += fi.Description;
				}
			}
			if (missingFilesCount > 0) {
				updateLabelText.Text = missingFilesCount + " files are missing or out of date.";
				launchGameBtn.Text = "Update";
			}
			else {
				updateLabelText.Text = "All files intact, we're ready to go.";
				ReadyToGo_ = true;
			}

			SuspendLayout();
			if (!Program.OfflineMode) {
				Awesomium.Windows.Forms.WebControl webControl1 = new Awesomium.Windows.Forms.WebControl(this.components);
				webControl1.Location = new System.Drawing.Point(22, 23);
				webControl1.Size = new System.Drawing.Size(595, 334);
				webControl1.TabIndex = 4;
				webControl1.ConsoleMessage += new Awesomium.Core.ConsoleMessageEventHandler(OnAweError);
				if (Program.launcherData.ChangelogSite.Length > 0) {
					webControl1.Source = new Uri(Program.launcherData.ChangelogSite);
				}
				else {
					webControl1.Source = new Uri("data:text/html;charset=UTF-8," + Program.launcherData.ChangelogHtml, UriKind.Absolute);
				}
				Controls.Add(webControl1);
			}

			if (Program.launcherData.UI.ContainsKey("UpdateLabel")) {
				UIElement updateLabelInfo = Program.launcherData.UI["UpdateLabel"];
				if (updateLabelInfo != null) {
					updateLabelText.BackColor = colorFromString(updateLabelInfo.BgColor ?? "transparent");
					updateLabelText.ForeColor = colorFromString(updateLabelInfo.Color ?? "black");
					setFont(updateLabelText, updateLabelInfo.Font, updateLabelInfo.FontSize);
				}
			}
			if (Program.launcherData.UI.ContainsKey("LaunchButton")) {
				UIElement launchButtonInfo = Program.launcherData.UI["LaunchButton"];
				if (launchButtonInfo != null) {
					launchGameBtn.BackColor = colorFromString(launchButtonInfo.BgColor ?? "control");
					launchGameBtn.ForeColor = colorFromString(launchButtonInfo.Color ?? "black");
					setFont(launchGameBtn, launchButtonInfo.Font, launchButtonInfo.FontSize);
					if (launchButtonInfo.CustomStyle) {
						launchGameBtn.FlatStyle = FlatStyle.Flat;
						launchGameBtn.ForeColor = colorFromString(launchButtonInfo.Color ?? "black");
						launchGameBtn.FlatAppearance.BorderSize = 0;

						if (stringHasText(launchButtonInfo.BgColorHover)) {
							launchGameBtn.FlatAppearance.MouseOverBackColor = colorFromString(launchButtonInfo.BgColorHover ?? "white");
						}
						if (stringHasText(launchButtonInfo.BgColorDown)) {
							launchGameBtn.FlatAppearance.MouseDownBackColor = colorFromString(launchButtonInfo.BgColorDown ?? "white");
						}
					}
					if (launchButtonInfo.BgImage != null) {
						launchGameBtn.BackgroundImageLayout = ImageLayout.Stretch;
						launchGameBtn.BackgroundImage = new Bitmap("images\\" + launchButtonInfo.BgImage.Path);
					}
					if (launchButtonInfo.Position != null) {
						launchGameBtn.Location = new Point(launchButtonInfo.Position.X, launchButtonInfo.Position.Y);
					}
					if (launchButtonInfo.Size != null) {
						launchGameBtn.Size = new Size(launchButtonInfo.Size.X, launchButtonInfo.Size.Y);
					}
				}
			}
			for (int i = 0; i < Program.launcherData.Buttons.Count; i++) {
				LinkButton lbtn = Program.launcherData.Buttons[i];
				if (lbtn != null) {

					YobaButton linkButton = new YobaButton(lbtn.Url);
					linkButton.Location = new Point(lbtn.Position.X, lbtn.Position.Y);
					linkButton.Name = "linkBtn" + (i + 1);
					linkButton.Size = new Size(lbtn.Size.X, lbtn.Size.Y);
					linkButton.TabIndex = 10 + i;
					linkButton.Text = lbtn.Caption;
					linkButton.UseVisualStyleBackColor = true;

					if (lbtn.CustomStyle) {
						linkButton.FlatStyle = FlatStyle.Flat;
						linkButton.BackColor = colorFromString(lbtn.BgColor ?? "dimgrey");
						linkButton.ForeColor = colorFromString(lbtn.Color ?? "white");
						linkButton.FlatAppearance.BorderSize = 0;
						if (stringHasText(lbtn.BgColorHover)) {
							linkButton.FlatAppearance.MouseOverBackColor = colorFromString(lbtn.BgColorHover ?? "white");
						}
						if (stringHasText(lbtn.BgColorDown)) {
							linkButton.FlatAppearance.MouseDownBackColor = colorFromString(lbtn.BgColorDown ?? "white");
						}
					}
					if (lbtn.BgImage != null) {
						linkButton.BackgroundImageLayout = ImageLayout.Stretch;
						linkButton.BackgroundImage = new Bitmap("images\\" + lbtn.BgImage.Path);
					}
					linkButton.Click += new EventHandler((object o, EventArgs a) => {
						string url = ((YobaButton)o).Url;
						if (stringHasText(url)) {
							Process.Start(url);
						}
					});
					this.Controls.Add(linkButton);
				}
			}

			BackgroundImageLayout = ImageLayout.Stretch;
			BackgroundImage = Program.launcherData.Background;

			PerformLayout();
		}

		private static bool stringHasText(string s) {
			return s != null && s.Length > 0;
		}

		private void setFont(Control comp, string fontName, string fontSize) {
			int fs = 12;
			if (stringHasText(fontSize)) {
				if (!int.TryParse(fontSize, out fs)) {
					fs = 12;
				}
			}
			Font font;
			if (stringHasText(fontName)) {
				font = new Font(fontName, fs, FontStyle.Regular, GraphicsUnit.Pixel);
				if (font.Name != fontName) {
					if (Program.launcherData.Fonts[fontName] == "local") {
						PrivateFontCollection pfc = new PrivateFontCollection();
						pfc.AddFontFile("fonts\\" + fontName);
						font = new Font(pfc.Families[0], fs, FontStyle.Regular, GraphicsUnit.Pixel);
					}
					else {
						font = new Font(comp.Font.Name, fs, FontStyle.Regular, GraphicsUnit.Pixel);
					}
				}
			}
			else {
				font = new Font(comp.Font.Name, fs, FontStyle.Regular, GraphicsUnit.Pixel);
			}
			comp.Font = font;
		}

		private Color colorFromString(string str) {
			try {
				if (str[0] == '#') {
					return Color.FromArgb(Int32.Parse(str.Substring(1), System.Globalization.NumberStyles.HexNumber));
				}
				else {
					return Color.FromName(str);
				}
			} catch {
				return Color.Black;
			}
		}

		/*private void MainForm_Shown(object sender, EventArgs e) {
			if (filesToUpload_.Count > 0) {
				currentFile_ = filesToUpload_.First;
				downloadProgressTracker_ = new DownloadProgressTracker(50, TimeSpan.FromMilliseconds(500));
				DownloadFile(currentFile_.Value);
			}
		}*/

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
			string uploadFilename = @"updates/" + fileInfo.Hash;
			if (File.Exists(uploadFilename)) {
				if (checkFileMD5(uploadFilename, fileInfo.Hash)) {
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
					MessageBox.Show("Cannot download file \"" + fileInfo.Description + "\"\r\n" + ex.Message, "Error", MessageBoxButtons.OK);
				}
			}
		}

		private void OnAweError(object sender, Awesomium.Core.ConsoleMessageEventArgs e) {
			MessageBox.Show(e.Message + " @ " + e.Source + ":" + e.LineNumber);
		}

		private void OnDownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e) {
			downloadProgressTracker_.SetProgress(e.BytesReceived, e.TotalBytesToReceive);
			updateProgressBar.Value = e.ProgressPercentage;
			updateLabelText.Text = string.Format(
				"Downloading: {3} - {0} of {1} @ {2}"
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
						File.Move(@"updates/" + fileInfo.Hash, ThePath + fileInfo.Path);
					}
					ReadyToGo_ = true;
					launchGameBtn.Text = "Launch!";
					if (MessageBox.Show("All files are up to date!\r\nShall we start the game now?", "Success", MessageBoxButtons.YesNo) == DialogResult.Yes) {
						launch();
					}
				}
				catch (Exception ex) {
					MessageBox.Show("Cannot move file " + filename + "\r\n" + ex.Message, "Error", MessageBoxButtons.OK);
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

		private static void ErrorAndKill(string msg) {
			if (MessageBox.Show(msg, "Error", MessageBoxButtons.OK) == DialogResult.OK) {
				Application.Exit();
			}
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
			string args = "/C \"" + ThePath + Program.launcherData.ExeName + "\"";
			if (ThePath.Contains("steamapps")) {
				args = "/C explorer steam://run/" + Program.launcherData.SteamID;
			}
			Process.Start(new ProcessStartInfo { Arguments = args, FileName = "cmd", WindowStyle = ProcessWindowStyle.Hidden });
			launchGameBtn.Enabled = false;
			System.Threading.Thread.Sleep(1800);
			launchGameBtn.Enabled = true;
		}
	}
}
