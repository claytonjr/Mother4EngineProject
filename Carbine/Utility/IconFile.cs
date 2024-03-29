﻿using System;
using System.Collections.Generic;
using fNbt;

namespace Carbine.Utility
{
	internal class IconFile
	{
		public IconFile(string filename)
		{
			this.icons = new Dictionary<int, byte[]>();
			NbtFile file = new NbtFile(filename);
			this.LoadFile(file);
		}

		private void LoadFile(NbtFile file)
		{
			NbtCompound rootTag = file.RootTag;
			foreach (NbtTag nbtTag in rootTag)
			{
				if (nbtTag is NbtByteArray)
				{
					NbtByteArray nbtByteArray = (NbtByteArray)nbtTag;
					byte[] byteArrayValue = nbtByteArray.ByteArrayValue;
					int key = (int)Math.Sqrt((double)(byteArrayValue.Length / 4));
					this.icons.Add(key, byteArrayValue);
				}
			}
		}

		public byte[] GetBytesForSize(int width)
		{
			byte[] result = null;
			this.icons.TryGetValue(width, out result);
			return result;
		}

		private const int BYTES_PER_PIXEL = 4;

		private Dictionary<int, byte[]> icons;
	}
}
