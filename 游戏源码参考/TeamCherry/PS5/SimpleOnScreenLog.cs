using System.Collections.Generic;
using TMProOld;
using UnityEngine;
using UnityEngine.Pool;

namespace TeamCherry.PS5
{
	public class SimpleOnScreenLog : MonoBehaviour, IMessagePrinter
	{
		public TextMeshProUGUI text;

		private ObjectPool<TextMeshProUGUI> textPool;

		public int maxLines = 40;

		private List<TextMeshProUGUI> activeList = new List<TextMeshProUGUI>();

		private void Awake()
		{
			PlaystationLogHandler.Printer = this;
			textPool = new ObjectPool<TextMeshProUGUI>(CreateFunc, ActionOnGet, ActionOnRelease);
			if ((bool)text)
			{
				text.gameObject.SetActive(value: false);
			}
		}

		private void OnDestroy()
		{
			if ((SimpleOnScreenLog)PlaystationLogHandler.Printer == this)
			{
				PlaystationLogHandler.Printer = null;
			}
		}

		private TextMeshProUGUI CreateFunc()
		{
			if ((bool)text)
			{
				return Object.Instantiate(text, base.transform);
			}
			return new GameObject().AddComponent<TextMeshProUGUI>();
		}

		private void ActionOnRelease(TextMeshProUGUI obj)
		{
			obj.gameObject.SetActive(value: false);
			activeList.Remove(obj);
		}

		private void ActionOnGet(TextMeshProUGUI obj)
		{
			obj.gameObject.SetActive(value: true);
			obj.gameObject.transform.SetAsLastSibling();
			activeList.Add(obj);
		}

		private TextMeshProUGUI GetText()
		{
			if (activeList.Count >= maxLines)
			{
				textPool.Release(activeList[0]);
			}
			return textPool.Get();
		}

		public void PrintMessage(Message message)
		{
			TextMeshProUGUI textMeshProUGUI = GetText();
			textMeshProUGUI.text = message.message;
			textMeshProUGUI.color = message.color;
		}
	}
}
