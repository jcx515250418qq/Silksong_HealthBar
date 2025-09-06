using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using UnityEngine;

public class RelicBoardOwner : MonoBehaviour
{
	[SerializeField]
	private CollectableRelicBoard relicBoard;

	[Space]
	[SerializeField]
	private CollectableRelicTypeList list;

	[SerializeField]
	[PlayerDataField(typeof(bool), false)]
	private string completedBool;

	[Space]
	[SerializeField]
	private CollectionGramaphone gramaphone;

	private bool didStartLoad;

	public CollectableRelicBoard RelicBoard => relicBoard;

	public bool HasGramaphone
	{
		get
		{
			if ((bool)gramaphone)
			{
				return gramaphone.gameObject.activeInHierarchy;
			}
			return false;
		}
	}

	private void Awake()
	{
		didStartLoad = true;
		foreach (CollectableRelic relic in GetRelics())
		{
			relic.LoadClips();
		}
	}

	private void OnDestroy()
	{
		if (!didStartLoad)
		{
			return;
		}
		didStartLoad = false;
		foreach (CollectableRelic relic in GetRelics())
		{
			relic.FreeClips();
		}
	}

	public void PlayOnGramaphone(CollectableRelic playingRelic)
	{
		StopPlayingRelic();
		gramaphone.Play(playingRelic, alreadyPlaying: false, this);
	}

	public void StopPlayingRelic()
	{
		gramaphone.Stop();
	}

	[UsedImplicitly]
	public bool IsAllRelicsDeposited()
	{
		foreach (CollectableItemRelicType item in list)
		{
			foreach (CollectableRelic relic in item.Relics)
			{
				if (!relic.SavedData.IsDeposited)
				{
					return false;
				}
			}
		}
		return true;
	}

	[UsedImplicitly]
	public bool IsAnyRelicsToDeposit()
	{
		foreach (CollectableItemRelicType item in list)
		{
			foreach (CollectableRelic relic in item.Relics)
			{
				CollectableRelicsData.Data savedData = relic.SavedData;
				if (savedData.IsCollected && !savedData.IsDeposited)
				{
					return true;
				}
			}
		}
		return false;
	}

	[UsedImplicitly]
	public void DepositCollectedRelics()
	{
		PlayerData instance = PlayerData.instance;
		int num = 0;
		int num2 = 0;
		foreach (CollectableItemRelicType item in list)
		{
			foreach (CollectableRelic relic in item.Relics)
			{
				num2++;
				CollectableRelicsData.Data savedData = relic.SavedData;
				if (savedData.IsDeposited)
				{
					num++;
				}
				else if (savedData.IsCollected)
				{
					savedData.IsDeposited = true;
					relic.SavedData = savedData;
					num++;
				}
			}
		}
		if (num == num2 && !string.IsNullOrEmpty(completedBool))
		{
			instance.SetBool(completedBool, value: true);
		}
	}

	public IEnumerable<CollectableRelic> GetRelics()
	{
		return list.SelectMany((CollectableItemRelicType type) => type.Relics);
	}

	public CollectableRelic GetPlayingRelic()
	{
		if (!gramaphone)
		{
			return null;
		}
		return gramaphone.PlayingRelic;
	}

	public IEnumerable<CollectableRelic> GetRelicsToDeposit()
	{
		foreach (CollectableItemRelicType item in list)
		{
			foreach (CollectableRelic relic in item.Relics)
			{
				CollectableRelicsData.Data savedData = relic.SavedData;
				if (savedData.IsCollected && !savedData.IsDeposited)
				{
					yield return relic;
				}
			}
		}
	}
}
