using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GlobalEnums;
using GlobalSettings;
using TMProOld;
using TeamCherry.Localization;
using TeamCherry.NestedFadeGroup;
using TeamCherry.SharedUtils;
using UnityEngine;

public class InventoryItemCollectable : InventoryItemUpdateable
{
	private const float CONSUME_HOLD_DURATION = 0.5f;

	private const float CONSUME_HOLD_END_PAUSE = 0.3f;

	private const float CONSUME_LAST_PAUSE = 0.5f;

	private const float CLOSE_CONSUME_HOLD_DURATION = 1.5f;

	private const float CLOSE_CONSUME_PAUSE = 0.5f;

	[Space]
	[SerializeField]
	private NestedFadeGroup group;

	[SerializeField]
	private SpriteRenderer spriteRenderer;

	[SerializeField]
	private TextMeshPro amountText;

	[SerializeField]
	private Color regularAmountTextColor;

	[SerializeField]
	private Color maxAmountTextColor;

	[Space]
	[SerializeField]
	private CollectableItem item;

	[SerializeField]
	private bool forceShowAmount;

	[Space]
	[SerializeField]
	private float breakFadeUpTime;

	[SerializeField]
	private float breakFadeUpScale = 0.5f;

	[SerializeField]
	private CaptureAnimationEvent consumeEffect;

	[SerializeField]
	private AudioSource audioPlayerPrefab;

	[SerializeField]
	private GameObject consumePrompt;

	[Space]
	[SerializeField]
	private InventoryItemButtonPromptDisplayList buttonPromptDisplay;

	[SerializeField]
	private LocalisedString consumeItemUsePrompt;

	[SerializeField]
	private LocalisedString consumeItemResponse;

	[SerializeField]
	private MinMaxFloat consumeShakeMagnitude;

	[SerializeField]
	private RandomAudioClipTable failedAudioTable;

	[SerializeField]
	private Animator failedAnimator;

	[Space]
	[SerializeField]
	private VibrationDataAsset consumeRumble;

	[SerializeField]
	private VibrationDataAsset consumeFinalShake;

	private static readonly Dictionary<CollectableItem, CustomInventoryItemCollectableDisplay> _spawnedCustomDisplays = new Dictionary<CollectableItem, CustomInventoryItemCollectableDisplay>();

	private CustomInventoryItemCollectableDisplay currentCustomDisplay;

	private Vector3 initialPosition;

	private Vector3 initialScale;

	private bool consumePromptIsVisible;

	private Vector3 consumePromptInitialScale;

	private readonly int failedAnimId = Animator.StringToHash("Failed");

	private Coroutine consumeRoutine;

	private bool isSelected;

	private bool isConsumePressed;

	private bool isPendingCloseUsage;

	private float consumeFadeUpDelay;

	private AudioSource spawnedUsePlayer;

	private GameObject spawnedExtraConsumeEffect;

	private InventoryItemExtraDescription extraDesc;

	private InventoryItemCollectableManager manager;

	private InventoryPaneList paneList;

	private VibrationEmission consumeRumbleEmission;

	public override string DisplayName
	{
		get
		{
			if (!item)
			{
				return string.Empty;
			}
			return item.GetDisplayName(CollectableItem.ReadSource.Inventory);
		}
	}

