using UnityEngine;

public class HeroDetect : MonoBehaviour
{
	public delegate void ColliderEvent(Collider2D collider);

	public event ColliderEvent OnEnter;

	public event ColliderEvent OnExit;

	private void OnTriggerEnter2D(Collider2D collision)
	{
		if (this.OnEnter != null)
		{
			this.OnEnter(collision);
		}
	}

	private void OnTriggerExit2D(Collider2D collision)
	{
		if (this.OnExit != null)
		{
			this.OnExit(collision);
		}
	}
}
