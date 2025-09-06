using UnityEngine.EventSystems;
using UnityEngine.Events;

namespace UnityEngine.UI
{
	public class MenuButton : MenuSelectable, ISubmitHandler, IEventSystemHandler, IPointerClickHandler
	{
		public enum MenuButtonType
		{
			Proceed = 0,
			Activate = 1
		}

		public MenuButtonType buttonType;

		public Animator flashEffect;

		private static readonly int _flashProp = Animator.StringToHash("Flash");

		private Navigation originalNavigation;

		private bool hasNavOverride;

		public UnityEvent OnSubmitPressed;

		public bool SkipNextFlashEffect { get; set; }

		public bool SkipNextSubmitSound { get; set; }

		private new void Start()
		{
			HookUpAudioPlayer();
			HookUpEventTrigger();
		}

		public void OnSubmit(BaseEventData eventData)
		{
			if (base.interactable)
			{
				if (buttonType != MenuButtonType.Activate)
				{
					ForceDeselect();
				}
				if (!SkipNextFlashEffect && (bool)flashEffect)
				{
					flashEffect.ResetTrigger(_flashProp);
					flashEffect.SetTrigger(_flashProp);
				}
				else
				{
					Debug.LogWarning("MenuButton missing flashEffect!", this);
				}
				SkipNextFlashEffect = false;
				if (SkipNextSubmitSound)
				{
					SkipNextSubmitSound = false;
				}
				else
				{
					PlaySubmitSound();
				}
				OnSubmitPressed?.Invoke();
			}
		}

		public void OnPointerClick(PointerEventData eventData)
		{
			OnSubmit(eventData);
		}

		public void SetNavigationOverride(Navigation nav)
		{
			if (!hasNavOverride)
			{
				originalNavigation = base.navigation;
				hasNavOverride = true;
			}
			base.navigation = nav;
		}

		public void RestoreOriginalNavigation()
		{
			if (hasNavOverride)
			{
				hasNavOverride = false;
				base.navigation = originalNavigation;
			}
		}
	}
}
