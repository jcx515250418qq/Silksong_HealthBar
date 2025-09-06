using System;
using TeamCherry.Localization;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Audio;
using UnityEngine.ResourceManagement.AsyncOperations;

[CreateAssetMenu(menuName = "Hornet/Collectable Items/Relics/Collectable Relic")]
public class CollectableRelic : QuestTargetCounter, ICollectionViewerItem
{
	[Space]
	[SerializeField]
	private LocalisedString description;

	[Space]
	[SerializeField]
	[ModifiableProperty]
	[Conditional("IsGramaphoneRelicPlayType", true, true, false)]
	private AssetReferenceT<AudioClip> gramaphoneClipRef;

	[SerializeField]
	[ModifiableProperty]
	[Conditional("IsGramaphoneRelicPlayType", true, true, false)]
	private AssetReferenceT<AudioClip> needolinClipRef;

	[SerializeField]
	[ModifiableProperty]
	[Conditional("IsGramaphoneRelicPlayType", true, true, false)]
	private AudioMixerGroup mixerOverride;

	[SerializeField]
	private bool playSyncedAudioSource;

	[Space]
	[SerializeField]
	private string playEventRegister;

	[SerializeField]
	private SavedItem eventConditionItem;

	[NonSerialized]
	private CollectableItemRelicType previousRelicType;

	private int loadCount;

	private int loadWaitingCount;

	private AudioClip gramaphoneClip;

	private AudioClip needolinClip;

	public string DisplayName => RelicType?.GetDisplayName(CollectableItem.ReadSource.Inventory);

	public string Description => description;

	public CollectableItemRelicType RelicType { get; set; }

	public bool PlaySyncedAudioSource => playSyncedAudioSource;

	public CollectableRelicsData.Data SavedData
	{
		get
		{
			return CollectableRelicManager.GetRelicData(this);
		}
		set
		{
			CollectableRelicManager.SetRelicData(this, value);
		}
	}

	public bool IsInInventory
	{
		get
		{
			CollectableRelicsData.Data savedData = SavedData;
			if (savedData.IsCollected)
			{
				return !savedData.IsDeposited;
			}
			return false;
		}
	}

	public bool IsPlayable
	{
		get
		{
			if ((bool)RelicType)
			{
				return RelicType.RelicPlayType != CollectableItemRelicType.RelicPlayTypes.None;
			}
			return false;
		}
	}

	public bool IsLoading => loadWaitingCount > 0;

	public AudioClip GramaphoneClip
	{
		get
		{
			if (IsGramaphoneRelicPlayType())
			{
				return gramaphoneClip;
			}
			return null;
		}
	}

	public AudioClip NeedolinClip
	{
		get
		{
			if (IsGramaphoneRelicPlayType())
			{
				return needolinClip;
			}
			return null;
		}
	}

	public AudioMixerGroup MixerOverride
	{
		get
		{
			if (IsGramaphoneRelicPlayType())
			{
				return mixerOverride;
			}
			return null;
		}
	}

	public bool WillSendPlayEvent
	{
		get
		{
			if (!string.IsNullOrEmpty(playEventRegister) && (!eventConditionItem || eventConditionItem.CanGetMore()))
			{
				return true;
			}
			return false;
		}
	}

	public bool IsSeen
	{
		get
		{
			return SavedData.HasSeenInRelicBoard;
		}
		set
		{
			CollectableRelicsData.Data savedData = SavedData;
			savedData.HasSeenInRelicBoard = value;
			SavedData = savedData;
		}
	}

	string ICollectionViewerItem.name => base.name;

	private bool IsGramaphoneRelicPlayType()
	{
		if (!RelicType)
		{
			return false;
		}
		return RelicType.RelicPlayType == CollectableItemRelicType.RelicPlayTypes.Gramaphone;
	}

