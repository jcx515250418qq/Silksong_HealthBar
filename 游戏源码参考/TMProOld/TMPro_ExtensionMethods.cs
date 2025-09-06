using System.Collections.Generic;
using UnityEngine;

namespace TMProOld
{
	public static class TMPro_ExtensionMethods
	{
		public static string ArrayToString(this char[] chars)
		{
			string text = string.Empty;
			for (int i = 0; i < chars.Length && chars[i] != 0; i++)
			{
				text += chars[i];
			}
			return text;
		}

		public static int FindInstanceID<T>(this List<T> list, T target) where T : Object
		{
			int instanceID = target.GetInstanceID();
			for (int i = 0; i < list.Count; i++)
			{
				if (list[i].GetInstanceID() == instanceID)
				{
					return i;
				}
			}
			return -1;
		}

		public static bool Compare(this Color32 a, Color32 b)
		{
			if (a.r == b.r && a.g == b.g && a.b == b.b)
			{
				return a.a == b.a;
			}
			return false;
		}

		public static bool CompareRGB(this Color32 a, Color32 b)
		{
			if (a.r == b.r && a.g == b.g)
			{
				return a.b == b.b;
			}
			return false;
		}

		public static bool Compare(this Color a, Color b)
		{
			if (a.r == b.r && a.g == b.g && a.b == b.b)
			{
				return a.a == b.a;
			}
			return false;
		}

		public static bool CompareRGB(this Color a, Color b)
		{
			if (a.r == b.r && a.g == b.g)
			{
				return a.b == b.b;
			}
			return false;
		}

		public static Color32 Multiply(this Color32 c1, Color32 c2)
		{
			byte r = (byte)((float)(int)c1.r / 255f * ((float)(int)c2.r / 255f) * 255f);
			byte g = (byte)((float)(int)c1.g / 255f * ((float)(int)c2.g / 255f) * 255f);
			byte b = (byte)((float)(int)c1.b / 255f * ((float)(int)c2.b / 255f) * 255f);
			byte a = (byte)((float)(int)c1.a / 255f * ((float)(int)c2.a / 255f) * 255f);
			return new Color32(r, g, b, a);
		}

		public static Color32 Tint(this Color32 c1, Color32 c2)
		{
			byte r = (byte)((float)(int)c1.r / 255f * ((float)(int)c2.r / 255f) * 255f);
			byte g = (byte)((float)(int)c1.g / 255f * ((float)(int)c2.g / 255f) * 255f);
			byte b = (byte)((float)(int)c1.b / 255f * ((float)(int)c2.b / 255f) * 255f);
			byte a = (byte)((float)(int)c1.a / 255f * ((float)(int)c2.a / 255f) * 255f);
			return new Color32(r, g, b, a);
		}

		public static Color32 Tint(this Color32 c1, float tint)
		{
			byte r = (byte)Mathf.Clamp((float)(int)c1.r / 255f * tint * 255f, 0f, 255f);
			byte g = (byte)Mathf.Clamp((float)(int)c1.g / 255f * tint * 255f, 0f, 255f);
			byte b = (byte)Mathf.Clamp((float)(int)c1.b / 255f * tint * 255f, 0f, 255f);
			byte a = (byte)Mathf.Clamp((float)(int)c1.a / 255f * tint * 255f, 0f, 255f);
			return new Color32(r, g, b, a);
		}

		public static bool Compare(this Vector3 v1, Vector3 v2, int accuracy)
		{
			bool num = (int)(v1.x * (float)accuracy) == (int)(v2.x * (float)accuracy);
			bool flag = (int)(v1.y * (float)accuracy) == (int)(v2.y * (float)accuracy);
			bool flag2 = (int)(v1.z * (float)accuracy) == (int)(v2.z * (float)accuracy);
			return num && flag && flag2;
		}

		public static bool Compare(this Quaternion q1, Quaternion q2, int accuracy)
		{
			bool num = (int)(q1.x * (float)accuracy) == (int)(q2.x * (float)accuracy);
			bool flag = (int)(q1.y * (float)accuracy) == (int)(q2.y * (float)accuracy);
			bool flag2 = (int)(q1.z * (float)accuracy) == (int)(q2.z * (float)accuracy);
			bool flag3 = (int)(q1.w * (float)accuracy) == (int)(q2.w * (float)accuracy);
			return num && flag && flag2 && flag3;
		}
	}
}
