using System;
using System.Collections.Generic;
using GlobalSettings;
using TeamCherry.NestedFadeGroup;
using UnityEngine;

public class CollectableRelicBoardItem : InventoryItemSelectableDirectional, InventoryItemListManager<CollectableRelicBoardItem, CollectableRelic>.IMoveNextButton
{
	[SerializeField]
	private GameObject emptyNotch;

	[SerializeField]
	private NestedFadeGroupSpriteRenderer relicIcon;

	[SerializeField]
	private GameObject newOrb;

	[SerializeField]
	private GameObject newBurst;

	[SerializeField]
	private AudioEvent newBurstSound;

	[SerializeField]
	private CameraShakeTarget newBurstShake;

	[SerializeField]
	private Animator playingIndicator;

	private bool isNewOrbVisible;

	private Vector3 newOrbInitialScale;

	private CustomInventoryItemCollectableDisplay currentIconOverride;

	private static readonly Dictionary<CollectableRelic, CustomInventoryItemCollectableDisplay> _spawnedIconOverrides = new Dictionary<CollectableRelic, CustomInventoryItemCollectableDisplay>();

	private static readonly int _isPlayingAnim = Animator.StringToHash("IsPlaying");

	private Transform iconTransform;

	public CollectableRelic Relic { get; private set; }

	public override string DisplayName
	{
		get
		{
			if (!Relic || !Relic.SavedData.IsDeposited)
			{
				return string.Empty;
			}
			return Relic.DisplayName;
		}
	}

	public override string Description
	{
		get
		{
			if (!Relic || !Relic.SavedData.IsDeposited)
			{
				return string.Empty;
			}
			return Relic.Description;
		}
	}

	public CollectableRelicBoard Board { get; set; }

	public bool WillSubmitMoveNext => true;

	public event Action Canceled;

	protected override void Awake()
	{
		base.Awake();
		if ((bool)newOrb)
		{
			newOrbInitialScale = newOrb.transform.localScale;
		}
		InventoryPaneBase componentInParent = GetComponentInParent<InventoryPaneBase>();
		if ((bool)componentInParent)
		{
			componentInParent.OnPaneEnd += delegate
			{
				MarkAsSeen();
				Relic = null;
				if ((bool)newOrb)
				{
					newOrb.transform.localScale = newOrbInitialScale;
				}
			};
		}
		if ((bool)playingIndicator)
		{
			playingIndicator.gameObject.SetActive(value: false);
		}
	}

	public void SetRelic(CollectableRelic relic, out bool isNew)
	{
		Relic = relic;
		base.gameObject.name = (relic ? relic.name : "null");
		isNew = false;
		if (!relic)
		{
			return;
		}
		CollectableItemRelicType relicType = relic.RelicType;
		if (!relicType)
		{
			return;
		}
		CollectableRelicsData.Data savedData = relic.SavedData;
		if ((bool)currentIconOverride && currentIconOverride.Owner == this)
		{
			currentIconOverride.gameObject.SetActive(value: false);
			currentIconOverride = null;
		}
		iconTransform = (relicIcon ? relicIcon.transform : base.transform);
		if (savedData.IsDeposited)
		{
			if ((bool)relic.RelicType && (bool)relic.RelicType.IconOverridePrefab)
			{
				if (!_spawnedIconOverrides.ContainsKey(relic) || _spawnedIconOverrides[relic] == null)
				{
					CustomInventoryItemCollectableDisplay customInventoryItemCollectableDisplay = UnityEngine.Object.Instantiate(relic.RelicType.IconOverridePrefab, iconTransform);
					customInventoryItemCollectableDisplay.transform.Reset();
					_spawnedIconOverrides[relic] = customInventoryItemCollectableDisplay;
					currentIconOverride = customInventoryItemCollectableDisplay;
				}
				else
				{
					currentIconOverride = _spawnedIconOverrides[relic];
					currentIconOverride.transform.SetParentReset(iconTransform);
					currentIconOverride.gameObject.SetActive(value: true);
				}
			}
			if ((bool)relicIcon)
			{
				relicIcon.Sprite = relicType.GetIcon(CollectableItem.ReadSource.Inventory);
				relicIcon.AlphaSelf = ((!currentIconOverride) ? 1f : 0f);
			}
			isNewOrbVisible = !savedData.HasSeenInRelicBoard;
			if ((bool)emptyNotch)
			{
				emptyNotch.SetActive(value: false);
			}
		}
		else
		{
			if ((bool)relicIcon)
			{
				relicIcon.AlphaSelf = 0f;
			}
			isNewOrbVisible = false;
			if ((bool)emptyNotch)
			{
				emptyNotch.SetActive(value: true);
			}
		}
		if ((bool)newBurst)
		{
			newBurst.SetActive(value: false);
		}
		Board.UpdateRelicItemsIsPlaying();
		if (isNewOrbVisible)
		{
			isNew = true;
			isNewOrbVisible = false;
			iconTransform.gameObject.SetActive(value: false);
			if ((bool)emptyNotch)
			{
				emptyNotch.SetActive(value: true);
			}
		}
		if ((bool)newOrb)
		{
			newOrb.SetActive(isNewOrbVisible);
		}
	}

	public void DoNewAppear()
	{
		iconTransform.gameObject.SetActive(value: true);
		if ((bool)emptyNotch)
		{
			emptyNotch.SetActive(value: false);
		}
		Animator animator = null;
		if ((bool)currentIconOverride)
		{
			animator = currentIconOverride.GetComponent<Animator>();
		}
		else if ((bool)relicIcon)
		{
			animator = relicIcon.GetComponent<Animator>();
		}
		if (animator != null)
		{
			animator.Play("Appear");
		}
		isNewOrbVisible = true;
		if ((bool)newOrb)
		{
			newOrb.SetActive(value: true);
		}
		if ((bool)newBurst)
		{
			newBurst.SetActive(value: true);
		}
		newBurstSound.SpawnAndPlayOneShot(Audio.DefaultUIAudioSourcePrefab, base.transform.position);
		newBurstShake.DoShake(this);
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
		if (!Relic)
		{
			return base.Extra();
		}
		if (Relic.SavedData.IsDeposited && Board.TryPlayRelic(this))
		{
			Board.UpdateRelicItemsIsPlaying();
			return true;
		}
		return base.Extra();
	}

	public override void Select(InventoryItemManager.SelectionDirection? direction)
	{
		base.Select(direction);
		if (isNewOrbVisible)
		{
			isNewOrbVisible = false;
			MarkAsSeen();
			if ((bool)newOrb)
			{
				newOrb.transform.ScaleTo(this, Vector3.zero, UI.NewDotScaleTime, UI.NewDotScaleDelay);
			}
		}
	}

	private void MarkAsSeen()
	{
		if ((bool)Relic)
		{
			CollectableRelicsData.Data savedData = Relic.SavedData;
			if (savedData.IsDeposited && !savedData.HasSeenInRelicBoard)
			{
				savedData.HasSeenInRelicBoard = true;
				Relic.SavedData = savedData;
			}
		}
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
}
