using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

[ExecuteInEditMode]
public class IconCounter : MonoBehaviour
{
	[Header("Required")]
	[SerializeField]
	private IconCounterItem templateItem;

	[Header("Layout")]
	[SerializeField]
	private LayoutGroup layoutGroup;

	[SerializeField]
	[FormerlySerializedAs("itemOffset")]
	[ModifiableProperty]
	[Conditional("layoutGroup", false, false, false)]
	private Vector2 maxItemOffset;

	[SerializeField]
	[ModifiableProperty]
	[Conditional("layoutGroup", false, false, false)]
	private int rowSplit;

	[SerializeField]
	[ModifiableProperty]
	[Conditional("layoutGroup", false, false, false)]
	private Vector2 maxSize;

	[SerializeField]
	private bool centreHorizontal;

	[Header("Parameters")]
	[SerializeField]
	private int maxValue;

	[SerializeField]
	private int currentValue;

	private Func<int, bool> getIsFilledFunc;

	private Func<int, Sprite> getSpriteFunc;

	private List<IconCounterItem> items;

	public int MaxValue
	{
		get
		{
			return maxValue;
		}
		set
		{
			int num = Mathf.Max(value, 0);
			if (maxValue != num)
			{
				maxValue = num;
				Setup();
			}
		}
	}

	public int CurrentValue
	{
		get
		{
			return currentValue;
		}
		set
		{
			int num = Mathf.Clamp(value, 0, maxValue);
			if (currentValue != num)
			{
				currentValue = num;
				UpdateStates();
			}
		}
	}

	public int RowSplit
	{
		get
		{
			return rowSplit;
		}
		set
		{
			if (rowSplit != value)
			{
				rowSplit = value;
				Setup();
			}
		}
	}

	public Vector2 MaxItemOffset
	{
		get
		{
			return maxItemOffset;
		}
		set
		{
			if (!(maxItemOffset == value))
			{
				maxItemOffset = value;
				Setup();
			}
		}
	}

	private void OnDrawGizmosSelected()
	{
		if (!layoutGroup)
		{
			Gizmos.matrix = base.transform.localToWorldMatrix;
			Vector3 size = maxSize;
			Gizmos.DrawWireCube(maxSize / 2f, size);
		}
	}

	private void OnValidate()
	{
		if (base.enabled && base.gameObject.activeInHierarchy)
		{
			if (currentValue < 0)
			{
				currentValue = 0;
			}
			else if (currentValue > maxValue)
			{
				currentValue = maxValue;
			}
			if (maxValue < 0)
			{
				maxValue = 0;
			}
			Setup();
		}
	}

	private void OnEnable()
	{
		Setup();
	}

	private void Setup(bool destroyExisting = false)
	{
		GetItems(destroyExisting);
		DoLayout();
		UpdateStates();
	}

	private void GetItems(bool destroyExisting = false)
	{
		if (!templateItem)
		{
			items = null;
			return;
		}
		items = new List<IconCounterItem>();
		List<IconCounterItem> list = new List<IconCounterItem>(base.transform.childCount);
		foreach (Transform item in base.transform)
		{
			IconCounterItem component = item.GetComponent<IconCounterItem>();
			if ((bool)component && component != templateItem)
			{
				list.Add(component);
			}
		}
		if (destroyExisting)
		{
			for (int num = list.Count - 1; num >= 0; num--)
			{
				UnityEngine.Object.DestroyImmediate(list[num].gameObject);
			}
			list.Clear();
		}
		items.AddRange(list);
		int i = maxValue - items.Count;
		templateItem.gameObject.SetActive(value: true);
		while (i > 0)
		{
			IconCounterItem iconCounterItem = UnityEngine.Object.Instantiate(templateItem);
			Transform obj = iconCounterItem.transform;
			obj.SetParent(base.transform);
			obj.localPosition = Vector3.zero;
			items.Add(iconCounterItem);
			i--;
		}
		templateItem.gameObject.SetActive(value: false);
		for (; i < 0; i++)
		{
			int index = items.Count - 1;
			items[index].gameObject.SetActive(value: false);
			items.RemoveAt(index);
		}
		for (int j = 0; j < items.Count; j++)
		{
			IconCounterItem iconCounterItem2 = items[j];
			iconCounterItem2.gameObject.SetActive(value: true);
			if (getSpriteFunc != null)
			{
				iconCounterItem2.Sprite = getSpriteFunc(j);
			}
		}
	}

