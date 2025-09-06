using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace TeamCherry.Localization.Platform
{
	[Serializable]
	public class LocalizedStringEntry
	{
		public LocalisedString localisedString;

		public string id;

		[NamedArray("GetElementName")]
		public List<LocalizedValue> values = new List<LocalizedValue>();

		private string GetElementName(int index)
		{
			try
			{
				LocalizedValue localizedValue = values[index];
				return $"{index}: {localizedValue.locale} : {localizedValue.text}";
			}
			catch (Exception message)
			{
				Debug.LogError(message);
			}
			return $"Element {index}";
		}

		public void MergeValues(LocalizedStringEntry other)
		{
			if (other != null)
			{
				if (!other.localisedString.IsEmpty)
				{
					localisedString = other.localisedString;
				}
				MergeValues(other.values);
			}
		}

		private void MergeValues(List<LocalizedValue> newValues)
		{
			foreach (LocalizedValue val in newValues)
			{
				LocalizedValue localizedValue = values.FirstOrDefault((LocalizedValue v) => v.locale == val.locale);
				if (localizedValue != null)
				{
					localizedValue.text = val.text;
				}
				else
				{
					values.Add(val);
				}
			}
		}

		private static string PrintLocalisedString(LocalisedString localisedString)
		{
			return "!!" + localisedString.Sheet + "/" + localisedString.Key + "!!";
		}

		public void AddOrUpdate(LocalizedValue value)
		{
			LocalizedValue localizedValue = values.FirstOrDefault((LocalizedValue v) => v.locale == value.locale);
			if (localizedValue != null)
			{
				localizedValue.text = value.text;
			}
			else
			{
				values.Add(value);
			}
		}
	}
}
