using System;
using System.Linq;
using TeamCherry.SharedUtils;
using UnityEngine;
using UnityEngine.Events;

public abstract class VectorCurveAnimator : BaseAnimator, IUpdateBatchableLateUpdate
{
	private enum CullingModes
	{
		None = 0,
		Visibility = 1
	}

	public Action UpdatedPosition;

	[SerializeField]
	private Transform overrideTransform;

	[SerializeField]
	private Vector3 offset;

	[SerializeField]
	[ModifiableProperty]
	[Conditional("UsesSpace", true, true, true)]
	protected Space space = Space.Self;

	[SerializeField]
	private AnimationCurve curve = AnimationCurve.Linear(0f, 0f, 1f, 1f);

	[SerializeField]
	private float duration = 1f;

	[SerializeField]
	private float delay;

	[SerializeField]
	private MinMaxFloat startElapsed;

	[SerializeField]
	private bool isRealtime;

	private Vector3? initialVector;

	private Vector3 currentOffset;

	private bool hasCurrentOffsetChanged;

	[SerializeField]
	private bool playOnEnable;

	[SerializeField]
	private bool loop;

	[SerializeField]
	private bool resetOnPlay = true;

	[SerializeField]
	private float framerate;

	[SerializeField]
	private CullingModes cullingMode = CullingModes.Visibility;

	[Space]
	public UnityEvent OnStart;

	public UnityEvent OnStop;

	private bool isAnimating;

	private float delayLeft;

	private float elapsed;

	private bool doExtraUpdate;

	private double nextUpdateTime;

	private float speedMultiplier = 1f;

	private VectorCurveAnimator[] gameObjectModifiers;

	private UpdateBatcher updateBatcher;

	private bool updateHandled;

	private Action<float> setLocalPosition;

	private bool hasRenderer;

	private bool isVisible;

	private Renderer[] childRenderers;

	protected abstract Vector3 Vector { get; set; }

	protected Transform CurrentTransform
	{
		get
		{
			if (!overrideTransform)
			{
				return base.transform;
			}
			return overrideTransform;
		}
	}

	public bool ShouldUpdate
	{
		get
		{
			if (cullingMode == CullingModes.None)
			{
				return true;
			}
			if (hasRenderer)
			{
				return isVisible;
			}
			if (childRenderers == null || childRenderers.Length == 0)
			{
				return true;
			}
			Renderer[] array = childRenderers;
			for (int i = 0; i < array.Length; i++)
			{
				if (array[i].isVisible)
				{
					return true;
				}
			}
			return false;
		}
	}

	public Vector3 Offset
	{
		get
		{
			return offset;
		}
		set
		{
			offset = value;
		}
	}

	public float OffsetX
	{
		get
		{
			return offset.x;
		}
		set
		{
			offset = offset.Where(value, null, null);
		}
	}

	public float OffsetY
	{
		get
		{
			return offset.y;
		}
		set
		{
			Vector3 original = offset;
			float? y = value;
			offset = original.Where(null, y, null);
		}
	}

	public float OffsetZ
	{
		get
		{
			return offset.z;
		}
		set
		{
			Vector3 original = offset;
			float? z = value;
			offset = original.Where(null, null, z);
		}
	}

	public float SpeedMultiplier
	{
		get
		{
			return speedMultiplier;
		}
		set
		{
			speedMultiplier = value;
		}
	}

	public bool FreezePosition { get; set; }

	protected virtual bool UsesSpace()
	{
		return true;
	}

