using System;
using System.Globalization;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;

namespace SecPlayerPrefs
{
	public class SecurePlayerPrefs : ScriptableObject
	{
		private static readonly byte[] Salt = new byte[17]
		{
			10, 20, 30, 40, 50, 60, 70, 80, 90, 12,
			13, 14, 15, 16, 17, 18, 19
		};

		private static byte[] keyArray = Encoding.UTF8.GetBytes("UKu52ePUBwetZ9wNX88o54dnfKRu0T1l");

		private static string Encrypt(string toEncrypt)
		{
			byte[] bytes = Encoding.UTF8.GetBytes(toEncrypt);
			byte[] array = new RijndaelManaged
			{
				Key = keyArray,
				Mode = CipherMode.ECB,
				Padding = PaddingMode.PKCS7
			}.CreateEncryptor().TransformFinalBlock(bytes, 0, bytes.Length);
			return Convert.ToBase64String(array, 0, array.Length);
		}

		private static string Decrypt(string toDecrypt)
		{
			byte[] array = Convert.FromBase64String(toDecrypt);
			byte[] bytes = new RijndaelManaged
			{
				Key = keyArray,
				Mode = CipherMode.ECB,
				Padding = PaddingMode.PKCS7
			}.CreateDecryptor().TransformFinalBlock(array, 0, array.Length);
			return Encoding.UTF8.GetString(bytes);
		}

		private static string UTF8ByteArrayToString(byte[] characters)
		{
			return new UTF8Encoding().GetString(characters);
		}

		private static byte[] StringToUTF8ByteArray(string pXmlString)
		{
			return new UTF8Encoding().GetBytes(pXmlString);
		}

		public static void SetInt(string Key, int Value)
		{
			PlayerPrefs.SetString(Encrypt(Key), Encrypt(Value.ToString()));
		}

		public static void SetString(string Key, string Value)
		{
			PlayerPrefs.SetString(Encrypt(Key), Encrypt(Value));
		}

		public static void SetFloat(string Key, float Value)
		{
			PlayerPrefs.SetString(Encrypt(Key), Encrypt(Value.ToString(CultureInfo.InvariantCulture)));
		}

		public static void SetBool(string Key, bool Value)
		{
			PlayerPrefs.SetString(Encrypt(Key), Encrypt(Value.ToString()));
		}

		public static string GetString(string Key)
		{
			string @string = PlayerPrefs.GetString(Encrypt(Key));
			if (@string == "")
			{
				return "";
			}
			return Decrypt(@string);
		}

		public static int GetInt(string Key)
		{
			string @string = PlayerPrefs.GetString(Encrypt(Key));
			if (@string == "")
			{
				return 0;
			}
			return int.Parse(Decrypt(@string));
		}

		public static float GetFloat(string Key)
		{
			string @string = PlayerPrefs.GetString(Encrypt(Key));
			if (@string == "")
			{
				return 0f;
			}
			return float.Parse(Decrypt(@string), NumberStyles.Float, CultureInfo.InvariantCulture);
		}

		public static bool GetBool(string Key)
		{
			string @string = PlayerPrefs.GetString(Encrypt(Key));
			if (@string == "")
			{
				return false;
			}
			return bool.Parse(Decrypt(@string));
		}

		public static void DeleteKey(string Key)
		{
			PlayerPrefs.DeleteKey(Encrypt(Key));
		}

		public static void DeleteAll()
		{
			PlayerPrefs.DeleteAll();
		}

		public static void Save()
		{
			PlayerPrefs.Save();
		}

		public static bool HasKey(string Key)
		{
			return PlayerPrefs.HasKey(Encrypt(Key));
		}
	}
}
