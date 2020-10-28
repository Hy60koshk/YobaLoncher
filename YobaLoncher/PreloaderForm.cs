using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Win32;

namespace YobaLoncher {
	public partial class PreloaderForm : Form {

		private DownloadProgressTracker downloadProgressTracker_;
		private WebClient wc_;
		private MainForm oldMainForm_ = null;
		private int progressBarLeft_ = 0;
		public const string IMGPATH = @"loncherData\images\";
		public const string UPDPATH = @"loncherData\updates\";
		public const string FNTPATH = @"loncherData\fonts\";
		public const string CFGFILE = @"loncherData\loncher.cfg";

		public PreloaderForm() : this(null) { }

		public PreloaderForm(MainForm oldMainForm) {
			InitializeComponent();
			oldMainForm_ = oldMainForm;
			this.BackgroundImageLayout = ImageLayout.Stretch;
			string bgfile = IMGPATH + @"loncherbg.png";
			if (File.Exists(bgfile)) {
				try {
					this.BackgroundImage = new Bitmap(bgfile);
				}
				catch {
					loadingLabelError.Text = "Failed to load your custom background";
				}
			}
			wc_ = new WebClient();
			wc_.Encoding = Encoding.UTF8;
			Text = Locale.Get("PreloaderTitle");
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
				YU.ErrorAndKill(ex.Message);
			}
			return paths;
		}

		private void incProgress() {
			progressBar1.Value++;
		}
		private void incProgress(int d) {
			progressBar1.Value += d;
		}

