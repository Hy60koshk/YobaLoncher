using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace YobaLoncher {
	static class Program {
		public readonly static string SETTINGS_URL = "https://www.dropbox.com/s/tlweb401krzcw9u/settings.json?dl=1";//"https://koshk.ru/brotherstest/settings.json";
		public static LauncherData LoncherSettings;
		public static bool OfflineMode = false;
		public static readonly string LoncherPath = Application.ExecutablePath.Substring(0, Application.ExecutablePath.LastIndexOf('\\') + 1);
		public static readonly string LoncherDataPath = LoncherPath + @"loncherData\";
		public static string GamePath = "" + LoncherPath;
		public static CheckResult GameFileCheckResult;
		public static string VersionInfo => String.Format(_about, _version, _buildNumber);
		private static string _version = "0.2.1.5";
		private static string _buildNumber = "";
		private static string _about = "YobaLöncher {0}-{1}\r\n\r\n" +
					"Build target: Battle Brothers ZoG Russifier\r\n\r\n" +
					"Build author: Hy60koshk\r\n\r\n";

		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main() {
			try {
				string[] build = Resource1.BuildDate.Split('T');
				DateTime date = DateTime.Parse(build[0]);
				_buildNumber = "" + (date.Year - 2000) + date.Month.ToString("D2") + date.Day.ToString("D2") + build[1].Substring(0, 5).Remove(2, 1);
			}
			catch {
				_buildNumber = Resource1.BuildDate.Split(',')[0];
			}
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
