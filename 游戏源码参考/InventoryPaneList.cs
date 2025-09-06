using System;
using System.Collections.Generic;
using System.Linq;
using GlobalEnums;
using HutongGames.PlayMaker;
using TMProOld;
using TeamCherry.NestedFadeGroup;
using UnityEngine;

public class InventoryPaneList : MonoBehaviour
{
	public enum PaneTypes
	{
		None = -1,
		Inv = 0,
		Tools = 1,
		Quests = 2,
		Journal = 3,
		Map = 4
	}

	[SerializeField]
	[ArrayForEnum(typeof(PaneTypes))]
	private InventoryPane[] panes;

	[SerializeField]
	private TextMeshPro currentPaneText;

	[SerializeField]
	private InventoryPaneListDisplay paneListDisplay;

	[SerializeField]
	private NestedFadeGroupBase paneArrows;

	[Space]
	[SerializeField]
	private AudioSource audioSourcePrefab;

	[SerializeField]
	private AudioEvent paneCycleSound;

	private float arrowFadeTime = 0.2f;

	private FsmBool allowSwapping;

	private FsmBool doNotClose;

	private FsmBool inSubMenu;

	private FsmBool isPaneMoveCustom;

	private float nextPaneOpenTimeLeft;

	private string nextPaneOpen;

	private bool panesDeactivated;

	private static InventoryPaneList _instance;

	public static string NextPaneOpen
	{
		get
		{
			if (_instance.nextPaneOpenTimeLeft <= 0f)
			{
				return string.Empty;
			}
			return _instance.nextPaneOpen ?? string.Empty;
		}
	}

	public bool CanSwitchPanes
	{
		get
		{
			if (allowSwapping != null)
			{
				return allowSwapping.Value;
			}
			return true;
		}
		set
		{
			if (allowSwapping == null)
			{
				PlayMakerFSM playMakerFSM = PlayMakerFSM.FindFsmOnGameObject(base.gameObject, "Inventory Control");
				if ((bool)playMakerFSM)
				{
					allowSwapping = playMakerFSM.FsmVariables.FindFsmBool("Allow Pane Swapping");
				}
			}
			if (allowSwapping != null && UnlockedPaneCount > 1 && allowSwapping.Value != value)
			{
				allowSwapping.Value = value;
				paneArrows.FadeTo(value ? 1f : 0f, arrowFadeTime, null, isRealtime: true);
			}
		}
	}

	public bool CloseBlocked
	{
		get
		{
			if (doNotClose != null)
			{
				return doNotClose.Value;
			}
			return true;
		}
		set
		{
			if (doNotClose == null)
			{
				PlayMakerFSM playMakerFSM = PlayMakerFSM.FindFsmOnGameObject(base.gameObject, "Inventory Control");
				if ((bool)playMakerFSM)
				{
					doNotClose = playMakerFSM.FsmVariables.FindFsmBool("Do Not Close");
				}
			}
			if (doNotClose != null && doNotClose.Value != value)
			{
				doNotClose.Value = value;
			}
		}
	}

	public bool InSubMenu
	{
		get
		{
			if (inSubMenu != null)
			{
				return inSubMenu.Value;
			}
			return true;
		}
		set
		{
			if (inSubMenu == null)
			{
				PlayMakerFSM playMakerFSM = PlayMakerFSM.FindFsmOnGameObject(base.gameObject, "Inventory Control");
				if ((bool)playMakerFSM)
				{
					inSubMenu = playMakerFSM.FsmVariables.FindFsmBool("In Sub Menu");
				}
			}
			if (inSubMenu != null && inSubMenu.Value != value)
			{
				inSubMenu.Value = value;
			}
		}
	}

	public bool IsPaneMoveCustom
	{
		get
		{
			if (isPaneMoveCustom != null)
			{
				return isPaneMoveCustom.Value;
			}
			return true;
		}
		set
		{
			if (isPaneMoveCustom == null)
			{
				PlayMakerFSM playMakerFSM = PlayMakerFSM.FindFsmOnGameObject(base.gameObject, "Inventory Control");
				if ((bool)playMakerFSM)
				{
					isPaneMoveCustom = playMakerFSM.FsmVariables.FindFsmBool("Pane Move Custom");
				}
			}
			if (isPaneMoveCustom != null && isPaneMoveCustom.Value != value)
			{
				isPaneMoveCustom.Value = value;
			}
		}
	}

	public int UnlockedPaneCount => panes.Where((InventoryPane t, int i) => IsPaneAvailable(t, (PaneTypes)i)).Count();

	public int TotalPaneCount => panes.Length;

	public event Action OpeningInventory;

	public event Action ClosingInventory;

	public event Action<int> MovedPaneIndex;

	private void OnValidate()
	{
		ArrayForEnumAttribute.EnsureArraySize(ref panes, typeof(PaneTypes));
	}

	private void Awake()
	{
		OnValidate();
		_instance = this;
	}

	private void OnDestroy()
	{
		if (_instance == this)
		{
			_instance = null;
		}
	}

