using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace YobaLoncher {
	public enum StartPageEnum {
		Changelog
			, Status
			, Links
			, Mods
	}

	class LauncherData {
#pragma warning disable 649

		public List<GameVersion> GameVersions;
		public List<FileInfo> Files;
		public List<LinkButton> Buttons;
		public Dictionary<string, UIElement> UI;
		public string GameName;
		public string SteamGameFolder;
		public string ExeName;
		public string SteamID;
		public string GogID;
		public string ChangelogHtml = "";
		public string ChangelogSite = "";
		public string LoncherHash;
		public string LoncherExe;
		public StartPageEnum StartPage;
		public Dictionary<string, string> Dlls;
		public Dictionary<string, string> Fonts;
		public string AwesomiumDll;
		public Image Background;
		public Image PreloaderBackground;
		public Icon Icon;
		private WebClient wc_;
		private LauncherDataRaw raw_;
		public LauncherDataRaw RAW => raw_;

		public class LauncherDataRaw {
			public string GameName;
			public string SteamGameFolder;
			public List<GameVersion> GameVersions;
			public List<FileInfo> Files;
			public FileInfo Background;
			public FileInfo PreloaderBackground;
			public FileInfo Localization;
			public FileInfo Icon;
			public string ExeName;
			public StartPageEnum StartPage = StartPageEnum.Status;
			public string SteamID;
			public string GogID;
			public string Changelog;
			public string ChangelogEscape;
			public string ChangelogTemplate;
			public string LoncherHash;
			public string LoncherExe;
			public Dictionary<string, string> Dlls;
			public string AwesomiumDll;
			public List<LinkButton> Buttons;
			public Dictionary<string, UIElement> UI;
		}

		public LauncherData(string json) {

			LauncherDataRaw raw = JsonConvert.DeserializeObject<LauncherDataRaw>(json);
			raw_ = raw;
			wc_ = new WebClient();
			wc_.Encoding = System.Text.Encoding.UTF8;

			Files = raw.Files ?? new List<FileInfo>();
			Buttons = raw.Buttons ?? new List<LinkButton>();
			foreach (LinkButton btn in Buttons) {
				if (btn.Position == null)
					btn.Position = new Vector();
				if (btn.Size == null)
					btn.Size = new Vector();
			}
			GameVersions = raw.GameVersions;

			StartPage = raw.StartPage;
			UI = raw.UI ?? new Dictionary<string, UIElement>();

			ExeName = raw.ExeName;
			SteamID = raw.SteamID;
			GogID = raw.GogID;
			LoncherHash = raw.LoncherHash;
			LoncherExe = raw.LoncherExe;
			Dlls = raw.Dlls;
			AwesomiumDll = raw.AwesomiumDll;

			GameName = raw.GameName;
			SteamGameFolder = raw.SteamGameFolder;
		}

		public async Task InitChangelog() {
			try {
				if (raw_.ChangelogTemplate != null) {
					ChangelogHtml = (await wc_.DownloadStringTaskAsync(new Uri(raw_.ChangelogTemplate)));
					if (raw_.Changelog != null && raw_.Changelog.Length > 0) {
						string cl = (await wc_.DownloadStringTaskAsync(new Uri(raw_.Changelog)));
						if (raw_.ChangelogEscape != null && raw_.ChangelogEscape.Length > 0) {
							string quote = raw_.ChangelogEscape;
							cl = cl.Replace("\\", "\\\\").Replace(quote, "\\" + quote);
							if (cl.Contains("\r")) {
								cl = cl.Replace("\r\n", "\\\r\n");
							}
							else {
								cl = cl.Replace("\n", "\\\n");
							}
						}
						ChangelogHtml = ChangelogHtml.Replace("[[[CHANGELOG]]]", cl);
					}
				}
				else {
					ChangelogSite = raw_.Changelog;
				}
			}
			catch (Exception ex) {
				YU.ErrorAndKill("Cannot get ChangeLog files:\r\n" + ex.Message);
			}
		}
	}
	class GameVersion {
		public string ExeVersion = null;
		public List<FileInfo> Files;
	}
	class UIElement {
		public string Color;
		public string BgColor;
		public string BgColorDown;
		public string BgColorHover;
		public string BorderColor = "gray";
		public int BorderSize = -1;
		public BgImageInfo BgImage;
		public string Caption;
		public string Font;
		public string FontSize;
		public Vector Position;
		public Vector Size;
		public bool CustomStyle;
		public System.Windows.Forms.DialogResult Result;
	}
	class LinkButton : UIElement {
		public string Url;
	}
	class FileInfo {
		public string Url;
		public string Path;
		public string Description;
		public List<string> Hashes;
		public bool IsOK = false;
		public string UploadAlias;
		public uint Size;

		public bool IsComplete {
			get {
				return Url != null && Path != null && Hashes != null
					&& Url.Length > 0 && Path.Length > 0 && Hashes.Count > 0;
			}
		}
	}
	class BgImageInfo : FileInfo {
		public string Layout;
	}
	class Vector {
		public int X = 0;
		public int Y = 0;
	}
}
