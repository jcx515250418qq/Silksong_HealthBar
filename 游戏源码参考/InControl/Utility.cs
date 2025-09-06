using System;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace InControl
{
	public static class Utility
	{
		public const float Epsilon = 1E-07f;

		private static readonly Vector2[] circleVertexList = new Vector2[25]
		{
			new Vector2(0f, 1f),
			new Vector2(0.2588f, 0.9659f),
			new Vector2(0.5f, 0.866f),
			new Vector2(0.7071f, 0.7071f),
			new Vector2(0.866f, 0.5f),
			new Vector2(0.9659f, 0.2588f),
			new Vector2(1f, 0f),
			new Vector2(0.9659f, -0.2588f),
			new Vector2(0.866f, -0.5f),
			new Vector2(0.7071f, -0.7071f),
			new Vector2(0.5f, -0.866f),
			new Vector2(0.2588f, -0.9659f),
			new Vector2(0f, -1f),
			new Vector2(-0.2588f, -0.9659f),
			new Vector2(-0.5f, -0.866f),
			new Vector2(-0.7071f, -0.7071f),
			new Vector2(-0.866f, -0.5f),
			new Vector2(-0.9659f, -0.2588f),
			new Vector2(-1f, -0f),
			new Vector2(-0.9659f, 0.2588f),
			new Vector2(-0.866f, 0.5f),
			new Vector2(-0.7071f, 0.7071f),
			new Vector2(-0.5f, 0.866f),
			new Vector2(-0.2588f, 0.9659f),
			new Vector2(0f, 1f)
		};

		internal static bool Is32Bit => IntPtr.Size == 4;

		internal static bool Is64Bit => IntPtr.Size == 8;

		public static void DrawCircleGizmo(Vector2 center, float radius)
		{
			Vector2 vector = circleVertexList[0] * radius + center;
			int num = circleVertexList.Length;
			for (int i = 1; i < num; i++)
			{
				Gizmos.DrawLine(vector, vector = circleVertexList[i] * radius + center);
			}
		}

		public static void DrawCircleGizmo(Vector2 center, float radius, Color color)
		{
			Gizmos.color = color;
			DrawCircleGizmo(center, radius);
		}

		public static void DrawOvalGizmo(Vector2 center, Vector2 size)
		{
			Vector2 b = size / 2f;
			Vector2 vector = Vector2.Scale(circleVertexList[0], b) + center;
			int num = circleVertexList.Length;
			for (int i = 1; i < num; i++)
			{
				Gizmos.DrawLine(vector, vector = Vector2.Scale(circleVertexList[i], b) + center);
			}
		}

		public static void DrawOvalGizmo(Vector2 center, Vector2 size, Color color)
		{
			Gizmos.color = color;
			DrawOvalGizmo(center, size);
		}

		public static void DrawRectGizmo(Rect rect)
		{
			Vector3 vector = new Vector3(rect.xMin, rect.yMin);
			Vector3 vector2 = new Vector3(rect.xMax, rect.yMin);
			Vector3 vector3 = new Vector3(rect.xMax, rect.yMax);
			Vector3 vector4 = new Vector3(rect.xMin, rect.yMax);
			Gizmos.DrawLine(vector, vector2);
			Gizmos.DrawLine(vector2, vector3);
			Gizmos.DrawLine(vector3, vector4);
			Gizmos.DrawLine(vector4, vector);
		}

		public static void DrawRectGizmo(Rect rect, Color color)
		{
			Gizmos.color = color;
			DrawRectGizmo(rect);
		}

		public static void DrawRectGizmo(Vector2 center, Vector2 size)
		{
			float num = size.x / 2f;
			float num2 = size.y / 2f;
			Vector3 vector = new Vector3(center.x - num, center.y - num2);
			Vector3 vector2 = new Vector3(center.x + num, center.y - num2);
			Vector3 vector3 = new Vector3(center.x + num, center.y + num2);
			Vector3 vector4 = new Vector3(center.x - num, center.y + num2);
			Gizmos.DrawLine(vector, vector2);
			Gizmos.DrawLine(vector2, vector3);
			Gizmos.DrawLine(vector3, vector4);
			Gizmos.DrawLine(vector4, vector);
		}

		public static void DrawRectGizmo(Vector2 center, Vector2 size, Color color)
		{
			Gizmos.color = color;
			DrawRectGizmo(center, size);
		}

		public static bool GameObjectIsCulledOnCurrentCamera(GameObject gameObject)
		{
			return (Camera.current.cullingMask & (1 << gameObject.layer)) == 0;
		}

		public static Color MoveColorTowards(Color color0, Color color1, float maxDelta)
		{
			float r = Mathf.MoveTowards(color0.r, color1.r, maxDelta);
			float g = Mathf.MoveTowards(color0.g, color1.g, maxDelta);
			float b = Mathf.MoveTowards(color0.b, color1.b, maxDelta);
			float a = Mathf.MoveTowards(color0.a, color1.a, maxDelta);
			return new Color(r, g, b, a);
		}

		public static float ApplyDeadZone(float value, float lowerDeadZone, float upperDeadZone)
		{
			float num = upperDeadZone - lowerDeadZone;
			if (value < 0f)
			{
				if (value > 0f - lowerDeadZone)
				{
					return 0f;
				}
				if (value < 0f - upperDeadZone)
				{
					return -1f;
				}
				return (value + lowerDeadZone) / num;
			}
			if (value < lowerDeadZone)
			{
				return 0f;
			}
			if (value > upperDeadZone)
			{
				return 1f;
			}
			return (value - lowerDeadZone) / num;
		}

		public static float ApplySmoothing(float thisValue, float lastValue, float deltaTime, float sensitivity)
		{
			if (Approximately(sensitivity, 1f))
			{
				return thisValue;
			}
			float maxDelta = deltaTime * sensitivity * 100f;
			if (IsNotZero(thisValue) && Sign(lastValue) != Sign(thisValue))
			{
				lastValue = 0f;
			}
			return Mathf.MoveTowards(lastValue, thisValue, maxDelta);
		}

		public static float ApplySnapping(float value, float threshold)
		{
			if (value < 0f - threshold)
			{
				return -1f;
			}
			if (value > threshold)
			{
				return 1f;
			}
			return 0f;
		}

		internal static bool TargetIsButton(InputControlType target)
		{
			if (target < InputControlType.Action1 || target > InputControlType.Action12)
			{
				if (target >= InputControlType.Button0)
				{
					return target <= InputControlType.Button19;
				}
				return false;
			}
			return true;
		}

		internal static bool TargetIsStandard(InputControlType target)
		{
			if (target < InputControlType.LeftStickUp || target > InputControlType.Action12)
			{
				if (target >= InputControlType.Command)
				{
					return target <= InputControlType.RightCommand;
				}
				return false;
			}
			return true;
		}

		internal static bool TargetIsAlias(InputControlType target)
		{
			if (target >= InputControlType.Command)
			{
				return target <= InputControlType.RightCommand;
			}
			return false;
		}

		public static string ReadFromFile(string path)
		{
			StreamReader streamReader = new StreamReader(path);
			string result = streamReader.ReadToEnd();
			streamReader.Close();
			return result;
		}

		public static void WriteToFile(string path, string data)
		{
			StreamWriter streamWriter = new StreamWriter(path);
			streamWriter.Write(data);
			streamWriter.Flush();
			streamWriter.Close();
		}

		public static float Abs(float value)
		{
			if (!(value < 0f))
			{
				return value;
			}
			return 0f - value;
		}

		public static bool Approximately(float v1, float v2)
		{
			float num = v1 - v2;
			if (num >= -1E-07f)
			{
				return num <= 1E-07f;
			}
			return false;
		}

		public static bool Approximately(Vector2 v1, Vector2 v2)
		{
			if (Approximately(v1.x, v2.x))
			{
				return Approximately(v1.y, v2.y);
			}
			return false;
		}

		public static bool IsNotZero(float value)
		{
			if (!(value < -1E-07f))
			{
				return value > 1E-07f;
			}
			return true;
		}

		public static bool IsZero(float value)
		{
			if (value >= -1E-07f)
			{
				return value <= 1E-07f;
			}
			return false;
		}

		public static int Sign(float f)
		{
			if (!((double)f < 0.0))
			{
				return 1;
			}
			return -1;
		}

		public static bool AbsoluteIsOverThreshold(float value, float threshold)
		{
			if (!(value < 0f - threshold))
			{
				return value > threshold;
			}
			return true;
		}

		public static float NormalizeAngle(float angle)
		{
			while (angle < 0f)
			{
				angle += 360f;
			}
			while (angle > 360f)
			{
				angle -= 360f;
			}
			return angle;
		}

		public static float VectorToAngle(Vector2 vector)
		{
			if (IsZero(vector.x) && IsZero(vector.y))
			{
				return 0f;
			}
			return NormalizeAngle(Mathf.Atan2(vector.x, vector.y) * 57.29578f);
		}

		public static float Min(float v0, float v1)
		{
			if (!(v0 >= v1))
			{
				return v0;
			}
			return v1;
		}

		public static float Max(float v0, float v1)
		{
			if (!(v0 <= v1))
			{
				return v0;
			}
			return v1;
		}

		public static float Min(float v0, float v1, float v2, float v3)
		{
			float num = ((v0 >= v1) ? v1 : v0);
			float num2 = ((v2 >= v3) ? v3 : v2);
			if (!(num >= num2))
			{
				return num;
			}
			return num2;
		}

		public static float Max(float v0, float v1, float v2, float v3)
		{
			float num = ((v0 <= v1) ? v1 : v0);
			float num2 = ((v2 <= v3) ? v3 : v2);
			if (!(num <= num2))
			{
				return num;
			}
			return num2;
		}

		internal static float ValueFromSides(float negativeSide, float positiveSide)
		{
			float num = Abs(negativeSide);
			float num2 = Abs(positiveSide);
			if (Approximately(num, num2))
			{
				return 0f;
			}
			if (!(num > num2))
			{
				return num2;
			}
			return 0f - num;
		}

		internal static float ValueFromSides(float negativeSide, float positiveSide, bool invertSides)
		{
			if (invertSides)
			{
				return ValueFromSides(positiveSide, negativeSide);
			}
			return ValueFromSides(negativeSide, positiveSide);
		}

		public static void ArrayResize<T>(ref T[] array, int capacity)
		{
			if (array == null || capacity > array.Length)
			{
				Array.Resize(ref array, NextPowerOfTwo(capacity));
			}
		}

		public static void ArrayExpand<T>(ref T[] array, int capacity)
		{
			if (array == null || capacity > array.Length)
			{
				array = new T[NextPowerOfTwo(capacity)];
			}
		}

		public static void ArrayAppend<T>(ref T[] array, T item)
		{
			if (array == null)
			{
				array = new T[1];
				array[0] = item;
			}
			else
			{
				Array.Resize(ref array, array.Length + 1);
				array[array.Length - 1] = item;
			}
		}

		public static void ArrayAppend<T>(ref T[] array, T[] items)
		{
			if (array == null)
			{
				array = new T[items.Length];
				Array.Copy(items, array, items.Length);
			}
			else
			{
				Array.Resize(ref array, array.Length + items.Length);
				Array.ConstrainedCopy(items, 0, array, array.Length - items.Length, items.Length);
			}
		}

		public static int NextPowerOfTwo(int value)
		{
			if (value > 0)
			{
				value--;
				value |= value >> 1;
				value |= value >> 2;
				value |= value >> 4;
				value |= value >> 8;
				value |= value >> 16;
				value++;
				return value;
			}
			return 0;
		}

		public static string GetPlatformName(bool uppercase = true)
		{
			string windowsVersion = GetWindowsVersion();
			if (!uppercase)
			{
				return windowsVersion;
			}
			return windowsVersion.ToUpper();
		}

		private static string GetHumanUnderstandableWindowsVersion()
		{
			Version version = Environment.OSVersion.Version;
			if (version.Major == 6)
			{
				return version.Minor switch
				{
					3 => "8.1", 
					2 => "8", 
					1 => "7", 
					_ => "Vista", 
				};
			}
			if (version.Major == 5)
			{
				int minor = version.Minor;
				if ((uint)(minor - 1) <= 1u)
				{
					return "XP";
				}
				return "2000";
			}
			return version.Major.ToString();
		}

		public static string GetWindowsVersion()
		{
			string humanUnderstandableWindowsVersion = GetHumanUnderstandableWindowsVersion();
			string text = (Is32Bit ? "32Bit" : "64Bit");
			int systemBuildNumber = GetSystemBuildNumber();
			return "Windows " + humanUnderstandableWindowsVersion + " " + text + " Build " + systemBuildNumber;
		}

		public static int GetSystemBuildNumber()
		{
			return Environment.OSVersion.Version.Build;
		}

		public static void LoadScene(string sceneName)
		{
			SceneManager.LoadScene(sceneName);
		}

		internal static string PluginFileExtension()
		{
			return ".dll";
		}
	}
}
