using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Text;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace YobaLoncher {
	static class YU {

		public static bool stringHasText(string s) {
			return s != null && s.Length > 0;
		}

		public static void setFont(Control comp, string fontName, string fontSize) {
			int fs = 12;
			if (stringHasText(fontSize)) {
				if (!int.TryParse(fontSize, out fs)) {
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

		public static void ErrorAndKill(string msg) {
			if (YobaDialog.ShowDialog(msg) != DialogResult.Ignore) {
				Application.Exit();
			}
		}
	}
}
