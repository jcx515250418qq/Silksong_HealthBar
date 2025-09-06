using System.Collections.Generic;
using System.Linq;
using GlobalEnums;
using GlobalSettings;
using TeamCherry.SharedUtils;
using UnityEngine;

public class IdleForceAnimator : MonoBehaviour, GameManager.ISceneManualSimulatePhysics
{
	[SerializeField]
	private float idleSwingForceMagnitude;

	[SerializeField]
	private float torqueMagnitude;

	[SerializeField]
	private AnimationCurve idleSwingForceCurve;

	[SerializeField]
	private float worldXTimeOffsetAmount;

	[SerializeField]
	private bool useChildren;

	[SerializeField]
	private Rigidbody2D[] bodies;

	[SerializeField]
	private bool disableCamShakeEventHandling;

	[SerializeField]
	private bool ignoreExistingCamEventReceiver;

	[SerializeField]
	private bool resetVelocityAfterSimulation;

	[SerializeField]
	private bool shakeReactionDiminishingReturn;

	private bool subscribedCameraShaked;

	private bool isIdleSwayActive;

	private double lastFullCamShakeTime;

	private float elapsedTime;

	public float SpeedMultiplier = 1f;

	public float ExtraHorizontalSpeed;

	private ParentSwayConfig parentConfig;

	private static readonly List<IdleForceAnimator> _activeAnimators = new List<IdleForceAnimator>();

	private HashSet<Rigidbody2D> rigidbody2Ds = new HashSet<Rigidbody2D>();

	private GameManager gm;

	private VisibilityGroup visibilityGroup;

	private bool isVisible;

	private bool started;

	private void OnValidate()
	{
		bool flag = false;
		if (bodies != null)
		{
			Rigidbody2D[] array = bodies;
			for (int i = 0; i < array.Length; i++)
			{
				if (!(array[i] != null))
				{
					flag = true;
					break;
				}
			}
		}
		if (flag)
		{
			bodies = bodies.Where((Rigidbody2D body) => body != null).ToArray();
		}
	}

