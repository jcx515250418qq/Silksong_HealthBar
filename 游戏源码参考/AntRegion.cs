using System;
using System.Collections;
using System.Collections.Generic;
using TeamCherry.SharedUtils;
using UnityEngine;

public class AntRegion : MonoBehaviour
{
	public interface ICheck
	{
		bool CanEnterAntRegion { get; }

		bool TryGetRenderer(out Renderer renderer)
		{
			renderer = null;
			return false;
		}
	}

	public interface INotify
	{
		void EnteredAntRegion(AntRegion antRegion);

		void ExitedAntRegion(AntRegion antRegion);
	}

	public interface IPickUpNotify
	{
		void PickUpStarted(AntRegion antRegion);

		void PickUpEnd(AntRegion antRegion);
	}

	private struct CarryTracker
	{
		public GameObject GameObject;

		public Rigidbody2D Body;

		public RigidbodyType2D InitialBodyType;

		public float InitialAngle;

		public Coroutine Routine;

		public ParticleSystem CarryAnts;
	}

	private enum ParticleFlipMode
	{
		Scale = 0,
		Rotation = 1
	}

	private enum CarryDirection
	{
		AwayFromHero = 0,
		Left = 1,
		Right = 2
	}

	[SerializeField]
	private ParticleSystem antParticleTemplate;

	[SerializeField]
	private ParticleFlipMode antParticleFlipMode;

	[SerializeField]
	private ParticleSystem pickupAntParticleTemplate;

	[SerializeField]
	private MinMaxFloat pickupAntsPerUnit;

	[SerializeField]
	private float pickupAntsWidth;

	[SerializeField]
	private Transform antLeftPoint;

	[SerializeField]
	private Transform antRightPoint;

	[SerializeField]
	private TriggerEnterEvent pickupRegion;

	[SerializeField]
	private MinMaxFloat pickupWaitTime;

	[SerializeField]
	private MinMaxFloat pickupHeight;

	[SerializeField]
	private MinMaxFloat pickupDuration;