	private void Start()
	{
		if ((bool)paneListDisplay)
		{
			paneListDisplay.PreInstantiate(panes.Length);
		}
		InventoryPane[] array = panes;
		foreach (InventoryPane inventoryPane in array)
		{
			if ((bool)inventoryPane)
			{
				GameObject gameObject = inventoryPane.gameObject;
				gameObject.SetActive(value: true);
				NestedFadeGroupBase component = gameObject.GetComponent<NestedFadeGroupBase>();
				if ((bool)component)
				{
					component.AlphaSelf = 0f;
					continue;
				}
				Debug.LogErrorFormat(gameObject, "Inventory pane {0} did not have fade group", gameObject.name);
			}
		}
		panesDeactivated = false;
	}

	private void Update()
	{
		if (!panesDeactivated)
		{
			InventoryPane[] array = panes;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].gameObject.SetActive(value: false);
			}
			panesDeactivated = true;
		}
		if (nextPaneOpenTimeLeft > 0f && GameManager.instance.GameState == GameState.PLAYING && InteractManager.BlockingInteractable == null && ManagerSingleton<InputHandler>.Instance.acceptingInput && !PlayerData.instance.disablePause && HeroController.instance.acceptingInput)
		{
			nextPaneOpenTimeLeft -= Time.deltaTime;
		}
	}

	private bool IsPaneAvailable(InventoryPane pane, PaneTypes paneType)
	{
		if (pane.IsAvailable)
		{
			return !CheatManager.IsInventoryPaneHidden(paneType);
		}
		return false;
	}

	public void OnOpeningInventory()
	{
		paneArrows.AlphaSelf = ((UnlockedPaneCount > 1) ? 1f : 0f);
		this.OpeningInventory?.Invoke();
	}

	public void OnClosingInventory()
	{
		this.ClosingInventory?.Invoke();
	}

	public InventoryPane SetCurrentPane(int index, InventoryPane currentPane)
	{
		if ((bool)currentPane)
		{
			currentPane.PaneEnd();
		}
		nextPaneOpenTimeLeft = 0f;
		InventoryPane inventoryPane = panes[index];
		inventoryPane = (IsPaneAvailable(inventoryPane, (PaneTypes)index) ? inventoryPane : GetNextPane(GetPaneIndex(inventoryPane), 1, panes.Length));
		return BeginPane(inventoryPane, 0);
	}

	public int GetPaneIndex(string paneName)
	{
		if (!string.IsNullOrEmpty(paneName))
		{
			for (int i = 0; i < panes.Length; i++)
			{
				if (panes[i].name == paneName)
				{
					return i;
				}
			}
		}
		return -1;
	}

	public InventoryPane SetNextPane(int direction, InventoryPane currentPane)
	{
		currentPane.PaneEnd();
		InventoryPane inventoryPane = GetNextPane(GetPaneIndex(currentPane), direction, panes.Length);
		if (inventoryPane == null)
		{
			inventoryPane = currentPane;
		}
		return BeginPane(inventoryPane, (int)Mathf.Sign(direction));
	}

	public InventoryPane BeginPane(InventoryPane pane, int cycleDirection)
	{
		InventoryPane inventoryPane = pane.Get();
		InventoryPane inventoryPane2 = inventoryPane.RootPane;
		if (!inventoryPane2)
		{
			inventoryPane2 = pane;
		}
		UpdateDisplay(inventoryPane, inventoryPane2, cycleDirection);
		inventoryPane.PaneStart();
		if (cycleDirection != 0)
		{
			paneCycleSound.SpawnAndPlayOneShot(audioSourcePrefab, base.transform.position);
		}
		this.MovedPaneIndex?.Invoke(GetPaneIndex(pane));
		return inventoryPane;
	}

	private InventoryPane GetNextPane(int index, int direction, int recursionsLeft)
	{
		if (recursionsLeft == 0)
		{
			return null;
		}
		index += direction;
		if (index >= panes.Length)
		{
			index = 0;
		}
		else if (index < 0)
		{
			index = panes.Length - 1;
		}
		InventoryPane inventoryPane = panes[index];
		if (!IsPaneAvailable(inventoryPane, (PaneTypes)index))
		{
			return GetNextPane(index, direction, recursionsLeft - 1);
		}
		return inventoryPane;
	}

	public int GetPaneIndex(InventoryPane pane)
	{
		if ((bool)pane.RootPane)
		{
			pane = pane.RootPane;
		}
		return Array.IndexOf(panes, pane);
	}

	public InventoryPane GetPane(PaneTypes paneTypes)
	{
		return GetPane((int)paneTypes);
	}

	public InventoryPane GetPane(int index)
	{
		if (index < 0 || index >= panes.Length)
		{
			return null;
		}
		return panes[index];
	}

	private void UpdateDisplay(InventoryPane displayPane, InventoryPane rootPane, int cycleDirection)
	{
		if ((bool)currentPaneText)
		{
			currentPaneText.text = displayPane.DisplayName;
		}
		if ((bool)paneListDisplay)
		{
			List<InventoryPane> list = panes.Where((InventoryPane t, int i) => IsPaneAvailable(t, (PaneTypes)i)).ToList();
			paneListDisplay.UpdateDisplay(list.IndexOf(rootPane), list, cycleDirection);
		}
	}

	public static void SetNextOpen(string paneName)
	{
		if ((bool)_instance)
		{
			_instance.nextPaneOpen = paneName;
			_instance.nextPaneOpenTimeLeft = 5f;
		}
	}
}