	private void DoLayout()
	{
		if ((bool)layoutGroup)
		{
			layoutGroup.ForceUpdateLayoutNoCanvas();
		}
		else
		{
			if (items == null || items.Count == 0)
			{
				return;
			}
			int num = ((rowSplit > 0) ? Mathf.Min(rowSplit, items.Count) : items.Count);
			int num2 = ((items.Count <= num) ? 1 : Mathf.CeilToInt((float)items.Count / (float)rowSplit));
			Vector2 vector = maxItemOffset.MultiplyElements(num, num2);
			Vector2 other = new Vector2((Mathf.Abs(maxSize.x) > 0f) ? (maxSize.x / vector.x) : vector.x, (Mathf.Abs(maxSize.y) > 0f) ? (maxSize.y / vector.y) : vector.y);
			if (Mathf.Abs(other.x) > 1f)
			{
				other.x = 1f;
			}
			if (Mathf.Abs(other.y) > 1f)
			{
				other.y = 1f;
			}
			Vector2 vector2 = maxItemOffset.MultiplyElements(other);
			for (int i = 0; i < items.Count; i++)
			{
				Transform t = items[i].transform;
				if (num > 0)
				{
					int num3 = i % num;
					int num4 = i / num;
					float num7;
					if (centreHorizontal)
					{
						int num5 = num2 - 1;
						int num6;
						if (num4 >= num5)
						{
							num6 = items.Count;
							for (int j = 0; j < num5; j++)
							{
								num6 -= num;
							}
						}
						else
						{
							num6 = num;
						}
						num7 = (float)num6 * vector2.x * -0.5f + vector2.x * 0.5f;
					}
					else
					{
						num7 = 0f;
					}
					t.SetLocalPosition2D(new Vector2(vector2.x * (float)num3 + num7, vector2.y * (float)num4));
				}
				else
				{
					t.SetLocalPosition2D(new Vector2(vector2.x * (float)i, 0f));
				}
			}
		}
	}

	private void UpdateStates()
	{
		if (items != null)
		{
			for (int i = 0; i < items.Count; i++)
			{
				items[i].SetFilled(getIsFilledFunc?.Invoke(i) ?? (i < currentValue));
			}
		}
	}

	public void SetFilledOverride(Func<int, bool> getIsFilled)
	{
		getIsFilledFunc = getIsFilled;
		UpdateStates();
	}

	public void SetItemSprites(Func<int, Sprite> getSprite, float scale)
	{
		getSpriteFunc = getSprite;
		for (int i = 0; i < items.Count; i++)
		{
			SetSprite(items[i], i);
		}
		void SetSprite(IconCounterItem item, int index)
		{
			item.Sprite = getSpriteFunc(index);
			item.Scale = new Vector3(scale, scale, 1f);
		}
	}

	public void SetColor(Color color)
	{
		foreach (IconCounterItem item in items)
		{
			item.TintColor = color;
		}
	}

	[ContextMenu("Destroy Existing")]
	public void DestroyExisting()
	{
		Setup(destroyExisting: true);
	}

	public void SetMaxComplete(int value)
	{
		maxValue = value;
		Setup(destroyExisting: true);
	}

	public void SetCurrent(int value)
	{
		CurrentValue = value;
	}

	public void IncrementCurrent()
	{
		CurrentValue++;
	}
}
