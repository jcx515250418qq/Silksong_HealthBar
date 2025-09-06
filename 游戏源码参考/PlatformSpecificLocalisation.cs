using System;
using TeamCherry.Localization;
using UnityEngine;

[RequireComponent(typeof(AutoLocalizeTextUI))]
public class PlatformSpecificLocalisation : MonoBehaviour
{
	[Serializable]
	public struct PlatformKey
	{
		public RuntimePlatform platform;

		public LocalisedString text;

		[HideInInspector]
		public string sheetTitle;

		[HideInInspector]
		public string textKey;
	}

	public PlatformKey[] platformKeys;

	private AutoLocalizeTextUI localisation;

	private void OnValidate()
	{
		for (int i = 0; i < platformKeys.Length; i++)
		{
			PlatformKey platformKey = platformKeys[i];
			if (!string.IsNullOrEmpty(platformKey.sheetTitle) || !string.IsNullOrEmpty(platformKey.textKey))
			{
				platformKey.text = new LocalisedString(platformKey.sheetTitle, platformKey.textKey);
				platformKey.sheetTitle = string.Empty;
				platformKey.textKey = string.Empty;
				platformKeys[i] = platformKey;
			}
		}
	}

	private void Awake()
	{
		OnValidate();
		localisation = GetComponent<AutoLocalizeTextUI>();
		if (!localisation)
		{
			return;
		}
		PlatformKey[] array = platformKeys;
		for (int i = 0; i < array.Length; i++)
		{
			PlatformKey platformKey = array[i];
			if (platformKey.platform == Application.platform)
			{
				localisation.Text = platformKey.text;
				break;
			}
		}
	}
}
