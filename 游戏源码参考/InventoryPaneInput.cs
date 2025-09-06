using System;
using UnityEngine;

public class InventoryPaneInput : MonoBehaviour
{
	private const float INPUT_COOLDOWN = 0.25f;

	private const float DIRECTION_REPEAT_TIME = 0.15f;

	private const float LIST_SCROLL_SPEED_FAST = 0.03f;

	[SerializeField]
	private bool allowHorizontalSelection;

	[SerializeField]
	private bool allowVerticalSelection;

	[SerializeField]
	private bool allowRepeat;

	[SerializeField]
	private bool allowRightStickSpeed;

	[SerializeField]
	private bool allowRepeatSubmit;

	[Space]
	[SerializeField]
	private InventoryPaneList.PaneTypes paneControl = InventoryPaneList.PaneTypes.None;

	private bool wasSubmitPressed;

	private bool wasExtraPressed;

	private float actionCooldown;

	private float directionRepeatTimer;

	private bool isRepeatingDirection;

	private InventoryPaneBase.InputEventType lastPressedDirection;

	private bool isScrollingFast;

	private bool isRepeatingSubmit;

	private InputHandler ih;

	private Platform platform;

	private InventoryPaneBase pane;

	private InventoryPaneList paneList;

	private VibrationDataAsset menuSubmitVibration;

	private VibrationDataAsset menuCancelVibration;

	private bool isInInventory;

	public float ListScrollSpeed
	{
		get
		{
			if (!isScrollingFast)
			{
				return 0.15f;
			}
			return 0.03f;
		}
	}

	public static bool IsInputBlocked
	{
		get
		{
			if (StaticVariableList.Exists("IsUIListInputBlocked"))
			{
				return StaticVariableList.GetValue<bool>("IsUIListInputBlocked");
			}
			return false;
		}
		set
		{
			StaticVariableList.SetValue("IsUIListInputBlocked", value);
		}
	}

	public event Action OnActivated;

	public event Action OnDeactivated;

	private void Awake()
	{
		pane = GetComponent<InventoryPaneBase>();
		paneList = GetComponentInParent<InventoryPaneList>();
		isInInventory = paneList != null;
	}

	private void OnEnable()
	{
		wasSubmitPressed = false;
		wasExtraPressed = false;
		actionCooldown = 0f;
		directionRepeatTimer = 0f;
		isRepeatingDirection = false;
		isScrollingFast = false;
		UIManager instance = UIManager.instance;
		if ((bool)instance)
		{
			menuSubmitVibration = instance.menuSubmitVibration;
			menuCancelVibration = instance.menuCancelVibration;
		}
		this.OnActivated?.Invoke();
	}

	private void OnDisable()
	{
		this.OnDeactivated?.Invoke();
	}

	private void Start()
	{
		ih = ManagerSingleton<InputHandler>.Instance;
		platform = Platform.Current;
	}

