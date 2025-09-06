using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using GlobalSettings;
using TMProOld;
using TeamCherry.NestedFadeGroup;
using UnityEngine;

[DefaultExecutionOrder(0)]
public class InventoryToolCrestList : InventoryItemSelectableDirectional
{
	[SerializeField]
	private TextMeshPro[] crestNameDisplays;

	[SerializeField]
	private TextMeshPro crestDescriptionDisplay;

	[SerializeField]
	private InventoryItemComboButtonPromptDisplay comboButtonPromptDisplay;

	[Space]
	[SerializeField]
	private InventoryToolCrest templateCrest;

	[SerializeField]
	private float crestSpacing = 4f;

	[SerializeField]
	private float adjacentCrestOffset = 1f;

	[SerializeField]
	private Transform scrollParent;

	[SerializeField]
	private float scrollTime = 0.3f;

	[SerializeField]
	private AudioEvent scrollAudio;

	[SerializeField]
	private BaseAnimator scrollLeftArrow;

	[SerializeField]
	private NestedFadeGroupBase scrollLeftArrowGroup;

	[SerializeField]
	private BaseAnimator scrollRightArrow;

	[SerializeField]
	private NestedFadeGroupBase scrollRightArrowGroup;

	[SerializeField]
	private float arrowFadeTime = 0.3f;

	[SerializeField]
	private AudioEvent changeCrestEnterAudio;

	[SerializeField]
	private AudioEvent changeCrestExitAudio;

	[SerializeField]
	private InventoryItemSelectableButtonEvent changeCrestButton;

	[SerializeField]
	private Animator changeCrestIconAnimator;

	[SerializeField]
	private Vector2 crestModeSwitchOffset;

	[Space]
	[SerializeField]
	private float crestModeSwitchMoveTime = 0.2f;

	[SerializeField]
	private GameObject nudgeIfActive;

	[SerializeField]
	private Vector2 nudgeOffset;

	private readonly List<InventoryToolCrest> crests = new List<InventoryToolCrest>();

	private readonly List<InventoryToolCrest> unlockedCrests = new List<InventoryToolCrest>();

	private Coroutine crestSwitchMoveRoutine;

	private Coroutine crestSwitchSequenceRoutine;

	private Coroutine scrollRoutine;

	private Action onScrollEnd;

	private Vector2 initialPosition;

	private bool wasChangeCrestButtonPressed;

	private InventoryPaneBase pane;

	private InventoryPaneInput paneInput;

	private InventoryItemToolManager manager;

	private InputHandler inputHandler;

	private bool queuedPaneEnded;

	private bool isWaitingForApply;

	private InventoryToolCrest previousSelectedCrest;

	private InventoryToolCrest previousEquippedCrest;

	private static readonly int _failed = Animator.StringToHash("Failed");

	public bool IsSwitchingCrests { get; private set; }

	public bool IsBlocked { get; set; }

	public bool IsSetupComplete { get; private set; }

	public InventoryToolCrest CurrentCrest { get; private set; }

	public Vector2 HomePosition
	{
		get
		{
			if ((bool)nudgeIfActive && nudgeIfActive.activeInHierarchy)
			{
				return initialPosition + nudgeOffset;
			}
			return initialPosition;
		}
	}

	protected override void Awake()
	{
		base.Awake();
		manager = GetComponentInParent<InventoryItemToolManager>();
		pane = GetComponentInParent<InventoryPaneBase>();
		paneInput = GetComponentInParent<InventoryPaneInput>();
		if ((bool)pane)
		{
			pane.OnPaneEnd += delegate
			{
				queuedPaneEnded = true;
				IsSwitchingCrests = false;
				isWaitingForApply = false;
			};
			pane.OnPaneStart += Setup;
			pane.OnInputLeft += delegate
			{
				SwitchSelectedCrest(-1);
			};
			pane.OnInputRight += delegate
			{
				SwitchSelectedCrest(1);
			};
		}
		initialPosition = base.transform.localPosition;
		inputHandler = GameManager.instance.inputHandler;
		Setup();
		if ((bool)changeCrestButton)
		{
			InventoryItemSelectableButtonEvent inventoryItemSelectableButtonEvent = changeCrestButton;
			inventoryItemSelectableButtonEvent.ButtonActivated = (Action)Delegate.Combine(inventoryItemSelectableButtonEvent.ButtonActivated, (Action)delegate
			{
				wasChangeCrestButtonPressed = true;
			});
		}
	}

