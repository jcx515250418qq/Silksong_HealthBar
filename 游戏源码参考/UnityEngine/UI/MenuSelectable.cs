using System;
using System.Collections;
using GlobalEnums;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

namespace UnityEngine.UI
{
	public class MenuSelectable : Selectable, ISelectHandler, IEventSystemHandler, IDeselectHandler, ICancelHandler, IPlaySelectSound
	{
		public delegate void OnSelectedEvent(MenuSelectable self);

		[Header("On Cancel")]
		public CancelAction cancelAction;

		[Header("Fleurs")]
		public Animator leftCursor;

		public Animator rightCursor;

		[Header("Highlight")]
		public Animator selectHighlight;

		public bool playSubmitSound = true;

		[Header("Description Text")]
		public Animator descriptionText;

		[Header("Vibrations")]
		public VibrationDataAsset menuSubmitVibration;

		public VibrationDataAsset menuCancelVibration;

		protected MenuAudioController uiAudioPlayer;

		protected GameObject prevSelectedObject;

		protected bool dontPlaySelectSound;

		protected bool deselectWasForced;

		private MenuButtonList parentList;

		private static readonly int _showPropId = Animator.StringToHash("show");

		private static readonly int _hidePropId = Animator.StringToHash("hide");

		private EventTrigger eventTrigger;

		private bool hasCancelEventTrigger;

		private Action<bool> customGoBack;

		public bool DontPlaySelectSound
		{
			get
			{
				return dontPlaySelectSound;
			}
			set
			{
				dontPlaySelectSound = value;
			}
		}

		public event OnSelectedEvent OnSelected;

		protected override void Awake()
		{
			base.Awake();
			base.transition = Transition.None;
			if (base.navigation.mode != Navigation.Mode.Explicit)
			{
				Navigation navigation = default(Navigation);
				navigation.mode = Navigation.Mode.Explicit;
				Navigation navigation2 = navigation;
				base.navigation = navigation2;
			}
		}

		protected override void Start()
		{
			base.Start();
			HookUpAudioPlayer();
			HookUpEventTrigger();
		}

		protected override void OnEnable()
		{
			base.OnEnable();
			CacheCancelEvent();
		}

		public new void OnSelect(BaseEventData eventData)
		{
			if (!base.interactable)
			{
				return;
			}
			if (this.OnSelected != null)
			{
				this.OnSelected(this);
			}
			if (leftCursor != null)
			{
				leftCursor.ResetTrigger(_hidePropId);
				leftCursor.SetTrigger(_showPropId);
			}
			if (rightCursor != null)
			{
				rightCursor.ResetTrigger(_hidePropId);
				rightCursor.SetTrigger(_showPropId);
			}
			if (selectHighlight != null)
			{
				selectHighlight.ResetTrigger(_hidePropId);
				selectHighlight.SetTrigger(_showPropId);
			}
			if (descriptionText != null)
			{
				descriptionText.ResetTrigger(_hidePropId);
				descriptionText.SetTrigger(_showPropId);
			}
			if (!DontPlaySelectSound)
			{
				try
				{
					uiAudioPlayer.PlaySelect();
					return;
				}
				catch (Exception ex)
				{
					Debug.LogError(base.name + " doesn't have a select sound specified. " + ex);
					return;
				}
			}
			dontPlaySelectSound = false;
		}

		public new void OnDeselect(BaseEventData eventData)
		{
			StartCoroutine(ValidateDeselect(eventData));
		}

		protected virtual void OnDeselected(BaseEventData eventData)
		{
		}

		public void ForceDeselect()
		{
			if (EventSystem.current.currentSelectedGameObject == base.gameObject)
			{
				deselectWasForced = true;
				EventSystem.current.SetSelectedGameObject(null);
			}
		}

		public void OnCancel(BaseEventData eventData)
		{
			if (!base.interactable)
			{
				return;
			}
			if (cancelAction != 0 || hasCancelEventTrigger)
			{
				ForceDeselect();
			}
			if (!parentList)
			{
				parentList = GetComponentInParent<MenuButtonList>();
			}
			if ((bool)parentList)
			{
				parentList.ClearLastSelected();
			}
			UIManager instance = UIManager.instance;
			switch (cancelAction)
			{
			case CancelAction.GoToMainMenu:
				if (!instance.UIGoBack())
				{
					instance.UIGoToMainMenu();
				}
				break;
			case CancelAction.GoToOptionsMenu:
				if (!instance.UIGoBack())
				{
					instance.UIGoToOptionsMenu();
				}
				break;
			case CancelAction.GoToVideoMenu:
				if (!instance.UIGoBack())
				{
					instance.UIGoToVideoMenu();
				}
				break;
			case CancelAction.GoToPauseMenu:
				if (!instance.UIGoBack())
				{
					instance.UIGoToPauseMenu();
				}
				break;
			case CancelAction.LeaveOptionsMenu:
				if (!instance.UIGoBack())
				{
					instance.UILeaveOptionsMenu();
				}
				break;
			case CancelAction.GoToExitPrompt:
				if (!instance.UIGoBack())
				{
					instance.UIShowQuitGamePrompt();
				}
				break;
			case CancelAction.GoToProfileMenu:
				if (!instance.UIGoBack())
				{
					instance.UIGoToProfileMenu();
				}
				break;
			case CancelAction.GoToControllerMenu:
				if (!instance.UIGoBack())
				{
					instance.UIGoToControllerMenu();
				}
				break;
			case CancelAction.ApplyRemapGamepadSettings:
				if (!instance.UIGoBack())
				{
					instance.ApplyRemapGamepadMenuSettings();
				}
				break;
			case CancelAction.ApplyAudioSettings:
				if (!instance.UIGoBack())
				{
					instance.ApplyAudioMenuSettings();
				}
				break;
			case CancelAction.ApplyVideoSettings:
				if (!instance.UIGoBack())
				{
					instance.ApplyVideoMenuSettings();
				}
				break;
			case CancelAction.ApplyGameSettings:
				if (!instance.UIGoBack())
				{
					instance.ApplyGameMenuSettings();
				}
				break;
			case CancelAction.ApplyKeyboardSettings:
				if (!instance.UIGoBack())
				{
					instance.ApplyKeyboardMenuSettings();
				}
				break;
			case CancelAction.ApplyControllerSettings:
				if (!instance.UIGoBack())
				{
					instance.ApplyControllerMenuSettings();
				}
				break;
			case CancelAction.GoToExtrasMenu:
				if ((bool)ContentPackDetailsUI.Instance)
				{
					ContentPackDetailsUI.Instance.UndoMenuStyle();
				}
				if (!instance.UIGoBack())
				{
					instance.UIGoToExtrasMenu();
				}
				break;
			default:
				Debug.LogError("CancelAction not implemented for this control");
				break;
			case CancelAction.DoNothing:
				break;
			}
			if (cancelAction != 0 || hasCancelEventTrigger)
			{
				PlayCancelSound();
			}
		}

