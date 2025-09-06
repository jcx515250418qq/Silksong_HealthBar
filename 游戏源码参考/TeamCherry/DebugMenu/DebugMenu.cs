using System;
using System.Collections.Generic;
using TMProOld;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Pool;
using UnityEngine.Serialization;

namespace TeamCherry.DebugMenu
{
	public class DebugMenu : MonoBehaviour
	{
		[Serializable]
		private class NamedObject
		{
			public string name;

			public GameObject gameObject;
		}

		public abstract class MenuStack
		{
			protected bool isDirty = true;

			protected GameObject lastSelected;

			public string name;

			public virtual void Enter()
			{
				isDirty = true;
			}

			public virtual void Exit()
			{
				isDirty = true;
			}

			public void SetDirty()
			{
				isDirty = true;
				lastSelected = EventSystem.current.currentSelectedGameObject;
			}

			public void DoMenu()
			{
				UpdateInput();
				if ((bool)EventSystem.current.currentSelectedGameObject)
				{
					if (EventSystem.current.currentSelectedGameObject.activeInHierarchy)
					{
						lastSelected = EventSystem.current.currentSelectedGameObject;
					}
				}
				else if ((bool)lastSelected && lastSelected.activeInHierarchy)
				{
					EventSystem.current.SetSelectedGameObject(lastSelected);
				}
				if (isDirty)
				{
					isDirty = false;
					ClearMenus();
					DrawMenu();
					if ((bool)lastSelected && lastSelected.activeInHierarchy)
					{
						EventSystem.current.SetSelectedGameObject(lastSelected);
					}
				}
			}

			protected abstract void UpdateInput();

			protected abstract void DrawMenu();
		}

		public class MainMenu : MenuStack
		{
			private TrophyMenu trophyMenu;

			private ActivityMenu activityMenu;

			private List<MenuStack> menus = new List<MenuStack>();

			public MainMenu()
			{
				trophyMenu = new TrophyMenu();
				activityMenu = new ActivityMenu();
			}

			public void AddRootMenu(GameObject root)
			{
				if ((bool)root)
				{
					isDirty = true;
				}
			}

			protected override void UpdateInput()
			{
			}

			protected override void DrawMenu()
			{
				GetButton().DoButton("Trophy Menu", delegate
				{
					isDirty = true;
					PushMenu(trophyMenu);
				});
				GetButton().DoButton("Activity Menu", delegate
				{
					isDirty = true;
					PushMenu(activityMenu);
				});
				foreach (MenuStack menu in menus)
				{
					GetButton().DoButton(menu.name, delegate
					{
						isDirty = true;
						PushMenu(menu);
					});
				}
			}
		}

		public abstract class SubMenu : MenuStack
		{
			protected void ReturnMenu()
			{
				HeroActions inputActions = ManagerSingleton<InputHandler>.Instance.inputActions;
				if (Platform.Current.GetMenuAction(inputActions) == Platform.MenuActions.Cancel)
				{
					PopMenu();
				}
			}
		}

		public abstract class NamedSubMenu : SubMenu
		{
			protected NamedSubMenu(string name)
			{
				base.name = name;
			}
		}

		public class SaveMenu : SubMenu
		{
			protected override void UpdateInput()
			{
				ReturnMenu();
			}

			protected override void DrawMenu()
			{
				DrawNamedObjects();
			}
		}

		public class ToggleMenu : SubMenu
		{
			protected override void UpdateInput()
			{
				ReturnMenu();
			}

			protected override void DrawMenu()
			{
				DrawToggleObjects(delegate
				{
					isDirty = true;
				});
			}
		}

		public class TargetRootMenu : SubMenu
		{
			private class Target
			{
				public GameObject gameObject;

				public AudioSource audioSource;

				public bool hasAudioSource;

				public Target(GameObject gameObject)
				{
					this.gameObject = gameObject;
					audioSource = this.gameObject.GetComponent<AudioSource>();
					hasAudioSource = audioSource;
				}
			}

			private List<Target> toggleObjects;

			private string rootName;

			public string Name => rootName;

			public TargetRootMenu(GameObject gameObject)
			{
				if ((bool)gameObject)
				{
					rootName = gameObject.name;
					toggleObjects = new List<Target>();
					for (int i = 0; i < gameObject.transform.childCount; i++)
					{
						GameObject gameObject2 = gameObject.transform.GetChild(i).gameObject;
						toggleObjects.Add(new Target(gameObject2));
					}
				}
			}

			protected override void UpdateInput()
			{
				ReturnMenu();
			}

			protected override void DrawMenu()
			{
				foreach (Target toggleObject in toggleObjects)
				{
					GameObject targetGO = toggleObject.gameObject;
					GetButton().DoButton(targetGO.name + " " + (targetGO.activeSelf ? "<color=\"green\">Enabled</color>" : "<color=\"red\">Disabled</color>"), delegate
					{
						targetGO.SetActive(!targetGO.activeSelf);
						activeMenu?.SetDirty();
					});
					if (toggleObject.hasAudioSource)
					{
						AudioSource audioSource = toggleObject.audioSource;
						GetButton().DoButton("Play Clip : " + (audioSource.clip ? audioSource.clip.name : "none"), delegate
						{
							audioSource.Play();
						});
					}
				}
			}
		}

		public class TrophyMenu : SubMenu
		{
			protected override void UpdateInput()
			{
				ReturnMenu();
			}

