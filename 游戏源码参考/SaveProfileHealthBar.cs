using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SaveProfileHealthBar : MonoBehaviour
{
	private enum CrestTypes
	{
		Hunter = 0,
		Hunter_v2 = 1,
		Hunter_v3 = 2,
		Cloakless = 3,
		Cursed = 4,
		Reaper = 5,
		Spell = 6,
		Toolmaster = 7,
		Wanderer = 8,
		Warrior = 9,
		Witch = 10
	}

	[Serializable]
	private struct CrestTypeInfo
	{
		public Sprite SpoolImage;

		public Sprite SpoolImageSteel;
	}

	[SerializeField]
	[ArrayForEnum(typeof(CrestTypes))]
	private CrestTypeInfo[] crests;

	[Space]
	[SerializeField]
	private Image spoolImage;

	[SerializeField]
	private Image healthTemplate;

	private readonly List<Image> healthImages = new List<Image>(10);

	private void OnValidate()
	{
		ArrayForEnumAttribute.EnsureArraySize(ref crests, typeof(CrestTypes));
	}

	private void Awake()
	{
		OnValidate();
		healthTemplate.gameObject.SetActive(value: false);
	}

	public void ShowHealth(int numberToShow, bool steelsoulMode, string crestId)
	{
		if (Enum.TryParse<CrestTypes>(crestId, out var result))
		{
			CrestTypeInfo crestTypeInfo = crests[(int)result];
			spoolImage.sprite = (steelsoulMode ? crestTypeInfo.SpoolImageSteel : crestTypeInfo.SpoolImage);
		}
		else
		{
			Debug.LogError("Could not parse crest id " + crestId, this);
		}
		for (int num = numberToShow - healthImages.Count; num > 0; num--)
		{
			healthImages.Add(UnityEngine.Object.Instantiate(healthTemplate, healthTemplate.transform.parent));
		}
		for (int i = 0; i < healthImages.Count; i++)
		{
			healthImages[i].gameObject.SetActive(i < numberToShow);
		}
	}
}