	public override string Description
	{
		get
		{
			if (!item)
			{
				return string.Empty;
			}
			string description = item.GetDescription(CollectableItem.ReadSource.Inventory);
			string[] useResponseDescriptions = item.GetUseResponseDescriptions();
			List<FullQuestBase> list = null;
			foreach (FullQuestBase activeQuest in QuestManager.GetActiveQuests())
			{
				if (activeQuest.InvItemAppendDesc.IsEmpty)
				{
					continue;
				}
				foreach (FullQuestBase.QuestTarget target in activeQuest.Targets)
				{
					if (!(target.Counter != item))
					{
						if (list == null)
						{
							list = new List<FullQuestBase>();
						}
						list.Add(activeQuest);
						break;
					}
				}
			}
			DeliveryQuestItemStandalone deliveryQuestItemStandalone = item as DeliveryQuestItemStandalone;
			if (useResponseDescriptions.Length == 0 && list == null && (!deliveryQuestItemStandalone || deliveryQuestItemStandalone.InvItemAppendDesc.IsEmpty))
			{
				return description;
			}
			StringBuilder tempStringBuilder = Helper.GetTempStringBuilder(description);
			if (list != null)
			{
				foreach (FullQuestBase item in list)
				{
					tempStringBuilder.AppendLine();
					tempStringBuilder.AppendLine();
					tempStringBuilder.AppendLine(item.InvItemAppendDesc);
				}
			}
			if ((bool)deliveryQuestItemStandalone && !deliveryQuestItemStandalone.InvItemAppendDesc.IsEmpty)
			{
				tempStringBuilder.AppendLine();
				tempStringBuilder.AppendLine();
				tempStringBuilder.AppendLine(deliveryQuestItemStandalone.InvItemAppendDesc);
			}
			string[] array = useResponseDescriptions;
			foreach (string value in array)
			{
				tempStringBuilder.AppendLine();
				tempStringBuilder.AppendLine();
				tempStringBuilder.AppendLine(value);
			}
			return tempStringBuilder.ToString();
		}
	}

	public Transform IconTransform
	{
		get
		{
			if (!spriteRenderer)
			{
				return base.transform;
			}
			return spriteRenderer.transform;
		}
	}

	public CollectableItem Item
	{
		get
		{
			return item;
		}
		set
		{
			item = value;
			UpdateItemDisplay();
		}
	}

	protected override bool IsSeen
	{
		get
		{
			if ((bool)item)
			{
				return item.IsSeen;
			}
			return true;
		}
		set
		{
			if ((bool)item)
			{
				item.IsSeen = value;
			}
		}
	}

	protected override void Awake()
	{
		base.Awake();
		Transform iconTransform = IconTransform;
		initialPosition = iconTransform.localPosition;
		initialScale = iconTransform.localScale;
		manager = GetComponentInParent<InventoryItemCollectableManager>();
		if ((bool)Pane)
		{
			Pane.OnPaneStart += delegate
			{
				if ((bool)currentCustomDisplay)
				{
					currentCustomDisplay.OnPaneStart();
				}
				ResetFadeUp();
			};
			Pane.OnPrePaneEnd += delegate
			{
				if ((bool)currentCustomDisplay)
				{
					currentCustomDisplay.OnPrePaneEnd();
				}
			};
			Pane.OnPaneEnd += delegate
			{
				if ((bool)currentCustomDisplay)
				{
					currentCustomDisplay.OnPaneEnd();
				}
				if ((bool)consumeEffect)
				{
					consumeEffect.gameObject.SetActive(value: false);
				}
				if ((bool)spawnedExtraConsumeEffect)
				{
					spawnedExtraConsumeEffect.Recycle();
					spawnedExtraConsumeEffect = null;
				}
			};
		}
		if ((bool)consumeEffect)
		{
			consumeEffect.gameObject.SetActive(value: false);
		}
		paneList = GetComponentInParent<InventoryPaneList>();
		extraDesc = GetComponent<InventoryItemExtraDescription>();
		if ((bool)extraDesc)
		{
			extraDesc.ActivatedDesc += delegate(GameObject obj)
			{
				if ((bool)item)
				{
					item.SetupExtraDescription(obj);
				}
			};
		}
		if ((bool)consumePrompt)
		{
			consumePrompt.SetActive(value: false);
			consumePromptInitialScale = consumePrompt.transform.localScale;
		}
	}

	protected override void OnDisable()
	{
		base.OnDisable();
		StopConsumeRumble();
	}

	private void Update()
	{
		if (!(consumeFadeUpDelay > 0f))
		{
			return;
		}
		consumeFadeUpDelay -= Time.unscaledDeltaTime;
		if (consumeFadeUpDelay <= 0f)
		{
			if ((bool)group)
			{
				group.FadeTo(1f, breakFadeUpTime, null, isRealtime: true);
			}
			IconTransform.ScaleTo(this, initialScale, breakFadeUpTime, 0f, dontTrack: false, isRealtime: true);
		}
	}