			protected override void DrawMenu()
			{
			}
		}

		public class ActivityMenu : NamedSubMenu
		{
			public ActivityMenu()
				: base("Activity Menu")
			{
			}

			public override void Enter()
			{
				base.Enter();
			}

			protected override void UpdateInput()
			{
				ReturnMenu();
			}

			protected override void DrawMenu()
			{
			}
		}

		[FormerlySerializedAs("simpleTextButton")]
		[SerializeField]
		private DebugMenuButton debugMenuButton;

		[SerializeField]
		private TextMeshProUGUI detailsText;

		[SerializeField]
		private NamedObject[] namedObjects;

		[SerializeField]
		private List<GameObject> toggleableObjects = new List<GameObject>();

		[SerializeField]
		private List<GameObject> targetRoots = new List<GameObject>();

		[SerializeField]
		private Transform buttonParent;

		private static ObjectPool<DebugMenuButton> buttonPool;

		private static List<DebugMenuButton> activeButtons = new List<DebugMenuButton>();

		private static DebugMenu instance;

		private static MenuStack activeMenu;

		private static Stack<MenuStack> menuStack = new Stack<MenuStack>();

		private GameObject lastSelected;

		public static bool IsActive { get; private set; }

		private void Awake()
		{
			instance = this;
			buttonPool = new ObjectPool<DebugMenuButton>(CreateFunc, ActionOnGet, ActionOnRelease);
			UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
			if (buttonParent == null)
			{
				if ((bool)debugMenuButton)
				{
					buttonParent = debugMenuButton.transform.parent;
				}
				else
				{
					buttonParent = base.transform;
				}
			}
			if ((bool)debugMenuButton)
			{
				debugMenuButton.gameObject.SetActive(value: false);
			}
		}

		private void Start()
		{
			if (!IsActive)
			{
				base.gameObject.SetActive(value: false);
			}
		}

		private void Update()
		{
			if (activeMenu == null)
			{
				if (menuStack.Count > 0)
				{
					activeMenu = menuStack.Pop();
				}
				else
				{
					MainMenu mainMenu = new MainMenu();
					foreach (GameObject targetRoot in targetRoots)
					{
						mainMenu.AddRootMenu(targetRoot);
					}
					PushMenu(mainMenu);
				}
			}
			if (activeMenu != null)
			{
				activeMenu.DoMenu();
			}
		}

		public static void Show()
		{
			if (!IsActive && (bool)instance)
			{
				IsActive = true;
				instance.gameObject.SetActive(value: true);
				if (activeMenu != null)
				{
					activeMenu.SetDirty();
				}
			}
		}

		public static void Hide()
		{
			if (IsActive && (bool)instance)
			{
				IsActive = false;
				instance.gameObject.SetActive(value: false);
			}
		}

		private DebugMenuButton CreateFunc()
		{
			return UnityEngine.Object.Instantiate(debugMenuButton, buttonParent);
		}

		private void ActionOnGet(DebugMenuButton obj)
		{
			activeButtons.Add(obj);
			obj.gameObject.SetActive(value: true);
			if ((bool)EventSystem.current && (EventSystem.current.currentSelectedGameObject == null || !EventSystem.current.currentSelectedGameObject.gameObject.activeInHierarchy))
			{
				EventSystem.current.SetSelectedGameObject(obj.gameObject);
			}
			obj.Button.onClick.RemoveAllListeners();
		}

		private void ActionOnRelease(DebugMenuButton obj)
		{
			activeButtons.Remove(obj);
			obj.gameObject.SetActive(value: false);
		}

		public static void ClearMenus()
		{
			for (int num = activeButtons.Count - 1; num >= 0; num--)
			{
				DebugMenuButton element = activeButtons[num];
				buttonPool.Release(element);
			}
		}

		public static void PopMenu()
		{
			if (activeMenu != null)
			{
				activeMenu.Exit();
			}
			activeMenu = menuStack.Pop();
			activeMenu?.Enter();
		}

		public static void PushMenu(MenuStack stack)
		{
			if (stack != null)
			{
				if (activeMenu != null)
				{
					activeMenu.Exit();
					menuStack.Push(activeMenu);
				}
				activeMenu = stack;
				activeMenu.Enter();
			}
		}

		public static DebugMenuButton GetButton()
		{
			return buttonPool.Get();
		}

		private static void DrawNamedObjects()
		{
			if (!instance)
			{
				return;
			}
			NamedObject[] array = instance.namedObjects;
			foreach (NamedObject namedObject in array)
			{
				GetButton().DoButton(namedObject.name, delegate
				{
					UnityEngine.Object.Instantiate(namedObject.gameObject);
				});
			}
		}

		private static void DrawToggleObjects(Action callback)
		{
			if (!instance)
			{
				return;
			}
			foreach (GameObject namedObject in instance.toggleableObjects)
			{
				if ((bool)namedObject)
				{
					GetButton().DoButton(namedObject.name + " " + (namedObject.activeSelf ? "<color=\"green\">Enabled</color>" : "<color=\"red\">Disabled</color>"), delegate
					{
						callback?.Invoke();
						namedObject.SetActive(!namedObject.activeSelf);
						activeMenu?.SetDirty();
					});
				}
			}
		}

		private static void DisplayDetailMessage(string message)
		{
			if ((bool)instance)
			{
				instance.detailsText.text = message;
			}
		}
	}
}
