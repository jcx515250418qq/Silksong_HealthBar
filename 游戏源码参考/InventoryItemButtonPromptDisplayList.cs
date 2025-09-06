using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;

public class InventoryItemButtonPromptDisplayList : MonoBehaviour
{
	public interface IPromptDisplayListOrder
	{
		int order { get; set; }

		Transform transform { get; }
	}

	private class PromptOrderComparer : IComparer<IPromptDisplayListOrder>
	{
		public int Compare(IPromptDisplayListOrder x, IPromptDisplayListOrder y)
		{
			return x.order.CompareTo(y.order);
		}
	}

	[FormerlySerializedAs("promptTemplate")]
	[SerializeField]
	private InventoryItemButtonPromptDisplay promptTemplateSingle;

	[SerializeField]
	private InventoryItemComboButtonPromptDisplay promptTemplateCombo;

	private readonly List<KeyValuePair<Type, GameObject>> promptDisplays = new List<KeyValuePair<Type, GameObject>>();

	private static readonly PromptOrderComparer orderComparer = new PromptOrderComparer();

	private List<IPromptDisplayListOrder> activeDisplays = new List<IPromptDisplayListOrder>();

	public int order { get; set; }

	private void Awake()
	{
		if ((bool)promptTemplateSingle)
		{
			promptTemplateSingle.gameObject.SetActive(value: false);
		}
		if ((bool)promptTemplateCombo)
		{
			promptTemplateCombo.gameObject.SetActive(value: false);
		}
	}

	public void Append(InventoryItemButtonPromptData promptData, bool forceDisabled, int order = 0)
	{
		InventoryItemButtonPromptDisplay display = GetDisplay(promptTemplateSingle);
		display.order = order;
		display.Show(promptData, forceDisabled);
		AddAndOrder(display);
	}

	public void Append(InventoryItemComboButtonPromptDisplay.Display promptData, int order = 0)
	{
		InventoryItemComboButtonPromptDisplay display = GetDisplay(promptTemplateCombo);
		display.order = order;
		display.Show(promptData);
		AddAndOrder(display);
	}

	private void AddAndOrder(IPromptDisplayListOrder display)
	{
		int i = activeDisplays.BinarySearch(display, orderComparer);
		if (i >= 0)
		{
			for (; i < activeDisplays.Count && activeDisplays[i].order == display.order; i++)
			{
				if (activeDisplays[i].Equals(display))
				{
					return;
				}
			}
		}
		else
		{
			i = ~i;
		}
		activeDisplays.Insert(i, display);
		if (i > 0)
		{
			Transform transform = activeDisplays[i - 1].transform;
			Transform transform2 = display.transform;
			if (transform2 != null && transform != null)
			{
				transform2.SetSiblingIndex(transform.GetSiblingIndex() + 1);
			}
		}
	}

	private T GetDisplay<T>(T template) where T : MonoBehaviour, IPromptDisplayListOrder
	{
		if (!template)
		{
			return null;
		}
		GameObject gameObject = (from kvp in promptDisplays
			where kvp.Key == typeof(T)
			select kvp.Value).FirstOrDefault((GameObject obj) => !obj.activeSelf);
		T val;
		if (gameObject != null)
		{
			val = gameObject.GetComponent<T>();
		}
		else
		{
			template.gameObject.SetActive(value: true);
			val = UnityEngine.Object.Instantiate(template, template.transform.parent);
			template.gameObject.SetActive(value: false);
			promptDisplays.Add(new KeyValuePair<Type, GameObject>(typeof(T), val.gameObject));
		}
		return val;
	}

	public void Clear()
	{
		foreach (KeyValuePair<Type, GameObject> promptDisplay in promptDisplays)
		{
			promptDisplay.Value.SetActive(value: false);
		}
		activeDisplays.Clear();
	}
}