	private void UpdateItemDisplay()
	{
		base.gameObject.name = item.name;
		if ((bool)spriteRenderer)
		{
			spriteRenderer.sprite = (item.CustomInventoryDisplay ? null : item.GetIcon(CollectableItem.ReadSource.Inventory));
		}
		if (consumePrompt != null && (!item.IsConsumable() || !item.CanConsumeRightNow()))
		{
			consumePrompt.SetActive(value: false);
		}
		if ((bool)amountText)
		{
			amountText.text = ((forceShowAmount || item.DisplayAmount) ? item.CollectedAmount.ToString() : string.Empty);
			amountText.color = (item.IsAtMax() ? maxAmountTextColor : regularAmountTextColor);
		}
		if (isSelected)
		{
			HidePromptData(isInstant: true);
			DisplayPromptData();
		}
		if ((bool)currentCustomDisplay)
		{
			if (currentCustomDisplay.Owner == this)
			{
				currentCustomDisplay.gameObject.SetActive(value: false);
			}
			currentCustomDisplay = null;
		}
		if ((bool)item.CustomInventoryDisplay)
		{
			if (!_spawnedCustomDisplays.ContainsKey(item) || _spawnedCustomDisplays[item] == null)
			{
				CustomInventoryItemCollectableDisplay customInventoryItemCollectableDisplay = UnityEngine.Object.Instantiate(item.CustomInventoryDisplay, IconTransform);
				customInventoryItemCollectableDisplay.OnDestroyed += OnInventoryItemDestroyed;
				customInventoryItemCollectableDisplay.transform.Reset();
				_spawnedCustomDisplays[item] = customInventoryItemCollectableDisplay;
				currentCustomDisplay = customInventoryItemCollectableDisplay;
			}
			else
			{
				currentCustomDisplay = _spawnedCustomDisplays[item];
				currentCustomDisplay.transform.SetParentReset(IconTransform);
				currentCustomDisplay.gameObject.SetActive(value: true);
			}
			currentCustomDisplay.transform.localScale = item.CustomInventoryDisplay.transform.localScale;
			currentCustomDisplay.Owner = this;
		}
		if ((bool)extraDesc)
		{
			extraDesc.ExtraDescPrefab = item.ExtraDescriptionSection;
		}
		UpdateDisplay();
	}

	private static void OnInventoryItemDestroyed(CustomInventoryItemCollectableDisplay display)
	{
		display.OnDestroyed -= OnInventoryItemDestroyed;
		KeyValuePair<CollectableItem, CustomInventoryItemCollectableDisplay>[] array = _spawnedCustomDisplays.Where((KeyValuePair<CollectableItem, CustomInventoryItemCollectableDisplay> kvp) => kvp.Value == display).ToArray();
		foreach (KeyValuePair<CollectableItem, CustomInventoryItemCollectableDisplay> keyValuePair in array)
		{
			_spawnedCustomDisplays.Remove(keyValuePair.Key);
		}
	}

	private void DisplayPromptData()
	{
		if (!buttonPromptDisplay)
		{
			return;
		}
		InventoryItemButtonPromptData[] array = item.GetButtonPromptData();
		if ((array == null || array.Length == 0) && item.CanConsumeRightNow())
		{
			array = new InventoryItemButtonPromptData[1]
			{
				new InventoryItemButtonPromptData
				{
					Action = HeroActionButton.JUMP,
					UseText = consumeItemUsePrompt,
					ResponseText = ((!item.UseResponseTextOverride.IsEmpty) ? item.UseResponseTextOverride : consumeItemResponse),
					IsMenuButton = true
				}
			};
		}
		if (array != null)
		{
			bool forceDisabled = item.IsConsumable() && !item.CanConsumeRightNow();
			InventoryItemButtonPromptData[] array2 = array;
			foreach (InventoryItemButtonPromptData promptData in array2)
			{
				buttonPromptDisplay.Append(promptData, forceDisabled);
			}
		}
		else
		{
			buttonPromptDisplay.Clear();
		}
	}

