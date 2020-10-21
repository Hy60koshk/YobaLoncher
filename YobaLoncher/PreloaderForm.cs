using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Win32;
using Newtonsoft.Json.Linq;

namespace YobaLoncher {
	public partial class PreloaderForm : Form {

		private DownloadProgressTracker downloadProgressTracker_;
		private WebClient wc_;
		private System.Security.Cryptography.MD5 md5_;
		private int progressBarLeft_ = 0;

		public PreloaderForm() {
			InitializeComponent();
			this.BackgroundImageLayout = ImageLayout.Stretch;
			string bgfile = @"images\loncherbg.png";
			if (File.Exists(bgfile)) {
				try {
					this.BackgroundImage = new Bitmap(bgfile);
				}
				catch {
					loadingLabelError.Text = "Failed to load your custom background";
				}
			}
			wc_ = new WebClient();
			md5_ = System.Security.Cryptography.MD5.Create();
		}

		public static void ErrorAndKill(string msg) {
			if (new YobaDialog(msg).ShowDialog() != DialogResult.Ignore) {
				Application.Exit();
			}
		}

		private void PreloaderForm_Load(object sender, EventArgs e) {

		}

		private List<string> getSteamLibraryPaths() {
			List<string> paths = new List<string>();
			try {
				string steamInstallPath = "";
				using (RegistryKey view64 = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64)) {
					using (RegistryKey clsid64 = view64.OpenSubKey(@"SOFTWARE\WOW6432Node\Valve\Steam", true)) {
						steamInstallPath = (string)clsid64.GetValue("InstallPath");
					}
				}
				if (steamInstallPath.Length > 0) {
					paths.Add(steamInstallPath + @"\steamapps\common\");
					string vdfPath = steamInstallPath + @"\steamapps\libraryfolders.vdf";
					if (File.Exists(vdfPath)) {
						string[] lines = File.ReadAllLines(vdfPath);
						foreach (string line in lines) {
							if (line.Length > 0 && line.StartsWith("\t\"")) {
								string val = line.Substring(3);
								if (val.Length > 0 && val.StartsWith("\"\t\t\"")) {
									val = val.Substring(4, val.Length - 5);
									if (val.Length > 2 && val.Substring(1, 2) == ":\\") {
										paths.Add(val.Replace("\\\\", "\\") + @"\steamapps\common\");
									}
								}
							}
						}
					}
				}
			}
			catch (Exception ex) {
				ErrorAndKill(ex.Message);
			}
			return paths;
		}

		private void incProgress() {
			progressBar1.Value++;
		}
		private void incProgress(int d) {
			progressBar1.Value += d;
		}
		private bool checkFileMD5(string path, string correctHash) {
			if (File.Exists(path)) {
				byte[] hash;
				using (FileStream stream = File.OpenRead(path)) {
					hash = md5_.ComputeHash(stream);
				}
				StringBuilder hashSB = new StringBuilder(hash.Length);
				for (int i = 0; i < hash.Length; i++) {
					hashSB.Append(hash[i].ToString("X2"));
				}
				string strHash = hashSB.ToString();
				if (correctHash.ToUpper().Equals(strHash)) {
					return true;
				}
			}
			return false;
		}

		private void OnDownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e) {
			downloadProgressTracker_.SetProgress(e.BytesReceived, e.TotalBytesToReceive);
			progressBar1.Value = e.ProgressPercentage;
		}

		private static bool stringHasText(string s) {
			return s != null && s.Length > 0;
		}

