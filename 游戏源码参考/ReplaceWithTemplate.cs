using UnityEngine;

public class ReplaceWithTemplate : MonoBehaviour
{
	[SerializeField]
	private GameObject template;

	public void Awake()
	{
		if ((bool)template)
		{
			Transform obj = base.transform;
			Transform parent = obj.parent;
			Vector3 localPosition = obj.localPosition;
			Vector3 localScale = obj.localScale;
			Quaternion localRotation = obj.localRotation;
			Transform obj2 = Object.Instantiate(template, parent).transform;
			obj2.localPosition = localPosition;
			obj2.localScale = localScale;
			obj2.localRotation = localRotation;
			Object.DestroyImmediate(base.gameObject);
		}
	}
}