	private void ShowConsumePrompt()
	{
		if ((bool)consumePrompt)
		{
			if (!consumePromptIsVisible)
			{
				consumePrompt.SetActive(item.IsConsumable() && item.CanConsumeRightNow());
				consumePrompt.transform.localScale = new Vector3(0f, 0f, 1f);
				consumePromptIsVisible = true;
			}
			consumePrompt.transform.ScaleTo(this, consumePromptInitialScale, 0.1f, 0.1f, dontTrack: false, isRealtime: true);
			consumePrompt.SendMessage("StopJitter", SendMessageOptions.DontRequireReceiver);
		}
	}

	private void HideConsumePrompt(bool isInstant)
	{
		if (!consumePrompt || (!isInstant && !consumePromptIsVisible) || (!isInstant && manager.NextSelected == this))
		{
			return;
		}
		if (isInstant)
		{
			consumePrompt.SetActive(value: false);
		}
		else
		{
			consumePrompt.transform.ScaleTo(this, new Vector3(0f, 0f, 1f), 0.05f, 0f, dontTrack: false, isRealtime: true, delegate
			{
				consumePrompt.SetActive(value: false);
			});
		}
		consumePromptIsVisible = false;
	}

	private void HidePromptData(bool isInstant)
	{
		HideConsumePrompt(isInstant);
		if ((bool)buttonPromptDisplay)
		{
			buttonPromptDisplay.Clear();
		}
	}

	public override void Select(InventoryItemManager.SelectionDirection? direction)
	{
		base.Select(direction);
		isSelected = true;
		DisplayPromptData();
		ShowConsumePrompt();
		if ((bool)currentCustomDisplay)
		{
			currentCustomDisplay.OnSelect();
		}
	}

	public override void Deselect()
	{
		base.Deselect();
		isSelected = false;
		HidePromptData(isInstant: false);
		if ((bool)currentCustomDisplay)
		{
			currentCustomDisplay.OnDeselect();
		}
	}

	public override bool Submit()
	{
		isConsumePressed = true;
		if (!item.IsConsumable())
		{
			return false;
		}
		if (item.CollectedAmount <= 0)
		{
			Debug.LogError("No items left to consume!");
			return false;
		}
		if (manager.ShowingMemoryUseMsg)
		{
			manager.HideMemoryUseMsg();
			return false;
		}
		if (!item.CanConsumeRightNow())
		{
			failedAnimator.Play(failedAnimId);
			failedAudioTable.SpawnAndPlayOneShot(Audio.DefaultUIAudioSourcePrefab, base.transform.position);
			if ((bool)currentCustomDisplay)
			{
				currentCustomDisplay.OnConsumeBlocked();
			}
			if (GameManager.instance.IsMemoryScene())
			{
				manager.ShowMemoryUseMsg();
			}
			return true;
		}
		ResetConsume();
		consumeRoutine = StartCoroutine(ConsumeRoutine());
		return true;
	}

	public override bool SubmitReleased()
	{
		isConsumePressed = false;
		ResetConsume();
		return true;
	}

	public override bool Cancel()
	{
		if (manager.ShowingMemoryUseMsg)
		{
			manager.HideMemoryUseMsg();
		}
		return base.Cancel();
	}

	private void ResetConsume()
	{
		if (consumeRoutine == null)
		{
			return;
		}
		StopCoroutine(consumeRoutine);
		consumeRoutine = null;
		if (isPendingCloseUsage)
		{
			isPendingCloseUsage = false;
			if (item != null)
			{
				ManagerSingleton<HeroChargeEffects>.Instance.DoUseBenchItem(item);
			}
		}
		EndConsume();
	}

	private void EndConsume()
	{
		ConsumeBlock(value: false);
		IconTransform.localPosition = initialPosition;
		if ((bool)spawnedUsePlayer)
		{
			spawnedUsePlayer.Stop();
			spawnedUsePlayer = null;
		}
		if ((bool)currentCustomDisplay)
		{
			currentCustomDisplay.OnConsumeEnd();
		}
		StopConsumeRumble();
	}

