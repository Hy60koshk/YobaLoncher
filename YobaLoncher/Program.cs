using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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


		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main() {
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);
			Application.Run(new PreloaderForm());
		}
	}
}