	private void Update()
	{
		if (IsBlocked || !pane || !manager || manager.IsActionsBlocked || isWaitingForApply || !paneInput || !paneInput.enabled)
		{
			return;
		}
		HeroActions inputActions = inputHandler.inputActions;
		Platform.MenuActions menuAction = Platform.Current.GetMenuAction(inputActions);
		switch (manager.EquipState)
		{
		case InventoryItemToolManager.EquipStates.None:
			if ((menuAction != Platform.MenuActions.Super && !wasChangeCrestButtonPressed) || !pane.IsPaneActive || !CanChangeCrests())
			{
				break;
			}
			if ((bool)CurrentCrest && CurrentCrest.IsHidden)
			{
				if ((bool)changeCrestIconAnimator)
				{
					changeCrestIconAnimator.SetTrigger(_failed);
				}
				if (manager.ShowingCursedMsg)
				{
					manager.HideCursedMsg();
				}
				else
				{
					manager.ShowCursedMsg(isCrestEquip: true, ToolItemType.Red);
				}
			}
			else if (manager.BeginSwitchingCrest())
			{
				StartSwitchingCrests();
			}
			else if (CanApplyCrest() && manager.EndSwitchingCrest())
			{
				StopSwitchingCrests(keepNewSelection: true);
			}
			break;
		case InventoryItemToolManager.EquipStates.SwitchCrest:
			if (((menuAction != Platform.MenuActions.Cancel && menuAction != Platform.MenuActions.Submit && !InventoryPaneInput.IsInventoryButtonPressed(inputActions)) || !pane.IsPaneActive) && !queuedPaneEnded)
			{
				break;
			}
			if (queuedPaneEnded || menuAction == Platform.MenuActions.Cancel)
			{
				if (manager.EndSwitchingCrest())
				{
					StopSwitchingCrests(keepNewSelection: false);
				}
			}
			else if (CanApplyCrest())
			{
				isWaitingForApply = true;
				if (CurrentCrest != previousEquippedCrest)
				{
					CurrentCrest.DoEquip(ApplyCurrentCrest);
					break;
				}
				ApplyCurrentCrest();
				changeCrestExitAudio.SpawnAndPlayOneShot(Audio.DefaultUIAudioSourcePrefab, base.transform.position);
			}
			break;
		}
		queuedPaneEnded = false;
		wasChangeCrestButtonPressed = false;
	}

	private void ApplyCurrentCrest()
	{
		isWaitingForApply = false;
		if (manager.EndSwitchingCrest())
		{
			StopSwitchingCrests(keepNewSelection: true);
			manager.OnAppliedCrest();
		}
	}

	private bool CanApplyCrest()
	{
		int num;
		if (!(CurrentCrest == previousEquippedCrest))
		{
			num = (manager.CanChangeEquips() ? 1 : 0);
			if (num == 0 && manager.EquipState == InventoryItemToolManager.EquipStates.SwitchCrest)
			{
				if (manager.ShowingCrestMsg)
				{
					manager.HideCrestEquipMsg();
					return (byte)num != 0;
				}
				manager.ShowCrestEquipMsg();
			}
		}
		else
		{
			num = 1;
		}
		return (byte)num != 0;
	}

	private void SetupCrests()
	{
		if ((bool)templateCrest)
		{
			templateCrest.gameObject.SetActive(value: true);
			List<ToolCrest> allCrests = ToolItemManager.GetAllCrests();
			for (int num = allCrests.Count - crests.Count; num > 0; num--)
			{
				InventoryToolCrest item = UnityEngine.Object.Instantiate(templateCrest, templateCrest.transform.parent);
				crests.Add(item);
			}
			for (int i = 0; i < crests.Count; i++)
			{
				InventoryItemManager.PropagateSelectables(this, crests[i]);
				crests[i].Setup((i < allCrests.Count) ? allCrests[i] : null);
			}
			templateCrest.gameObject.SetActive(value: false);
			if ((bool)changeCrestButton)
			{
				changeCrestButton.gameObject.SetActive(CanChangeCrests());
			}
		}
	}

	public bool CanChangeCrests()
	{
		return crests.Count((InventoryToolCrest crest) => crest.IsUnlocked) > 1;
	}