		private void OnDownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e) {
			downloadProgressTracker_.SetProgress(e.BytesReceived, e.TotalBytesToReceive);
			progressBar1.Value = e.ProgressPercentage;
		}

		private async Task loadFile(string src, string filename) {
			progressBarLeft_ = progressBar1.Value;
			loadingLabel.Text = string.Format(Locale.Get("AllFilesIntact"), filename);
			await wc_.DownloadFileTaskAsync(src, filename);
			downloadProgressTracker_.Reset();
			progressBar1.Value = progressBarLeft_ + 5;
		}

		private async Task<bool> assertFile(FileInfo fi, string dir) {
			if (fi != null && YU.stringHasText(fi.Path) && YU.stringHasText(fi.Url)) {
				if (!FileChecker.CheckFileMD5(dir, fi)) {
					await loadFile(fi.Url, dir + fi.Path);
				}
				return true;
			}
			return false;
		}
		private bool assertOfflineFile(FileInfo fi, string dir) {
			if (fi != null && YU.stringHasText(fi.Path) && File.Exists(dir + fi.Path)) {
				return true;
			}
			return false;
		}

		private string showPathSelection(string path) {
			if (path.Length == 0) {
				path = Program.GamePath;
			}
			GamePathSelectForm gamePathSelectForm = new GamePathSelectForm();
			gamePathSelectForm.Icon = Program.LoncherSettings.Icon;
			gamePathSelectForm.ThePath = path;
			if (gamePathSelectForm.ShowDialog(this) == DialogResult.Yes) {
				path = gamePathSelectForm.ThePath;
				gamePathSelectForm.Dispose();
				try {
					File.WriteAllLines(CFGFILE, new string[] {
						"path = " + path
					});
				}
				catch (Exception ex) {
					YobaDialog.ShowDialog(Locale.Get("CannotWriteCfg") + ":\r\n" + ex.Message);
				}
				if (path.Length == 0) {
					path = Program.GamePath;
				}
				else {
					if (path[path.Length - 1] != '\\') {
						path += "\\";
					}
					Program.GamePath = path;
				}
				return path;
			}
			else {
				Application.Exit();
				return null;
			}
		}

		private void PreloaderForm_ShownAsync(object sender, EventArgs e) {
			Initialize();
		}
		private async void Initialize() {
			progressBar1.Value = 0;
			try {
				if (!Directory.Exists("loncherData")) {
					Directory.CreateDirectory("loncherData");
				}
				if (!Directory.Exists(IMGPATH)) {
					Directory.CreateDirectory(IMGPATH);
				}
				if (!Directory.Exists(UPDPATH)) {
					Directory.CreateDirectory(UPDPATH);
				}
				//WebBrowserHelper.FixBrowserVersion();
				//ErrorAndKill("Cannot get Images:\r\n");
				string settingsJson = (await wc_.DownloadStringTaskAsync(Program.SETTINGS_URL));
				incProgress(5);
				try {
					Program.LoncherSettings = new LauncherData(settingsJson);
					try {
						File.WriteAllText("loncherData\\settings", settingsJson);
					}
					catch { }
					incProgress(5);
					try {
						string locFile = "loncherData\\loc";
						if (Program.LoncherSettings.RAW.Localization != null) {
							FileInfo locInfo = Program.LoncherSettings.RAW.Localization;
							if (YU.stringHasText(locInfo.Url)) {
								locInfo.Path = locFile;
								if (!FileChecker.CheckFileMD5("", locInfo)) {
									string loc = await wc_.DownloadStringTaskAsync(locInfo.Url);
									File.WriteAllText(locFile, loc, Encoding.UTF8);
									Locale.LoadCustomLoc(loc.Replace("\r\n", "\n").Split('\n'));
								}
								Locale.LoadCustomLoc(File.ReadAllLines(locFile, Encoding.UTF8));
							}
							else if (File.Exists(locFile)) {
								Locale.LoadCustomLoc(File.ReadAllLines(locFile, Encoding.UTF8));
							}
						}
						else if (File.Exists(locFile)) {
							Locale.LoadCustomLoc(File.ReadAllLines(locFile, Encoding.UTF8));
						}
						incProgress(5);
					}
					catch (Exception ex) {
						YobaDialog.ShowDialog(Locale.Get("CannotGetLocaleFile") + ":\r\n" + ex.Message);
					}
					try {
						downloadProgressTracker_ = new DownloadProgressTracker(50, TimeSpan.FromMilliseconds(500));
						wc_.DownloadProgressChanged += new DownloadProgressChangedEventHandler(OnDownloadProgressChanged);
						//
						if (false && !FileChecker.CheckFileMD5(Application.ExecutablePath, Program.LoncherSettings.LoncherHash)) {
							if (YU.stringHasText(Program.LoncherSettings.LoncherExe)) {
								loadingLabel.Text = Locale.Get("UpdatingLoncher");
								string newLoncherPath = Application.ExecutablePath + ".new";
								string appname = Application.ExecutablePath;
								appname = appname.Substring(appname.LastIndexOf('\\') + 1);
								progressBarLeft_ = progressBar1.Value;
								await wc_.DownloadFileTaskAsync(Program.LoncherSettings.LoncherExe, newLoncherPath);
								downloadProgressTracker_.Reset();
								if (FileChecker.CheckFileMD5(newLoncherPath, Program.LoncherSettings.LoncherHash)) {
									Process.Start(new ProcessStartInfo {
										Arguments = String.Format("/C choice /C Y /N /D Y /T 1 & Del \"{0}\" & Rename \"{1}\" \"{2}\" & \"{0}\""
											, Application.ExecutablePath, newLoncherPath, appname)
										, FileName = "cmd"
										, WindowStyle = ProcessWindowStyle.Hidden
									});
									Application.Exit();
									return;
								}
								else {
									YU.ErrorAndKill(Locale.Get("LoncherOutOfDate2"));
									return;
								}
							}
							else {
								YU.ErrorAndKill(Locale.Get("LoncherOutOfDate1"));
								return;
							}
						}
					}
					catch (Exception ex) {
						YU.ErrorAndKill(Locale.Get("CannotUpdateLoncher") + ":\r\n" + ex.Message + "\r\n" + ex.StackTrace);
						return;
					}
					try {
						LauncherData.LauncherDataRaw raw = Program.LoncherSettings.RAW;
						if (await assertFile(raw.Background, IMGPATH)) {
							Program.LoncherSettings.Background = new Bitmap(IMGPATH + raw.Background.Path);
						}
						if (await assertFile(raw.Icon, IMGPATH)) {
							Bitmap bm = new Bitmap(IMGPATH + raw.Icon.Path);
							if (bm != null) {
								Program.LoncherSettings.Icon = Icon.FromHandle(bm.GetHicon());
							}
						}
						if (await assertFile(raw.PreloaderBackground, IMGPATH)) {
							this.BackgroundImage = new Bitmap(IMGPATH + raw.PreloaderBackground.Path);
						}
						if (Program.LoncherSettings.Icon == null) {
							Program.LoncherSettings.Icon = this.Icon;
						}
						if (Program.LoncherSettings.UI.Count > 0) {
							string[] keys = Program.LoncherSettings.UI.Keys.ToArray();
							foreach (string key in keys) {
								if (!(await assertFile(Program.LoncherSettings.UI[key].BgImage, IMGPATH))) {
									Program.LoncherSettings.UI[key].BgImage = null;
								}
							}
						}
						if (Program.LoncherSettings.Buttons.Count > 0) {
							foreach (LinkButton lbtn in Program.LoncherSettings.Buttons) {
								if (!(await assertFile(lbtn.BgImage, IMGPATH))) {
									lbtn.BgImage = null;
								}
							}
						}
					}
					catch (Exception ex) {
						YU.ErrorAndKill(Locale.Get("CannotGetImages") + ":\r\n" + ex.Message);
						return;
					}
					if (Program.LoncherSettings.Fonts != null) {
						List<string> keys = Program.LoncherSettings.Fonts.Keys.ToList();
						if (keys.Count > 0) {
							try {
								if (!Directory.Exists(FNTPATH)) {
									Directory.CreateDirectory(FNTPATH);
								}
								foreach (string key in keys) {
									using (Font fontTester = new Font(key, 12, FontStyle.Regular, GraphicsUnit.Pixel)) {
										if (fontTester.Name == key) {
											Program.LoncherSettings.Fonts[key] = "win";
										}
										else if (File.Exists(FNTPATH + key)) {
											Program.LoncherSettings.Fonts[key] = "local";
										}
										else {
											string status = "none";
											string src = Program.LoncherSettings.Fonts[key];
											string filename = FNTPATH + key;
											if (YU.stringHasText(src)) {
												await loadFile(src, filename);
												if (File.Exists(filename)) {
													status = "local";
												}
											}
											Program.LoncherSettings.Fonts[key] = status;
										}
									}
								}
							}
							catch (Exception ex) {
								YU.ErrorAndKill(Locale.Get("CannotGetFonts") + ":\r\n" + ex.Message);
								return;
							}
						}
					}
					try {
						await Program.LoncherSettings.InitChangelog();
						incProgress(5);
						try {
							string path = "";
							if (File.Exists(CFGFILE)) {
								string[] lines = File.ReadAllLines(CFGFILE);
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
								if (YU.stringHasText(Program.LoncherSettings.SteamGameFolder)) {
									List<string> steampaths = getSteamLibraryPaths();
									for (int i = 0; i < steampaths.Count; i++) {
										string spath = steampaths[i] + Program.LoncherSettings.SteamGameFolder;
										if (Directory.Exists(spath)) {
											path = spath + "\\";
											break;
										}
									}
								}
								incProgress(5);
								path = showPathSelection(path);
							}
							if (path is null || path.Length == 0) {
								path = Program.GamePath;
							}
							else {
								if (path[path.Length - 1] != '\\') {
									path += "\\";
								}
								Program.GamePath = path;
							}
							try {
								while (!File.Exists(path + Program.LoncherSettings.ExeName)) {
									YobaDialog.ShowDialog(Locale.Get("NoExeInPath"));
									path = showPathSelection(path);
									if (path is null) {
										return;
									}
								}
								if (Program.LoncherSettings.GameVersions != null && Program.LoncherSettings.GameVersions.Count > 0) {
									string curVer = FileVersionInfo.GetVersionInfo(path + Program.LoncherSettings.ExeName).FileVersion.Replace(',', '.');
									bool gotVer = false;
									foreach (GameVersion gv in Program.LoncherSettings.GameVersions) {
										if (!YU.stringHasText(gv.ExeVersion) || gv.ExeVersion.Equals(curVer)) {
											gotVer = true;
											if (gv.Files != null && gv.Files.Count > 0) {
												Program.LoncherSettings.Files.AddRange(gv.Files);
											}
											break;
										}
									}
									if (!gotVer) {
										YU.ErrorAndKill(string.Format(Locale.Get("AllFilesIntact"), curVer));
										return;
									}
								}
								incProgress(10);
								if (oldMainForm_ != null) {
									oldMainForm_.Dispose();
								}
								Program.GameFileCheckResult = FileChecker.CheckFiles(Program.LoncherSettings.Files);
								progressBar1.Value = 90;
								MainForm mainForm = new MainForm();
								mainForm.Icon = Program.LoncherSettings.Icon;
								progressBar1.Value = 99;
								mainForm.Show(this);
								Hide();
							}
							catch (Exception ex) {
								YU.ErrorAndKill(Locale.Get("CannotCheckFiles") + ":\r\n" + ex.Message);
							}
						}
						catch (Exception ex) {
							YU.ErrorAndKill(Locale.Get("CannotParseConfig") + ":\r\n" + ex.Message);
						}
					}
					catch (Exception ex) {
						YU.ErrorAndKill(Locale.Get("CannotLoadIcon") + ":\r\n" + ex.Message);
					}
				}
				catch (Exception ex) {
					YU.ErrorAndKill(Locale.Get("CannotParseSettings") + ":\r\n" + ex.Message);
				}
			}
			catch (Exception ex) {
				UIElement[] btns;
				UIElement btnQuit = new UIElement();
				btnQuit.Caption = Locale.Get("Quit");
				btnQuit.Result = DialogResult.Abort;
				UIElement btnRetry = new UIElement();
				btnRetry.Caption = Locale.Get("Retry");
				btnRetry.Result = DialogResult.Retry;
				string msg;
				if (File.Exists("settings")) {
					msg = Locale.Get("WebClientErrorOffline");
					UIElement btnOffline = new UIElement();
					btnOffline.Caption = Locale.Get("RunOffline");
					btnOffline.Result = DialogResult.Ignore;
					btns = new UIElement[] { btnQuit, btnRetry, btnOffline };
				}
				else {
					msg = Locale.Get("WebClientError");
					btns = new UIElement[] { btnQuit, btnRetry };
				}
				YobaDialog yobaDialog = new YobaDialog(msg, btns);
				yobaDialog.Icon = Program.LoncherSettings != null ? (Program.LoncherSettings.Icon ?? this.Icon) : this.Icon;
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
				Program.LoncherSettings = new LauncherData(settingsJson);
				incProgress(10);
				try {
					LauncherData.LauncherDataRaw raw = Program.LoncherSettings.RAW;
					try {
						string locFile = "loncherData\\loc";
						if (File.Exists(locFile)) {
							Locale.LoadCustomLoc(File.ReadAllLines(locFile, Encoding.UTF8));
						}
					}
					catch (Exception ex) {
						YobaDialog.ShowDialog(Locale.Get("CannotGetLocaleFile") + ":\r\n" + ex.Message);
					}
					if (assertOfflineFile(raw.Background, IMGPATH)) {
						Program.LoncherSettings.Background = new Bitmap(IMGPATH + raw.Background.Path);
					}
					if (assertOfflineFile(raw.Icon, IMGPATH)) {
						Bitmap bm = new Bitmap(IMGPATH + raw.Icon.Path);
						if (bm != null) {
							Program.LoncherSettings.Icon = Icon.FromHandle(bm.GetHicon());
						}
					}
					if (assertOfflineFile(raw.PreloaderBackground, IMGPATH)) {
						this.BackgroundImage = new Bitmap(IMGPATH + raw.PreloaderBackground.Path);
					}
					if (Program.LoncherSettings.Icon == null) {
						Program.LoncherSettings.Icon = this.Icon;
					}
					if (Program.LoncherSettings.UI.Count > 0) {
						string[] keys = Program.LoncherSettings.UI.Keys.ToArray();
						foreach (string key in keys) {
							if (!(assertOfflineFile(Program.LoncherSettings.UI[key].BgImage, IMGPATH))) {
								Program.LoncherSettings.UI[key].BgImage = null;
							}
						}
					}
					if (Program.LoncherSettings.Buttons.Count > 0) {
						foreach (LinkButton lbtn in Program.LoncherSettings.Buttons) {
							if (!(assertOfflineFile(lbtn.BgImage, IMGPATH))) {
								lbtn.BgImage = null;
							}
						}
					}
				}
				catch (Exception ex) {
					YU.ErrorAndKill(Locale.Get("CannotGetImages") + ":\r\n" + ex.Message);
					return;
				}
				if (Program.LoncherSettings.Fonts != null) {
					List<string> keys = Program.LoncherSettings.Fonts.Keys.ToList();
					if (keys.Count > 0) {
						try {
							if (!Directory.Exists(FNTPATH)) {
								Directory.CreateDirectory(FNTPATH);
							}
							foreach (string key in keys) {
								using (Font fontTester = new Font(key, 12, FontStyle.Regular, GraphicsUnit.Pixel)) {
									if (fontTester.Name == key) {
										Program.LoncherSettings.Fonts[key] = "win";
									}
									else if (File.Exists(FNTPATH + key)) {
										Program.LoncherSettings.Fonts[key] = "local";
									}
									else {
										Program.LoncherSettings.Fonts[key] = "none";
									}
								}
							}
						}
						catch (Exception ex) {
							YU.ErrorAndKill(Locale.Get("CannotGetFonts") + ":\r\n" + ex.Message);
							return;
						}
					}
				}
				try {
					try {
						string path = "";
						if (File.Exists(CFGFILE)) {
							string[] lines = File.ReadAllLines(CFGFILE);
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
							if (YU.stringHasText(Program.LoncherSettings.SteamGameFolder)) {
								List<string> steampaths = getSteamLibraryPaths();
								for (int i = 0; i < steampaths.Count; i++) {
									string spath = steampaths[i] + Program.LoncherSettings.SteamGameFolder;
									if (Directory.Exists(spath)) {
										path = spath + "\\";
										break;
									}
								}
							}
							incProgress(5);
							path = showPathSelection(path);
						}
						try {
							if (path is null || path.Length == 0) {
								path = Program.GamePath;
							}
							else {
								if (path[path.Length - 1] != '\\') {
									path += "\\";
								}
								Program.GamePath = path;
							}
							if (!File.Exists(path + Program.LoncherSettings.ExeName)) {
								YobaDialog.ShowDialog(Locale.Get("NoExeInPath"));
								path = showPathSelection(path);
								if (path is null) {
									return;
								}
							}
							if (Program.LoncherSettings.GameVersions != null && Program.LoncherSettings.GameVersions.Count > 0) {
								string curVer = FileVersionInfo.GetVersionInfo(path + Program.LoncherSettings.ExeName).FileVersion.Replace(',', '.');
								bool gotVer = false;
								foreach (GameVersion gv in Program.LoncherSettings.GameVersions) {
									if (!YU.stringHasText(gv.ExeVersion) || gv.ExeVersion.Equals(curVer)) {
										gotVer = true;
										if (gv.Files != null && gv.Files.Count > 0) {
											Program.LoncherSettings.Files.AddRange(gv.Files);
										}
										break;
									}
								}
								if (!gotVer) {
									YU.ErrorAndKill(string.Format(Locale.Get("OldGameVersion"), curVer));
									return;
								}
							}
							incProgress(10);
							Program.GameFileCheckResult = FileChecker.CheckFiles(Program.LoncherSettings.Files);
							progressBar1.Value = 90;
							MainForm mainForm = new MainForm();
							mainForm.Icon = Program.LoncherSettings.Icon;
							mainForm.ThePath = path;
							progressBar1.Value = 99;
							mainForm.Show(this);
							Hide();
						}
						catch (Exception ex) {
							YU.ErrorAndKill(Locale.Get("CannotCheckFiles") + ":\r\n" + ex.Message);
						}
					}
					catch (Exception ex) {
						YU.ErrorAndKill(Locale.Get("CannotParseConfig") + ":\r\n" + ex.Message);
					}
				}
				catch (Exception ex) {
					YU.ErrorAndKill(Locale.Get("CannotLoadIcon") + ":\r\n" + ex.Message);
				}
			}
			catch (Exception ex) {
				YU.ErrorAndKill(Locale.Get("CannotParseSettings") + ":\r\n" + ex.Message);
			}
		}
	}
}