using UnityEngine;

public class InstantiateAsChild : MonoBehaviour
{
	[SerializeField]
	private Transform transformOverride;

	[SerializeField]
	private GameObject prefab;

	[SerializeField]
	private bool usePrefabLocalPosition;

	[SerializeField]
	[ModifiableProperty]
	[Conditional("usePrefabLocalPosition", false, false, false)]
	private Vector3 localPosition;

	[SerializeField]
	private Vector3 scaleMultiplier = Vector3.one;

	private void Awake()
	{
		if ((bool)prefab)
		{
			Transform parent = (transformOverride ? transformOverride : base.transform);
			Vector3 vector = (usePrefabLocalPosition ? prefab.transform.localPosition : localPosition);
			Transform obj = Object.Instantiate(prefab, parent).transform;
			obj.localPosition = vector;
			obj.localScale = obj.localScale.MultiplyElements(scaleMultiplier);
		}
	}
}