	private void Setup()
	{
		IsSetupComplete = false;
		base.transform.SetLocalPosition2D(HomePosition);
		SetupCrests();
		foreach (InventoryToolCrest crest in crests)
		{
			crest.GetEquippedForSlots();
		}
		SetupUnlockedCrests();
		InventoryToolCrest inventoryToolCrest = null;
		string currentCrestId = GameManager.instance.playerData.CurrentCrestID;
		if (!string.IsNullOrEmpty(currentCrestId))
		{
			inventoryToolCrest = crests.FirstOrDefault((InventoryToolCrest c) => c.gameObject.name == currentCrestId);
		}
		if (!inventoryToolCrest && crests.Count > 0)
		{
			inventoryToolCrest = crests[0];
		}
		if ((bool)inventoryToolCrest)
		{
			SetCurrentCrest(inventoryToolCrest, doScroll: false, doSave: false);
			foreach (InventoryToolCrest crest2 in crests)
			{
				crest2.UpdateListDisplay(isInstant: true);
				crest2.Show(crest2 == CurrentCrest, isInstant: true);
			}
		}
		UpdateEnabledCrests(setAllEnabled: false);
		if ((bool)manager)
		{
			manager.RefreshTools();
		}
		IsSetupComplete = true;
	}

	public override InventoryItemSelectable Get(InventoryItemManager.SelectionDirection? direction)
	{
		List<InventoryToolCrest> list = crests.Where((InventoryToolCrest crest) => crest.gameObject.activeSelf).ToList();
		if ((bool)CurrentCrest)
		{
			return CurrentCrest.Get(direction);
		}
		if (list.Count > 0)
		{
			return list[0].Get(direction);
		}
		return base.Get(direction);
	}

	public bool CrestHasSlot(ToolItemType type)
	{
		if ((bool)CurrentCrest)
		{
			return CurrentCrest.HasSlot(type);
		}
		return false;
	}

	public bool CrestHasAnySlots()
	{
		if ((bool)CurrentCrest)
		{
			return CurrentCrest.HasAnySlots();
		}
		return false;
	}

	public InventoryToolCrestSlot GetEquippedToolSlot(ToolItem itemData)
	{
		if ((bool)CurrentCrest)
		{
			return CurrentCrest.GetEquippedToolSlot(itemData);
		}
		return null;
	}

	public IEnumerable<InventoryToolCrestSlot> GetSlots()
	{
		if ((bool)CurrentCrest)
		{
			return CurrentCrest.GetSlots();
		}
		return Enumerable.Empty<InventoryToolCrestSlot>();
	}

	private void SetCurrentCrest(InventoryToolCrest crest, bool doScroll, bool doSave)
	{
		previousSelectedCrest = CurrentCrest;
		CurrentCrest = crest;
		if ((bool)previousSelectedCrest)
		{
			previousSelectedCrest.UpdateListDisplay(!doScroll);
		}
		if ((bool)CurrentCrest)
		{
			CurrentCrest.UpdateListDisplay(!doScroll);
		}
		ScrollToCrest(crest, doScroll ? scrollTime : 0f);
		if (Application.isPlaying)
		{
			if (doSave)
			{
				ToolItemManager.SetEquippedCrest(crest.gameObject.name);
			}
			if ((bool)manager)
			{
				manager.RefreshTools(isInstant: true, updateCrest: false);
			}
			TextMeshPro[] array = crestNameDisplays;
			foreach (TextMeshPro textMeshPro in array)
			{
				if ((bool)textMeshPro)
				{
					textMeshPro.text = crest.DisplayName;
				}
			}
			if ((bool)crestDescriptionDisplay)
			{
				crestDescriptionDisplay.text = crest.Description;
			}
			ToolCrest crestData = crest.CrestData;
			if (crestData.HasCustomAction)
			{
				comboButtonPromptDisplay.Show(crestData.CustomButtonCombo);
			}
			else
			{
				comboButtonPromptDisplay.Hide();
			}
		}
		if (CurrentCrest == previousSelectedCrest)
		{
			return;
		}
		if ((bool)CurrentCrest)
		{
			foreach (InventoryToolCrestSlot slot in CurrentCrest.GetSlots())
			{
				slot.SetIsVisible(isVisible: true);
			}
		}
		if (!previousSelectedCrest)
		{
			return;
		}
		foreach (InventoryToolCrestSlot slot2 in previousSelectedCrest.GetSlots())
		{
			slot2.SetIsVisible(isVisible: false);
		}
	}