	[SerializeField]
	private AnimationCurve pickupCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);

	[SerializeField]
	private MinMaxFloat pickupMoveWaitTime;

	[SerializeField]
	private MinMaxFloat pickupMoveSpeed;

	[SerializeField]
	private CarryDirection carryDirection;

	[SerializeField]
	private AnimationCurve moveWobbleCurve = AnimationCurve.Constant(0f, 1f, 0f);

	[SerializeField]
	private AnimationCurve moveBobCurve = AnimationCurve.Constant(0f, 1f, 0f);

	[SerializeField]
	private MinMaxFloat sinkWaitTime;

	[SerializeField]
	private MinMaxFloat sinkSpeed;

	[SerializeField]
	private AnimationCurve sinkCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);

	private bool hasStoppedEmitting;

	private ParticleSystem leftParticle;

	private ParticleSystem rightParticle;

	private readonly List<CarryTracker> carryTrackers = new List<CarryTracker>();

	private readonly List<ParticleSystem> pickupAnts = new List<ParticleSystem>();

	private bool IsGroundType => pickupRegion;

	private void OnDrawGizmos()
	{
		if ((bool)antLeftPoint && (bool)antRightPoint)
		{
			Gizmos.matrix = base.transform.localToWorldMatrix;
			Vector3 localPosition = antLeftPoint.localPosition;
			Vector3 localPosition2 = antRightPoint.localPosition;
			Color gray = Color.gray;
			float? a = 0.5f;
			Gizmos.color = gray.Where(null, null, null, a);
			Gizmos.DrawLine(localPosition, localPosition2);
			Gizmos.color = Color.gray;
			Vector2 vector = (Vector2)localPosition2 - (Vector2)localPosition;
			if (Mathf.Abs(vector.x) > Mathf.Abs(vector.y))
			{
				Vector3 from = localPosition;
				Vector3 original = localPosition2;
				a = localPosition.y;
				Gizmos.DrawLine(from, original.Where(null, a, null));
				Vector3 from2 = localPosition2;
				Vector3 original2 = localPosition;
				a = localPosition2.y;
				Gizmos.DrawLine(from2, original2.Where(null, a, null));
			}
			else
			{
				Gizmos.DrawLine(localPosition, localPosition2.Where(localPosition.x, null, null));
				Gizmos.DrawLine(localPosition2, localPosition.Where(localPosition2.x, null, null));
			}
		}
	}

	private void Awake()
	{
		if ((bool)pickupRegion)
		{
			pickupRegion.OnTriggerEntered += delegate(Collider2D col, GameObject _)
			{
				OnPickupRegionEntered(col.gameObject);
			};
			pickupRegion.OnTriggerExited += delegate(Collider2D col, GameObject _)
			{
				OnPickupRegionExited(col.gameObject);
			};
		}
	}

	private void Start()
	{
		Vector2 vector = antLeftPoint.position;
		Vector2 vector2 = antRightPoint.position;
		if (IsGroundType && Helper.IsRayHittingNoTriggers(Vector2.Lerp(vector, vector2, 0.5f) + new Vector2(0f, 1f), Vector2.down, 2f, 256, out var closestHit))
		{
			vector.y = (vector2.y = closestHit.point.y);
			base.transform.SetPositionY(closestHit.point.y);
			pickupAntParticleTemplate.transform.SetPositionY(closestHit.point.y);
		}
		float num = Vector2.Distance(vector, vector2);
		ParticleSystem.MainModule main = antParticleTemplate.main;
		ParticleSystem.MinMaxCurve startLifetime = main.startLifetime;
		float constant = startLifetime.constant;
		startLifetime.constant = num / Mathf.Abs(antParticleTemplate.velocityOverLifetime.x.constantMax);
		main.startLifetime = startLifetime;
		ParticleSystem.TextureSheetAnimationModule textureSheetAnimation = antParticleTemplate.textureSheetAnimation;
		textureSheetAnimation.cycleCount *= Mathf.RoundToInt(startLifetime.constant / constant);
		Transform parent = antParticleTemplate.transform.parent;
		leftParticle = UnityEngine.Object.Instantiate(antParticleTemplate, parent);
		rightParticle = UnityEngine.Object.Instantiate(antParticleTemplate, parent);
		antParticleTemplate.gameObject.SetActive(value: false);
		Transform transform = leftParticle.transform;
		transform.SetPosition2D(vector);
		switch (antParticleFlipMode)
		{
		case ParticleFlipMode.Scale:
			transform.FlipLocalScale(x: true);
			break;
		case ParticleFlipMode.Rotation:
			transform.Rotate(new Vector3(0f, 0f, 180f), Space.Self);
			break;
		default:
			throw new ArgumentOutOfRangeException();
		}
		rightParticle.transform.SetPosition2D(vector2);
		if (IsGroundType)
		{
			BoxCollider2D component = pickupRegion.GetComponent<BoxCollider2D>();
			component.size += new Vector2(0f, 1f);
			component.transform.Translate(new Vector3(0f, -0.5f, 0f), Space.Self);
			while (pickupAnts.Count < 10)
			{
				ParticleSystem particleSystem = UnityEngine.Object.Instantiate(pickupAntParticleTemplate, pickupAntParticleTemplate.transform.parent);
				particleSystem.gameObject.SetActive(value: false);
				pickupAnts.Add(particleSystem);
			}
		}
	}

	private void OnDisable()
	{
		ReleaseAllTrackedObjects();
	}

	private void OnPickupRegionEntered(GameObject obj, bool breakRecursion = false)
	{
		if (hasStoppedEmitting)
		{
			return;
		}
		int layer = obj.layer;
		if (layer != 18 && layer != 26 && !obj.GetComponent<AntRegionHandler>())
		{
			return;
		}
		foreach (CarryTracker carryTracker in carryTrackers)
		{
			if (carryTracker.GameObject == obj)
			{
				return;
			}
		}
		ICheck component = obj.GetComponent<ICheck>();
		Renderer renderer = null;
		if (component != null)
		{
			if (!component.CanEnterAntRegion)
			{
				return;
			}
			component.TryGetRenderer(out renderer);
		}
		ParticleSystem particleSystem = null;
		foreach (ParticleSystem pickupAnt in pickupAnts)
		{
			if (!pickupAnt.gameObject.activeSelf)
			{
				particleSystem = pickupAnt;
				particleSystem.gameObject.SetActive(value: true);
				break;
			}
		}
		if (particleSystem == null)
		{
			particleSystem = UnityEngine.Object.Instantiate(pickupAntParticleTemplate, pickupAntParticleTemplate.transform.parent);
			particleSystem.gameObject.SetActive(value: true);
			pickupAnts.Add(particleSystem);
		}
		Rigidbody2D component2 = obj.GetComponent<Rigidbody2D>();
		bool flag = component2 != null;
		Transform transform = obj.transform;
		carryTrackers.Add(new CarryTracker
		{
			GameObject = obj,
			Body = component2,
			InitialBodyType = (flag ? component2.bodyType : RigidbodyType2D.Dynamic),
			InitialAngle = transform.localEulerAngles.z,
			Routine = StartCoroutine(PickupGameObject(obj, component2, particleSystem, renderer)),
			CarryAnts = particleSystem
		});
		if (!breakRecursion)
		{
			INotify[] components = obj.GetComponents<INotify>();
			for (layer = 0; layer < components.Length; layer++)
			{
				components[layer].EnteredAntRegion(this);
			}
		}
	}

	private void OnPickupRegionExited(GameObject obj, bool breakRecursion = false)
	{
		for (int num = carryTrackers.Count - 1; num >= 0; num--)
		{
			CarryTracker tracker = carryTrackers[num];
			if (!(tracker.GameObject != obj))
			{
				StopCoroutine(tracker.Routine);
				carryTrackers.RemoveAt(num);
				DropGameObject(tracker);
			}
		}
		if (!breakRecursion)
		{
			INotify[] components = obj.GetComponents<INotify>();
			for (int i = 0; i < components.Length; i++)
			{
				components[i].ExitedAntRegion(this);
			}
		}
	}

	public void ResetTracker(GameObject obj)
	{
		OnPickupRegionExited(obj, breakRecursion: true);
		OnPickupRegionEntered(obj, breakRecursion: true);
	}

	private void ReleaseAllTrackedObjects()
	{
		CarryTracker[] array = carryTrackers.ToArray();
		for (int i = 0; i < array.Length; i++)
		{
			CarryTracker carryTracker = array[i];
			OnPickupRegionExited(carryTracker.GameObject);
		}
		carryTrackers.Clear();
	}

	private IEnumerator PickupGameObject(GameObject obj, Rigidbody2D body, ParticleSystem carryAnts, Renderer rend)
	{
		WaitFrameAndPaused frameWait = new WaitFrameAndPaused();
		yield return new WaitForSeconds(pickupWaitTime.GetRandomValue());
		bool flag = body != null;
		if (flag)
		{
			body.bodyType = RigidbodyType2D.Kinematic;
			body.linearVelocity = Vector2.zero;
			body.angularVelocity = 0f;
		}
		AntRegionHandler component = obj.GetComponent<AntRegionHandler>();
		if ((bool)component)
		{
			component.SetPickedUp(value: true);
		}
		IPickUpNotify[] pickNoNotifier = obj.GetComponents<IPickUpNotify>();
		IPickUpNotify[] array = pickNoNotifier;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].PickUpStarted(this);
		}
		HeroController instance = HeroController.instance;
		if ((bool)instance)
		{
			Transform root = obj.transform;
			if (flag && root.IsChildOf(body.transform))
			{
				root = body.transform;
			}
			Vector2 startPos = root.position;
			bool flag2 = rend != null;
			if (!flag2)
			{
				rend = obj.GetComponent<Renderer>();
				flag2 = rend != null;
			}
			Bounds bounds = (flag2 ? rend.bounds : new Bounds(root.position, new Vector3(0.5f, 0.5f, 1f)));
			Vector3 size = bounds.size;
			Vector3 vector = size * 0.5f;
			Vector3 center = bounds.center;
			Vector2 boundsOffset = (Vector2)center - startPos;
			float num = carryDirection switch
			{
				CarryDirection.AwayFromHero => (startPos.x < instance.transform.position.x) ? 1 : (-1), 
				CarryDirection.Left => 1, 
				CarryDirection.Right => -1, 
				_ => throw new ArgumentOutOfRangeException(), 
			};
			float targetX = ((num > 0f) ? (antLeftPoint.position.x + vector.x) : (antRightPoint.position.x - vector.x));
			float num2 = Mathf.Abs(targetX - startPos.x);
			float moveSpeed = pickupMoveSpeed.GetRandomValue();
			float moveDuration = num2 / moveSpeed;
			float height = pickupHeight.GetRandomValue();
			float raiseDuration = pickupDuration.GetRandomValue();
			float moveWaitTime = pickupMoveWaitTime.GetRandomValue();
			float dropWaitTime = sinkWaitTime.GetRandomValue();
			float num3 = raiseDuration + moveWaitTime + moveDuration + dropWaitTime + 0.5f;
			ParticleSystem.MainModule main = carryAnts.main;
			main.startLifetime = new ParticleSystem.MinMaxCurve(num3);
			main.duration = num3;
			float num4 = size.x * pickupAntsWidth;
			int num5 = Mathf.RoundToInt(pickupAntsPerUnit.GetRandomValue() * num4);
			carryAnts.emission.SetBurst(0, new ParticleSystem.Burst(0f, num5));
			ParticleSystem.ShapeModule shape = carryAnts.shape;
			if (num5 > 1)
			{
				shape.radius = num4 * 0.5f;
				shape.randomPositionAmount = pickupAntParticleTemplate.shape.randomPositionAmount;
			}
			else
			{
				shape.radius = 0f;
				shape.randomPositionAmount = 0f;
			}
			ParticleSystem.TextureSheetAnimationModule textureSheetAnimation = carryAnts.textureSheetAnimation;
			float constant = pickupAntParticleTemplate.main.startLifetime.constant;
			textureSheetAnimation.cycleCount = Mathf.RoundToInt((float)pickupAntParticleTemplate.textureSheetAnimation.cycleCount * (num3 / constant) * moveSpeed);
			carryAnts.Play(withChildren: true);
			Transform carryAntsTrans = carryAnts.transform;
			carryAntsTrans.SetPosition2D(new Vector2(startPos.x + boundsOffset.x, pickupAntParticleTemplate.transform.position.y));
			float scaleX = carryAntsTrans.GetScaleX();
			carryAntsTrans.SetScaleX(Mathf.Abs(scaleX) * num);
			Vector2 raisePos = startPos + new Vector2(0f, height);
			float elapsed;
			for (elapsed = 0f; elapsed < raiseDuration; elapsed += Time.deltaTime)
			{
				float t = pickupCurve.Evaluate(elapsed / raiseDuration);
				Vector2 position = Vector2.Lerp(startPos, raisePos, t);
				root.SetPosition2D(position);
				yield return frameWait;
			}
			root.SetPosition2D(raisePos);
			yield return new WaitForSeconds(moveWaitTime);
			float wobbleMult = 1f / size.x;
			if ((bool)obj.GetComponent<Corpse>() || (bool)obj.GetComponent<ActiveCorpse>())
			{
				wobbleMult *= 0.15f;
			}
			raisePos = root.position;
			elapsed = root.localEulerAngles.z;
			float elapsed2;
			for (elapsed2 = 0f; elapsed2 < moveDuration; elapsed2 += Time.deltaTime)
			{
				float num6 = Mathf.Lerp(raisePos.x, targetX, elapsed2 / moveDuration);
				float time = elapsed2 * moveSpeed;
				float y = raisePos.y + moveBobCurve.Evaluate(time);
				float angle = moveWobbleCurve.Evaluate(time) * wobbleMult;
				Vector2 vector2 = new Vector2(num6, y);
				root.SetPosition2D(vector2);
				root.SetLocalRotation2D(elapsed);
				root.RotateAround(vector2, Vector3.forward, angle);
				carryAntsTrans.SetPositionX(num6 + boundsOffset.x);
				yield return frameWait;
			}
			yield return new WaitForSeconds(dropWaitTime);
			float num7 = size.y + height;
			elapsed2 = num7 / sinkSpeed.GetRandomValue();
			elapsed = root.position.y;
			float dropY = elapsed - num7;
			for (float elapsed3 = 0f; elapsed3 < elapsed2; elapsed3 += Time.deltaTime)
			{
				float t2 = sinkCurve.Evaluate(elapsed3 / elapsed2);
				float newY = Mathf.Lerp(elapsed, dropY, t2);
				root.SetPositionY(newY);
				yield return frameWait;
			}
			root.SetPositionY(dropY);
			array = pickNoNotifier;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].PickUpEnd(this);
			}
			if (ObjectPool.IsSpawned(obj))
			{
				obj.Recycle();
			}
			else
			{
				obj.SetActive(value: false);
			}
		}
	}

	private void DropGameObject(CarryTracker tracker)
	{
		tracker.GameObject.transform.SetLocalRotation2D(tracker.InitialAngle);
		if ((bool)tracker.Body)
		{
			tracker.Body.bodyType = tracker.InitialBodyType;
		}
		tracker.CarryAnts.gameObject.SetActive(value: false);
		AntRegionHandler component = tracker.GameObject.GetComponent<AntRegionHandler>();
		if ((bool)component)
		{
			component.SetPickedUp(value: false);
		}
	}

	public void StopEmitting()
	{
		hasStoppedEmitting = true;
		leftParticle.Stop(withChildren: true, ParticleSystemStopBehavior.StopEmitting);
		rightParticle.Stop(withChildren: true, ParticleSystemStopBehavior.StopEmitting);
	}
}