	private void Update()
	{
		if (actionCooldown > 0f)
		{
			actionCooldown -= Time.unscaledDeltaTime;
		}
		if (IsInputBlocked || CheatManager.IsOpen)
		{
			return;
		}
		HeroActions inputActions = ih.inputActions;
		switch (platform.GetMenuAction(inputActions))
		{
		case Platform.MenuActions.Submit:
			PressSubmit();
			return;
		case Platform.MenuActions.Cancel:
			PressCancel();
			return;
		case Platform.MenuActions.Extra:
			if (actionCooldown <= 0f)
			{
				FSMUtility.SendEventToGameObject(base.gameObject, "UI EXTRA");
				wasExtraPressed = true;
				isRepeatingDirection = false;
				actionCooldown = 0.25f;
			}
			return;
		case Platform.MenuActions.Super:
			if (actionCooldown <= 0f)
			{
				FSMUtility.SendEventToGameObject(base.gameObject, "UI SUPER");
				isRepeatingDirection = false;
				actionCooldown = 0.25f;
			}
			return;
		}
		Platform.MenuActions menuAction = platform.GetMenuAction(inputActions, ignoreAttack: false, isContinuous: true);
		InventoryPaneList.PaneTypes inventoryInputPressed = GetInventoryInputPressed(inputActions);
		if (wasSubmitPressed && menuAction != Platform.MenuActions.Submit)
		{
			ReleaseSubmit();
		}
		if (wasExtraPressed && menuAction != Platform.MenuActions.Extra)
		{
			FSMUtility.SendEventToGameObject(base.gameObject, "UI EXTRA RELEASED");
			wasExtraPressed = false;
			isRepeatingDirection = false;
		}
		else if (inputActions.Right.WasPressed)
		{
			PressDirection(InventoryPaneBase.InputEventType.Right);
		}
		else if (inputActions.Left.WasPressed)
		{
			PressDirection(InventoryPaneBase.InputEventType.Left);
		}
		else if (inputActions.Up.WasPressed)
		{
			PressDirection(InventoryPaneBase.InputEventType.Up);
		}
		else if (inputActions.Down.WasPressed)
		{
			PressDirection(InventoryPaneBase.InputEventType.Down);
		}
		else if (inventoryInputPressed != InventoryPaneList.PaneTypes.None)
		{
			bool flag = paneControl switch
			{
				InventoryPaneList.PaneTypes.None => true, 
				InventoryPaneList.PaneTypes.Inv => inputActions.OpenInventory.WasPressed, 
				InventoryPaneList.PaneTypes.Map => inputActions.OpenInventoryMap.WasPressed || inputActions.SwipeInventoryMap.WasPressed, 
				InventoryPaneList.PaneTypes.Journal => inputActions.OpenInventoryJournal.WasPressed || inputActions.SwipeInventoryJournal.WasPressed, 
				InventoryPaneList.PaneTypes.Tools => inputActions.OpenInventoryTools.WasPressed || inputActions.SwipeInventoryTools.WasPressed, 
				InventoryPaneList.PaneTypes.Quests => inputActions.OpenInventoryQuests.WasPressed || (bool)inputActions.SwipeInventoryQuests, 
				_ => throw new ArgumentOutOfRangeException(), 
			};
			if (!flag)
			{
				InventoryPane inventoryPane = paneList.GetPane(inventoryInputPressed);
				if (inventoryPane == null || !inventoryPane.IsAvailable)
				{
					flag = true;
				}
			}
			if (flag)
			{
				PressCancel();
				return;
			}
			PlayMakerFSM playMakerFSM = PlayMakerFSM.FindFsmOnGameObject(paneList.gameObject, "Inventory Control");
			playMakerFSM.FsmVariables.FindFsmInt("Target Pane Index").Value = (int)inventoryInputPressed;
			playMakerFSM.SendEvent("MOVE PANE TO");
		}
		else if (inputActions.RsDown.WasPressed)
		{
			FSMUtility.SendEventToGameObject(base.gameObject, "UI RS DOWN");
			if (allowRightStickSpeed)
			{
				PressDirection(InventoryPaneBase.InputEventType.Down);
				isScrollingFast = true;
				directionRepeatTimer = ListScrollSpeed;
			}
		}
		else if (inputActions.RsUp.WasPressed)
		{
			FSMUtility.SendEventToGameObject(base.gameObject, "UI RS UP");
			if (allowRightStickSpeed)
			{
				PressDirection(InventoryPaneBase.InputEventType.Up);
				isScrollingFast = true;
				directionRepeatTimer = ListScrollSpeed;
			}
		}
		else if (inputActions.RsLeft.WasPressed)
		{
			if (isInInventory)
			{
				FSMUtility.SendEventToGameObject(base.gameObject, "UI RS LEFT");
			}
			if (allowRightStickSpeed)
			{
				PressDirection(InventoryPaneBase.InputEventType.Left);
				isScrollingFast = true;
				directionRepeatTimer = ListScrollSpeed;
			}
		}
		else if (inputActions.RsRight.WasPressed)
		{
			if (isInInventory)
			{
				FSMUtility.SendEventToGameObject(base.gameObject, "UI RS RIGHT");
			}
			if (allowRightStickSpeed)
			{
				PressDirection(InventoryPaneBase.InputEventType.Right);
				isScrollingFast = true;
				directionRepeatTimer = ListScrollSpeed;
			}
		}
		else if (isRepeatingDirection)
		{
			if (lastPressedDirection switch
			{
				InventoryPaneBase.InputEventType.Left => inputActions.Left.IsPressed, 
				InventoryPaneBase.InputEventType.Right => inputActions.Right.IsPressed, 
				InventoryPaneBase.InputEventType.Up => isScrollingFast ? inputActions.RsUp.IsPressed : inputActions.Up.IsPressed, 
				InventoryPaneBase.InputEventType.Down => isScrollingFast ? inputActions.RsDown.IsPressed : inputActions.Down.IsPressed, 
				_ => throw new ArgumentOutOfRangeException(), 
			})
			{
				directionRepeatTimer -= Time.unscaledDeltaTime;
				if (directionRepeatTimer <= 0f)
				{
					PressDirection(lastPressedDirection);
					directionRepeatTimer = ListScrollSpeed;
				}
			}
			else
			{
				isRepeatingDirection = false;
			}
		}
		else if (isRepeatingSubmit)
		{
			directionRepeatTimer -= Time.unscaledDeltaTime;
			if (directionRepeatTimer <= 0f)
			{
				ReleaseSubmit();
				PressSubmit();
				directionRepeatTimer = ListScrollSpeed;
			}
		}
		else
		{
			isScrollingFast = false;
		}
	}

