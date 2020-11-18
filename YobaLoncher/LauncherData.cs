using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace YobaLoncher {
	public enum StartPageEnum {
		Changelog = 0
		, Status = 1
		, Links = 2
		, Mods = 3
	}

	static class LauncherConfig {
		public static string GameDir = null;
		public static string GalaxyDir = null;
		public static bool LaunchFromGalaxy = false;
		public static bool StartOffline = false;
		public static StartPageEnum StartPage = StartPageEnum.Status;
		private const string CFGFILE = @"loncherData\loncher.cfg";

		public static void Save() {
			try {
				File.WriteAllLines(CFGFILE, new string[] {
					"path = " + GameDir
					, "startpage = " + (int)StartPage
					, "startviagalaxy = " + (LaunchFromGalaxy ? 1 : 0)
					, "offlinemode = " + (StartOffline ? 1 : 0)
				});
			}
			catch (Exception ex) {
				YobaDialog.ShowDialog(Locale.Get("CannotWriteCfg") + ":\r\n" + ex.Message);
			}
		}

		public static void Load() {
			GalaxyDir = YU.GetGogGalaxyPath();
			try {
				if (File.Exists(CFGFILE)) {
					string[] lines = File.ReadAllLines(CFGFILE);
					foreach (string line in lines) {
						if (line.Length > 0) {
							if (line.StartsWith("path")) {
								string[] vals = line.Split('=');
								if (vals.Length > 1) {
									GameDir = vals[1].Trim();
								}
							}
							else if (line.StartsWith("startpage")) {
								string[] vals = line.Split('=');
								if (vals.Length > 1) {
									if (int.TryParse(vals[1].Trim(), out int intval) && intval > -1 && intval < 4) {
										StartPage = (StartPageEnum)intval;
									}
								}
							}
							else if (line.StartsWith("startviagalaxy")) {
								if (GalaxyDir != null) {
									string[] vals = line.Split('=');
									if (vals.Length > 1) {
										string val = vals[1].Trim();
										LaunchFromGalaxy = !"0".Equals(val) && val.Length != 5;
									}
								}
							}
							else if (line.StartsWith("offlinemode")) {
								if (GalaxyDir != null) {
									string[] vals = line.Split('=');
									if (vals.Length > 1) {
										string val = vals[1].Trim();
										StartOffline = !"0".Equals(val) && val.Length != 5;
									}
								}
							}
						}
					}
				}
			}
			catch (Exception ex) {
				YobaDialog.ShowDialog(Locale.Get("CannotReadCfg") + ":\r\n" + ex.Message);
			}
		}
	}

	class LauncherData {
#pragma warning disable 649

		public Dictionary<string, GameVersion> GameVersions = new Dictionary<string, GameVersion>();
		public GameVersion GameVersion = null;
		public List<FileInfo> Files = new List<FileInfo>();
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

			//Files = raw.Files ?? new List<FileInfo>();
			Buttons = raw.Buttons ?? new List<LinkButton>();
			foreach (LinkButton btn in Buttons) {
				if (btn.Position == null)
					btn.Position = new Vector();
				if (btn.Size == null)
					btn.Size = new Vector();
			}
			//GameVersions = raw.GameVersions;

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

			foreach (GameVersion gv in raw.GameVersions) {
				string key = YU.stringHasText(gv.ExeVersion) ? gv.ExeVersion : "DEFAULT";
				if (GameVersions.ContainsKey(key)) {
					throw new Exception(string.Format(Locale.Get("MultipleFileBlocksForSingleGameVersion"), key));
				}
				GameVersions.Add(key, gv);
			}
			List<string> gvkeys = GameVersions.Keys.ToList();
			string[] anyKeys = new string[] { "ANY", "=", "DEFAULT" };
			gvkeys.RemoveAll(s => anyKeys.Contains(s));

			if (gvkeys.Count == 0) {
				GameVersions.Add("DEFAULT", new GameVersion());
				GameVersion singleGV = GameVersions["DEFAULT"];
				foreach (string anyKey in anyKeys) {
					if (GameVersions.ContainsKey(anyKey)) {
						GameVersion versionToMerge = GameVersions[anyKey];
						singleGV.FileGroups.AddRange(versionToMerge.FileGroups);
						singleGV.Files.AddRange(versionToMerge.Files);
						GameVersions.Remove(anyKey);
					}
				}
			} else {
				foreach (string anyKey in anyKeys) {
					if (GameVersions.ContainsKey(anyKey)) {
						GameVersion versionToMerge = GameVersions[anyKey];
						foreach (string gvkey in gvkeys) {
							GameVersion targetVersion = GameVersions[gvkey];
							foreach (FileGroup fg in versionToMerge.FileGroups) {
								FileGroup targetGroup = targetVersion.FileGroups.Find(x => x.Name == fg.Name);
								if (targetGroup is null) {
									targetGroup = FileGroup.CopyOf(fg);
									targetVersion.FileGroups.Add(targetGroup);
								}
								else {
									targetGroup.Files.AddRange(fg.Files);
								}
							}
							targetVersion.Files.AddRange(versionToMerge.Files);
						}
						GameVersions.Remove(anyKey);
					}
				}
			}
			foreach (string gvkey in gvkeys) {
				GameVersion gv = GameVersions[gvkey];
				gv.FileGroups.Sort();
			}
		}

		public void LoadFileListForVersion(string curVer) {
			if (YU.stringHasText(curVer)) {
				if (GameVersions.ContainsKey(curVer)) {
					GameVersion = GameVersions[curVer];
				}
			}
			if (GameVersions.ContainsKey("DEFAULT")) {
				GameVersion = GameVersions["DEFAULT"];
			}
			if (GameVersions.ContainsKey("OTHER")) {
				GameVersion = GameVersions["OTHER"];
			}
			if (GameVersion is null) {
				throw new Exception(string.Format(Locale.Get("OldGameVersion"), curVer));
			}
			foreach (FileGroup fileGroup in GameVersion.FileGroups) {
				if (fileGroup.Files != null) {
					Files.AddRange(fileGroup.Files);
				}
			}
			if (GameVersion.Files != null) {
				Files.AddRange(GameVersion.Files);
			}
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
		public List<FileInfo> Files = new List<FileInfo>();
		public List<FileGroup> FileGroups = new List<FileGroup>();
	}
	class FileGroup : IComparable<FileGroup> {
		public string Name = null;
		public List<FileInfo> Files = new List<FileInfo>();
		public int OrderIndex = 1;

		public int CompareTo(FileGroup fg) {
			return OrderIndex.CompareTo(fg.OrderIndex);
		}

		public static FileGroup CopyOf(FileGroup fg) {
			FileGroup targetGroup = new FileGroup() {
				Name = fg.Name,
				OrderIndex = fg.OrderIndex
			};
			targetGroup.Files.AddRange(fg.Files);
			return targetGroup;
		}
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
		public int Importance = 0;
		public bool CheckedToDl = true;

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
