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

	public class ModCfgInfo {
		public string Name;
		public string GameVersion;
		public bool Active = false;
		public List<string> FileList;

		public ModCfgInfo(string name, string gameVersion, List<string> fileList, bool active) {
			Name = name;
			GameVersion = gameVersion;
			Active = active;
			FileList = fileList;
		}
	}
	static class LauncherConfig {
		public static string GameDir = null;
		public static string GalaxyDir = null;
		public static bool LaunchFromGalaxy = false;
		public static bool StartOffline = false;
		public static bool CloseOnLaunch = false;
		public static string LastSurveyId = null;
		public static StartPageEnum StartPage = StartPageEnum.Status;
		private const string CFGFILE = @"loncherData\loncher.cfg";
		private const string MODINFOFILE = @"loncherData\installedMods.json";
		public static List<ModCfgInfo> InstalledMods = new List<ModCfgInfo>();

		public static void Save() {
			try {
				File.WriteAllLines(CFGFILE, new string[] {
					"path = " + GameDir
					, "startpage = " + (int)StartPage
					, "startviagalaxy = " + (LaunchFromGalaxy ? 1 : 0)
					, "offlinemode = " + (StartOffline ? 1 : 0)
					, "closeonlaunch = " + (CloseOnLaunch ? 1 : 0)
					, "lastsrvchk = " + LastSurveyId
				});
			}
			catch (Exception ex) {
				YobaDialog.ShowDialog(Locale.Get("CannotWriteCfg") + ":\r\n" + ex.Message);
			}
		}

		public static void SaveMods() {
			try {
				File.WriteAllText(MODINFOFILE, JsonConvert.SerializeObject(InstalledMods));
			}
			catch (Exception ex) {
				YobaDialog.ShowDialog(Locale.Get("CannotWriteCfg") + ":\r\n" + ex.Message);
			}
		}

		private static bool ParseBooleanParam(string line) {
			string[] vals = line.Split('=');
			if (vals.Length > 1) {
				string val = vals[1].Trim();
				return !"0".Equals(val) && val.Length != 5;
			}
			return false;
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
							else if (line.StartsWith("lastsrvchk")) {
								string[] vals = line.Split('=');
								if (vals.Length > 1) {
									LastSurveyId = vals[1].Trim();
								}
							}
							else if (line.StartsWith("startviagalaxy")) {
								if (GalaxyDir != null) {
									LaunchFromGalaxy = ParseBooleanParam(line);
								}
							}
							else if (line.StartsWith("offlinemode")) {
								StartOffline = ParseBooleanParam(line);
							}
							else if (line.StartsWith("closeonlaunch")) {
								CloseOnLaunch = ParseBooleanParam(line);
							}
						}
					}
				}
				if (File.Exists(MODINFOFILE)) {
					InstalledMods = JsonConvert.DeserializeObject<List<ModCfgInfo>>(File.ReadAllText(MODINFOFILE));
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
		public List<ModInfo> Mods = new List<ModInfo>();
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
		public SurveyInfo Survey;
		public Dictionary<string, string> Fonts;
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
			public List<RawModInfo> Mods;
			public BgImageInfo Background;
			public FileInfo PreloaderBackground;
			public FileInfo Localization;
			public FileInfo Icon;
			public SurveyInfo Survey;
			public string ExeName;
			public StartPageEnum StartPage = StartPageEnum.Status;
			public string SteamID;
			public string GogID;
			public string Changelog;
			public string ChangelogEscape;
			public string ChangelogTemplate;
			public string LoncherHash;
			public string LoncherExe;
			public List<LinkButton> Buttons;
			public List<RandomBgImageInfo> RandomBackgrounds;
			public Dictionary<string, UIElement> UI;
		}

		public LauncherData(string json) {

			LauncherDataRaw raw = JsonConvert.DeserializeObject<LauncherDataRaw>(json);
			raw_ = raw;
			wc_ = new WebClient();
			wc_.Encoding = System.Text.Encoding.UTF8;

			Buttons = raw.Buttons ?? new List<LinkButton>();
			foreach (LinkButton btn in Buttons) {
				if (btn.Position == null)
					btn.Position = new Vector();
				if (btn.Size == null)
					btn.Size = new Vector();
			}

			StartPage = raw.StartPage;
			UI = raw.UI ?? new Dictionary<string, UIElement>();

			ExeName = raw.ExeName;
			SteamID = raw.SteamID;
			GogID = raw.GogID;
			LoncherHash = raw.LoncherHash;
			LoncherExe = raw.LoncherExe;
			Survey = raw.Survey;

			GameName = raw.GameName;
			SteamGameFolder = raw.SteamGameFolder;

			GameVersions = PrepareGameVersions(raw.GameVersions);
			if (raw.Mods != null && raw.Mods.Count > 0) {
				foreach (RawModInfo rmi in raw.Mods) {
					if (YU.stringHasText(rmi.Name) && rmi.GameVersions != null) {
						Mods.Add(new ModInfo(rmi.Name, rmi.Description, PrepareGameVersions(rmi.GameVersions)));
					}
				}
			}
		}

		public Dictionary<string, GameVersion> PrepareGameVersions(List<GameVersion> rawGameVersions) {
			Dictionary<string, GameVersion> resultingGameVersions = new Dictionary<string, GameVersion>();
			foreach (GameVersion gv in rawGameVersions) {
				string key = YU.stringHasText(gv.ExeVersion) ? gv.ExeVersion : "DEFAULT";
				if (resultingGameVersions.ContainsKey(key)) {
					throw new Exception(string.Format(Locale.Get("MultipleFileBlocksForSingleGameVersion"), key));
				}
				resultingGameVersions.Add(key, gv);
			}
			List<string> gvkeys = resultingGameVersions.Keys.ToList();
			string[] anyKeys = new string[] { "ANY", "=", "DEFAULT" };
			gvkeys.RemoveAll(s => anyKeys.Contains(s));

			if (gvkeys.Count == 0) {
				if (!resultingGameVersions.ContainsKey("DEFAULT")) {
					resultingGameVersions.Add("DEFAULT", new GameVersion());
				}
				GameVersion singleGV = resultingGameVersions["DEFAULT"];
				foreach (string anyKey in anyKeys) {
					if (resultingGameVersions.ContainsKey(anyKey)) {
						GameVersion versionToMerge = resultingGameVersions[anyKey];
						singleGV.FileGroups.AddRange(versionToMerge.FileGroups);
						singleGV.Files.AddRange(versionToMerge.Files);
						resultingGameVersions.Remove(anyKey);
					}
				}
			}
			else {
				foreach (string anyKey in anyKeys) {
					if (resultingGameVersions.ContainsKey(anyKey)) {
						GameVersion versionToMerge = resultingGameVersions[anyKey];
						foreach (string gvkey in gvkeys) {
							GameVersion targetVersion = resultingGameVersions[gvkey];
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
						resultingGameVersions.Remove(anyKey);
					}
				}
			}
			foreach (string gvkey in gvkeys) {
				GameVersion gv = resultingGameVersions[gvkey];
				gv.FileGroups.Sort();
			}
			return resultingGameVersions;
		}

		public void LoadFileListForVersion(string curVer) {
			if (YU.stringHasText(curVer)) {
				if (GameVersions.ContainsKey(curVer)) {
					GameVersion = GameVersions[curVer];
				}
			}
			if (GameVersion is null) {
				if (GameVersions.ContainsKey("DEFAULT")) {
					GameVersion = GameVersions["DEFAULT"];
				}
				else if (GameVersions.ContainsKey("OTHER")) {
					GameVersion = GameVersions["OTHER"];
				}
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
			foreach (ModInfo mi in Mods) {
				mi.InitCurrentVersion(curVer);
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
		public BgImageInfo BgImageClick;
		public BgImageInfo BgImageHover;
		public string Caption;
		public string Font;
		public string FontSize;
		public Vector Position;
		public Vector Size;
		public bool CustomStyle = false;
		public System.Windows.Forms.DialogResult Result;
	}
	class LinkButton : UIElement {
		public string Url;
	}
	class SurveyInfo {
		public string Text;
		public string ID;
		public string Url;
	}
	class FileInfo {
		public string Url;
		public string Path;
		public string Description;
		public string Tooltip;
		public List<string> Hashes;
		public bool IsOK = false;
		public bool IsPresent = false;
		public string UploadAlias;
		public uint Size = 0;
		public int Importance = 0;
		public bool IsCheckedToDl = false;
		public ModInfo LastFileOfMod;

		public bool IsComplete {
			get {
				return Url != null && Path != null && Hashes != null
					&& Url.Length > 0 && Path.Length > 0 && Hashes.Count > 0;
			}
		}
	}
	class BgImageInfo : FileInfo {
		public string Layout;
		public System.Windows.Forms.ImageLayout ImageLayout = System.Windows.Forms.ImageLayout.Stretch;
	}
	class RandomBgImageInfo {
		public BgImageInfo Background;
		public int Chance = 100;
	}
	class Vector {
		public int X = 0;
		public int Y = 0;
	}
	class RawModInfo {
		public string Name;
		public string Description = "";
		public List<GameVersion> GameVersions;
	}
	class ModInfo {
		public string Name;
		public string Description;
		public Dictionary<string, GameVersion> GameVersions;
		public ModInfo(string name, string descr, Dictionary<string, GameVersion> gv) {
			Description = descr;
			Name = name;
			GameVersions = gv;
		}
		public List<FileInfo> CurrentVersion;
		public ModCfgInfo CfgInfo;
		public bool DlInProgress = false;

		public void InitCurrentVersion(string curVer) {
			CfgInfo = LauncherConfig.InstalledMods.Find(x => x.Name.Equals(Name));
			GameVersion gv = null;
			CurrentVersion = new List<FileInfo>();
			if (YU.stringHasText(curVer)) {
				if (GameVersions.ContainsKey(curVer)) {
					gv = GameVersions[curVer];
				}
			}
			if (gv is null) {
				if (GameVersions.ContainsKey("DEFAULT")) {
					gv = GameVersions["DEFAULT"];
				}
				else if (GameVersions.ContainsKey("OTHER")) {
					gv = GameVersions["OTHER"];
				}
			}
			if (gv is null) {
				CurrentVersion = null;
			}
			else {
				foreach (FileGroup fileGroup in gv.FileGroups) {
					if (fileGroup.Files != null) {
						CurrentVersion.AddRange(fileGroup.Files);
					}
				}
				if (gv.Files != null) {
					CurrentVersion.AddRange(gv.Files);
				}
				if (CurrentVersion.Count > 0) {
					CurrentVersion[CurrentVersion.Count - 1].LastFileOfMod = this;
				}
				else {
					CurrentVersion = null;
				}
			}
			if (CfgInfo != null) {
				if (CurrentVersion is null) {
					if (CfgInfo.Active) {
						DisableOld();
					}
					else {
						CfgInfo = null;
					}
				}
				else if (CfgInfo.Active) {
					bool altered = false;
					foreach (string file in CfgInfo.FileList) {
						if (CurrentVersion.FindIndex(x => x.Path == file) < 0) {
							File.Delete(Program.GamePath + file);
							altered = true;
						}
					}
					if (altered) {
						CfgInfo.FileList = new List<string>();
						foreach (FileInfo fi in CurrentVersion) {
							CfgInfo.FileList.Add(fi.Path);
						}
					}
				}
			}
		}
		public void Install() {
			List<string> fileList = new List<string>();
			foreach (FileInfo fi in CurrentVersion) {
				fileList.Add(fi.Path);
			}
			CfgInfo = new ModCfgInfo(Name, Program.GameVersion, fileList, true);
			LauncherConfig.InstalledMods.Add(CfgInfo);
			LauncherConfig.SaveMods();
		}
		public void Delete() {
			string prefix = CfgInfo.Active ? Program.GamePath : Program.ModsDisabledPath;
			foreach (FileInfo fi in CurrentVersion) {
				File.Delete(prefix + fi.Path);
				fi.IsOK = false;
			}
			LauncherConfig.InstalledMods.Remove(CfgInfo);
			CfgInfo = null;
			LauncherConfig.SaveMods();
		}
		public void Enable() {
			foreach (FileInfo fi in CurrentVersion) {
				if (File.Exists(Program.ModsDisabledPath + fi.Path)) {
					File.Move(Program.ModsDisabledPath + fi.Path, Program.GamePath + fi.Path);
				}
			}
			CfgInfo.Active = true;
			LauncherConfig.SaveMods();
		}
		public void Disable() {
			MoveToDisabled(CurrentVersion);
			CfgInfo.Active = false;
			LauncherConfig.SaveMods();
		}
		private void MoveToDisabled(List<FileInfo> version) {
			Directory.CreateDirectory(Program.ModsDisabledPath);
			List<string> disdirs = new List<string>();
			foreach (FileInfo fi in version) {
				if (File.Exists(Program.GamePath + fi.Path)) {
					string path = fi.Path.Replace('/', '\\');
					bool hasSubdir = path.Contains('\\');
					path = Program.ModsDisabledPath + path;
					if (hasSubdir) {
						string disdir = path.Substring(0, path.LastIndexOf('\\'));
						if (!disdirs.Contains(disdir)) {
							Directory.CreateDirectory(disdir);
							disdirs.Add(disdir);
						}
					}
					File.Move(Program.GamePath + fi.Path, path);
				}
			}
		}
		public void DisableOld() {
			List<FileInfo> oldVersionFiles = new List<FileInfo>();
			GameVersion gv = null;
			string curVer = CfgInfo.GameVersion;
			if (GameVersions.ContainsKey(curVer)) {
				gv = GameVersions[curVer];
			}
			if (gv is null) {
				if (GameVersions.ContainsKey("DEFAULT")) {
					gv = GameVersions["DEFAULT"];
				}
				else if (GameVersions.ContainsKey("OTHER")) {
					gv = GameVersions["OTHER"];
				}
			}
			if (gv is null) {
				oldVersionFiles = null;
			}
			else {
				foreach (FileGroup fileGroup in gv.FileGroups) {
					if (fileGroup.Files != null) {
						oldVersionFiles.AddRange(fileGroup.Files);
					}
				}
				if (gv.Files != null) {
					oldVersionFiles.AddRange(gv.Files);
				}
			}
			MoveToDisabled(oldVersionFiles);
			CfgInfo.Active = false;
			CfgInfo = null;
		}
	}
}