	private IEnumerator ConsumeRoutine()
	{
		Transform iconTransform = IconTransform;
		bool closeInventoryConsume = item.ConsumeClosesInventory(extraCondition: true);
		ConsumeBlock(value: true);
		while (isConsumePressed)
		{
			if ((bool)manager)
			{
				manager.IsActionsBlocked = false;
			}
			spawnedUsePlayer = item.UseSounds.SpawnAndPlayOneShot(audioPlayerPrefab, base.transform.position, delegate(AudioSource source)
			{
				if (spawnedUsePlayer == source)
				{
					spawnedUsePlayer = null;
				}
			});
			float jitterMagnitude;
			if ((bool)currentCustomDisplay)
			{
				currentCustomDisplay.OnConsumeStart();
				jitterMagnitude = currentCustomDisplay.JitterMagnitudeMultiplier;
			}
			else
			{
				jitterMagnitude = 1f;
			}
			WaitForSecondsRealtime wait = new WaitForSecondsRealtime(1f / 60f);
			float holdDuration = (item.ConsumeClosesInventory(extraCondition: false) ? 1.5f : 0.5f);
			double beforeWaitTime;
			for (float elapsed = 0f; elapsed < holdDuration; elapsed += (float)(Time.unscaledTimeAsDouble - beforeWaitTime))
			{
				SetConsumeShakeAmount(elapsed / holdDuration, jitterMagnitude);
				beforeWaitTime = Time.unscaledTimeAsDouble;
				yield return wait;
			}
			if ((bool)manager)
			{
				manager.IsActionsBlocked = true;
			}
			if (item.AlwaysPlayInstantUse && item.InstantUseSounds.HasClips())
			{
				spawnedUsePlayer.Stop();
				spawnedUsePlayer = null;
				item.InstantUseSounds.SpawnAndPlayOneShot(audioPlayerPrefab, base.transform.position);
			}
			else
			{
				spawnedUsePlayer = null;
			}
			StopConsumeRumble();
			PlayConsumeFinalShake();
			if ((bool)currentCustomDisplay)
			{
				currentCustomDisplay.OnConsumeComplete();
			}
			iconTransform.localPosition = initialPosition;
			GameObject extraConsumeEffectPrefab = item.ExtraUseEffect;
			if (!closeInventoryConsume)
			{
				item.ConsumeItemResponse();
			}
			else
			{
				isPendingCloseUsage = true;
			}
			if (item.TakeItemOnConsume)
			{
				item.Take(1, showCounter: false);
			}
			UpdateItemDisplay();
			if ((bool)consumeEffect)
			{
				consumeEffect.gameObject.SetActive(value: false);
				consumeEffect.gameObject.SetActive(value: true);
				bool hitTrigger = false;
				Action temp = null;
				temp = delegate
				{
					hitTrigger = true;
					consumeEffect.EventFired -= temp;
				};
				consumeEffect.EventFired += temp;
				while (!hitTrigger)
				{
					yield return null;
				}
			}
			if ((bool)extraConsumeEffectPrefab)
			{
				spawnedExtraConsumeEffect = extraConsumeEffectPrefab.Spawn(base.transform.parent);
				Vector3 position = base.transform.position;
				position.z -= 0.0001f;
				spawnedExtraConsumeEffect.transform.position = position;
			}
			consumeFadeUpDelay = 0f;
			if ((bool)group)
			{
				group.AlphaSelf = 0f;
			}
			IconTransform.ScaleTo(this, new Vector3(breakFadeUpScale, breakFadeUpScale, initialScale.z), 0f, 0f, dontTrack: false, isRealtime: true);
			if (item.CollectedAmount <= 0 || closeInventoryConsume)
			{
				if ((bool)group)
				{
					if (closeInventoryConsume)
					{
						yield return new WaitForSecondsRealtime(0.5f);
					}
					else
					{
						yield return new WaitForSecondsRealtime(0.5f);
						ResetFadeUp();
					}
				}
				else
				{
					yield return new WaitForSecondsRealtime(0.5f);
				}
				HidePromptData(isInstant: true);
				if (closeInventoryConsume)
				{
					ConsumeBlock(value: false);
					EventRegister.SendEvent(EventRegisterEvents.InventoryCancel);
					ManagerSingleton<HeroChargeEffects>.Instance.DoUseBenchItem(item);
					isPendingCloseUsage = false;
					yield break;
				}
				isPendingCloseUsage = false;
				if ((bool)manager)
				{
					manager.SetSelected(null);
					int gridSectionIndex = base.GridSectionIndex;
					int gridItemIndex = base.GridItemIndex;
					manager.UpdateList();
					if ((bool)base.Grid)
					{
						InventoryItemSelectable itemOrFallback = base.Grid.GetItemOrFallback(gridSectionIndex, gridItemIndex);
						manager.SetSelected(itemOrFallback ? itemOrFallback : this, null);
					}
					else
					{
						Debug.LogError("Grid was null!", this);
					}
				}
				ConsumeBlock(value: false);
				HidePromptData(isInstant: true);
				yield break;
			}
			consumeFadeUpDelay = 0.3f - breakFadeUpTime;
			yield return new WaitForSecondsRealtime(0.3f);
			if (item.PreventUseChaining || item.IsConsumeAtMax())
			{
				break;
			}
			HeroActions inputActions = ManagerSingleton<InputHandler>.Instance.inputActions;
			if (Platform.Current.GetMenuAction(inputActions, ignoreAttack: false, isContinuous: true) != Platform.MenuActions.Submit)
			{
				isConsumePressed = false;
			}
		}
		EndConsume();
		consumeRoutine = null;
	}

