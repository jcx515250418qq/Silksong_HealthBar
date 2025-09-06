using System;
using System.Collections;
using System.Collections.Generic;
using GlobalEnums;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class MenuButtonList : MonoBehaviour
{
	[Serializable]
	private class Entry
	{
		[SerializeField]
		[FormerlySerializedAs("button")]
		private Selectable selectable;

		[SerializeField]
		private MenuButtonListCondition condition;

		[SerializeField]
		private bool alsoAffectParent;

		[SerializeField]
		private bool forceEnable;

		public Selectable Selectable => selectable;

		public MenuButtonListCondition Condition => condition;

		public bool ForceEnable => forceEnable;

		public bool AlsoAffectParent => alsoAffectParent;
	}

	[SerializeField]
	private Entry[] entries;

	[SerializeField]
	private bool isTopLevelMenu;

	[SerializeField]
	private bool skipDisabled;

	private MenuSelectable lastSelected;

	private List<Selectable> activeSelectables;

	private static readonly HashSet<MenuButtonList> _menuButtonLists = new HashSet<MenuButtonList>();

	private bool started;

	private static readonly int _hide = Animator.StringToHash("hide");

	private static readonly int _show = Animator.StringToHash("show");

	private bool isDirty;

	private void Awake()
	{
		MenuScreen component = GetComponent<MenuScreen>();
		if ((bool)component)
		{
			component.defaultHighlight = null;
		}
	}

	protected void Start()
	{
		SetupActive();
		DoSelect();
		started = true;
		Platform.OnSaveStoreStateChanged += OnSaveStoreStateChanged;
	}

	private void OnEnable()
	{
		if (started)
		{
			if (isDirty)
			{
				SetupActive();
			}
			DoSelect();
		}
	}

	private void OnSaveStoreStateChanged(bool mounted)
	{
		if (mounted)
		{
			isDirty = true;
		}
	}

	public void SetupActive()
	{
		_menuButtonLists.Add(this);
		if (activeSelectables == null)
		{
			activeSelectables = new List<Selectable>();
		}
		else
		{
			activeSelectables.Clear();
		}
		Entry[] array = entries;
		foreach (Entry entry in array)
		{
			Selectable selectable = entry.Selectable;
			MenuButtonListCondition condition = entry.Condition;
			bool flag = skipDisabled && !entry.ForceEnable;
			if (condition == null || condition.IsFulfilledAllComponents())
			{
				if (!flag)
				{
					selectable.gameObject.SetActive(value: true);
					if ((bool)condition)
					{
						condition.OnActiveStateSet(isActive: true);
					}
					selectable.interactable = true;
					if (entry.AlsoAffectParent)
					{
						selectable.transform.parent.gameObject.SetActive(value: true);
					}
				}
				if (!flag || (selectable.gameObject.activeInHierarchy && selectable.interactable))
				{
					activeSelectables.Add(selectable);
				}
			}
			else
			{
				condition.OnActiveStateSet(isActive: false);
				if (!condition.AlwaysVisible())
				{
					selectable.gameObject.SetActive(value: false);
				}
				selectable.interactable = false;
				if (entry.AlsoAffectParent)
				{
					selectable.transform.parent.gameObject.SetActive(value: false);
				}
			}
		}
		for (int j = 0; j < activeSelectables.Count; j++)
		{
			Selectable selectable2 = activeSelectables[j];
			Selectable selectOnUp = activeSelectables[(j + activeSelectables.Count - 1) % activeSelectables.Count];
			Selectable selectOnDown = activeSelectables[(j + 1) % activeSelectables.Count];
			Navigation navigation = selectable2.navigation;
			if (navigation.mode == Navigation.Mode.Explicit)
			{
				navigation.selectOnUp = selectOnUp;
				navigation.selectOnDown = selectOnDown;
				selectable2.navigation = navigation;
			}
			if (isTopLevelMenu)
			{
				CancelAction cancelAction = (Platform.Current.WillDisplayQuitButton ? CancelAction.GoToExitPrompt : CancelAction.DoNothing);
				MenuButton menuButton = selectable2 as MenuButton;
				if (menuButton != null)
				{
					menuButton.cancelAction = cancelAction;
				}
			}
		}
		foreach (Selectable activeSelectable in activeSelectables)
		{
			MenuSelectable menuSelectable = (MenuSelectable)activeSelectable;
			menuSelectable.OnSelected += delegate(MenuSelectable self)
			{
				if (!isTopLevelMenu)
				{
					MenuSelectable menuSelectable2 = menuSelectable;
					List<Selectable> list = activeSelectables;
					if (!(menuSelectable2 != (MenuSelectable)list[list.Count - 1]))
					{
						return;
					}
				}
				lastSelected = self;
			};
		}
	}

	private void DoSelect()
	{
		if ((bool)lastSelected)
		{
			StartCoroutine(SelectDelayed(lastSelected));
		}
		else if (activeSelectables != null && activeSelectables.Count > 0)
		{
			StartCoroutine(SelectDelayed(activeSelectables[0].GetFirstInteractable()));
		}
	}

	private void OnDestroy()
	{
		_menuButtonLists.Remove(this);
		Platform.OnSaveStoreStateChanged -= OnSaveStoreStateChanged;
	}

	private IEnumerator SelectDelayed(Selectable selectable)
	{
		while (!selectable.gameObject.activeInHierarchy)
		{
			yield return null;
		}
		UIManager.HighlightSelectableNoSound(selectable);
		Animator[] componentsInChildren = selectable.GetComponentsInChildren<Animator>();
		foreach (Animator animator in componentsInChildren)
		{
			if (animator.HasParameter(_hide, null))
			{
				animator.ResetTrigger(_hide);
			}
			if (animator.HasParameter(_show, null))
			{
				animator.SetTrigger(_show);
			}
		}
	}

	public void ClearLastSelected()
	{
		lastSelected = null;
	}

	public static void ClearAllLastSelected()
	{
		foreach (MenuButtonList menuButtonList in _menuButtonLists)
		{
			menuButtonList.ClearLastSelected();
		}
	}
}
