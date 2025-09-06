using UnityEngine;

public abstract class MenuButtonListCondition : MonoBehaviour
{
	private MenuButtonListCondition[] components;

	public abstract bool IsFulfilled();

	public bool IsFulfilledAllComponents()
	{
		if (!IsFulfilled())
		{
			return false;
		}
		if (components == null)
		{
			components = GetComponents<MenuButtonListCondition>();
		}
		MenuButtonListCondition[] array = components;
		for (int i = 0; i < array.Length; i++)
		{
			if (!array[i].IsFulfilled())
			{
				return false;
			}
		}
		return true;
	}

	public virtual bool AlwaysVisible()
	{
		return false;
	}

	public virtual void OnActiveStateSet(bool isActive)
	{
	}
}
