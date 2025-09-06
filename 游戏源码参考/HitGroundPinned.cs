using System;
using UnityEngine;
using UnityEngine.Events;

public class HitGroundPinned : MonoBehaviour
{
	[SerializeField]
	private Rigidbody2D body;

	[SerializeField]
	private TinkEffect tinker;

	[SerializeField]
	private ObjectBounce objectBounce;

	[SerializeField]
	private GameObject terrainDetector;

	[Space]
	[SerializeField]
	private Vector2 hitVelocityHMin;

	[SerializeField]
	private Vector2 hitVelocityHMax;

	[SerializeField]
	private Vector2 hitVelocityVMin;

	[SerializeField]
	private Vector2 hitVelocityVMax;

	[SerializeField]
	private CogRollThenFallOver fallOverControl;

	[SerializeField]
	[Range(0f, 1f)]
	private float fallChancePerHit = 1f;

	[Space]
	public UnityEvent OnUnpinned;

	[Space]
	public UnityEvent OnHit;

	private void Awake()
	{
		if (!base.transform.IsOnHeroPlane())
		{
			if ((bool)tinker)
			{
				UnityEngine.Object.Destroy(tinker);
			}
			if ((bool)objectBounce)
			{
				UnityEngine.Object.Destroy(objectBounce);
			}
			Collider2D[] componentsInChildren = GetComponentsInChildren<Collider2D>(includeInactive: true);
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				UnityEngine.Object.Destroy(componentsInChildren[i]);
			}
			if ((bool)body)
			{
				UnityEngine.Object.Destroy(body);
			}
			UnityEngine.Object.Destroy(this);
			return;
		}
		if ((bool)body)
		{
			body.bodyType = RigidbodyType2D.Static;
		}
		if ((bool)tinker)
		{
			tinker.HitInDirection += OnHitInDirection;
			if ((bool)fallOverControl)
			{
				fallOverControl.Fallen += delegate
				{
					tinker.enabled = false;
				};
			}
		}
		if ((bool)objectBounce)
		{
			objectBounce.StopBounce();
		}
		if ((bool)terrainDetector)
		{
			terrainDetector.SetActive(value: false);
		}
	}

	private void OnHitInDirection(GameObject source, HitInstance.HitDirection direction)
	{
		if (!body)
		{
			return;
		}
		Vector2 linearVelocity;
		switch (direction)
		{
		case HitInstance.HitDirection.Left:
			linearVelocity = new Vector2(0f - UnityEngine.Random.Range(hitVelocityHMin.x, hitVelocityHMax.x), UnityEngine.Random.Range(hitVelocityHMin.y, hitVelocityHMax.y));
			break;
		case HitInstance.HitDirection.Right:
			linearVelocity = new Vector2(UnityEngine.Random.Range(hitVelocityHMin.x, hitVelocityHMax.x), UnityEngine.Random.Range(hitVelocityHMin.y, hitVelocityHMax.y));
			break;
		case HitInstance.HitDirection.Up:
		case HitInstance.HitDirection.Down:
			linearVelocity = new Vector2(UnityEngine.Random.Range(hitVelocityVMin.x, hitVelocityVMax.x), UnityEngine.Random.Range(hitVelocityVMin.y, hitVelocityVMax.y));
			break;
		default:
			throw new NotImplementedException();
		}
		if (body.bodyType != 0)
		{
			body.bodyType = RigidbodyType2D.Dynamic;
			Vector2 position = body.position;
			position += new Vector2(0f, 0.5f);
			body.position = position;
			body.transform.SetPosition2D(position);
			body.transform.SetLocalPositionZ(UnityEngine.Random.Range(0.003f, 0.006f));
			body.linearVelocity = linearVelocity;
			OnUnpinned.Invoke();
			if ((bool)objectBounce)
			{
				objectBounce.StartBounce();
			}
			if ((bool)terrainDetector)
			{
				terrainDetector.SetActive(value: true);
			}
		}
		else
		{
			body.linearVelocity = linearVelocity;
		}
		OnHit.Invoke();
		if ((bool)fallOverControl && UnityEngine.Random.Range(0f, 1f) < fallChancePerHit)
		{
			fallOverControl.Activate();
		}
	}
}