		private async Task loadFile(string src, string filename) {
			progressBarLeft_ = progressBar1.Value;
			loadingLabel.Text = "Updating Yoba Löncher - Downloading " + filename + " ...";
			await wc_.DownloadFileTaskAsync(src, filename);
			downloadProgressTracker_.Reset();
			progressBar1.Value = progressBarLeft_ + 5;
		}
		private async Task<bool> assertFile(FileInfo fi) {
			if (fi != null && stringHasText(fi.Path) && stringHasText(fi.Url)) {
				if (!File.Exists(fi.Path)
					|| (stringHasText(fi.Hash) && !checkFileMD5(fi.Path, fi.Hash))) {
					await loadFile(fi.Url, fi.Path);
				}
				return true;
			}
			return false;
		}
		private async Task<bool> assertFile(FileInfo fi, string dir) {
			if (fi != null && stringHasText(fi.Path) && stringHasText(fi.Url)) {
				string path = dir + "\\" + fi.Path;
				if (!File.Exists(path)
					|| (stringHasText(fi.Hash) && !checkFileMD5(path, fi.Hash))) {
					await loadFile(fi.Url, path);
				}
				return true;
			}
			return false;
		}
		private bool assertOfflineFile(FileInfo fi, string dir) {
			if (fi != null && stringHasText(fi.Path) && File.Exists(dir + "\\" + fi.Path)) {
				return true;
			}
			return false;
		}

