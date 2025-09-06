using UnityEngine;

[ExecuteInEditMode]
public class SetWorldScale : MonoBehaviour
{
	[SerializeField]
	private Vector3 worldScale = Vector3.one;

	private void Awake()
	{
		DoSet();
	}

	private void DoSet()
	{
		Transform transform = base.transform;
		Transform parent = transform.parent;
		if ((bool)parent)
		{
			Vector3 lossyScale = parent.lossyScale;
			transform.localScale = new Vector3(worldScale.x / lossyScale.x, worldScale.y / lossyScale.y, worldScale.z / lossyScale.z);
		}
		else
		{
			transform.localScale = worldScale;
		}
	}
}
