using System.Collections;
using GlobalEnums;
using UnityEngine.EventSystems;

namespace UnityEngine.UI
{
	public class MenuPreventDeselect : MonoBehaviour, ISelectHandler, IEventSystemHandler, IDeselectHandler, ICancelHandler, IPlaySelectSound
	{
		[Header("On Cancel")]
		public CancelAction cancelAction;

		[Header("Fleurs")]
		public Animator leftCursor;

		public Animator rightCursor;

		[Space]
		[SerializeField]
		private bool playSelectSound = true;

		private MenuAudioController uiAudioPlayer;

		private GameObject prevSelectedObject;

		private bool dontPlaySelectSound;

		private bool deselectWasForced;

		private static readonly int _showProp = Animator.StringToHash("show");

		private static readonly int _hideProp = Animator.StringToHash("hide");

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

		private void Start()
		{
			HookUpAudioPlayer();
		}

		public void OnSelect(BaseEventData eventData)
		{
			if (leftCursor != null)
			{
				leftCursor.SetTrigger(_showProp);
				leftCursor.ResetTrigger(_hideProp);
			}
			if (rightCursor != null)
			{
				rightCursor.SetTrigger(_showProp);
				rightCursor.ResetTrigger(_hideProp);
			}
			if (playSelectSound && !dontPlaySelectSound)
			{
				uiAudioPlayer.PlaySelect();
			}
			else
			{
				dontPlaySelectSound = false;
			}
		}

		public void OnDeselect(BaseEventData eventData)
		{
			StartCoroutine(ValidateDeselect());
		}

		public void OnCancel(BaseEventData eventData)
		{
			bool flag = true;
			if (cancelAction != 0)
			{
				ForceDeselect();
			}
			UIManager instance = UIManager.instance;
			switch (cancelAction)
			{
			case CancelAction.DoNothing:
				flag = false;
				break;
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
			default:
				Debug.LogError("CancelAction not implemented for this control");
				flag = false;
				break;
			}
			if (flag)
			{
				uiAudioPlayer.PlayCancel();
			}
		}

		private IEnumerator ValidateDeselect()
		{
			prevSelectedObject = EventSystem.current.currentSelectedGameObject;
			yield return new WaitForEndOfFrame();
			if (EventSystem.current.currentSelectedGameObject != null)
			{
				if (leftCursor != null)
				{
					leftCursor.ResetTrigger(_showProp);
					leftCursor.SetTrigger(_hideProp);
				}
				if (rightCursor != null)
				{
					rightCursor.ResetTrigger(_showProp);
					rightCursor.SetTrigger(_hideProp);
				}
				yield break;
			}
			if (deselectWasForced)
			{
				if (leftCursor != null)
				{
					leftCursor.ResetTrigger(_showProp);
					leftCursor.SetTrigger(_hideProp);
				}
				if (rightCursor != null)
				{
					rightCursor.ResetTrigger(_showProp);
					rightCursor.SetTrigger(_hideProp);
				}
				deselectWasForced = false;
				yield break;
			}
			if (!ManagerSingleton<InputHandler>.Instance.acceptingInput)
			{
				while (!ManagerSingleton<InputHandler>.Instance.acceptingInput)
				{
					yield return null;
				}
			}
			yield return null;
			if (EventSystem.current.currentSelectedGameObject != null)
			{
				if (leftCursor != null)
				{
					leftCursor.ResetTrigger(_showProp);
					leftCursor.SetTrigger(_hideProp);
				}
				if (rightCursor != null)
				{
					rightCursor.ResetTrigger(_showProp);
					rightCursor.SetTrigger(_hideProp);
				}
			}
			else if (deselectWasForced)
			{
				if (leftCursor != null)
				{
					leftCursor.ResetTrigger(_showProp);
					leftCursor.SetTrigger(_hideProp);
				}
				if (rightCursor != null)
				{
					rightCursor.ResetTrigger(_showProp);
					rightCursor.SetTrigger(_hideProp);
				}
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
			if (!uiAudioPlayer)
			{
				uiAudioPlayer = UIManager.instance.uiAudioPlayer;
			}
		}

		public void ForceDeselect()
		{
			if (EventSystem.current.currentSelectedGameObject != null)
			{
				deselectWasForced = true;
				EventSystem.current.SetSelectedGameObject(null);
			}
		}

		public void SimulateSubmit()
		{
			ForceDeselect();
			UIManager.instance.uiAudioPlayer.PlaySubmit();
		}
	}
}
