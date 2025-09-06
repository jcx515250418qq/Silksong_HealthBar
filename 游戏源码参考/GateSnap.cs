using UnityEngine;

[ExecuteInEditMode]
public class GateSnap : MonoBehaviour
{
	private float snapX = 0.5f;

	private float snapY = 0.5f;

	private void Update()
	{
		Vector2 vector = base.transform.position;
		vector.x = Mathf.Round(vector.x / snapX) * snapX;
		vector.y = Mathf.Round(vector.y / snapY) * snapY;
		base.transform.position = vector;
	}
}
