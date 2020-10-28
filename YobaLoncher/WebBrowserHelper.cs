using System;

namespace YobaLoncher {
	public class WebBrowserHelper {
		public static int GetEmbVersion() {
			int ieVer = GetBrowserVersion();

			if (ieVer > 9)
				return ieVer * 1000 + 1;

			if (ieVer > 7)
				return ieVer * 1111;

			return 7000;
		}

		public static void FixBrowserVersion() {
			string appName = System.IO.Path.GetFileNameWithoutExtension(System.Reflection.Assembly.GetExecutingAssembly().Location);
			FixBrowserVersion(appName);
		}

		public static void FixBrowserVersion(string appName) {
			FixBrowserVersion(appName, GetEmbVersion());
		}

		// FixBrowserVersion("<YourAppName>", 9000);
		public static void FixBrowserVersion(string appName, int ieVer) {
			FixBrowserVersion_Internal("HKEY_LOCAL_MACHINE", appName + ".exe", ieVer);
			FixBrowserVersion_Internal("HKEY_CURRENT_USER", appName + ".exe", ieVer);
			FixBrowserVersion_Internal("HKEY_LOCAL_MACHINE", appName + ".vshost.exe", ieVer);
			FixBrowserVersion_Internal("HKEY_CURRENT_USER", appName + ".vshost.exe", ieVer);
		}

		private static void FixBrowserVersion_Internal(string root, string appName, int ieVer) {
			try {
				//For 64 bit Machine 
				if (Environment.Is64BitOperatingSystem)
					Microsoft.Win32.Registry.SetValue(root + @"\Software\Wow6432Node\Microsoft\Internet Explorer\Main\FeatureControl\FEATURE_BROWSER_EMULATION", appName, ieVer);
				else  //For 32 bit Machine 
					Microsoft.Win32.Registry.SetValue(root + @"\Software\Microsoft\Internet Explorer\Main\FeatureControl\FEATURE_BROWSER_EMULATION", appName, ieVer);


			}
			catch (Exception) {
				// some config will hit access rights exceptions
				// this is why we try with both LOCAL_MACHINE and CURRENT_USER
			}
		}

		public static int GetBrowserVersion() {
			string strKeyPath = @"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Internet Explorer";
			string[] ls = new string[] { "svcVersion", "svcUpdateVersion", "Version", "W2kVersion" };

			int maxVer = 0;
			for (int i = 0; i < ls.Length; ++i) {
				object objVal = Microsoft.Win32.Registry.GetValue(strKeyPath, ls[i], "0");
				string strVal = System.Convert.ToString(objVal);
				if (strVal != null) {
					int iPos = strVal.IndexOf('.');
					if (iPos > 0)
						strVal = strVal.Substring(0, iPos);

					int res = 0;
					if (int.TryParse(strVal, out res))
						maxVer = Math.Max(maxVer, res);
				}
			}
			return maxVer;
		}
	}
}