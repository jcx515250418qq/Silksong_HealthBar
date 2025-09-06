using UnityEngine;

public class IgnoreCollision : MonoBehaviour
{
	[SerializeField]
	private GameObject obj1;

	[SerializeField]
	[ModifiableProperty]
	[Conditional("getParent", false, false, false)]
	private GameObject obj2;

	[SerializeField]
	private bool getParent;

	private void Reset()
	{
		obj1 = base.gameObject;
	}

	private void Awake()
	{
		if (getParent)
		{
			Transform parent = base.transform.parent;
			if (!parent)
			{
				Debug.LogError("Did not have parent!", this);
				return;
			}
			obj2 = parent.gameObject;
		}
		if (!obj1 || !obj2)
		{
			Debug.LogError("Both objects need to be assigned to ignore collision!", this);
			return;
		}
		Collider2D[] componentsInChildren = obj1.GetComponentsInChildren<Collider2D>(includeInactive: true);
		Collider2D[] componentsInChildren2 = obj2.GetComponentsInChildren<Collider2D>(includeInactive: true);
		Collider2D[] array = componentsInChildren;
		foreach (Collider2D collider in array)
		{
			Collider2D[] array2 = componentsInChildren2;
			foreach (Collider2D collider2 in array2)
			{
				Physics2D.IgnoreCollision(collider, collider2);
			}
		}
	}
}
