using UnityEngine;

public class MatchColliderSize : MonoBehaviour
{
	public BoxCollider2D target;

	private bool hasStarted;

	private void Start()
	{
		hasStarted = true;
		DoMatch();
	}

	private void OnEnable()
	{
		if (hasStarted)
		{
			DoMatch();
		}
	}

	private void DoMatch()
	{
		BoxCollider2D component = GetComponent<BoxCollider2D>();
		if ((bool)component)
		{
			component.offset = target.offset;
			component.size = target.size;
		}
	}
}
