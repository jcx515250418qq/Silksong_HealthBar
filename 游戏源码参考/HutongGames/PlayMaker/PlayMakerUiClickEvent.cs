using UnityEngine;
using UnityEngine.UI;

namespace HutongGames.PlayMaker
{
	[AddComponentMenu("PlayMaker/UI/UI Click Event")]
	public class PlayMakerUiClickEvent : PlayMakerUiEventBase
	{
		public Button button;

		protected override void Initialize()
		{
			if (!initialized)
			{
				initialized = true;
				if (button == null)
				{
					button = GetComponent<Button>();
				}
				if (button != null)
				{
					button.onClick.AddListener(DoOnClick);
				}
			}
		}

		protected void OnDisable()
		{
			initialized = false;
			if (button != null)
			{
				button.onClick.RemoveListener(DoOnClick);
			}
		}

		private void DoOnClick()
		{
			SendEvent(FsmEvent.UiClick);
		}
	}
}
