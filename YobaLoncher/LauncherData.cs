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
		, FAQ = 4
	}

	public class ModCfgInfo {
		public string Name;
		public string GameVersion;
		public bool Active = false;
		public bool Altered = false;
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
					List<string> names = new List<string>();
					for (int i = InstalledMods.Count - 1; i > -1; i--) {
						ModCfgInfo mci = InstalledMods[i];
						if (names.FindIndex(x => x.Equals(mci.Name)) > -1) {
							InstalledMods.RemoveAt(i);
						}
						else {
							names.Add(mci.Name);
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
		public List<ModInfo> Mods = new List<ModInfo>();
		public List<LinkButton> Buttons;
		public Dictionary<string, UIElement> UI;
		public Dictionary<string, FileInfo> UIStyle;
		public string GameName;
		public string SteamGameFolder;
		public string ExeName;
		public string SteamID;
		public string GogID;
		public StaticTabData Changelog = new StaticTabData();
		public StaticTabData FAQ = new StaticTabData();
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
		public List<string> zzModFilesInUse = new List<string>();
		public List<string> zzModFilesToDelete = new List<string>();

		public class StaticTabData {
			public string Html = null;
			public string Site = null;
			public string Error = null;
		}

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
			public string FAQFile;
			public string QuoteToEscape;
			public string LoncherHash;
			public string LoncherExe;
			public List<LinkButton> Buttons;
			public List<RandomBgImageInfo> RandomBackgrounds;
			public Dictionary<string, UIElement> UI;
			public Dictionary<string, FileInfo> UIStyle;

			// Deprecated
			public string ChangelogEscape;
			public string ChangelogTemplate;
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
			UIStyle = raw.UIStyle ?? new Dictionary<string, FileInfo>();

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
			foreach (string file in zzModFilesToDelete) {
				if (zzModFilesInUse.FindIndex(x => x.Equals(file)) < 0) {
					File.Delete(Program.GamePath + file);
				}
			}
		}
	}
	class GameVersion {
		public string ExeVersion = null;
		public List<FileInfo> Files = new List<FileInfo>();
		public List<FileGroup> FileGroups = new List<FileGroup>();
		public string Name = null;
		public string Description = null;
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
		public ModInfo LastFileOfModToUpdate;

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
		public List<FileInfo> CurrentVersionFiles;
		public GameVersion CurrentVersionData;
		public ModCfgInfo ModConfigurationInfo;
		public bool DlInProgress = false;

		public void InitCurrentVersion(string curVer) {
			ModConfigurationInfo = LauncherConfig.InstalledMods.Find(x => x.Name.Equals(Name));
			CurrentVersionData = null;
			CurrentVersionFiles = new List<FileInfo>();
			if (YU.stringHasText(curVer)) {
				if (GameVersions.ContainsKey(curVer)) {
					CurrentVersionData = GameVersions[curVer];
				}
			}
			if (CurrentVersionData is null) {
				if (GameVersions.ContainsKey("DEFAULT")) {
					CurrentVersionData = GameVersions["DEFAULT"];
				}
				else if (GameVersions.ContainsKey("OTHER")) {
					CurrentVersionData = GameVersions["OTHER"];
				}
			}
			if (CurrentVersionData is null) {
				CurrentVersionFiles = null;
			}
			else {
				foreach (FileGroup fileGroup in CurrentVersionData.FileGroups) {
					if (fileGroup.Files != null) {
						CurrentVersionFiles.AddRange(fileGroup.Files);
					}
				}
				if (CurrentVersionData.Files != null) {
					CurrentVersionFiles.AddRange(CurrentVersionData.Files);
				}
				if (CurrentVersionFiles.Count > 0) {
					CurrentVersionFiles[CurrentVersionFiles.Count - 1].LastFileOfMod = this;
				}
				else {
					CurrentVersionFiles = null;
				}
			}
			if (ModConfigurationInfo != null) {
				if (CurrentVersionFiles is null) {
					if (ModConfigurationInfo.Active) {
						DisableOld();
					}
					else {
						ModConfigurationInfo = null;
					}
				}
				else if (ModConfigurationInfo.Active) {
					foreach (string file in ModConfigurationInfo.FileList) {
						if (CurrentVersionFiles.FindIndex(x => x.Path.Equals(file)) < 0) {
							Program.LoncherSettings.zzModFilesToDelete.Add(file);//File.Delete(Program.GamePath + file);
							ModConfigurationInfo.Altered = true;
						}
						else {
							Program.LoncherSettings.zzModFilesInUse.Add(file);
						}
					}
					if (ModConfigurationInfo.Altered) {
						ModConfigurationInfo.FileList = new List<string>();
						foreach (FileInfo fi in CurrentVersionFiles) {
							ModConfigurationInfo.FileList.Add(fi.Path);
						}
					}
				}
			}
		}
		public void Install() {
			List<string> fileList = new List<string>();
			foreach (FileInfo fi in CurrentVersionFiles) {
				fileList.Add(fi.Path);
			}
			ModConfigurationInfo = new ModCfgInfo(Name, Program.GameVersion, fileList, true);
			int oldModConfInfoIdx = LauncherConfig.InstalledMods.FindIndex(x => x.Name.Equals(Name));
			if (oldModConfInfoIdx > -1) {
				LauncherConfig.InstalledMods.RemoveAt(oldModConfInfoIdx);
			}
			LauncherConfig.InstalledMods.Add(ModConfigurationInfo);
			LauncherConfig.SaveMods();
		}
		public void Delete() {
			string prefix = ModConfigurationInfo.Active ? Program.GamePath : Program.ModsDisabledPath;
			foreach (FileInfo fi in CurrentVersionFiles) {
				File.Delete(prefix + fi.Path);
				fi.IsOK = false;
			}
			LauncherConfig.InstalledMods.Remove(ModConfigurationInfo);
			ModConfigurationInfo = null;
			LauncherConfig.SaveMods();
		}
		public void Enable() {
			foreach (FileInfo fi in CurrentVersionFiles) {
				if (File.Exists(Program.ModsDisabledPath + fi.Path)) {
					File.Move(Program.ModsDisabledPath + fi.Path, Program.GamePath + fi.Path);
				}
			}
			ModConfigurationInfo.Active = true;
			LauncherConfig.SaveMods();
		}
		public void Disable() {
			MoveToDisabled(CurrentVersionFiles);
			ModConfigurationInfo.Active = false;
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
			string curVer = ModConfigurationInfo.GameVersion;
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
			ModConfigurationInfo.Active = false;
			ModConfigurationInfo = null;
		}
	}
}