	private void OnEnable()
	{
		Renderer component = GetComponent<Renderer>();
		hasRenderer = (bool)component && (!(component is SpriteRenderer spriteRenderer) || (bool)spriteRenderer.sprite);
		childRenderers = ((!hasRenderer) ? GetComponentsInChildren<Renderer>(includeInactive: true) : null);
		if (!updateHandled)
		{
			Type type = GetType();
			gameObjectModifiers = (from c in GetComponents(type).OfType<VectorCurveAnimator>()
				where c.enabled
				select c).ToArray();
			VectorCurveAnimator[] array = gameObjectModifiers;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].updateHandled = true;
			}
			updateBatcher = GameManager.instance.GetComponent<UpdateBatcher>();
			updateBatcher.Add(this);
		}
		if (playOnEnable)
		{
			StartAnimation();
		}
	}

	private void OnDisable()
	{
		if (updateBatcher != null)
		{
			if (updateBatcher.Remove(this))
			{
				VectorCurveAnimator[] array = gameObjectModifiers;
				for (int i = 0; i < array.Length; i++)
				{
					array[i].updateHandled = false;
				}
			}
			updateBatcher = null;
		}
		childRenderers = null;
		isAnimating = false;
		setLocalPosition = null;
	}

	public void BatchedLateUpdate()
	{
		UpdateThis();
		UpdateWholeObject();
	}

	private void UpdateThis()
	{
		if (!isAnimating)
		{
			return;
		}
		if (delayLeft > 0f)
		{
			delayLeft -= Time.deltaTime;
			if (delayLeft > 0f)
			{
				return;
			}
		}
		if (duration <= 0f)
		{
			Debug.LogError("Vector Curve Animator duration can not be less than or equal to 0!", this);
			return;
		}
		float num = (isRealtime ? Time.unscaledDeltaTime : Time.deltaTime);
		elapsed += num * speedMultiplier;
		bool flag;
		if (FreezePosition)
		{
			flag = false;
		}
		else
		{
			flag = true;
			double time = GetTime();
			if (framerate > 0f)
			{
				if (time >= nextUpdateTime)
				{
					nextUpdateTime = time + (double)(1f / framerate);
				}
				else
				{
					flag = false;
				}
			}
		}
		bool num2 = elapsed >= duration && !loop;
		if (elapsed > duration)
		{
			elapsed %= duration;
		}
		if (flag)
		{
			setLocalPosition(elapsed / duration);
		}
		if (num2)
		{
			setLocalPosition(1f);
			setLocalPosition = null;
			OnStop.Invoke();
			isAnimating = false;
			doExtraUpdate = true;
		}
	}

	private void UpdateWholeObject()
	{
		if (gameObjectModifiers == null)
		{
			return;
		}
		VectorCurveAnimator vectorCurveAnimator = null;
		VectorCurveAnimator[] array = gameObjectModifiers;
		foreach (VectorCurveAnimator vectorCurveAnimator2 in array)
		{
			bool num = isAnimating || doExtraUpdate;
			vectorCurveAnimator2.doExtraUpdate = false;
			if (num)
			{
				vectorCurveAnimator = vectorCurveAnimator2;
			}
		}
		if (vectorCurveAnimator == null || vectorCurveAnimator != this)
		{
			return;
		}
		Vector3 zero = Vector3.zero;
		bool flag = false;
		array = gameObjectModifiers;
		foreach (VectorCurveAnimator vectorCurveAnimator3 in array)
		{
			if (vectorCurveAnimator3.hasCurrentOffsetChanged)
			{
				zero += vectorCurveAnimator3.currentOffset;
				hasCurrentOffsetChanged = false;
				flag = true;
			}
		}
		if (flag)
		{
			Vector = initialVector.Value + zero;
		}
	}

	private void OnBecameVisible()
	{
		isVisible = true;
	}

	private void OnBecameInvisible()
	{
		isVisible = false;
	}

	public override void StartAnimation()
	{
		StartAnimation(isFlipped: false);
	}

	public void StartAnimation(bool isFlipped)
	{
		if (!resetOnPlay || !initialVector.HasValue)
		{
			initialVector = Vector;
		}
		isAnimating = false;
		if (base.gameObject.activeInHierarchy)
		{
			isAnimating = true;
			setLocalPosition = delegate(float time)
			{
				SetAnimTime(time, isFlipped);
			};
			elapsed = startElapsed.GetRandomValue();
			delayLeft = delay;
			OnStart.Invoke();
		}
	}

	public void StartAnimationFlipHeroSideX()
	{
		StartAnimation(HeroController.instance.transform.position.x > base.transform.position.x);
	}

	public void StartAnimationRandomFlip()
	{
		StartAnimation(UnityEngine.Random.Range(0, 2) == 0);
	}

	public void SetAtStart()
	{
		SetAtEnd(isFlipped: false);
	}

	public void SetAtStart(bool isFlipped)
	{
		if (!initialVector.HasValue)
		{
			initialVector = Vector;
		}
		SetAnimTime(0f, isFlipped);
		doExtraUpdate = true;
	}

	public void SetAtEnd()
	{
		SetAtEnd(isFlipped: false);
	}

	public void SetAtEnd(bool isFlipped)
	{
		if (!initialVector.HasValue)
		{
			initialVector = Vector;
		}
		SetAnimTime(1f, isFlipped);
		doExtraUpdate = true;
	}

	public void ForceStop()
	{
		Stop(setAtEnd: true);
	}

	public void StopAtCurrentPoint()
	{
		Stop(setAtEnd: false);
	}

	private void Stop(bool setAtEnd)
	{
		isAnimating = false;
		if (setLocalPosition != null)
		{
			if (setAtEnd)
			{
				setLocalPosition(1f);
			}
			setLocalPosition = null;
			BatchedLateUpdate();
		}
	}

	private void SetAnimTime(float time, bool isFlipped)
	{
		time = curve.Evaluate(time);
		if (isFlipped)
		{
			time *= -1f;
		}
		currentOffset = offset * time;
		hasCurrentOffsetChanged = true;
		if (UpdatedPosition != null)
		{
			UpdatedPosition();
		}
	}

	private double GetTime()
	{
		if (!isRealtime)
		{
			return Time.timeAsDouble;
		}
		return Time.unscaledTimeAsDouble;
	}
}
