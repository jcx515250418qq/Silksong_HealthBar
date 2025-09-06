using System;
using HutongGames.PlayMaker;
using UnityEngine;
using UnityEngine.Events;

public class SurfaceWaterFloater : MonoBehaviour
{
	[SerializeField]
	private Rigidbody2D body;

	[SerializeField]
	private float floatHeight;

	[SerializeField]
	private float dropTimer;

	[SerializeField]
	private float dropDuration;

	[SerializeField]
	private float despawnDelay = 5f;

	[SerializeField]
	private float waterDampX = 0.9f;

	[SerializeField]
	private float waterDampAngular = 0.9f;

	[SerializeField]
	private RandomAudioClipTable splashInAudio;

	[Space]
	[SerializeField]
	private PlayMakerFSM eventTarget;

	[Space]
	public UnityEvent OnLandInWater;

	public UnityEvent OnExitWater;

	private double dropTime;

	private FsmBool inWaterFsmBool;

	private bool shouldSink;

	private FeatherPhysics feather;

	private ObjectBounce objectBounce;

	private bool waitingForDrop;

	private bool canRecycle;

	public float FloatHeight => floatHeight;

	public float GravityScale => body.gravityScale;

	public float Velocity => body.linearVelocity.y;

	public float FloatMultiplier
	{
		get
		{
			if (shouldSink)
			{
				return 0f;
			}
			if (dropTimer <= 0f || Time.timeAsDouble < dropTime)
			{
				return 1f;
			}
			if (waitingForDrop)
			{
				waitingForDrop = false;
				if ((bool)objectBounce)
				{
					objectBounce.StopBounce();
				}
			}
			float num = (float)(Time.timeAsDouble - dropTime);
			float num2 = Mathf.Clamp01(num / dropDuration);
			if (num - dropDuration > despawnDelay && canRecycle)
			{
				canRecycle = false;
				base.gameObject.Recycle();
			}
			return 1f - num2;
		}
	}

	private void Awake()
	{
		if ((bool)eventTarget)
		{
			inWaterFsmBool = eventTarget.FsmVariables.FindFsmBool("Is Floating");
		}
		feather = GetComponent<FeatherPhysics>();
		objectBounce = GetComponent<ObjectBounce>();
	}

	private void OnEnable()
	{
		canRecycle = true;
	}

	private void OnDisable()
	{
		if ((bool)feather)
		{
			feather.enabled = true;
		}
		shouldSink = false;
		canRecycle = false;
	}

	private void OnDrawGizmosSelected()
	{
		Vector3 position = base.transform.position;
		position.y += floatHeight;
		Gizmos.DrawWireSphere(position, 0.1f);
	}

	public void AddForce(float force)
	{
		force *= body.mass;
		body.AddForce(new Vector2(0f, force));
	}

	public void AddFlowSpeed(float flowSpeed, Quaternion surfaceRotation)
	{
		if (!(Math.Abs(flowSpeed) <= Mathf.Epsilon))
		{
			Vector2 linearVelocity = body.linearVelocity;
			linearVelocity.x = flowSpeed;
			body.linearVelocity = linearVelocity;
		}
	}

	public void MoveWithSurface(float flowSpeed, Quaternion surfaceRotation)
	{
		if (!(Math.Abs(flowSpeed) <= Mathf.Epsilon))
		{
			Vector3 vector = surfaceRotation * new Vector2(flowSpeed, 0f);
			vector.y += body.gravityScale * Physics2D.gravity.y * -1f * Time.fixedDeltaTime;
			body.linearVelocity = vector;
		}
	}

	public void SetInWater(bool value)
	{
		if (inWaterFsmBool != null)
		{
			inWaterFsmBool.Value = value;
		}
		if (value)
		{
			dropTime = Time.timeAsDouble + (double)dropTimer;
			waitingForDrop = true;
			splashInAudio.SpawnAndPlayOneShot(base.transform.position);
			OnLandInWater.Invoke();
			if ((bool)eventTarget)
			{
				eventTarget.SendEvent("ENTERED FLOAT");
			}
		}
		else
		{
			OnExitWater.Invoke();
			if ((bool)eventTarget)
			{
				eventTarget.SendEvent("EXITED FLOAT");
			}
		}
	}

	public void Dampen()
	{
		Vector2 linearVelocity = body.linearVelocity;
		linearVelocity.x *= waterDampX;
		body.linearVelocity = linearVelocity;
		float angularVelocity = body.angularVelocity;
		angularVelocity *= waterDampAngular;
		body.angularVelocity = angularVelocity;
	}

	public void Sink()
	{
		shouldSink = true;
	}

	public void StopSink()
	{
		shouldSink = false;
	}
}
