using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Drawing;

namespace YobaLoncher {
	// Credits: https://gist.github.com/darkfall

	class PngIconConverter {
		/* input image with width = height is suggested to get the best result */

		public static bool Convert(Stream input_stream, Stream output_stream, int size = -1, bool keep_aspect_ratio = false) {
			Bitmap input_bit = (Bitmap)Bitmap.FromStream(input_stream);
			return Convert(input_bit, output_stream, size, keep_aspect_ratio);
		}
		public static bool Convert(Bitmap input_bit, Stream output_stream, int size = -1, bool keep_aspect_ratio = false) {
			if (input_bit != null) {
				int width, height;
				Bitmap new_bit;
				if (size > -1) {
					if (keep_aspect_ratio) {
						width = size;
						height = input_bit.Height / input_bit.Width * size;
					}
					else {
						width = height = size;
					}
					new_bit = new Bitmap(input_bit, new Size(width, height));
				}
				else {
					new_bit = new Bitmap(input_bit);
					width = new_bit.Width;
					height = new_bit.Height;
				}
				if (new_bit != null) {
					// save the resized png into a memory stream for future use
					MemoryStream mem_data = new MemoryStream();
					new_bit.Save(mem_data, System.Drawing.Imaging.ImageFormat.Png);

					BinaryWriter icon_writer = new BinaryWriter(output_stream);
					if (output_stream != null && icon_writer != null) {
						// 0-1 reserved, 0
						icon_writer.Write((byte)0);
						icon_writer.Write((byte)0);

						// 2-3 image type, 1 = icon, 2 = cursor
						icon_writer.Write((short)1);

						// 4-5 number of images
						icon_writer.Write((short)1);

						// image entry 1
						// 0 image width
						icon_writer.Write((byte)width);
						// 1 image height
						icon_writer.Write((byte)height);

						// 2 number of colors
						icon_writer.Write((byte)0);

						// 3 reserved
						icon_writer.Write((byte)0);

						// 4-5 color planes
						icon_writer.Write((short)0);

						// 6-7 bits per pixel
						icon_writer.Write((short)32);

						// 8-11 size of image data
						icon_writer.Write((int)mem_data.Length);

						// 12-15 offset of image data
						icon_writer.Write((int)(6 + 16));

						// write image data
						// png data must contain the whole png data file
						icon_writer.Write(mem_data.ToArray());

						icon_writer.Flush();

						return true;
					}
				}
				return false;
			}
			return false;
		}

		public static bool Convert(string input_image, string output_icon, int size = -1, bool keep_aspect_ratio = false) {
			FileStream input_stream = new FileStream(input_image, FileMode.Open);
			FileStream output_stream = new FileStream(output_icon, FileMode.OpenOrCreate);

			bool result = Convert(input_stream, output_stream, size, keep_aspect_ratio);

			input_stream.Close();
			output_stream.Close();

			return result;
		}
		public static bool Convert(Bitmap input_bit, string output_icon, int size = -1, bool keep_aspect_ratio = false) {
			FileStream output_stream = new FileStream(output_icon, FileMode.OpenOrCreate);
			bool result = Convert(input_bit, output_stream, size, keep_aspect_ratio);
			output_stream.Close();

			return result;
		}
	}
}