		private IEnumerator ValidateDeselect(BaseEventData eventData)
		{
			prevSelectedObject = EventSystem.current.currentSelectedGameObject;
			yield return new WaitForEndOfFrame();
			if (EventSystem.current.currentSelectedGameObject != null || deselectWasForced)
			{
				if (leftCursor != null)
				{
					leftCursor.ResetTrigger(_showPropId);
					leftCursor.SetTrigger(_hidePropId);
				}
				if (rightCursor != null)
				{
					rightCursor.ResetTrigger(_showPropId);
					rightCursor.SetTrigger(_hidePropId);
				}
				if (selectHighlight != null)
				{
					selectHighlight.ResetTrigger(_showPropId);
					selectHighlight.SetTrigger(_hidePropId);
				}
				if (descriptionText != null)
				{
					descriptionText.ResetTrigger(_showPropId);
					descriptionText.SetTrigger(_hidePropId);
				}
				OnDeselected(eventData);
				deselectWasForced = false;
				yield break;
			}
			InputHandler ih = ManagerSingleton<InputHandler>.Instance;
			if ((bool)ih && !ih.acceptingInput)
			{
				while (!ih.acceptingInput)
				{
					yield return null;
				}
			}
			yield return null;
			if (EventSystem.current.currentSelectedGameObject != null || deselectWasForced)
			{
				if (leftCursor != null)
				{
					leftCursor.ResetTrigger(_showPropId);
					leftCursor.SetTrigger(_hidePropId);
				}
				if (rightCursor != null)
				{
					rightCursor.ResetTrigger(_showPropId);
					rightCursor.SetTrigger(_hidePropId);
				}
				if (selectHighlight != null)
				{
					selectHighlight.ResetTrigger(_showPropId);
					selectHighlight.SetTrigger(_hidePropId);
				}
				if (descriptionText != null)
				{
					descriptionText.ResetTrigger(_showPropId);
					descriptionText.SetTrigger(_hidePropId);
				}
				OnDeselected(eventData);
				deselectWasForced = false;
			}
			else if (prevSelectedObject != null && prevSelectedObject.activeInHierarchy)
			{
				deselectWasForced = false;
				dontPlaySelectSound = true;
				EventSystem.current.SetSelectedGameObject(prevSelectedObject);
			}
		}

		protected void HookUpAudioPlayer()
		{
			if (!Application.isPlaying)
			{
				return;
			}
			UIManager instance = UIManager.instance;
			uiAudioPlayer = ((SceneManager.GetActiveScene().name == "Pre_Menu_Intro") ? Object.FindObjectOfType<MenuAudioController>() : instance.uiAudioPlayer);
			if ((bool)instance)
			{
				if (menuSubmitVibration == null)
				{
					menuSubmitVibration = instance.menuSubmitVibration;
				}
				if (menuCancelVibration == null)
				{
					menuCancelVibration = instance.menuCancelVibration;
				}
			}
		}

		protected void HookUpEventTrigger()
		{
			eventTrigger = GetComponent<EventTrigger>();
			CacheCancelEvent();
		}

		protected void CacheCancelEvent()
		{
			if ((bool)eventTrigger)
			{
				hasCancelEventTrigger = eventTrigger.triggers.Exists((EventTrigger.Entry trigger) => trigger.eventID == EventTriggerType.Cancel);
			}
		}

		protected void PlaySubmitSound()
		{
			if (playSubmitSound)
			{
				uiAudioPlayer.PlaySubmit();
			}
			if ((bool)menuSubmitVibration)
			{
				VibrationManager.PlayVibrationClipOneShot(menuSubmitVibration.VibrationData, null);
			}
		}

		protected void PlayCancelSound()
		{
			uiAudioPlayer.PlayCancel();
			if ((bool)menuCancelVibration)
			{
				VibrationManager.PlayVibrationClipOneShot(menuCancelVibration.VibrationData, null);
			}
		}

		protected void PlaySelectSound()
		{
			uiAudioPlayer.PlaySelect();
		}
	}
}
