using System.Linq;
using UnityEngine;

public abstract class Rigidbody2DDisturberBase : MonoBehaviour
{
	[SerializeField]
	protected Rigidbody2D[] bodies;

	protected virtual void Awake()
	{
		if (bodies == null || bodies.Length == 0)
		{
			bodies = (from body in GetComponentsInChildren<Rigidbody2D>()
				where !body.transform.parent || !body.transform.parent.GetComponentInParent<Rigidbody2D>()
				select body).ToArray();
		}
	}
}
