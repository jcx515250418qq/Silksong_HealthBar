using System.Collections;
using HKMenu;
using UnityEngine.EventSystems;

namespace UnityEngine.UI
{
	public class MenuResolutionSetting : MenuOptionHorizontal, ISubmitHandler, IEventSystemHandler, IMoveHandler, IPointerClickHandler, IMenuOptionListSetting
	{
		private Resolution[] availableResolutions;

		private Resolution previousRes;

		private bool foundResolutionInList;

		private int currentlyActiveResIndex = -1;

		private int previousResIndex;

		public Resolution currentRes { get; private set; }

		public Resolution screenRes { get; private set; }

		public new void OnEnable()
		{
			RefreshControls();
			UpdateApplyButton();
		}

		public new void OnSubmit(BaseEventData eventData)
		{
			if (currentlyActiveResIndex != selectedOptionIndex)
			{
				ForceDeselect();
				uiAudioPlayer.PlaySubmit();
				ApplySettings();
			}
		}

		public new void OnMove(AxisEventData move)
		{
			if (MoveOption(move.moveDir))
			{
				UpdateApplyButton();
			}
			else
			{
				base.OnMove(move);
			}
		}

		public new void OnPointerClick(PointerEventData eventData)
		{
			base.OnPointerClick(eventData);
			if (eventData.button == PointerEventData.InputButton.Left || eventData.button == PointerEventData.InputButton.Right)
			{
				UpdateApplyButton();
			}
		}

		public override void ApplySettings()
		{
			if (selectedOptionIndex >= 0)
			{
				previousRes = currentRes;
				previousResIndex = currentlyActiveResIndex;
				Resolution resolution = availableResolutions[selectedOptionIndex];
				Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen, resolution.refreshRate);
				currentRes = resolution;
				currentlyActiveResIndex = selectedOptionIndex;
				HideApplyButton();
				UIManager.instance.UIShowResolutionPrompt(startTimer: true);
			}
		}

		public override void UpdateApplyButton()
		{
			if (currentlyActiveResIndex == selectedOptionIndex)
			{
				HideApplyButton();
			}
			else
			{
				ShowApplyButton();
			}
		}

		public void ResetToDefaultResolution()
		{
			Screen.SetResolution(1920, 1080, Screen.fullScreen);
			currentRes = Screen.currentResolution;
			StartCoroutine(RefreshOnNextFrame());
		}

		public void RefreshControls()
		{
			RefreshAvailableResolutions();
			RefreshCurrentIndex();
			PushUpdateOptionList();
			UpdateText();
		}

		public void RollbackResolution()
		{
			Screen.SetResolution(previousRes.width, previousRes.height, Screen.fullScreen);
			currentRes = Screen.currentResolution;
			StartCoroutine(RefreshOnNextFrame());
		}

		private string GetResolutionString(Resolution resolution)
		{
			return $"{resolution.width} x {resolution.height} @ {resolution.refreshRateRatio.value:0.##}Hz";
		}

		public override void RefreshCurrentIndex()
		{
			foundResolutionInList = false;
			for (int i = 0; i < availableResolutions.Length; i++)
			{
				if (currentRes.Equals(availableResolutions[i]))
				{
					selectedOptionIndex = i;
					currentlyActiveResIndex = i;
					foundResolutionInList = true;
					break;
				}
			}
			if (!foundResolutionInList)
			{
				Resolution[] array = new Resolution[availableResolutions.Length + 1];
				array[0] = currentRes;
				for (int j = 0; j < availableResolutions.Length; j++)
				{
					array[j + 1] = availableResolutions[j];
				}
				availableResolutions = array;
				selectedOptionIndex = 0;
				currentlyActiveResIndex = 0;
			}
		}

		public void PushUpdateOptionList()
		{
			string[] array = new string[availableResolutions.Length];
			for (int i = 0; i < availableResolutions.Length; i++)
			{
				Resolution resolution = availableResolutions[i];
				array[i] = GetResolutionString(resolution);
			}
			SetOptionList(array);
		}

		private void RefreshAvailableResolutions()
		{
			screenRes = Screen.currentResolution;
			if (!Screen.fullScreen)
			{
				Resolution resolution = default(Resolution);
				resolution.width = Screen.width;
				resolution.height = Screen.height;
				resolution.refreshRateRatio = screenRes.refreshRateRatio;
				Resolution resolution2 = resolution;
				currentRes = resolution2;
			}
			else
			{
				currentRes = screenRes;
			}
			availableResolutions = Screen.resolutions;
		}

		private IEnumerator RefreshOnNextFrame()
		{
			yield return null;
			RefreshAvailableResolutions();
			RefreshCurrentIndex();
			PushUpdateOptionList();
			UpdateApplyButton();
			UpdateText();
		}
	}
}
