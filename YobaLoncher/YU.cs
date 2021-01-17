using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Text;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Win32;

namespace YobaLoncher {
	static class YU {

		public static bool stringHasText(string s) {
			return s != null && s.Length > 0;
		}

		public static void Log(string text) {
#if DEBUG
			File.AppendAllText(Program.LoncherDataPath + "log.txt", text + "\r\n");
#endif
		}

		public static Bitmap readBitmap(string path) {
			FileStream ffs = File.OpenRead(path);
			Bitmap bmp = new Bitmap(ffs);
			ffs.Close();
			return bmp;
		}

		public static void setFont(Control comp, string fontName, string fontSize) {
			float fs = 12;
			if (stringHasText(fontSize)) {
				if (!float.TryParse(fontSize, out fs)) {
					fs = 12;
				}
			}
			Font font;
			if (stringHasText(fontName)) {
				font = new Font(fontName, fs, FontStyle.Regular, GraphicsUnit.Pixel);
				if (font.Name != fontName) {
					if (Program.LoncherSettings.Fonts[fontName] == "local") {
						PrivateFontCollection pfc = new PrivateFontCollection();
						pfc.AddFontFile(PreloaderForm.FNTPATH + fontName);
						font = new Font(pfc.Families[0], fs, FontStyle.Regular, GraphicsUnit.Pixel);
					}
					else {
						font = new Font(comp.Font.Name, fs, FontStyle.Regular, GraphicsUnit.Pixel);
					}
				}
			}
			else {
				font = new Font(comp.Font.Name, fs, FontStyle.Regular, GraphicsUnit.Pixel);
			}
			comp.Font = font;
		}

		public static void assertLucida(Control component) {
			if (component.Font.Name != "Lucida Sans Unicode") {
				try {
					PrivateFontCollection pfc = new PrivateFontCollection();
					byte[] fontbytes = Resource1.lucida_sans_unicode;
					IntPtr fontMemPointer = Marshal.AllocCoTaskMem(fontbytes.Length);
					Marshal.Copy(fontbytes, 0, fontMemPointer, fontbytes.Length);
					pfc.AddMemoryFont(fontMemPointer, fontbytes.Length);
					Marshal.FreeCoTaskMem(fontMemPointer);
					component.Font = new Font(pfc.Families[0], 12F, FontStyle.Regular, GraphicsUnit.Pixel);
				}
				catch {
					component.Font = new Font("Consolas", 12F, FontStyle.Regular, GraphicsUnit.Pixel, 204);
				}
			}
		}

		public static Color colorFromString(string str, Color def) {
			if (!stringHasText(str)) {
				return def;
			}
			try {
				if (str[0] == '#') {
					return Color.FromArgb(Int32.Parse(str.Substring(1), System.Globalization.NumberStyles.HexNumber));
				}
				else {
					return Color.FromName(str);
				}
			}
			catch {
				return def;
			}
		}

		public static string GetRegistryInstallPath(string[] paths, bool lethal) {
			try {
				using (RegistryKey view64 = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64)) {
					foreach (string location in paths) {
						using (RegistryKey clsid64 = view64.OpenSubKey(location)) {
							if (clsid64 != null) {
								string installLoc = (string)clsid64.GetValue("InstallLocation");
								if (installLoc != null && installLoc.Length > 1) {
									return installLoc;
								}
							}
						}
					}
				}
			}
			catch (Exception ex) {
				if (lethal) {
					ErrorAndKill(ex.Message);
				}
			}
			return null;
		}
		public static string GetGogGalaxyPath() {
			try {
				using (RegistryKey view64 = RegistryKey.OpenBaseKey(RegistryHive.ClassesRoot, RegistryView.Registry64)) {
					using (RegistryKey clsid64 = view64.OpenSubKey(@"goggalaxy\shell\open\command")) {
						if (clsid64 != null) {
							string installLoc = (string)clsid64.GetValue("");
							if (installLoc != null && installLoc.Length > 1) {
								installLoc = installLoc.Substring(1, installLoc.IndexOf('"', 2) - 1);
								if (installLoc.Length > 0) {
									YU.Log("GalaxyInstalloc: " + installLoc);
									return installLoc;
								}
							}
						}
					}
				}
			}
			catch { }
			return null;
		}

		public static void ErrorAndKill(string msg) {
			if (YobaDialog.ShowDialog(msg) != DialogResult.Ignore) {
				Application.Exit();
			}
		}

		internal static void ShowHelpDialog() {
			if (Program.Disclaimer.Length > 0) {
				YobaDialog helpDialog = new YobaDialog(Program.VersionInfo, new Size(400, 400));
				RichTextBox rtb = new RichTextBox();
				
				rtb.BorderStyle = BorderStyle.None;
				rtb.Font = new Font("Verdana", 12F, FontStyle.Regular, GraphicsUnit.Pixel);
				//rtb.BackColor = Color.DimGray;
				//rtb.ForeColor = Color.White;
				rtb.ForeColor = Color.Black;
				rtb.BackColor = Color.LightGray;
				rtb.Location = new Point(16, 120);
				rtb.ReadOnly = true;
				rtb.Size = new Size(368, 224);
				rtb.TabIndex = 2;
				rtb.Text = Program.Disclaimer;
				rtb.LinkClicked += (sa, ea) => {
					Process.Start(ea.LinkText);
				};
				helpDialog.MessageLabel.Size = new Size(helpDialog.Size.Width - 56, 92);
				helpDialog.Controls.Add(rtb);
				helpDialog.Shown += (o, e) => {
					helpDialog.ResetWinStyle();
				};
				helpDialog.ShowDialog();
			}
			else {
				YobaDialog.ShowDialog(Program.VersionInfo);
			}
		}
	}
}