		private void PreloaderForm_ShownAsync(object sender, EventArgs e) {
			Initialize();
		}
		private async void Initialize() {
			progressBar1.Value = 0;
			try {
				//throw new Exception("hui");
				string settingsJson = (await wc_.DownloadStringTaskAsync("hui" + Program.SETTINGS_URL));
				incProgress(5);
				Thread.Sleep(2);
				try {
					Program.launcherData = new LauncherData(settingsJson);
					try {
						File.WriteAllText("settings", settingsJson);
					}
					catch { }
					incProgress(5);
					Thread.Sleep(2);
					try {
						downloadProgressTracker_ = new DownloadProgressTracker(50, TimeSpan.FromMilliseconds(500));
						wc_.DownloadProgressChanged += new DownloadProgressChangedEventHandler(OnDownloadProgressChanged);

						if (stringHasText(Program.launcherData.LoncherHash)) {
							if (!checkFileMD5(Application.ExecutablePath, Program.launcherData.LoncherHash)) {
								if (stringHasText(Program.launcherData.LoncherExe)) {
									loadingLabel.Text = "Updating Yoba Löncher...";
									string newLoncherPath = Application.ExecutablePath + ".new";
									string appname = Application.ExecutablePath;
									appname = appname.Substring(appname.LastIndexOf('\\') + 1);
									progressBarLeft_ = progressBar1.Value;
									await wc_.DownloadFileTaskAsync(Program.launcherData.LoncherExe, newLoncherPath);
									downloadProgressTracker_.Reset();
									if (checkFileMD5(newLoncherPath, Program.launcherData.LoncherHash)) {
										Process.Start(new ProcessStartInfo {
											Arguments = String.Format("/C choice /C Y /N /D Y /T 1 & Del \"{0}\" & Rename \"{1}\" \"{2}\" & \"{0}\""
												, Application.ExecutablePath, newLoncherPath, appname)
											,
											FileName = "cmd"
											,
											WindowStyle = ProcessWindowStyle.Hidden
										});
										Application.Exit();
										return;
									}
								}
								else {
									ErrorAndKill("Launcher is out of date.\r\n\r\nAdmin eblan ne polozhil the link for autoupdate,\r\npoetomu we will just ne dadim zapustit the Launcher.");
									return;
								}
								progressBar1.Value = 50;
							}
						}
						if (Program.launcherData.Dlls != null) {
							if (!checkFileMD5("awesomium.dll", "1ACC23DE75A61143B76FAB2D437B144B")) {
								if (Program.launcherData.Dlls.ContainsKey("awesomium")) {
									await loadFile(Program.launcherData.Dlls["awesomium"], "awesomium.dll");
								}
								else {
									ErrorAndKill("awesomium.dll is missing!");
									return;
								}
							}
							if (!checkFileMD5("icudt.dll", "AA30FF1C0C294BCFCED7E5CF8F4449F0")) {
								if (Program.launcherData.Dlls.ContainsKey("icudt")) {
									await loadFile(Program.launcherData.Dlls["icudt"], "icudt.dll");
								}
								else {
									ErrorAndKill("icudt.dll is missing!");
									return;
								}
							}
							if (!checkFileMD5("Awesomium.Core.dll", "CB37BDA5C5BF2B428961C42AAE992EA8")) {
								if (Program.launcherData.Dlls.ContainsKey("awecore")) {
									await loadFile(Program.launcherData.Dlls["awecore"], "Awesomium.Core.dll");
								}
								else {
									ErrorAndKill("Awesomium.Core.dll is missing!");
									return;
								}
							}
							if (Directory.Exists("awesomium_process")) {
								Directory.Delete("awesomium_process");
							}
							if (!checkFileMD5("awesomium_process", "A30AF369EA985FF564F20E7C05C4456F")) {
								if (Program.launcherData.Dlls.ContainsKey("aweproc")) {
									await loadFile(Program.launcherData.Dlls["aweproc"], "awesomium_process");
								}
								else {
									ErrorAndKill("awesomium_process is missing!");
									return;
								}
							}
						}
						else if (!(checkFileMD5("awesomium.dll", "1ACC23DE75A61143B76FAB2D437B144B")
							&& checkFileMD5("icudt.dll", "AA30FF1C0C294BCFCED7E5CF8F4449F0")
							&& checkFileMD5("awesomium_process", "A30AF369EA985FF564F20E7C05C4456F")
							&& checkFileMD5("Awesomium.Core.dll", "CB37BDA5C5BF2B428961C42AAE992EA8"))) {
							ErrorAndKill("Important DLLs are missing!");
							return;
						}
						Thread.Sleep(2);
					}
					catch (Exception ex) {
						ErrorAndKill("Cannot update the Löncher:\r\n" + ex.Message + "\r\n" + ex.StackTrace);
						return;
					}
					try {
						LauncherData.LauncherDataRaw raw = Program.launcherData.RAW;
						if (!Directory.Exists("images")) {
							Directory.CreateDirectory("images");
						}
						string imgName = "";
						if (await assertFile(raw.Background, "images")) {
							Program.launcherData.Background = new Bitmap("images\\" + raw.Background.Path);
						}
						if (await assertFile(raw.Icon, "images")) {
							Bitmap bm = new Bitmap("images\\" + raw.Icon.Path);
							if (bm != null) {
								Program.launcherData.Icon = Icon.FromHandle(bm.GetHicon());
							}
						}
						if (await assertFile(raw.PreloaderBackground, "images")) {
							this.BackgroundImage = new Bitmap("images\\" + raw.PreloaderBackground.Path);
						}
						if (Program.launcherData.Icon == null) {
							Program.launcherData.Icon = this.Icon;
						}
						if (Program.launcherData.UI.Count > 0) {
							string[] keys = Program.launcherData.UI.Keys.ToArray();
							foreach (string key in keys) {
								if (!(await assertFile(Program.launcherData.UI[key].BgImage, "images"))) {
									Program.launcherData.UI[key].BgImage = null;
								}
							}
						}
						if (Program.launcherData.Buttons.Count > 0) {
							foreach (LinkButton lbtn in Program.launcherData.Buttons) {
								if (!(await assertFile(lbtn.BgImage, "images"))) {
									lbtn.BgImage = null;
								}
							}
						}
					}
					catch (Exception ex) {
						ErrorAndKill("Cannot get Images:\r\n" + ex.Message);
						return;
					}
					if (Program.launcherData.Fonts != null) {
						List<string> keys = Program.launcherData.Fonts.Keys.ToList();
						if (keys.Count > 0) {
							try {
								if (!Directory.Exists("fonts")) {
									Directory.CreateDirectory("fonts");
								}
								foreach (string key in keys) {
									using (Font fontTester = new Font(key, 12, FontStyle.Regular, GraphicsUnit.Pixel)) {
										if (fontTester.Name == key) {
											Program.launcherData.Fonts[key] = "win";
										}
										else if (File.Exists("fonts\\" + key)) {
											Program.launcherData.Fonts[key] = "local";
										}
										else {
											string status = "none";
											string src = Program.launcherData.Fonts[key];
											string filename = "fonts\\" + key;
											if (stringHasText(src)) {
												await loadFile(src, filename);
												if (File.Exists(filename)) {
													status = "local";
												}
											}
											Program.launcherData.Fonts[key] = status;
										}
									}
								}
							}
							catch (Exception ex) {
								ErrorAndKill("Cannot get Fonts:\r\n" + ex.Message);
								return;
							}
						}
					}
					try {
						await Program.launcherData.InitChangelog();
						incProgress(5);
						Thread.Sleep(2);
						try {
							if (!Directory.Exists("updates")) {
								Directory.CreateDirectory("updates");
							}
							string path = "";
							if (File.Exists("loncher.cfg")) {
								string[] lines = File.ReadAllLines("loncher.cfg");
								foreach (string line in lines) {
									if (line.Length > 0 && line.StartsWith("path")) {
										string[] vals = line.Split('=');
										if (vals.Length > 1) {
											path = vals[1].Trim();
										}
									}
								}
							}
							else {
								if (stringHasText(Program.launcherData.SteamGameFolder)) {
									List<string> steampaths = getSteamLibraryPaths();
									for (int i = 0; i < steampaths.Count; i++) {
										string spath = steampaths[i] + Program.launcherData.SteamGameFolder;
										if (Directory.Exists(spath)) {
											path = spath + "\\";
											break;
										}
									}
								}
								incProgress(5);
								Thread.Sleep(2);
								GamePathSelectForm gamePathSelectForm = new GamePathSelectForm();
								gamePathSelectForm.Icon = Program.launcherData.Icon;
								gamePathSelectForm.ThePath = path;
								if (gamePathSelectForm.ShowDialog(this) == DialogResult.Yes) {
									path = gamePathSelectForm.ThePath;
									gamePathSelectForm.Dispose();
									try {
										File.WriteAllLines("loncher.cfg", new string[]{
											"path = " + path
										});
									}
									catch (Exception ex) {
										MessageBox.Show("Cannot write the configuration file:\r\n" + ex.Message);
									}
								}
								else {
									Application.Exit();
									return;
								}
							}
							try {
								if (path.Length == 0) {
									path = Application.ExecutablePath;
									path = path.Substring(path.LastIndexOf('\\') + 1);
								}
								else if (path[path.Length - 1] != '\\') {
									path += "\\";
								}
								if (!File.Exists(path + Program.launcherData.ExeName)) {
									ErrorAndKill("No executable found in the specified folder!");
									return;
								}
								string ver = Program.launcherData.ExeVersion;
								if (ver != null) {
									string curVer = FileVersionInfo.GetVersionInfo(path + Program.launcherData.ExeName).FileVersion.Replace(',', '.');
									if (curVer != ver) {
										ErrorAndKill("Exe file version doesn't match the prerequisites!\r\nWe need version " + ver + " to operate.\r\nAnd yours is " + curVer);
										return;
									}
								}
								foreach (FileInfo fi in Program.launcherData.Files) {
									fi.IsOK = checkFileMD5(path + fi.Path, fi.Hash);
								}
								incProgress(10);
								progressBar1.Value = 100;
								Thread.Sleep(2);
								MainForm mainForm = new MainForm();
								mainForm.Icon = Program.launcherData.Icon;
								mainForm.ThePath = path;
								mainForm.Show(this);
								Hide();
							}
							catch (Exception ex) {
								ErrorAndKill("Cannot check files:\r\n" + ex.Message);
							}
						}
						catch (Exception ex) {
							ErrorAndKill("Cannot access or parse config:\r\n" + ex.Message);
						}
					}
					catch (Exception ex) {
						ErrorAndKill("Cannot load the icon:\r\n" + ex.Message);
					}
				}
				catch (Exception ex) {
					ErrorAndKill("Cannot parse the Settings file:\r\n" + ex.Message);
				}
			}
			catch (Exception ex) {
				UIElement[] btns;
				UIElement btnQuit = new UIElement();
				btnQuit.Caption = "Quit";
				btnQuit.Result = DialogResult.Abort;
				UIElement btnRetry = new UIElement();
				btnRetry.Caption = "Retry";
				btnRetry.Result = DialogResult.Retry;
				string msg;
				if (File.Exists("settings")) {
					msg = "Couldn't get settings for the Löncher.\r\nProbably, you're out of Ethernet.\r\n\r\nShall we try to connect again, or make an attempt to start the louncher in offline mode?";
					UIElement btnOffline = new UIElement();
					btnOffline.Caption = "Run offline";
					btnOffline.Result = DialogResult.Ignore;
					btns = new UIElement[] { btnQuit, btnRetry, btnOffline };
				}
				else {
					msg = "Couldn't get settings for the Löncher.\r\nProbably, you're out of Ethernet.";
					btns = new UIElement[] { btnQuit, btnRetry };
				}
				YobaDialog yobaDialog = new YobaDialog(msg, btns);
				yobaDialog.Icon = Program.launcherData != null ? (Program.launcherData.Icon ?? this.Icon) : this.Icon;
				DialogResult result = yobaDialog.ShowDialog(this);
				switch (result) {
					case DialogResult.Retry:
						Initialize();
						break;
					case DialogResult.Ignore:
						InitializeOffline();
						break;
					case DialogResult.Abort: {
						Application.Exit();
						return;
					}
				}
			}
		}

