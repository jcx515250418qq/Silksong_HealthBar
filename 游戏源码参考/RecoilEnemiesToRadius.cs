using System.Collections.Generic;
using UnityEngine;

public class RecoilEnemiesToRadius : MonoBehaviour
{
	private struct EnemyData
	{
		public GameObject Obj;

		public Transform Transform;

		public Collider2D Collider;

		public Recoil Recoil;

		public Rigidbody2D Body;
	}

	[SerializeField]
	private Vector2 offset;

	[SerializeField]
	private float innerRadius;

	[SerializeField]
	private float outerRadius;

	[SerializeField]
	private AnimationCurve innerForceCurve = AnimationCurve.Linear(0f, 1f, 1f, 0f);

	[SerializeField]
	private AnimationCurve outerForceCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);

	[SerializeField]
	private float recoilMultipler = 1f;

	private readonly List<EnemyData> enemies = new List<EnemyData>();

	public Vector3 Position => base.transform.TransformPoint(offset);

	public float ScaledInnerRadius => base.transform.TransformRadius(innerRadius);

	public float ScaledOuterRadius => base.transform.TransformRadius(outerRadius);

	private void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.yellow;
		Gizmos.DrawWireSphere(Position, ScaledInnerRadius);
		Gizmos.color = Color.green;
		Gizmos.DrawWireSphere(Position, ScaledOuterRadius);
	}

	private void OnValidate()
	{
		if (outerRadius < innerRadius)
		{
			outerRadius = innerRadius;
		}
	}

	private void OnEnable()
	{
		enemies.Clear();
		int collidingLayerMaskForLayer = Helper.GetCollidingLayerMaskForLayer(base.gameObject.layer);
		Collider2D[] components = GetComponents<Collider2D>();
		foreach (Collider2D obj in components)
		{
			Collider2D[] array = new Collider2D[10];
			if (obj.Overlap(new ContactFilter2D
			{
				useTriggers = true,
				useLayerMask = true,
				layerMask = collidingLayerMaskForLayer
			}, array) <= 0)
			{
				continue;
			}
			Collider2D[] array2 = array;
			foreach (Collider2D collider2D in array2)
			{
				if ((bool)collider2D)
				{
					OnTriggerEnter2D(collider2D);
				}
			}
		}
	}

	private void OnTriggerEnter2D(Collider2D collision)
	{
		GameObject gameObject = collision.gameObject;
		foreach (EnemyData enemy in enemies)
		{
			if (enemy.Obj == gameObject)
			{
				return;
			}
		}
		Recoil component = gameObject.GetComponent<Recoil>();
		if ((bool)component)
		{
			enemies.Add(new EnemyData
			{
				Obj = gameObject,
				Transform = gameObject.transform,
				Collider = collision,
				Recoil = component,
				Body = gameObject.GetComponent<Rigidbody2D>()
			});
		}
	}

	private void OnTriggerExit2D(Collider2D collision)
	{
		int num = enemies.FindIndex((EnemyData e) => e.Obj == collision.gameObject);
		if (num >= 0)
		{
			enemies.RemoveAt(num);
		}
	}

	private void FixedUpdate()
	{
		foreach (EnemyData enemy in enemies)
		{
			RecoilEnemy(enemy);
		}
	}

	private void RecoilEnemy(EnemyData enemy)
	{
		if (!enemy.Recoil.FreezeInPlace)
		{
			float num = enemy.Recoil.RecoilSpeedBase * recoilMultipler;
			Vector2 vector = enemy.Transform.position - Position;
			Vector2 vector2 = vector.normalized * (num * Time.deltaTime);
			float magnitude = vector.magnitude;
			float scaledInnerRadius = ScaledInnerRadius;
			float scaledOuterRadius = ScaledOuterRadius;
			if (magnitude < scaledInnerRadius)
			{
				float time = Mathf.Clamp01(magnitude / scaledInnerRadius);
				vector2 *= innerForceCurve.Evaluate(time);
			}
			else
			{
				float num2 = scaledOuterRadius - scaledInnerRadius;
				float time2 = Mathf.Clamp01((magnitude - scaledInnerRadius) / num2);
				vector2 *= outerForceCurve.Evaluate(time2) * -1f;
			}
			Sweep sweepForwards = new Sweep(enemy.Collider, (vector2.x < 0f) ? 2 : 0, 3);
			Sweep sweepForwards2 = new Sweep(enemy.Collider, (!(vector2.y < 0f)) ? 1 : 3, 3);
			Sweep sweepBackwards = new Sweep(enemy.Collider, (vector2.x > 0f) ? 2 : 0, 3);
			Sweep sweepBackwards2 = new Sweep(enemy.Collider, (!(vector2.y > 0f)) ? 1 : 3, 3);
			CheckInDirection(sweepForwards, sweepBackwards, ref vector2.x);
			CheckInDirection(sweepForwards2, sweepBackwards2, ref vector2.y);
			if (enemy.Recoil.IsUpBlocked && vector2.y > 0f)
			{
				vector2.y = 0f;
			}
			if (enemy.Recoil.IsDownBlocked && vector2.y < 0f)
			{
				vector2.y = 0f;
			}
			if (enemy.Recoil.IsLeftBlocked && vector2.x < 0f)
			{
				vector2.x = 0f;
			}
			if (enemy.Recoil.IsRightBlocked && vector2.x > 0f)
			{
				vector2.x = 0f;
			}
			if ((bool)enemy.Body && Mathf.Abs(enemy.Body.gravityScale) > 0f && vector2.y > 0f)
			{
				vector2.y = 0f;
			}
			enemy.Transform.Translate(vector2);
		}
	}

	private static void CheckInDirection(Sweep sweepForwards, Sweep sweepBackwards, ref float moveAmount)
	{
		float distance = Mathf.Abs(moveAmount);
		if (sweepForwards.Check(distance, 256, out var clippedDistance) && (!sweepBackwards.Check(distance, 256, out var clippedDistance2) || clippedDistance2 < clippedDistance))
		{
			moveAmount = clippedDistance * Mathf.Sign(moveAmount);
		}
	}
}
