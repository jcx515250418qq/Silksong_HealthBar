using UnityEngine;

public class BigCentipedeSection : MonoBehaviour
{
	private BigCentipede parent;

	private MeshRenderer meshRenderer;

	private Collider2D bodyCollider;

	private bool hasLeft;

	protected void Awake()
	{
		parent = GetComponentInParent<BigCentipede>();
		meshRenderer = GetComponent<MeshRenderer>();
	}

	protected void Update()
	{
		Vector2 lhs = base.transform.position;
		if (!hasLeft)
		{
			if (Vector2.Dot(lhs, parent.Direction) > Vector2.Dot(parent.ExitPoint, parent.Direction))
			{
				meshRenderer.enabled = false;
				hasLeft = true;
			}
		}
		else if (Vector2.Dot(lhs, parent.Direction) > Vector2.Dot(parent.EntryPoint, parent.Direction) - 1.75f && Vector2.Dot(lhs, parent.Direction) < Vector2.Dot(parent.ExitPoint, parent.Direction))
		{
			meshRenderer.enabled = true;
			hasLeft = false;
		}
	}
}