	private void SetupUnlockedCrests()
	{
		unlockedCrests.Clear();
		foreach (InventoryToolCrest crest in crests)
		{
			if (crest.IsUnlocked)
			{
				unlockedCrests.Add(crest);
			}
		}
	}

	private void UpdateEnabledCrests(bool setAllEnabled)
	{
		foreach (InventoryToolCrest crest in crests)
		{
			crest.gameObject.SetActive(setAllEnabled || crest == CurrentCrest);
		}
	}

	private void StartSwitchingCrests()
	{
		IsSwitchingCrests = true;
		previousEquippedCrest = CurrentCrest;
		UpdateEnabledCrests(setAllEnabled: true);
		scrollLeftArrowGroup.AlphaSelf = 0f;
		scrollRightArrowGroup.AlphaSelf = 0f;
		CurrentCrest.UpdateListDisplay();
		ScrollToCrest(CurrentCrest, 0f);
		foreach (InventoryToolCrestSlot slot in CurrentCrest.GetSlots())
		{
			slot.Deselect();
		}
		changeCrestEnterAudio.SpawnAndPlayOneShot(Audio.DefaultUIAudioSourcePrefab, base.transform.position);
		if (crestSwitchSequenceRoutine != null)
		{
			StopCoroutine(crestSwitchSequenceRoutine);
		}
		crestSwitchSequenceRoutine = StartCoroutine(ModeSwitchSequence(isSwitching: true));
	}

	public void StopSwitchingCrests(bool keepNewSelection)
	{
		if (IsSwitchingCrests)
		{
			if (keepNewSelection)
			{
				SetCurrentCrest(CurrentCrest, doScroll: true, doSave: true);
			}
			else
			{
				SetCurrentCrest(previousEquippedCrest, doScroll: false, doSave: true);
				changeCrestExitAudio.SpawnAndPlayOneShot(Audio.DefaultUIAudioSourcePrefab, base.transform.position);
			}
		}
		IsSwitchingCrests = false;
		previousEquippedCrest = null;
		if (crestSwitchSequenceRoutine != null)
		{
			StopCoroutine(crestSwitchSequenceRoutine);
		}
		crestSwitchSequenceRoutine = StartCoroutine(ModeSwitchSequence(isSwitching: false));
	}

	private IEnumerator ModeSwitchSequence(bool isSwitching)
	{
		if (crestSwitchMoveRoutine != null)
		{
			StopCoroutine(crestSwitchMoveRoutine);
		}
		if (isSwitching)
		{
			yield return new WaitForSecondsRealtime(manager.FadeToolGroup(fadeIn: false));
			manager.RefreshTools();
			crestSwitchMoveRoutine = StartCoroutine(CrestListMove(initialPosition + crestModeSwitchOffset));
			yield return crestSwitchMoveRoutine;
			foreach (InventoryToolCrest unlockedCrest in unlockedCrests)
			{
				if (unlockedCrest != CurrentCrest)
				{
					unlockedCrest.Show(value: true, isInstant: false);
				}
			}
			manager.FadeCrestGroup(fadeIn: true);
		}
		else
		{
			float num = manager.FadeCrestGroup(fadeIn: false);
			foreach (InventoryToolCrest unlockedCrest2 in unlockedCrests)
			{
				if (unlockedCrest2 != CurrentCrest)
				{
					num = Mathf.Max(num, unlockedCrest2.Show(value: false, isInstant: false));
				}
			}
			yield return new WaitForSecondsRealtime(num);
			UpdateEnabledCrests(setAllEnabled: false);
			CurrentCrest.GetEquippedForSlots();
			crestSwitchMoveRoutine = StartCoroutine(CrestListMove(HomePosition));
			yield return crestSwitchMoveRoutine;
			manager.FadeToolGroup(fadeIn: true);
			manager.RefreshTools();
		}
		if ((bool)CurrentCrest)
		{
			CurrentCrest.UpdateListDisplay();
		}
		crestSwitchSequenceRoutine = null;
	}

	public void PaneMovePrevented()
	{
		HeroActions inputActions = ManagerSingleton<InputHandler>.Instance.inputActions;
		if (inputActions.PaneLeft.IsPressed)
		{
			SwitchSelectedCrest(-1);
		}
		else if (inputActions.PaneRight.IsPressed)
		{
			SwitchSelectedCrest(1);
		}
	}

