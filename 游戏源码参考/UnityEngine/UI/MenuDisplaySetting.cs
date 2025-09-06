using System.Collections;
using HKMenu;
using UnityEngine.EventSystems;

namespace UnityEngine.UI
{
	public class MenuDisplaySetting : MenuOptionHorizontal, IMoveHandler, IEventSystemHandler, IMenuOptionListSetting, IPointerClickHandler
	{
		private Display[] availableDisplays;

		public new void OnEnable()
		{
			RefreshControls();
		}

		public new void OnMove(AxisEventData move)
		{
			if (MoveOption(move.moveDir))
			{
				UpdateMonitorSetting();
			}
			else
			{
				base.OnMove(move);
			}
		}

		public new void OnPointerClick(PointerEventData eventData)
		{
			PointerClickCheckArrows(eventData);
			UpdateMonitorSetting();
		}

		public void RefreshControls()
		{
			RefreshCurrentIndex();
			PushUpdateOptionList();
			UpdateText();
		}

		public void DisableMonitorSelectSetting()
		{
			SetOptionTo(0);
			UpdateMonitorSetting();
		}

		private void UpdateMonitorSetting()
		{
			Debug.Log("UpdateMonitorSetting...");
			StartCoroutine(TargetDisplayHack(selectedOptionIndex));
		}

		public override void RefreshCurrentIndex()
		{
			availableDisplays = Display.displays;
			bool flag = false;
			for (int i = 0; i < availableDisplays.Length; i++)
			{
				if (Display.main == availableDisplays[i])
				{
					selectedOptionIndex = i;
					flag = true;
				}
			}
			if (!flag)
			{
				Debug.LogError("Could not find currently active display");
			}
			base.RefreshCurrentIndex();
		}

		public void PushUpdateOptionList()
		{
			string[] array = new string[availableDisplays.Length];
			for (int i = 0; i < availableDisplays.Length; i++)
			{
				array[i] = (i + 1).ToString();
			}
			SetOptionList(array);
		}

		private IEnumerator TargetDisplayHack(int targetDisplay)
		{
			int screenWidth = Screen.width;
			int screenHeight = Screen.height;
			PlayerPrefs.SetInt("UnitySelectMonitor", targetDisplay);
			Screen.SetResolution(800, 600, Screen.fullScreen);
			yield return null;
			Screen.SetResolution(screenWidth, screenHeight, Screen.fullScreen);
		}
	}
}
