using UnityEngine;

[RequireComponent(typeof(PolygonCollider2D))]
public class GradeZone : MonoBehaviour
{
	private void Start()
	{
		Debug.LogError("GrazeZone has been deprecated, please remove this object: " + base.name);
	}
}
