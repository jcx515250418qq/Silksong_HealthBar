using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class RegionDebugger : MonoBehaviour
{
	private void Start()
	{
		Debug.LogErrorFormat(this, "Region debugger is exists in scene! These should be removed before release.");
	}
}
