using UnityEngine;

public class SetWorldTransform : MonoBehaviour
{
	[SerializeField]
	private Vector3 eulerAngles;

	[SerializeField]
	private Vector3 scale;

	[SerializeField]
	private bool deParent;

	private void Start()
	{
		Transform transform = base.transform;
		Transform parent = transform.parent;
		transform.SetParent(null, worldPositionStays: true);
		transform.localEulerAngles = eulerAngles;
		transform.localScale = scale;
		if (!deParent)
		{
			transform.SetParent(parent, worldPositionStays: true);
		}
	}
}