		private void InitializeOffline() {
			try {
				Program.OfflineMode = true;
				string settingsJson = File.ReadAllText("settings");
				Program.launcherData = new LauncherData(settingsJson);
				incProgress(10);
				try {
					LauncherData.LauncherDataRaw raw = Program.launcherData.RAW;
					if (!Directory.Exists("images")) {
						Directory.CreateDirectory("images");
					}
					string imgName = "";
					if (assertOfflineFile(raw.Background, "images")) {
						Program.launcherData.Background = new Bitmap("images\\" + raw.Background.Path);
					}
					if (assertOfflineFile(raw.Icon, "images")) {
						Bitmap bm = new Bitmap("images\\" + raw.Icon.Path);
						if (bm != null) {
							Program.launcherData.Icon = Icon.FromHandle(bm.GetHicon());
						}
					}
					if (assertOfflineFile(raw.PreloaderBackground, "images")) {
						this.BackgroundImage = new Bitmap("images\\" + raw.PreloaderBackground.Path);
					}
					if (Program.launcherData.Icon == null) {
						Program.launcherData.Icon = this.Icon;
					}
					if (Program.launcherData.UI.Count > 0) {
						string[] keys = Program.launcherData.UI.Keys.ToArray();
						foreach (string key in keys) {
							if (!(assertOfflineFile(Program.launcherData.UI[key].BgImage, "images"))) {
								Program.launcherData.UI[key].BgImage = null;
							}
						}
					}
					if (Program.launcherData.Buttons.Count > 0) {
						foreach (LinkButton lbtn in Program.launcherData.Buttons) {
							if (!(assertOfflineFile(lbtn.BgImage, "images"))) {
								lbtn.BgImage = null;
							}
						}
					}
				}
				catch (Exception ex) {
					ErrorAndKill("Cannot get Images:\r\n" + ex.Message);
					return;
				}
				if (Program.launcherData.Fonts != null) {
					List<string> keys = Program.launcherData.Fonts.Keys.ToList();
					if (keys.Count > 0) {
						try {
							if (!Directory.Exists("fonts")) {
								Directory.CreateDirectory("fonts");
							}
							foreach (string key in keys) {
								using (Font fontTester = new Font(key, 12, FontStyle.Regular, GraphicsUnit.Pixel)) {
									if (fontTester.Name == key) {
										Program.launcherData.Fonts[key] = "win";
									}
									else if (File.Exists("fonts\\" + key)) {
										Program.launcherData.Fonts[key] = "local";
									}
									else {
										Program.launcherData.Fonts[key] = "none";
									}
								}
							}
						}
						catch (Exception ex) {
							ErrorAndKill("Cannot get Fonts:\r\n" + ex.Message);
							return;
						}
					}
				}
				try {
					try {
						string path = "";
						if (File.Exists("loncher.cfg")) {
							string[] lines = File.ReadAllLines("loncher.cfg");
							foreach (string line in lines) {
								if (line.Length > 0 && line.StartsWith("path")) {
									string[] vals = line.Split('=');
									if (vals.Length > 1) {
										path = vals[1].Trim();
									}
								}
							}
						}
						else {
							if (stringHasText(Program.launcherData.SteamGameFolder)) {
								List<string> steampaths = getSteamLibraryPaths();
								for (int i = 0; i < steampaths.Count; i++) {
									string spath = steampaths[i] + Program.launcherData.SteamGameFolder;
									if (Directory.Exists(spath)) {
										path = spath + "\\";
										break;
									}
								}
							}
							incProgress(5);
							Thread.Sleep(2);
							GamePathSelectForm gamePathSelectForm = new GamePathSelectForm();
							gamePathSelectForm.Icon = Program.launcherData.Icon;
							gamePathSelectForm.ThePath = path;
							if (gamePathSelectForm.ShowDialog(this) == DialogResult.Yes) {
								path = gamePathSelectForm.ThePath;
								gamePathSelectForm.Dispose();
								try {
									File.WriteAllLines("loncher.cfg", new string[]{
											"path = " + path
										});
								}
								catch (Exception ex) {
									MessageBox.Show("Cannot write the configuration file:\r\n" + ex.Message);
								}
							}
							else {
								Application.Exit();
								return;
							}
						}
						try {
							if (path.Length == 0) {
								path = Application.ExecutablePath;
								path = path.Substring(path.LastIndexOf('\\') + 1);
							}
							else if (path[path.Length - 1] != '\\') {
								path += "\\";
							}
							if (!File.Exists(path + Program.launcherData.ExeName)) {
								ErrorAndKill("No executable found in the specified folder!");
								return;
							}
							string ver = Program.launcherData.ExeVersion;
							if (ver != null) {
								string curVer = FileVersionInfo.GetVersionInfo(path + Program.launcherData.ExeName).FileVersion.Replace(',', '.');
								if (curVer != ver) {
									ErrorAndKill("Exe file version doesn't match the prerequisites!\r\nWe need version " + ver + " to operate.\r\nAnd yours is " + curVer);
									return;
								}
							}
							foreach (FileInfo fi in Program.launcherData.Files) {
								fi.IsOK = checkFileMD5(path + fi.Path, fi.Hash);
							}
							incProgress(10);
							progressBar1.Value = 90;
							Thread.Sleep(2);
							MainForm mainForm = new MainForm();
							mainForm.Icon = Program.launcherData.Icon;
							mainForm.ThePath = path;
							mainForm.Show(this);
							progressBar1.Value = 99;
							Hide();
						}
						catch (Exception ex) {
							ErrorAndKill("Cannot check files:\r\n" + ex.Message);
						}
					}
					catch (Exception ex) {
						ErrorAndKill("Cannot access or parse config:\r\n" + ex.Message);
					}
				}
				catch (Exception ex) {
					ErrorAndKill("Cannot load the icon:\r\n" + ex.Message);
				}
			}
			catch (Exception ex) {
				ErrorAndKill("Couldn't get settings for the Löncher:\r\n" + ex.Message);
			}
		}
	}
}