	public override void Get(bool showPopup = true)
	{
		CollectableRelicsData.Data savedData = SavedData;
		savedData.IsCollected = true;
		SavedData = savedData;
		if ((bool)RelicType)
		{
			bool flag = true;
			if (showPopup)
			{
				CollectableUIMsg itemUiMsg = CollectableUIMsg.Spawn(this);
				if (QuestManager.MaybeShowQuestUpdated(this, itemUiMsg))
				{
					flag = false;
				}
			}
			if (flag)
			{
				CollectableItemManager.CollectedItem = RelicType;
				InventoryPaneList.SetNextOpen("Inv");
				PlayerData.instance.InvPaneHasNew = true;
			}
		}
		CollectableItemHeroReaction.DoReaction();
	}

	public override bool CanGetMore()
	{
		return !SavedData.IsCollected;
	}

	public void Deposit()
	{
		CollectableRelicsData.Data savedData = SavedData;
		if (!savedData.IsCollected)
		{
			Debug.LogError("Can't deposit a relic that hasn't been collected!", this);
			return;
		}
		savedData.IsDeposited = true;
		SavedData = savedData;
	}

	public override Sprite GetPopupIcon()
	{
		if (!RelicType)
		{
			return null;
		}
		return RelicType.GetUIMsgSprite();
	}

	public override string GetPopupName()
	{
		if (!RelicType)
		{
			return "!!NO_RELIC_TYPE!!";
		}
		return RelicType.GetDisplayName(CollectableItem.ReadSource.Inventory);
	}

	public override float GetUIMsgIconScale()
	{
		if (!RelicType)
		{
			return base.GetUIMsgIconScale();
		}
		return RelicType.GetUIMsgIconScale();
	}

	public string GetCollectionName()
	{
		return DisplayName;
	}

	public string GetCollectionDesc()
	{
		return Description;
	}

	public Sprite GetCollectionIcon()
	{
		return RelicType.GetIcon(CollectableItem.ReadSource.Inventory);
	}

	public bool IsVisibleInCollection()
	{
		return SavedData.IsDeposited;
	}

	public bool IsRequiredInCollection()
	{
		return true;
	}

	public void OnPlayedEvent()
	{
		EventRegister.SendEvent(playEventRegister);
	}

	public override Sprite GetQuestCounterSprite(int index)
	{
		return GetCollectionIcon();
	}

	public override int GetCompletionAmount(QuestCompletionData.Completion sourceCompletion)
	{
		if (!SavedData.IsCollected)
		{
			return 0;
		}
		return 1;
	}

	public void LoadClips()
	{
		loadCount++;
		if (loadCount <= 1)
		{
			Debug.Log("Started loading clips for " + base.name);
			if (gramaphoneClipRef.RuntimeKeyIsValid())
			{
				loadWaitingCount++;
				AsyncOperationHandle<AudioClip> asyncOperationHandle = gramaphoneClipRef.LoadAssetAsync();
				asyncOperationHandle.Completed += OnCompletedGramaphoneClip;
			}
			if (needolinClipRef.RuntimeKeyIsValid())
			{
				loadWaitingCount++;
				AsyncOperationHandle<AudioClip> asyncOperationHandle = needolinClipRef.LoadAssetAsync();
				asyncOperationHandle.Completed += OnCompletedNeedolinClip;
			}
		}
		void OnCompletedGramaphoneClip(AsyncOperationHandle<AudioClip> op)
		{
			gramaphoneClip = op.Result;
			loadWaitingCount--;
			Debug.Log("Finished loading gramaphone clip for " + base.name);
			op.Completed -= OnCompletedGramaphoneClip;
		}
		void OnCompletedNeedolinClip(AsyncOperationHandle<AudioClip> op)
		{
			needolinClip = op.Result;
			loadWaitingCount--;
			Debug.Log("Finished loading needolin clip for " + base.name);
			op.Completed -= OnCompletedNeedolinClip;
		}
	}

	public void FreeClips()
	{
		if (loadCount <= 0)
		{
			return;
		}
		loadCount--;
		if (loadCount <= 0)
		{
			Debug.Log("Freed clips for " + base.name);
			if (gramaphoneClipRef.IsValid())
			{
				gramaphoneClipRef.ReleaseAsset();
			}
			if (needolinClipRef.IsValid())
			{
				needolinClipRef.ReleaseAsset();
			}
			loadWaitingCount = 0;
		}
	}
}
