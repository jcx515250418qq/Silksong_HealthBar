using System;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(CircleCollider2D))]
public class CogRollThenFallOver : SpriteExtruder
{
	[Space]
	[SerializeField]
	private AnimationCurve fallPositionCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);

	[SerializeField]
	private AnimationCurve fallTimeCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);

	[SerializeField]
	private float fallDuration;

	[SerializeField]
	private float fallFpsLimit;

	[Space]
	[SerializeField]
	private float velocityThreshold;

	[SerializeField]
	private float angularVelocityThreshold;

	[SerializeField]
	private float fallWaitTime;

	[Space]
	[SerializeField]
	private Rigidbody2D fallenObject;

	[SerializeField]
	private Vector2 fallenObjectForce;

	[SerializeField]
	private CircleCollider2D colliderOverride;

	[Space]
	[SerializeField]
	private AudioSource rollAudio;

	[Space]
	[SerializeField]
	private Transform moveDownWithFall;

	[Space]
	[SerializeField]
	private bool waitUntilVisible;

	[SerializeField]
	[ModifiableProperty]
	[Conditional("waitUntilVisible", true, false, false)]
	private float maxVisibleWaitTime;

	[SerializeField]
	[ModifiableProperty]
	[Conditional("waitUntilVisible", true, false, false)]
	private float visibleFallDelay;

	[SerializeField]
	private bool startInactive;

	[SerializeField]
	private bool preventCleanup;

	[Space]
	[SerializeField]
	private bool disableSelfColliderOnFall;

	private bool isActive;

	private bool hasFallen;

	private float fallWaitTimeLeft;

	private Coroutine fallRoutine;

	private bool markedForCleanup;

	private Transform fallenObjectTransform;

	private Vector2 fallenObjectStartPos;

	private Quaternion fallenObjectStartRot;

	private Collider2D fallenObjectCollider;

	private Rigidbody2D body;

	private CircleCollider2D collider;

	private Collider2D selfCollider;

	public event Action Fallen;

	public event Action FallenCleanup;

	protected override void Awake()
	{
		base.Awake();
		body = GetComponent<Rigidbody2D>();
		collider = (colliderOverride ? colliderOverride : GetComponent<CircleCollider2D>());
		selfCollider = GetComponent<Collider2D>();
		if ((bool)fallenObject)
		{
			fallenObjectTransform = fallenObject.transform;
		}
	}

	protected override void OnEnable()
	{
		base.OnEnable();
		if ((bool)fallenObject)
		{
			fallenObject.gameObject.SetActive(value: false);
			fallenObjectStartPos = fallenObjectTransform.localPosition;
			fallenObjectStartRot = fallenObjectTransform.localRotation;
			fallenObjectCollider = fallenObject.GetComponent<Collider2D>();
			if ((bool)fallenObjectCollider)
			{
				fallenObjectCollider.enabled = true;
			}
		}
		if (body.bodyType != RigidbodyType2D.Static)
		{
			body.bodyType = RigidbodyType2D.Dynamic;
		}
		SetFallOverAmount(0f);
		fallWaitTimeLeft = 0f;
		hasFallen = false;
		isActive = !startInactive;
		if ((bool)rollAudio)
		{
			rollAudio.Play();
		}
		markedForCleanup = false;
	}

	private void OnDisable()
	{
		if ((bool)fallenObject)
		{
			fallenObjectTransform.localPosition = fallenObjectStartPos;
			fallenObjectTransform.localRotation = fallenObjectStartRot;
		}
		if (disableSelfColliderOnFall && (bool)selfCollider)
		{
			selfCollider.enabled = true;
		}
	}

	private void FixedUpdate()
	{
		if (hasFallen || !isActive)
		{
			return;
		}
		if (IsBodyMoving())
		{
			fallWaitTimeLeft = fallWaitTime;
		}
		else if (fallWaitTimeLeft > 0f)
		{
			if ((bool)rollAudio)
			{
				rollAudio.Stop();
			}
			fallWaitTimeLeft -= Time.deltaTime;
			if (fallWaitTimeLeft <= 0f)
			{
				DoFallOver();
			}
		}
	}

	public void Activate()
	{
		isActive = true;
	}

	private void DoFallOver()
	{
		if (fallRoutine != null)
		{
			StopCoroutine(fallRoutine);
		}
		fallRoutine = StartCoroutine(FallOver());
	}

	private bool IsBodyMoving()
	{
		float magnitude = body.linearVelocity.magnitude;
		float num = Mathf.Abs(body.angularVelocity);
		if (!(velocityThreshold > 0f) || !(magnitude > velocityThreshold))
		{
			if (angularVelocityThreshold > 0f)
			{
				return num > angularVelocityThreshold;
			}
			return false;
		}
		return true;
	}

	private IEnumerator FallOver()
	{
		if (waitUntilVisible && !base.OriginalDisplay.isVisible)
		{
			float waitTimeLeft = maxVisibleWaitTime;
			while (!base.OriginalDisplay.isVisible)
			{
				yield return null;
				waitTimeLeft -= Time.deltaTime;
				if (waitTimeLeft <= 0f)
				{
					break;
				}
			}
			if (visibleFallDelay > 0f)
			{
				yield return new WaitForSeconds(visibleFallDelay);
			}
		}
		hasFallen = true;
		if (this.Fallen != null)
		{
			this.Fallen();
		}
		if (body.bodyType != RigidbodyType2D.Static)
		{
			body.bodyType = RigidbodyType2D.Kinematic;
		}
		body.linearVelocity = Vector2.zero;
		body.angularVelocity = 0f;
		if (disableSelfColliderOnFall && (bool)selfCollider)
		{
			selfCollider.enabled = false;
		}
		Vector3 localScale = base.transform.localScale;
		float num = Mathf.Sign(localScale.x) * Mathf.Sign(localScale.y);
		base.OriginalDisplay.transform.SetLocalRotation2D(base.transform.localEulerAngles.z * num);
		base.transform.SetLocalRotation2D(0f);
		float frameTime = ((fallFpsLimit > 0f) ? (1f / fallFpsLimit) : 0f);
		WaitForSeconds wait = new WaitForSeconds(frameTime);
		for (float elapsed = 0f; elapsed < fallDuration; elapsed += Mathf.Max(Time.deltaTime, frameTime))
		{
			SetFallOverAmount(elapsed / fallDuration);
			yield return wait;
		}
		SetFallOverAmount(1f);
		base.OriginalDisplay.gameObject.SetActive(value: false);
		if ((bool)fallenObject)
		{
			fallenObject.gameObject.SetActive(value: true);
			fallenObject.linearVelocity = Vector2.zero;
			fallenObject.angularVelocity = 0f;
			fallenObject.AddForce(fallenObjectForce, ForceMode2D.Impulse);
			if ((bool)moveDownWithFall)
			{
				moveDownWithFall.transform.SetParent(fallenObject.transform);
				moveDownWithFall.transform.SetLocalPosition2D(Vector2.zero);
			}
		}
		if (!preventCleanup && (bool)fallenObjectCollider)
		{
			float elapsed = 0f;
			while (elapsed < 20f && !markedForCleanup)
			{
				yield return null;
				elapsed = ((!fallenObject.IsSleeping()) ? 0f : (elapsed + Time.deltaTime));
			}
			fallenObjectCollider.enabled = false;
			yield return new WaitForSeconds(2.5f);
			fallenObjectCollider.enabled = true;
			if (this.FallenCleanup != null)
			{
				this.FallenCleanup();
			}
			else
			{
				base.gameObject.Recycle();
			}
		}
	}

	private void SetFallOverAmount(float amount)
	{
		amount = fallTimeCurve.Evaluate(amount);
		Transform transform = base.OriginalDisplay.transform;
		float b = 0f - collider.radius + base.ExtrusionDepth;
		float x = Mathf.LerpUnclamped(0f, 90f, amount);
		Vector3 localEulerAngles = transform.localEulerAngles;
		localEulerAngles.x = x;
		transform.localEulerAngles = localEulerAngles;
		amount = fallPositionCurve.Evaluate(amount);
		float y = Mathf.LerpUnclamped(0f, b, amount);
		transform.localPosition = new Vector3(0f, y, 0f);
		if ((bool)moveDownWithFall)
		{
			moveDownWithFall.SetPosition2D(transform.position);
		}
	}

	public void MarkForCleanup()
	{
	}
}