	private void ConsumeBlock(bool value)
	{
		if (!value && (bool)manager)
		{
			manager.IsActionsBlocked = false;
		}
		if ((bool)paneList)
		{
			paneList.CloseBlocked = value;
		}
		if (item.IsConsumable() && !value)
		{
			ShowConsumePrompt();
			return;
		}
		consumePrompt.transform.ScaleTo(this, new Vector3(consumePromptInitialScale.x * 0.8f, consumePromptInitialScale.y * 0.8f, consumePromptInitialScale.z), 0.05f, 0f, dontTrack: false, isRealtime: true);
		consumePrompt.SendMessage("StartJitter", SendMessageOptions.DontRequireReceiver);
	}

	private void ResetFadeUp()
	{
		consumeFadeUpDelay = 0f;
		if ((bool)group)
		{
			group.AlphaSelf = 1f;
		}
		IconTransform.ScaleTo(this, initialScale, 0f);
	}

	public void SetConsumeShakeAmount(float t, float jitterMagnitude)
	{
		if (t <= Mathf.Epsilon)
		{
			IconTransform.localPosition = initialPosition;
		}
		else
		{
			IconTransform.localPosition = initialPosition + (Vector3)UnityEngine.Random.insideUnitCircle * (consumeShakeMagnitude.GetLerpedValue(t) * jitterMagnitude);
		}
		UpdateConsumeRumble(t);
	}

	public void PlayConsumeEffect()
	{
		if ((bool)consumeEffect)
		{
			consumeEffect.gameObject.SetActive(value: false);
			consumeEffect.gameObject.SetActive(value: true);
			StopConsumeRumble();
			PlayConsumeFinalShake();
		}
	}

	private void UpdateConsumeRumble(float strength)
	{
		if (consumeRumbleEmission == null)
		{
			consumeRumbleEmission = VibrationManager.PlayVibrationClipOneShot(consumeRumble, null, isLooping: true, "", isRealtime: true);
		}
		consumeRumbleEmission?.SetStrength(strength);
	}

	public void StopConsumeRumble()
	{
		consumeRumbleEmission?.Stop();
		consumeRumbleEmission = null;
	}

	private void PlayConsumeFinalShake()
	{
		VibrationManager.PlayVibrationClipOneShot(consumeFinalShake, null, isLooping: false, "", isRealtime: true);
	}
}
