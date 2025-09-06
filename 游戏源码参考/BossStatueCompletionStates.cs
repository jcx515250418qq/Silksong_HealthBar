using System;
using UnityEngine;

public class BossStatueCompletionStates : MonoBehaviour
{
	public enum Tiers
	{
		Tier1 = 0,
		Tier2 = 1,
		Tier3 = 2
	}

	[Serializable]
	public struct State
	{
		[SerializeField]
		private GameObject gameObject;

		[SerializeField]
		private string playmakerEvent;

		public void SetActive(bool value)
		{
			if ((bool)gameObject)
			{
				gameObject.SetActive(value);
				if (value && !string.IsNullOrEmpty(playmakerEvent))
				{
					FSMUtility.SendEventToGameObject(gameObject, playmakerEvent);
				}
			}
		}
	}

	public BossSummaryBoard bossListSource;

	public State defaultState;

	[ArrayForEnum(typeof(Tiers))]
	public State[] tierStates;

	public bool checkTiersAdditive = true;

	private void OnValidate()
	{
		ArrayForEnumAttribute.EnsureArraySize(ref tierStates, typeof(Tiers));
	}

	private void Start()
	{
		Tiers? highestCompletedTier = GetHighestCompletedTier();
		if (!highestCompletedTier.HasValue)
		{
			defaultState.SetActive(value: true);
			return;
		}
		for (int i = 0; i < tierStates.Length; i++)
		{
			tierStates[i].SetActive(value: false);
		}
		for (int j = 0; j < tierStates.Length; j++)
		{
			if (j == (int)highestCompletedTier.Value)
			{
				tierStates[j].SetActive(value: true);
			}
		}
	}

	public Tiers? GetHighestCompletedTier()
	{
		for (int num = Enum.GetNames(typeof(Tiers)).Length - 1; num >= 0; num--)
		{
			if (GetIsTierCompleted((Tiers)num))
			{
				return (Tiers)num;
			}
		}
		return null;
	}

	public bool GetIsTierCompleted(Tiers tier)
	{
		int completed = 0;
		int total = 0;
		CountCompletion(tier, out completed, out total);
		return completed >= total;
	}

	public void CountCompletion(Tiers tier, out int completed, out int total)
	{
		completed = 0;
		total = 0;
		BossStatue[] array = (bossListSource ? bossListSource.bossStatues.ToArray() : new BossStatue[0]);
		foreach (BossStatue bossStatue in array)
		{
			if (!bossStatue.gameObject.activeInHierarchy || bossStatue.hasNoTiers || bossStatue.dontCountCompletion)
			{
				continue;
			}
			if (bossStatue.HasRegularVersion)
			{
				total++;
				if (HasCompletedTier(bossStatue.StatueState, tier))
				{
					completed++;
				}
			}
			if (bossStatue.HasDreamVersion)
			{
				total++;
				if (HasCompletedTier(bossStatue.DreamStatueState, tier))
				{
					completed++;
				}
			}
		}
	}

	private bool HasCompletedTier(BossStatue.Completion completion, Tiers tier)
	{
		switch (tier)
		{
		case Tiers.Tier1:
			if (completion.completedTier1)
			{
				return true;
			}
			break;
		case Tiers.Tier2:
			if (completion.completedTier2)
			{
				return true;
			}
			break;
		case Tiers.Tier3:
			if (completion.completedTier3)
			{
				return true;
			}
			break;
		}
		if (checkTiersAdditive && (int)tier < Enum.GetNames(typeof(Tiers)).Length - 1)
		{
			return HasCompletedTier(completion, tier + 1);
		}
		return false;
	}
}
