using System.Collections.Generic;
using UnityEngine;

public class DamageStack
{
	private readonly List<float> multipliers = new List<float>();

	private float totalOffset;

	public int BaseDamage { get; private set; }

	public void SetupNew(int baseDamage)
	{
		BaseDamage = baseDamage;
		multipliers.Clear();
		totalOffset = 0f;
	}

	public void AddMultiplier(float multiplier)
	{
		multipliers.Add(multiplier);
	}

	public void AddOffset(float offset)
	{
		totalOffset += offset;
	}

	public int PopDamage()
	{
		float num = BaseDamage;
		for (int i = 0; i < multipliers.Count; i++)
		{
			float num2 = multipliers[i];
			float value = num * num2 - num;
			multipliers[i] = value;
		}
		foreach (float multiplier in multipliers)
		{
			num += multiplier;
		}
		num += totalOffset;
		SetupNew(0);
		return Mathf.RoundToInt(num);
	}
}
