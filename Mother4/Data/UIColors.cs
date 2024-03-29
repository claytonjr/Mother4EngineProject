﻿using System;
using SFML.Graphics;

namespace Mother4.Data
{
	internal static class UIColors
	{
		public static Color HighlightColor
		{
			get
			{
				return UIColors.highlightColors[(int)(checked((IntPtr)Math.Max(0L, unchecked(Math.Min((long)(UIColors.highlightColors.Length - 1), (long)((ulong)Settings.WindowFlavor))))))];
			}
		}

		private static Color[] highlightColors = new Color[]
		{
			new Color(66, 240, 15),
			new Color(142, 234, 172),
			new Color(240, 121, 145),
			new Color(182, 96, 10),
			new Color(201, 144, 111),
			new Color(101, 206, 237),
			new Color(106, 103, 199),
			new Color(209, 3, 10)
		};
	}
}
