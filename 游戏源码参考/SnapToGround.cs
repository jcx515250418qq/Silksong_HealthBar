using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class SnapToGround : MonoBehaviour
{
	private Collider2D col;

	private void Awake()
	{
		col = GetComponent<Collider2D>();
	}

	private void OnEnable()
	{
		float y = col.bounds.min.y;
		float num = base.transform.position.y - y;
		RaycastHit2D raycastHit2D = Helper.Raycast2D(base.transform.position, Vector2.down, 10f, 256);
		if (raycastHit2D.collider != null)
		{
			base.transform.SetPositionY(raycastHit2D.point.y + num);
		}
	}
}