	public static InventoryPaneList.PaneTypes GetInventoryButtonPressed(HeroActions ia)
	{
		InventoryPaneList.PaneTypes result = InventoryPaneList.PaneTypes.None;
		if (ia.OpenInventory.WasPressed)
		{
			result = InventoryPaneList.PaneTypes.Inv;
		}
		else if (ia.OpenInventoryMap.WasPressed)
		{
			result = InventoryPaneList.PaneTypes.Map;
		}
		else if (ia.OpenInventoryJournal.WasPressed)
		{
			result = InventoryPaneList.PaneTypes.Journal;
		}
		else if (ia.OpenInventoryTools.WasPressed)
		{
			result = InventoryPaneList.PaneTypes.Tools;
		}
		else if (ia.OpenInventoryQuests.WasPressed)
		{
			result = InventoryPaneList.PaneTypes.Quests;
		}
		return result;
	}

	public static InventoryPaneList.PaneTypes GetInventoryInputPressed(HeroActions ia)
	{
		InventoryPaneList.PaneTypes result = InventoryPaneList.PaneTypes.None;
		if (ia.OpenInventory.WasPressed)
		{
			result = InventoryPaneList.PaneTypes.Inv;
		}
		else if (ia.OpenInventoryMap.WasPressed || (bool)ia.SwipeInventoryMap)
		{
			result = InventoryPaneList.PaneTypes.Map;
		}
		else if (ia.OpenInventoryJournal.WasPressed || (bool)ia.SwipeInventoryJournal)
		{
			result = InventoryPaneList.PaneTypes.Journal;
		}
		else if (ia.OpenInventoryTools.WasPressed || (bool)ia.SwipeInventoryTools)
		{
			result = InventoryPaneList.PaneTypes.Tools;
		}
		else if (ia.OpenInventoryQuests.WasPressed || (bool)ia.SwipeInventoryQuests)
		{
			result = InventoryPaneList.PaneTypes.Quests;
		}
		return result;
	}

	public static bool IsInventoryButtonPressed(HeroActions ia)
	{
		return GetInventoryButtonPressed(ia) != InventoryPaneList.PaneTypes.None;
	}

	private void PressCancel()
	{
		FSMUtility.SendEventToGameObject(base.gameObject, "UI CANCEL");
		actionCooldown = 0.25f;
		isRepeatingDirection = false;
		VibrationManager.PlayVibrationClipOneShot(menuCancelVibration, null);
	}

	private void PressDirection(InventoryPaneBase.InputEventType direction)
	{
		if ((allowHorizontalSelection || (direction != 0 && direction != InventoryPaneBase.InputEventType.Right)) && (allowVerticalSelection || (direction != InventoryPaneBase.InputEventType.Up && direction != InventoryPaneBase.InputEventType.Down)))
		{
			if (allowRepeat)
			{
				isRepeatingDirection = true;
				isRepeatingSubmit = false;
				lastPressedDirection = direction;
				directionRepeatTimer = 0.25f;
			}
			GameObject go = base.gameObject;
			FSMUtility.SendEventToGameObject(go, direction switch
			{
				InventoryPaneBase.InputEventType.Right => "UI RIGHT", 
				InventoryPaneBase.InputEventType.Left => "UI LEFT", 
				InventoryPaneBase.InputEventType.Up => "UI UP", 
				InventoryPaneBase.InputEventType.Down => "UI DOWN", 
				_ => throw new ArgumentOutOfRangeException("direction", direction, null), 
			});
			pane.SendInputEvent(direction);
		}
	}

	private void PressSubmit()
	{
		FSMUtility.SendEventToGameObject(base.gameObject, "UI CONFIRM");
		wasSubmitPressed = true;
		isRepeatingDirection = false;
		VibrationManager.PlayVibrationClipOneShot(menuSubmitVibration, null);
		if (allowRepeatSubmit)
		{
			isRepeatingSubmit = true;
			directionRepeatTimer = 0.25f;
		}
		else
		{
			actionCooldown = 0.25f;
		}
	}

	private void ReleaseSubmit()
	{
		FSMUtility.SendEventToGameObject(base.gameObject, "UI CONFIRM RELEASED");
		wasSubmitPressed = false;
		isRepeatingDirection = false;
		isRepeatingSubmit = false;
	}

	public void CancelRepeat()
	{
		isRepeatingDirection = false;
	}
}
