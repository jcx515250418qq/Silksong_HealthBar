using UnityEngine;

public static class RandomTable
{
	public static bool TrySelectValue<Ty>(this Ty[] items, out Ty value) where Ty : WeightedItem
	{
		if (items.Length == 0)
		{
			value = null;
			return false;
		}
		float num = 0f;
		foreach (Ty val in items)
		{
			num += val.Weight;
		}
		float num2 = Random.Range(0f, num);
		float num3 = 0f;
		for (int j = 0; j < items.Length - 1; j++)
		{
			Ty val2 = items[j];
			num3 += val2.Weight;
			if (num2 < num3)
			{
				value = val2;
				return true;
			}
		}
		value = items[^1];
		return true;
	}

	public static Ty SelectValue<Ty>(this Ty[] items) where Ty : WeightedItem
	{
		if (!items.TrySelectValue(out var value))
		{
			return null;
		}
		return value;
	}
}
