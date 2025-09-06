using System;
using TMProOld;
using UnityEngine;
using UnityEngine.UI;

namespace TeamCherry.DebugMenu
{
	public class DebugMenuButton : MonoBehaviour
	{
		[SerializeField]
		private TextMeshProUGUI text;

		[SerializeField]
		private Button button;

		public TextMeshProUGUI Text
		{
			get
			{
				return text;
			}
			set
			{
				text = value;
			}
		}

		public Button Button
		{
			get
			{
				return button;
			}
			set
			{
				button = value;
			}
		}

		private void OnValidate()
		{
			if (!text)
			{
				text = base.gameObject.GetComponentInChildren<TextMeshProUGUI>();
			}
			if (!button)
			{
				button = base.gameObject.GetComponentInChildren<Button>();
			}
		}

		public void DoButton(string text, Action onClick)
		{
			this.text.text = text;
			button.onClick.AddListener(delegate
			{
				onClick?.Invoke();
			});
		}
	}
}