	private void Awake()
	{
		OnValidate();
		ReplaceWithTemplate[] componentsInChildren = GetComponentsInChildren<ReplaceWithTemplate>();
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			componentsInChildren[i].Awake();
		}
		parentConfig = GetComponentInParent<ParentSwayConfig>();
		isIdleSwayActive = !parentConfig || parentConfig.HasIdleSway;
		if (useChildren)
		{
			bodies = GetComponentsInChildren<Rigidbody2D>();
		}
		Rigidbody2D[] array = bodies;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].WakeUp();
		}
		visibilityGroup = base.gameObject.AddComponentIfNotPresent<VisibilityGroup>();
		isVisible = visibilityGroup.IsVisible;
		visibilityGroup.OnVisibilityChanged += delegate(bool visible)
		{
			isVisible = visible;
		};
		if (!disableCamShakeEventHandling && (ignoreExistingCamEventReceiver || GetComponentsInChildren<CameraShakeEventReceiver>().Length == 0))
		{
			GlobalSettings.Camera.MainCameraShakeManager.CameraShakedWorldForce += OnCameraShaked;
			subscribedCameraShaked = true;
		}
	}

	private void Start()
	{
		gm = GameManager.instance;
		started = true;
		ComponentSingleton<IdleForceAnimatorCallbackHooks>.Instance.OnFixedUpdate += OnFixedUpdate;
	}

	private void OnDestroy()
	{
		if (subscribedCameraShaked)
		{
			GlobalSettings.Camera.MainCameraShakeManager.CameraShakedWorldForce -= OnCameraShaked;
		}
	}

	private void OnEnable()
	{
		if (started)
		{
			ComponentSingleton<IdleForceAnimatorCallbackHooks>.Instance.OnFixedUpdate += OnFixedUpdate;
		}
		_activeAnimators.Add(this);
	}

	private void OnDisable()
	{
		ComponentSingleton<IdleForceAnimatorCallbackHooks>.Instance.OnFixedUpdate -= OnFixedUpdate;
		_activeAnimators.Remove(this);
		StopAllCoroutines();
	}

	private void OnFixedUpdate()
	{
		if (isVisible && isIdleSwayActive)
		{
			OnManualPhysics(Time.deltaTime);
		}
	}

	public void OnManualPhysics(float deltaTime)
	{
		elapsedTime += deltaTime * SpeedMultiplier;
		float num = idleSwingForceCurve.Evaluate(elapsedTime + base.transform.position.x * worldXTimeOffsetAmount);
		float num2 = num * idleSwingForceMagnitude * SpeedMultiplier;
		float num3 = num * torqueMagnitude * SpeedMultiplier;
		if (gm.GetCurrentMapZoneEnum() == MapZone.JUDGE_STEPS && (!parentConfig || parentConfig.ApplyMapZoneSway))
		{
			num2 *= 4f;
			num3 *= 4f;
			num2 += 10f * SpeedMultiplier;
		}
		num2 += ExtraHorizontalSpeed;
		Rigidbody2D[] array = bodies;
		foreach (Rigidbody2D rigidbody2D in array)
		{
			if ((bool)rigidbody2D)
			{
				if (Mathf.Abs(num2) > Mathf.Epsilon)
				{
					rigidbody2D.AddForce(new Vector2(num2, 0f), ForceMode2D.Force);
				}
				if (Mathf.Abs(num3) > Mathf.Epsilon)
				{
					rigidbody2D.AddTorque(num3, ForceMode2D.Force);
				}
			}
		}
	}

	public void PrepareManualSimulate()
	{
		gm = GameManager.instance;
		Rigidbody2D[] array = bodies;
		foreach (Rigidbody2D rigidbody2D in array)
		{
			rigidbody2D.WakeUp();
			HingeJoint2D component = rigidbody2D.GetComponent<HingeJoint2D>();
			if ((bool)component)
			{
				component.autoConfigureConnectedAnchor = false;
			}
			rigidbody2Ds.Add(rigidbody2D);
		}
		if (useChildren)
		{
			return;
		}
		array = GetComponentsInChildren<Rigidbody2D>();
		foreach (Rigidbody2D rigidbody2D2 in array)
		{
			rigidbody2D2.WakeUp();
			HingeJoint2D component2 = rigidbody2D2.GetComponent<HingeJoint2D>();
			if ((bool)component2)
			{
				component2.autoConfigureConnectedAnchor = false;
			}
			rigidbody2Ds.Add(rigidbody2D2);
		}
	}

	public void OnManualSimulateFinished()
	{
		foreach (Rigidbody2D rigidbody2D in rigidbody2Ds)
		{
			rigidbody2D.Sleep();
			if (resetVelocityAfterSimulation)
			{
				rigidbody2D.linearVelocity = Vector2.zero;
				rigidbody2D.angularVelocity = 0f;
			}
		}
	}

	private void OnCameraShaked(Vector2 cameraPosition, CameraShakeWorldForceIntensities intensity)
	{
		if (!base.isActiveAndEnabled)
		{
			return;
		}
		float num;
		if (intensity >= CameraShakeWorldForceIntensities.Intense)
		{
			num = 15f;
		}
		else
		{
			if (intensity < CameraShakeWorldForceIntensities.Medium)
			{
				return;
			}
			num = 5f;
		}
		num *= idleSwingForceMagnitude;
		float num2 = (float)(Time.timeAsDouble - lastFullCamShakeTime);
		if (num2 < 0.01f)
		{
			float num3 = num2 / 0.01f;
			if (num3 < 0.5f)
			{
				num3 = 0.5f;
			}
			num *= num3;
			lastFullCamShakeTime = Time.timeAsDouble;
		}
		Vector2 vector = new Vector2(-0.5f, 1f);
		Vector2 vector2 = new Vector2(0.5f, 1f);
		num = Mathf.Min(num, 175f);
		Rigidbody2D[] array = bodies;
		foreach (Rigidbody2D rigidbody2D in array)
		{
			if ((bool)rigidbody2D && (shakeReactionDiminishingReturn || !(rigidbody2D.linearVelocity.sqrMagnitude > 625f)))
			{
				Vector2 vector3 = new Vector2(Random.Range(vector.x, vector2.x), Random.Range(vector.y, vector2.y));
				vector3.Normalize();
				float num4 = num;
				if (shakeReactionDiminishingReturn)
				{
					float magnitude = rigidbody2D.linearVelocity.magnitude;
					num4 *= Mathf.Lerp(1f, 0.25f, magnitude / 25f);
				}
				Vector2 force = vector3 * num4;
				rigidbody2D.AddForce(force, ForceMode2D.Impulse);
			}
		}
	}

	public static IEnumerable<IdleForceAnimator> EnumerateActiveAnimators()
	{
		foreach (IdleForceAnimator activeAnimator in _activeAnimators)
		{
			yield return activeAnimator;
		}
	}
}
