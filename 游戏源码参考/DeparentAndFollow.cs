using UnityEngine;

public class DeparentAndFollow : MonoBehaviour
{
	private Transform parent;

	private Vector3 offset;

	private void Start()
	{
		parent = base.transform.parent;
		offset = base.transform.localPosition;
		base.transform.parent = null;
		if (parent == null)
		{
			base.enabled = false;
		}
	}

	private void Update()
	{
		if (parent == null)
		{
			base.gameObject.SetActive(value: false);
		}
		else
		{
			base.transform.position = parent.position + offset;
		}
	}
}
