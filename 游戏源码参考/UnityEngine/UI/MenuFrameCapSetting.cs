using System.Collections.Generic;
using HKMenu;
using TeamCherry.Localization;
using UnityEngine.EventSystems;

namespace UnityEngine.UI
{
	public class MenuFrameCapSetting : MenuOptionHorizontal, IMoveHandler, IEventSystemHandler, IMenuOptionListSetting, IPointerClickHandler, ISubmitHandler
	{
		[Space]
		[SerializeField]
		private LocalisedString offOption = new LocalisedString("MainMenu", "MOH_OFF");

		private static readonly int[] FRAME_CAP_VALUES = new int[4] { -1, 30, 60, 120 };

		private List<int> runtimeRefreshRates = new List<int>();

		private FixVerticalAlign fixVerticalAlign;

		private GameSettings gs;

		private bool init;

		private int maxFrameRate;

		private new void Awake()
		{
			Init();
			PushUpdateOptionList();
		}

		public new void OnEnable()
		{
			RefreshControls();
		}

		public new void OnMove(AxisEventData move)
		{
			if (base.interactable)
			{
				if (MoveOption(move.moveDir))
				{
					UpdateFrameCapSetting();
				}
				else
				{
					base.OnMove(move);
				}
			}
		}

		public new void OnPointerClick(PointerEventData eventData)
		{
			if (base.interactable)
			{
				PointerClickCheckArrows(eventData);
				UpdateFrameCapSetting();
			}
		}

		public new void OnSubmit(BaseEventData eventData)
		{
			if (base.interactable)
			{
				MoveOption(MoveDirection.Right);
				UpdateFrameCapSetting();
			}
		}

		public void RefreshControls()
		{
			RefreshCurrentIndex();
			if (selectedOptionIndex == 0)
			{
				optionText.text = offOption;
				if ((bool)fixVerticalAlign)
				{
					fixVerticalAlign.AlignText();
				}
			}
			else
			{
				UpdateText();
			}
		}

		public void DisableFrameCapSetting()
		{
			Init();
			SetOptionTo(0);
			UpdateFrameCapSetting();
			if (base.gameObject.activeInHierarchy)
			{
				RefreshControls();
			}
		}

		public void RefreshValueFromGameSettings()
		{
			if (gs == null)
			{
				gs = GameManager.instance.gameSettings;
			}
			Init();
			int num = Mathf.Min(maxFrameRate, gs.targetFrameRate);
			if (runtimeRefreshRates.IndexOf(gs.targetFrameRate) < 0)
			{
				num = 60;
			}
			else if (num >= 0 && num <= 30)
			{
				num = 30;
			}
			if (num >= 0)
			{
				UIManager.instance.DisableVsyncSetting();
			}
			Application.targetFrameRate = num;
			if (base.gameObject.activeInHierarchy)
			{
				RefreshControls();
			}
		}

		private void Init()
		{
			if (init)
			{
				return;
			}
			init = true;
			gm = GameManager.instance;
			fixVerticalAlign = optionText.GetComponent<FixVerticalAlign>();
			runtimeRefreshRates = new List<int>();
			maxFrameRate = -1;
			Resolution[] resolutions = Screen.resolutions;
			for (int i = 0; i < resolutions.Length; i++)
			{
				Resolution resolution = resolutions[i];
				if (resolution.refreshRateRatio.value > (double)maxFrameRate)
				{
					maxFrameRate = (int)resolution.refreshRateRatio.value;
				}
			}
			int[] fRAME_CAP_VALUES = FRAME_CAP_VALUES;
			foreach (int item in fRAME_CAP_VALUES)
			{
				runtimeRefreshRates.Add(item);
			}
			if (maxFrameRate > 120)
			{
				runtimeRefreshRates.Add(maxFrameRate);
			}
			else
			{
				maxFrameRate = 120;
			}
		}

		private void UpdateFrameCapSetting()
		{
			Init();
			if (selectedOptionIndex == 0)
			{
				optionText.text = offOption;
				if ((bool)fixVerticalAlign)
				{
					fixVerticalAlign.AlignText();
				}
			}
			else
			{
				UIManager.instance.DisableVsyncSetting();
			}
			if (selectedOptionIndex >= 0 && selectedOptionIndex < runtimeRefreshRates.Count)
			{
				int targetFrameRate = (Application.targetFrameRate = runtimeRefreshRates[selectedOptionIndex]);
				GameManager.instance.gameSettings.targetFrameRate = targetFrameRate;
			}
			else
			{
				Debug.LogError("Failed to update frame cap setting. Selected index out of range.", this);
			}
		}

		public override void RefreshCurrentIndex()
		{
			bool flag = false;
			for (int i = 0; i < runtimeRefreshRates.Count; i++)
			{
				if (Application.targetFrameRate == runtimeRefreshRates[i])
				{
					selectedOptionIndex = i;
					flag = true;
				}
			}
			if (!flag)
			{
				selectedOptionIndex = -1;
				Debug.LogError("Couldn't match current Target Frame Rate setting - " + Application.targetFrameRate);
			}
			base.RefreshCurrentIndex();
		}

		public void PushUpdateOptionList()
		{
			Init();
			string[] array = optionList;
			if (array.Length != runtimeRefreshRates.Count)
			{
				array = new string[runtimeRefreshRates.Count];
			}
			for (int i = 0; i < runtimeRefreshRates.Count; i++)
			{
				int num = runtimeRefreshRates[i];
				if (num <= 0)
				{
					array[i] = offOption;
				}
				else
				{
					array[i] = num.ToString();
				}
			}
			SetOptionList(array);
		}
	}
}
