
using System.Collections.Generic;

namespace YobaLoncher {
	class Locale {
		private static Dictionary<string, string> CustomLoc = new Dictionary<string, string>();
		public static void LoadCustomLoc(string[] lines) {
			foreach (string line in lines) {
				string[] keyval = line.Split('=');
				if (keyval.Length > 1) {
					string key = keyval[0].Trim();
					if (key.Length > 0) {
						string value = keyval[1].Trim().Replace("[n]", "\r\n");
						if (CustomLoc.ContainsKey(key)) {
							CustomLoc[key] = value;
						}
						else {
							CustomLoc.Add(key, value);
						}
					}
				}
			}
		}

		public static string Get(string key) {
			return Get(key, key);
		}

		public static string Get(string key, string def) {
			if (CustomLoc.ContainsKey(key)) {
				return CustomLoc[key];
			}
			if (DefaultLoc.ContainsKey(key)) {
				return DefaultLoc[key];
			}
			return def;
		}

		private static Dictionary<string, string> DefaultLoc = new Dictionary<string, string>() {
			{ "LaunchBtn", "Launch!" }
			, { "UpdateBtn", "Update" }
			, { "OK", "OK" }
			, { "Yes", "Yes" }
			, { "No", "No" }
			, { "Done", "Done" }
			, { "Proceed", "Proceed" }
			, { "Cancel", "Cancel" }
			, { "Close", "Close" }
			, { "Minimize", "Minimize" }
			, { "Apply", "Apply" }
			, { "Quit", "Quit" }
			, { "Retry", "Retry" }
			, { "RunOffline", "Run offline" }
			, { "Browse", "Browse..." }
			, { "ChangelogBtn", "Changelog" }
			, { "LinksBtn", "Links" }
			, { "StatusBtn", "Status" }
			, { "SettingsBtn", "Settings" }
			, { "ChangelogTooltip", "Changelog" }
			, { "StatusTooltip", "Status" }
			, { "LinksTooltip", "Links" }
			, { "SettingsTooltip", "Settings" }
			, { "Error", "Error" }
			, { "DLRate", "Downloading: {3} - {0} of {1} @ {2}" }
			, { "AllFilesIntact", "All files intact, we're ready to go." }
			, { "FilesMissing", "{0} files are missing or out of date." }
			, { "StatusCopyingFiles", "Updating files in the game directory..." }
			, { "StatusUpdatingDone", "Done!" }
			, { "StatusDownloadError", "Download failed" }
			, { "CannotDownloadFile", "Cannot download file \"{0}\"" }
			, { "CannotMoveFile", "Cannot move file \"{0}\"" }
			, { "DirectoryAccessDenied", "Cannot move file \"{0}\": Access denied. Restart the Launcher as Administrator." }
			, { "UpdateSuccessful", "All files are up to date!\r\nShall we start the game now?" }
			, { "CannotWriteCfg", "Cannot write the configuration file" }
			, { "PreloaderTitle", "YobaLöncher — Loading..." }
			, { "MainFormTitle", "YobaLöncher" }
			, { "SettingsTitle", "Settings" }
			, { "SettingsGamePath", "The game installation folder" }
			, { "GamePathSelectionTitle", "Select the game folder" }
			, { "EnterThePath", "Enter the path to the game installation folder" }
			, { "NoExeInPath", "No game executable found in the specified folder!" }
			, { "OldGameVersion", "Your current version of the game ({0}) is not supported!" }
			, { "UpdatingLoncher", "Updating Yoba Löncher..." }
			, { "UpdDownloading", "Updating Yoba Löncher - Downloading {0} ..." }
			, { "CannotGetLocaleFile", "Cannot get or apply localization files" }
			, { "CannotGetImages", "Cannot get Images" }
			, { "CannotGetFonts", "Cannot get Fonts" }
			, { "CannotCheckFiles", "Cannot check files" }
			, { "CannotParseConfig", "Cannot access or parse config" }
			, { "CannotLoadIcon", "Cannot load the icon" }
			, { "CannotParseSettings", "Cannot parse the Settings file" }
			, { "CannotUpdateLoncher", "Cannot update the Löncher" }
			, { "LoncherOutOfDate1", "Launcher is out of date.\r\n\r\nAdmin eblan ne polozhil the link for autoupdate,\r\nPoetomu we will just ne dadim zapustit the Launcher." }
			, { "LoncherOutOfDate2", "New Launcher is still out of date.\r\n\r\nAdmin eblan did not update either the link or the launcher executable download link.\r\nPoetomu we will just ne dadim zapustit the Launcher, mamke admina privet." }
			, { "WebClientError", "Couldn't get settings for the Löncher.\r\nProbably, you're out of Ethernet." }
			, { "WebClientErrorOffline", "Couldn't get settings for the Löncher.\r\nProbably, you're out of Ethernet.\r\n\r\nShall we try to connect again, or make an attempt to start the louncher in offline mode?" }
		};
	}
}
