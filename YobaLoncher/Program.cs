using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

namespace YobaLoncher {
	static class Program {
		public readonly static string SETTINGS_URL = "https://koshk.ru/battlebrothers/settings.json";
		//public readonly static string SETTINGS_URL = "https://www.dropbox.com/s/tlweb401krzcw9u/settings.json?dl=1";
		public static LauncherData LoncherSettings;
		public static bool OfflineMode = false;
		public static string PreviousVersionHash = null;
		public static bool FirstRun = false;
		public static readonly string LoncherPath = Application.ExecutablePath.Substring(0, Application.ExecutablePath.LastIndexOf('\\') + 1);
		public static readonly string LoncherDataPath = LoncherPath + @"loncherData\";
		public static string GamePath = "" + LoncherPath;
		public static CheckResult GameFileCheckResult;

		public static string Disclaimer => _disclaimer;
		public static string LoncherName => _loncherName;
		public static string VersionInfo => String.Format(_about, _version, _buildNumber, _buildVersion);

		private static string _loncherName = "YobaLoncher";
		private static string _version = "0.2.7.2";
		private static string _buildVersion = "0.2";
		private static string _buildNumber = "";
		private static string _about = "YobaLöncher {0}-{1}";
		private static string _disclaimer = "";

		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main(string[] args) {
			try {
				if (Resource1.BuildTargetOpts.Length > 0) {
					string[] build = Resource1.BuildTargetOpts.Replace("\r", "").Split('\n');
					if (build.Length > 0) {
						ParseBuildData();
					}
					void ParseBuildData() {
						StringBuilder sb = new StringBuilder();
						int i = 0;
						int li = 0;
						int stage = 0;
						while (build.Length >= i) {
							if (build.Length == i || build[i] == "---========---") {
								switch (stage) {
									case 0:
										_loncherName = sb.ToString();
										break;
									case 1:
										_buildVersion = sb.ToString();
										break;
									case 2:
										_about = sb.ToString();
										break;
									case 3:
										_disclaimer = sb.ToString();
										return;
								}
								sb.Clear();
								li = 0;
								stage++;
							}
							else {
								if (li > 0) {
									sb.Append("\r\n");
								}
								sb.Append(build[i]);
								li++;
							}
							i++;
						}
					};
				}
			}
			catch (Exception ex) {
				MessageBox.Show("Invalid Build info:\r\n\r\n" + ex.StackTrace);
			}
			try {
				string[] buildDate = Resource1.BuildDate.Split('T');
				DateTime date = DateTime.Parse(buildDate[0]);
				_buildNumber = "" + (date.Year - 2000) + date.Month.ToString("D2") + date.Day.ToString("D2") + buildDate[1].Substring(0, 5).Replace(' ', '0').Remove(2, 1);
			}
			catch {
				_buildNumber = Resource1.BuildDate.Split(',')[0];
			}
			for (int aii = 0; aii < args.Length; aii++) {
				string arg = args[aii];
				if (arg == "-oldhash") {
					aii++;
					if (args.Length > aii && args[aii].Length == 32) {
						PreviousVersionHash = args[aii];
						FirstRun = true;
					}
					else {
						MessageBox.Show("Usage error: a string MD5 hash must follow the -oldhash key");
					}
				}
			}
			Locale.LoadCustomLoc(Resource1.locale_default.Replace("\r", "").Split('\n'));
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);
			Application.Run(new PreloaderForm());
		}
	}

	static class NativeWinAPI {
		internal static readonly int GWL_EXSTYLE = -20;
		internal static readonly int WS_EX_COMPOSITED = 0x02000000;

		[DllImport("user32")]
		internal static extern int GetWindowLong(IntPtr hWnd, int nIndex);

		[DllImport("user32")]
		internal static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);
	}
}
