using System;
using System.Linq;
using TeamCherry.Localization;
using UnityEngine;

[Serializable]
public class LocalisedTextCollectionData : ILocalisedTextCollection
{
	[Serializable]
	private class ProbabilityLocalisedString : Probability.ProbabilityBase<LocalisedString>
	{
		[SerializeField]
		private LocalisedString text;

		public override LocalisedString Item => text;

		public ProbabilityLocalisedString()
		{
			text = default(LocalisedString);
		}

		public ProbabilityLocalisedString(LocalisedString text)
		{
			this.text = text;
		}
	}

	[Serializable]
	private class Alternative
	{
		public LocalisedTextCollection Collection;

		public PlayerDataTest Condition;
	}

	[SerializeField]
	[LocalisedString.NoKeyValidation]
	private LocalisedString template;

	[SerializeField]
	[AssetPickerDropdown]
	private NeedolinTextConfig configOverride;

	[Space]
	[SerializeField]
	private Alternative[] alternatives;

	[Space]
	[SerializeField]
	private bool doNotPlay;

	private ProbabilityLocalisedString[] currentTexts;

	private float[] currentProbabilities;

	private int previousIndex = -1;

	private bool isInitialised;

	public bool IsActive => !doNotPlay;

	public LocalisedTextCollectionData()
	{
		template = default(LocalisedString);
	}

	public LocalisedTextCollectionData(LocalisedString template)
	{
		this.template = template;
	}

	private void Init()
	{
		string sheetTitle = template.Sheet;
		string templateKey = template.Key;
		if (Language.HasSheet(sheetTitle))
		{
			currentTexts = (from key in Language.GetKeys(sheetTitle).Where(delegate(string key)
				{
					if (!key.StartsWith(templateKey, StringComparison.Ordinal))
					{
						return false;
					}
					for (int i = templateKey.Length; i < key.Length; i++)
					{
						char c = key[i];
						if (c != '_' && !char.IsDigit(c))
						{
							return false;
						}
					}
					return true;
				})
				select new ProbabilityLocalisedString(new LocalisedString(sheetTitle, key))).ToArray();
		}
		else
		{
			Debug.LogErrorFormat("Localisation Sheet: \"{0}\" does not exist!", sheetTitle);
		}
		if (currentTexts == null || currentTexts.Length == 0)
		{
			currentTexts = new ProbabilityLocalisedString[1]
			{
				new ProbabilityLocalisedString(template)
			};
		}
		isInitialised = true;
	}

	public LocalisedTextCollectionData ResolveAlternatives()
	{
		if (alternatives == null)
		{
			return this;
		}
		Alternative[] array = alternatives;
		foreach (Alternative alternative in array)
		{
			if (alternative.Condition.IsFulfilled)
			{
				LocalisedTextCollection collection = alternative.Collection;
				if (!(collection != null))
				{
					return null;
				}
				return collection.ResolveAlternatives();
			}
		}
		return this;
	}

	public LocalisedString GetRandom(LocalisedString skipString)
	{
		LocalisedTextCollectionData localisedTextCollectionData = ResolveAlternatives();
		if (localisedTextCollectionData != this)
		{
			return localisedTextCollectionData.GetRandom(skipString);
		}
		if (!isInitialised)
		{
			Init();
		}
		int num = ((currentTexts.Length <= 1) ? 1 : Mathf.Min(10, currentTexts.Length));
		int chosenIndex = previousIndex;
		LocalisedString localisedString = default(LocalisedString);
		while (num > 0)
		{
			num--;
			localisedString = Probability.GetRandomItemByProbabilityFair<ProbabilityLocalisedString, LocalisedString>(currentTexts, out chosenIndex, ref currentProbabilities);
			if (chosenIndex != previousIndex && (skipString.IsEmpty || !((string)localisedString == (string)skipString)))
			{
				break;
			}
		}
		previousIndex = chosenIndex;
		return localisedString;
	}

	public NeedolinTextConfig GetConfig()
	{
		LocalisedTextCollectionData localisedTextCollectionData = ResolveAlternatives();
		if (localisedTextCollectionData == this)
		{
			return configOverride;
		}
		return localisedTextCollectionData.configOverride;
	}
}
