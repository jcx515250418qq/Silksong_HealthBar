using UnityEngine;

public class SetGravityOnEnable : MonoBehaviour
{
	public float gravity;

	private void Start()
	{
		SetGravity();
	}

	private void OnEnable()
	{
		SetGravity();
	}

	private void SetGravity()
	{
		Rigidbody2D component = GetComponent<Rigidbody2D>();
		if ((bool)component)
		{
			component.gravityScale = gravity;
		}
	}
}
