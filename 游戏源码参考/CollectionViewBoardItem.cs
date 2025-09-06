using System;
using System.Collections.Generic;
using GlobalSettings;
using TeamCherry.NestedFadeGroup;
using UnityEngine;

public class CollectionViewBoardItem : InventoryItemUpdateable, InventoryItemListManager<CollectionViewBoardItem, ICollectionViewerItem>.IMoveNextButton
{
	[SerializeField]
	private GameObject emptyNotch;

	[SerializeField]
	private NestedFadeGroupSpriteRenderer itemIcon;

	[SerializeField]
	private Animator playingIndicator;

	[SerializeField]
	private GameObject newBurst;

	[SerializeField]
	private AudioEvent newBurstSound;

	[SerializeField]
	private CameraShakeTarget newBurstShake;

	private CustomInventoryItemCollectableDisplay currentIconOverride;

	private static readonly Dictionary<ICollectionViewerItem, CustomInventoryItemCollectableDisplay> _spawnedIconOverrides = new Dictionary<ICollectionViewerItem, CustomInventoryItemCollectableDisplay>();

	private static readonly int _isPlayingAnim = Animator.StringToHash("IsPlaying");

	private Transform iconTransform;

	public ICollectionViewerItem Item { get; private set; }

	public override string DisplayName
	{
		get
		{
			if (Item == null)
			{
				return string.Empty;
			}
			return Item.GetCollectionName();
		}
	}

	public override string Description
	{
		get
		{
			if (Item == null)
			{
				return string.Empty;
			}
			return Item.GetCollectionDesc();
		}
	}

	public CollectionViewBoard Board { get; set; }

	public bool WillSubmitMoveNext => true;

	protected override bool IsSeen
	{
		get
		{
			ICollectionViewerItem item = Item;
			if (item == null || !item.IsVisibleInCollection())
			{
				return true;
			}
			if (!item.IsSeenOverridden)
			{
				return item.IsSeen;
			}
			return item.IsSeenOverrideValue;
		}
		set
		{
			ICollectionViewerItem item = Item;
			if (item != null && item.IsVisibleInCollection())
			{
				if (item.IsSeenOverridden)
				{
					item.IsSeenOverrideValue = value;
				}
				else
				{
					item.IsSeen = value;
				}
			}
		}
	}

	public event Action Canceled;

	protected override void Awake()
	{
		base.Awake();
		InventoryPaneBase componentInParent = GetComponentInParent<InventoryPaneBase>();
		if ((bool)componentInParent)
		{
			componentInParent.OnPaneEnd += delegate
			{
				Item = null;
			};
		}
		if ((bool)playingIndicator)
		{
			playingIndicator.gameObject.SetActive(value: false);
		}
	}

	public void SetItem(ICollectionViewerItem item, out bool isNew)
	{
		Item = item;
		UpdateDisplay();
		base.gameObject.name = ((item != null) ? item.name : "null");
		if ((bool)newBurst)
		{
			newBurst.SetActive(value: false);
		}
		if (item == null)
		{
			isNew = false;
			return;
		}
		if ((bool)currentIconOverride && currentIconOverride.Owner == this)
		{
			currentIconOverride.gameObject.SetActive(value: false);
			currentIconOverride = null;
		}
		iconTransform = (itemIcon ? itemIcon.transform : base.transform);
		if (item.IsVisibleInCollection())
		{
			isNew = false;
			CollectableRelic collectableRelic = item as CollectableRelic;
			if (collectableRelic != null && (bool)collectableRelic.RelicType && (bool)collectableRelic.RelicType.IconOverridePrefab)
			{
				if (!_spawnedIconOverrides.ContainsKey(item) || _spawnedIconOverrides[item] == null)
				{
					CustomInventoryItemCollectableDisplay customInventoryItemCollectableDisplay = UnityEngine.Object.Instantiate(collectableRelic.RelicType.IconOverridePrefab, iconTransform);
					customInventoryItemCollectableDisplay.transform.Reset();
					_spawnedIconOverrides[item] = customInventoryItemCollectableDisplay;
					currentIconOverride = customInventoryItemCollectableDisplay;
				}
				else
				{
					currentIconOverride = _spawnedIconOverrides[item];
					currentIconOverride.transform.SetParentReset(iconTransform);
					currentIconOverride.gameObject.SetActive(value: true);
				}
			}
			ShowItemIcon();
		}
		else
		{
			isNew = item is CollectableItemMemento collectableItemMemento && collectableItemMemento.CollectedAmount > 0;
			if ((bool)itemIcon)
			{
				itemIcon.AlphaSelf = 0f;
			}
			if ((bool)emptyNotch)
			{
				emptyNotch.SetActive(value: true);
			}
		}
		Board.UpdateRelicItemsIsPlaying();
	}

	public override bool Cancel()
	{
		if (this.Canceled != null)
		{
			this.Canceled();
			return true;
		}
		return base.Cancel();
	}

	public override bool Extra()
	{
		if (Item == null)
		{
			return base.Extra();
		}
		if (Item.IsVisibleInCollection() && Board.TryPlayRelic(this))
		{
			Board.UpdateRelicItemsIsPlaying();
			return true;
		}
		return base.Extra();
	}

	public void UpdatedIsPlaying()
	{
		if (!currentIconOverride)
		{
			return;
		}
		bool flag = Board.IsPlaying(this);
		Animator component = currentIconOverride.GetComponent<Animator>();
		if ((bool)component)
		{
			component.SetBool(_isPlayingAnim, flag);
		}
		if ((bool)playingIndicator)
		{
			if (flag && !playingIndicator.gameObject.activeSelf)
			{
				playingIndicator.gameObject.SetActive(value: true);
			}
			playingIndicator.SetBool(_isPlayingAnim, flag);
		}
	}

	private void ShowItemIcon()
	{
		if ((bool)itemIcon)
		{
			itemIcon.Sprite = Item.GetCollectionIcon();
			if (currentIconOverride == null)
			{
				itemIcon.AlphaSelf = 1f;
			}
			else
			{
				itemIcon.AlphaSelf = 0f;
				SpriteRenderer component = currentIconOverride.GetComponent<SpriteRenderer>();
				if ((bool)component)
				{
					component.maskInteraction = itemIcon.GetComponent<SpriteRenderer>().maskInteraction;
				}
			}
		}
		if ((bool)emptyNotch)
		{
			emptyNotch.SetActive(value: false);
		}
	}

	public void DoNewAppear()
	{
		iconTransform.gameObject.SetActive(value: true);
		ShowItemIcon();
		UpdateDisplay();
		Animator animator = null;
		if ((bool)currentIconOverride)
		{
			animator = currentIconOverride.GetComponent<Animator>();
		}
		else if ((bool)itemIcon)
		{
			animator = itemIcon.GetComponent<Animator>();
		}
		if (animator != null)
		{
			animator.Play("Appear");
		}
		if ((bool)newBurst)
		{
			newBurst.SetActive(value: true);
		}
		newBurstSound.SpawnAndPlayOneShot(Audio.DefaultUIAudioSourcePrefab, base.transform.position);
		newBurstShake.DoShake(this);
	}
}
