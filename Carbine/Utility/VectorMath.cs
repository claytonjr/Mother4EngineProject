﻿using System;
using SFML.System;

namespace Carbine.Utility
{
	public static class VectorMath
	{
		public static Vector2f DirectionToVector(int direction)
		{
			return VectorMath.DIR_TO_VECTOR[direction % VectorMath.DIR_TO_VECTOR.Length];
		}

		public static int VectorToDirection(Vector2f v)
		{
			double num = Math.Atan2((double)(-(double)v.Y), (double)v.X) + 0.39269908169872414;
			int num2 = (int)Math.Floor(num / 0.7853981633974483);
			if (num2 < 0)
			{
				num2 += 8;
			}
			return num2;
		}

		public static float Magnitude(Vector2f v)
		{
			return (float)Math.Sqrt((double)(v.X * v.X + v.Y * v.Y));
		}

		public static Vector2f Normalize(Vector2f v)
		{
			float num = VectorMath.Magnitude(v);
			Vector2f zero_VECTOR;
			if (num > 0f)
			{
				float x = v.X / num;
				float y = v.Y / num;
				zero_VECTOR = new Vector2f(x, y);
			}
			else
			{
				zero_VECTOR = VectorMath.ZERO_VECTOR;
			}
			return zero_VECTOR;
		}

		public static Vector2f LeftNormal(Vector2f v)
		{
			return new Vector2f(v.Y, -v.X);
		}

		public static Vector2f RightNormal(Vector2f v)
		{
			return new Vector2f(-v.Y, v.X);
		}

		public static float DotProduct(Vector2f a, Vector2f b)
		{
			return a.X * b.X + a.Y * b.Y;
		}

		public static Vector2f Truncate(Vector2f v)
		{
			int num = (int)v.X;
			int num2 = (int)v.Y;
			return new Vector2f((float)num, (float)num2);
		}

		public const double PI_OVER_FOUR = 0.7853981633974483;

		public const double PI_OVER_EIGHT = 0.39269908169872414;

		public static readonly Vector2f ZERO_VECTOR = new Vector2f(0f, 0f);

		private static Vector2f[] DIR_TO_VECTOR = new Vector2f[]
		{
			new Vector2f(1f, 0f),
			new Vector2f(1f, -1f),
			new Vector2f(0f, -1f),
			new Vector2f(-1f, -1f),
			new Vector2f(-1f, 0f),
			new Vector2f(-1f, 1f),
			new Vector2f(0f, 1f),
			new Vector2f(1f, 1f)
		};
	}
}