	private void SwitchSelectedCrest(int direction)
	{
		if (!IsSwitchingCrests || direction == 0 || isWaitingForApply || crestSwitchSequenceRoutine != null)
		{
			return;
		}
		manager.HideCrestEquipMsg(force: true);
		direction = (int)Mathf.Sign(direction);
		int num = unlockedCrests.IndexOf(CurrentCrest);
		num += direction;
		if (num >= 0 && num < unlockedCrests.Count)
		{
			BaseAnimator baseAnimator = ((direction > 0) ? scrollRightArrow : scrollLeftArrow);
			if ((bool)baseAnimator)
			{
				baseAnimator.StartAnimation();
			}
			SetCurrentCrest(unlockedCrests[num], doScroll: true, doSave: false);
		}
	}

	private void ScrollToCrest(InventoryToolCrest crest, float duration)
	{
		if (scrollRoutine != null)
		{
			StopCoroutine(scrollRoutine);
			scrollRoutine = null;
		}
		if (onScrollEnd != null)
		{
			onScrollEnd();
		}
		if (base.isActiveAndEnabled)
		{
			scrollRoutine = StartCoroutine(ScrollToCrestRoutine(crest, duration));
		}
		else
		{
			ScrollToCrestRoutine(crest, 0f).MoveNext();
		}
	}

	private IEnumerator ScrollToCrestRoutine(InventoryToolCrest crest, float duration)
	{
		UpdateCrestPositions(null, null, 0f);
		float x = 0f - crest.transform.localPosition.x;
		Vector3 targetPosition;
		Vector3 localPosition = (targetPosition = scrollParent.localPosition);
		targetPosition.x = x;
		int? previousCrestIndex = null;
		if ((bool)previousSelectedCrest)
		{
			previousCrestIndex = unlockedCrests.IndexOf(previousSelectedCrest);
		}
		int currentCrestIndex = unlockedCrests.IndexOf(CurrentCrest);
		if (duration > 0f && previousCrestIndex.HasValue && previousCrestIndex != currentCrestIndex)
		{
			scrollAudio.SpawnAndPlayOneShot(Audio.DefaultUIAudioSourcePrefab, base.transform.position);
		}
		scrollLeftArrowGroup.FadeTo((currentCrestIndex > 0) ? 1 : 0, arrowFadeTime, null, isRealtime: true);
		scrollRightArrowGroup.FadeTo((currentCrestIndex < unlockedCrests.Count - 1) ? 1 : 0, arrowFadeTime, null, isRealtime: true);
		onScrollEnd = delegate
		{
			scrollParent.localPosition = targetPosition;
			UpdateCrestPositions(previousCrestIndex, currentCrestIndex, 1f);
			onScrollEnd = null;
		};
		for (float elapsed = 0f; elapsed < duration; elapsed += Time.unscaledDeltaTime)
		{
			float num = elapsed / duration;
			scrollParent.localPosition = Vector3.Lerp(localPosition, targetPosition, num);
			UpdateCrestPositions(previousCrestIndex, currentCrestIndex, num);
			yield return null;
		}
		onScrollEnd();
	}

	private IEnumerator CrestListMove(Vector2 toPosition)
	{
		Vector2 fromPosition = base.transform.localPosition;
		for (float elapsed = 0f; elapsed < crestModeSwitchMoveTime; elapsed += Time.unscaledDeltaTime)
		{
			base.transform.SetLocalPosition2D(Vector2.Lerp(fromPosition, toPosition, elapsed / crestModeSwitchMoveTime));
			yield return null;
		}
		base.transform.SetLocalPosition2D(toPosition);
	}

	private void UpdateCrestPositions(int? previousCrestIndex, int? currentCrestIndex, float blend)
	{
		for (int i = 0; i < unlockedCrests.Count; i++)
		{
			float b = 0f;
			float a = 0f;
			if (i == currentCrestIndex + 1)
			{
				b = 1f;
			}
			else if (i == currentCrestIndex - 1)
			{
				b = -1f;
			}
			if (i == previousCrestIndex + 1)
			{
				a = 1f;
			}
			else if (i == previousCrestIndex - 1)
			{
				a = -1f;
			}
			float num = Mathf.Lerp(a, b, blend);
			unlockedCrests[i].transform.SetLocalPositionX(crestSpacing * (float)i + num * adjacentCrestOffset);
		}
	}

	private void OnDrawGizmosSelected()
	{
		Gizmos.matrix = base.transform.localToWorldMatrix;
		Gizmos.DrawWireSphere(crestModeSwitchOffset, 0.25f);
	}
}
