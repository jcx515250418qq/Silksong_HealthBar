using UnityEngine;
using UnityEngine.Events;

public class DetectGameObjectEntry : MonoBehaviour
{
	[SerializeField]
	private GameObject target;

	[Space]
	public UnityEvent OnTargetEntered;

	public void FindTarget(string targetName)
	{
		target = GameObject.Find(targetName);
	}

	private void OnTriggerEnter2D(Collider2D collision)
	{
		if (!(target == null) && IsTarget(collision.gameObject))
		{
			OnTargetEntered.Invoke();
		}
	}

	private bool IsTarget(GameObject obj)
	{
		if (obj == target)
		{
			return true;
		}
		Transform parent = obj.transform.parent;
		if ((bool)parent)
		{
			return IsTarget(parent.gameObject);
		}
		return false;
	}
}
