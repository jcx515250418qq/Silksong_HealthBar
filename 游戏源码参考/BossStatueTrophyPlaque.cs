using System.Collections;
using UnityEngine;

public class BossStatueTrophyPlaque : MonoBehaviour
{
	public enum DisplayType
	{
		None = -1,
		Tier1 = 0,
		Tier2 = 1,
		Tier3 = 2
	}

	[ArrayForEnum(typeof(DisplayType))]
	public GameObject[] displayObjects;

	[Space]
	public Transform tierCompleteEffectPoint;

	public float tierCompleteEffectDelay = 1f;

	[ArrayForEnum(typeof(DisplayType))]
	public GameObject[] tierCompleteEffectPrefabs;

	private GameObject spawnedCompleteEffect;

	public void SetDisplay(DisplayType type)
	{
		SetDisplayObject(type);
	}

	public void DoTierCompleteEffect(DisplayType type)
	{
		if (type >= DisplayType.Tier1)
		{
			GameObject gameObject = tierCompleteEffectPrefabs[(int)type];
			if ((bool)gameObject)
			{
				spawnedCompleteEffect = Object.Instantiate(gameObject, tierCompleteEffectPoint.position, gameObject.transform.rotation);
				spawnedCompleteEffect.SetActive(value: false);
				StartCoroutine(TierCompleteEffectDelayed());
			}
		}
	}

	private IEnumerator TierCompleteEffectDelayed()
	{
		yield return new WaitForSeconds(tierCompleteEffectDelay);
		spawnedCompleteEffect.SetActive(value: true);
	}

	private void SetDisplayObject(DisplayType type)
	{
		GameObject[] array = displayObjects;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].SetActive(value: false);
		}
		if (type >= DisplayType.Tier1)
		{
			displayObjects[(int)type].SetActive(value: true);
		}
	}

	public static DisplayType GetDisplayType(BossStatue.Completion completion)
	{
		DisplayType result = DisplayType.None;
		if (completion.completedTier3)
		{
			result = DisplayType.Tier3;
		}
		else if (completion.completedTier2)
		{
			result = DisplayType.Tier2;
		}
		else if (completion.completedTier1)
		{
			result = DisplayType.Tier1;
		}
		return result;
	}
}
