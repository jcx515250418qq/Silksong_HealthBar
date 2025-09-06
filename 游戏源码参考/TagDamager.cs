using System.Collections.Generic;
using UnityEngine;

public class TagDamager : MonoBehaviour
{
	[SerializeField]
	[AssetPickerDropdown]
	private DamageTag damageTag;

	[SerializeField]
	private int hitAmountOverride;

	private readonly List<TagDamageTaker> addedTo = new List<TagDamageTaker>();

	private void OnDisable()
	{
		foreach (TagDamageTaker item in addedTo)
		{
			item.RemoveDamageTagFromStack(damageTag);
		}
		addedTo.Clear();
	}

	private void OnTriggerEnter2D(Collider2D other)
	{
		if (!damageTag)
		{
			return;
		}
		HealthManager componentInParent = other.GetComponentInParent<HealthManager>();
		if (!componentInParent)
		{
			return;
		}
		TagDamageTaker component = componentInParent.GetComponent<TagDamageTaker>();
		if (!(component == null))
		{
			if (hitAmountOverride <= 0)
			{
				addedTo.Add(component);
			}
			component.AddDamageTagToStack(damageTag, hitAmountOverride);
		}
	}

	private void OnTriggerExit2D(Collider2D other)
	{
		if (!damageTag)
		{
			return;
		}
		HealthManager componentInParent = other.GetComponentInParent<HealthManager>();
		if ((bool)componentInParent)
		{
			TagDamageTaker component = componentInParent.GetComponent<TagDamageTaker>();
			if (addedTo.Remove(component))
			{
				component.RemoveDamageTagFromStack(damageTag);
			}
		}
	}
}
