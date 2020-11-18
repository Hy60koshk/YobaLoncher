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
		private string _GOGpath = null;
		public const string IMGPATH = @"loncherData\images\";
		public const string UPDPATH = @"loncherData\updates\";
		public const string FNTPATH = @"loncherData\fonts\";
		public const string SETTINGSPATH = @"loncherData\settings";
		public const string	LOCPATH = @"loncherData\loc";

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
					loadingLabelError.Text = Locale.Get("CannotSetBackground");
				}
			}
			wc_ = new WebClient();
			wc_.Encoding = Encoding.UTF8;
			Text = Locale.Get("PreloaderTitle");
			loadingLabel.Text = string.Format(Locale.Get("LoncherLoading"), Program.LoncherName);
			labelAbout.Text = Locale.Get("PressF1About");
		}

		private string getSteamGameInstallPath() {
			string steamId = Program.LoncherSettings.SteamID;
			if (steamId is null) {
				return null;
			}
			string[] locations = new string[] {
				@"SOFTWARE\WOW6432Node\Microsoft\Windows\CurrentVersion\Uninstall\Steam App " + steamId
				, @"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall\Steam App " + steamId
			};
			return YU.GetRegistryInstallPath(locations, true);
		}
		private string getGogGameInstallPath() {
			string gogId = Program.LoncherSettings.GogID;
			if (gogId is null) {
				return null;
			}
			string[] locations = new string[] {
				@"SOFTWARE\WOW6432Node\Microsoft\Windows\CurrentVersion\Uninstall\" + gogId + "_is1"
				, @"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall\" + gogId + "_is1"
			};
			return YU.GetRegistryInstallPath(locations, true);
		}		

		private List<string> getSteamLibraryPaths() {
			List<string> paths = new List<string>();
			try {
				string steamInstallPath = "";
				using (RegistryKey view64 = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64)) {
					using (RegistryKey clsid64 = view64.OpenSubKey(@"SOFTWARE\WOW6432Node\Valve\Steam")) {
						if (clsid64 != null) {
							steamInstallPath = (string)clsid64.GetValue("InstallPath");
						}
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
				YobaDialog.ShowDialog(ex.Message);
			}
			return paths;
		}

		private void incProgress() {
			_progressBar1.Value++;
		}
		private void incProgress(int d) {
			_progressBar1.Value += d;
		}

		private void OnDownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e) {
			downloadProgressTracker_.SetProgress(e.BytesReceived, e.TotalBytesToReceive);
			_progressBar1.Value = e.ProgressPercentage;
		}

		private async Task loadFile(string src, string filename) {
			progressBarLeft_ = _progressBar1.Value;
			loadingLabel.Text = string.Format(Locale.Get("UpdDownloading"), filename);
			await wc_.DownloadFileTaskAsync(src, filename);
			downloadProgressTracker_.Reset();
			progressBarLeft_ += 3;
			if (progressBarLeft_ > 87) {
				progressBarLeft_ = 50;
			}
			_progressBar1.Value = progressBarLeft_;
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
			if (!YU.stringHasText(path)) {
				path = Program.GamePath;
			}
			GamePathSelectForm gamePathSelectForm = new GamePathSelectForm();
			gamePathSelectForm.Icon = Program.LoncherSettings.Icon;
			gamePathSelectForm.ThePath = path;
			if (gamePathSelectForm.ShowDialog(this) == DialogResult.Yes) {
				path = gamePathSelectForm.ThePath;
				gamePathSelectForm.Dispose();
				if (path != null && path.Equals(_GOGpath) && LauncherConfig.GalaxyDir != null) {
					if (YobaDialog.ShowDialog(Locale.Get("GogGalaxyDetected"), YobaDialog.YesNoBtns) == DialogResult.Yes) {
						LauncherConfig.LaunchFromGalaxy = true;
						LauncherConfig.Save();
					}
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
				LauncherConfig.GameDir = path;
				LauncherConfig.Save();
				return path;
			}
			else {
				Application.Exit();
				return null;
			}
		}

		private bool findGamePath() {
			string path = LauncherConfig.GameDir;
			if (path is null) {
				path = getSteamGameInstallPath();
				if (path is null && YU.stringHasText(Program.LoncherSettings.SteamGameFolder)) {
					List<string> steampaths = getSteamLibraryPaths();
					for (int i = 0; i < steampaths.Count; i++) {
						string spath = steampaths[i] + Program.LoncherSettings.SteamGameFolder;
						if (Directory.Exists(spath)) {
							path = spath + "\\";
							break;
						}
					}
				}
				if (path is null) {
					path = getGogGameInstallPath();
					if (path != null) {
						_GOGpath = "" + path;
					}
				}
				path = showPathSelection(path);
			}
			if (path is null || path.Length == 0) {
				path = Program.GamePath;
			}
			if (path[path.Length - 1] != '\\') {
				path += "\\";
			}
			while (!File.Exists(path + Program.LoncherSettings.ExeName)) {
				YobaDialog.ShowDialog(Locale.Get("NoExeInPath"));
				path = showPathSelection(path);
				if (path is null) {
					return false;
				}
			}
			Program.GamePath = path;
			YU.Log("GamePath: " + path);
			return true;
		}

		private void updateGameVersion() {
			string curVer = FileVersionInfo.GetVersionInfo(Program.GamePath + Program.LoncherSettings.ExeName).FileVersion.Replace(',', '.');
			Program.LoncherSettings.LoadFileListForVersion(curVer);
		}

		private void showMainForm() {
			_progressBar1.Value = 90;
			MainForm mainForm = new MainForm();
			mainForm.Icon = Program.LoncherSettings.Icon;
			_progressBar1.Value = 99;
			mainForm.Show(this);
			Hide();
		}

		private void PreloaderForm_ShownAsync(object sender, EventArgs e) {
			LauncherConfig.Load();
			if (LauncherConfig.StartOffline) {
				InitializeOffline();
			}
			else {
				Initialize();
			}
		}
		private async void Initialize() {
			_progressBar1.Value = 0;
			Program.OfflineMode = false;
			long startingTicks = DateTime.Now.Ticks;
			long lastTicks = startingTicks;
			void logDeltaTicks(string point) {
				long current = DateTime.Now.Ticks;
				YU.Log(point + ": " + (current - lastTicks) + " (" + (current - startingTicks) + ')');
				lastTicks = current;
			}
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
				logDeltaTicks("settings");
				incProgress(5);
				try {
					Program.LoncherSettings = new LauncherData(settingsJson);
					try {
						File.WriteAllText(SETTINGSPATH, settingsJson, Encoding.UTF8);
					}
					catch { }
					incProgress(5);
					try {
						if (Program.LoncherSettings.RAW.Localization != null) {
							FileInfo locInfo = Program.LoncherSettings.RAW.Localization;
							if (YU.stringHasText(locInfo.Url)) {
								locInfo.Path = LOCPATH;
								if (!FileChecker.CheckFileMD5("", locInfo)) {
									string loc = await wc_.DownloadStringTaskAsync(locInfo.Url);
									File.WriteAllText(LOCPATH, loc, Encoding.UTF8);
									Locale.LoadCustomLoc(loc.Replace("\r\n", "\n").Split('\n'));
								}
								Locale.LoadCustomLoc(File.ReadAllLines(LOCPATH, Encoding.UTF8));
							}
							else if (File.Exists(LOCPATH)) {
								Locale.LoadCustomLoc(File.ReadAllLines(LOCPATH, Encoding.UTF8));
							}
						}
						else if (File.Exists(LOCPATH)) {
							Locale.LoadCustomLoc(File.ReadAllLines(LOCPATH, Encoding.UTF8));
						}
						incProgress(5);
						logDeltaTicks("locales");
					}
					catch (Exception ex) {
						YobaDialog.ShowDialog(Locale.Get("CannotGetLocaleFile") + ":\r\n" + ex.Message);
					}
					try {
						downloadProgressTracker_ = new DownloadProgressTracker(50, TimeSpan.FromMilliseconds(500));
						wc_.DownloadProgressChanged += new DownloadProgressChangedEventHandler(OnDownloadProgressChanged);
#if DEBUG
#else
						if (!FileChecker.CheckFileMD5(Application.ExecutablePath, Program.LoncherSettings.LoncherHash)) {
							if (YU.stringHasText(Program.LoncherSettings.LoncherExe)) {
								loadingLabel.Text = Locale.Get("UpdatingLoncher");
								string newLoncherPath = Application.ExecutablePath + ".new";
								string appname = Application.ExecutablePath;
								appname = appname.Substring(appname.LastIndexOf('\\') + 1);
								progressBarLeft_ = _progressBar1.Value;
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
#endif
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
						logDeltaTicks("images");
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
								logDeltaTicks("fonts");
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
						logDeltaTicks("changelog");
						try {
							if (findGamePath()) {
								try {
									updateGameVersion();
									if (oldMainForm_ != null) {
										oldMainForm_.Dispose();
									}
									int progressBarPerFile = 94 - _progressBar1.Value;
									if (progressBarPerFile < Program.LoncherSettings.Files.Count) {
										progressBarPerFile = 88;
										_progressBar1.Value = 6;
									}
									progressBarPerFile = progressBarPerFile / Program.LoncherSettings.Files.Count;
									if (progressBarPerFile < 1) {
										progressBarPerFile = 1;
									}
									Program.GameFileCheckResult = await FileChecker.CheckFiles(
										Program.LoncherSettings.Files
										, new EventHandler<FileCheckedEventArgs>((object o, FileCheckedEventArgs a) => {
											_progressBar1.Value += progressBarPerFile;
											if (_progressBar1.Value > 96) {
												_progressBar1.Value = 40;
											}
										})
									);
									logDeltaTicks("filecheck");
									showMainForm();
								}
								catch (Exception ex) {
									YU.ErrorAndKill(Locale.Get("CannotCheckFiles") + ":\r\n" + ex.Message);
								}
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
				if (File.Exists(SETTINGSPATH)) {
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
				string settingsJson = File.ReadAllText(SETTINGSPATH);
				Program.LoncherSettings = new LauncherData(settingsJson);
				incProgress(10);
				try {
					LauncherData.LauncherDataRaw raw = Program.LoncherSettings.RAW;
					try {
						if (File.Exists(LOCPATH)) {
							Locale.LoadCustomLoc(File.ReadAllLines(LOCPATH, Encoding.UTF8));
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
						if (findGamePath()) {
							try {
								updateGameVersion();
								incProgress(10);
								Program.GameFileCheckResult = FileChecker.CheckFilesOffline(Program.LoncherSettings.Files);
								showMainForm();
							}
							catch (Exception ex) {
								YU.ErrorAndKill(Locale.Get("CannotCheckFiles") + ":\r\n" + ex.Message);
							}
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

		private void PreloaderForm_KeyUp(object sender, KeyEventArgs e) {
			if (e.KeyCode == Keys.F1) {
				YU.ShowHelpDialog();
			}
		}
	}
}