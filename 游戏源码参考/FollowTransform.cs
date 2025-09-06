using System;
using TeamCherry.SharedUtils;
using UnityEngine;

public class FollowTransform : MonoBehaviour, IUpdateBatchableLateUpdate, IUpdateBatchableUpdate, ChildUpdateOrdered.IUpdateOrderUpdate
{
	[Flags]
	private enum FollowAxes
	{
		None = 0,
		X = 1,
		Y = 2,
		Z = 4,
		All = -1
	}

	private enum UpdateOrder
	{
		Update = 0,
		LateUpdate = 1,
		Custom = 2
	}

	[SerializeField]
	private Transform target;

	[SerializeField]
	private Transform[] fallbackTargets;

	[SerializeField]
	private bool useParent;

	[SerializeField]
	private bool useHero;

	[SerializeField]
	private bool keepOffset = true;

	[SerializeField]
	[ModifiableProperty]
	[Conditional("keepOffset", true, false, false)]
	private bool keepOffsetZ = true;

	[SerializeField]
	private float zOffset;

	[SerializeField]
	[EnumPickerBitmask]
	private FollowAxes followAxes = FollowAxes.All;

	[SerializeField]
	private bool useRigidbody;

	[SerializeField]
	private UpdateOrder updateOrder;

	[SerializeField]
	private float lerpSpeed;

	[SerializeField]
	private bool deparent;

	[SerializeField]
	[Range(0f, 1f)]
	private float lerpFromInitialPosition = 1f;

	private bool shouldClearTargetOnDisable;

	private bool didDeparent;

	private Vector3 initialPosition;

	private UpdateBatcher updateBatcher;

	private Rigidbody2D body;

	public Vector3 Offset { get; set; }

	public Transform Target
	{
		get
		{
			return target;
		}
		set
		{
			target = value;
			UpdateInitialValues();
		}
	}

	public Transform CurrentTarget
	{
		get
		{
			if ((bool)target && target.gameObject.activeInHierarchy)
			{
				return target;
			}
			if (fallbackTargets != null)
			{
				Transform[] array = fallbackTargets;
				foreach (Transform transform in array)
				{
					if ((bool)transform && transform.gameObject.activeInHierarchy)
					{
						return transform;
					}
				}
			}
			return null;
		}
	}

	public bool UseParent
	{
		get
		{
			return useParent;
		}
		set
		{
			useParent = value;
			UpdateInitialValues();
		}
	}

	public bool ShouldUpdate => true;

	private void OnValidate()
	{
		if (lerpSpeed < 0f)
		{
			lerpSpeed = 0f;
		}
	}

	private void Awake()
	{
		if (useRigidbody)
		{
			body = GetComponent<Rigidbody2D>();
		}
	}

	private void OnEnable()
	{
		if (updateOrder != UpdateOrder.Custom)
		{
			updateBatcher = GameManager.instance.GetComponent<UpdateBatcher>();
			updateBatcher.Add(this);
		}
	}

	private void OnDisable()
	{
		if ((bool)updateBatcher)
		{
			updateBatcher.Remove(this);
			updateBatcher = null;
		}
		if (shouldClearTargetOnDisable)
		{
			shouldClearTargetOnDisable = false;
			target = null;
		}
	}

	private void Start()
	{
		UpdateInitialValues();
	}

	private void UpdateInitialValues()
	{
		if (useParent)
		{
			target = base.transform.parent;
			base.transform.SetParent(null, worldPositionStays: true);
		}
		if (useHero)
		{
			HeroController instance = HeroController.instance;
			if ((bool)instance)
			{
				target = instance.transform;
			}
		}
		Transform currentTarget = CurrentTarget;
		if (!currentTarget)
		{
			return;
		}
		initialPosition = base.transform.position;
		if (keepOffset)
		{
			Vector3 offset = initialPosition - currentTarget.position;
			if (!keepOffsetZ)
			{
				offset.z = 0f;
			}
			Offset = offset;
		}
	}

	public void BatchedUpdate()
	{
		if (updateOrder == UpdateOrder.Update)
		{
			DoUpdate();
		}
	}

	public void BatchedLateUpdate()
	{
		if (updateOrder == UpdateOrder.LateUpdate)
		{
			DoUpdate();
		}
	}

	private void DoUpdate()
	{
		if (deparent && !didDeparent)
		{
			base.transform.SetParent(null);
			didDeparent = true;
		}
		Transform currentTarget = CurrentTarget;
		if (!currentTarget)
		{
			return;
		}
		Vector3 vector = base.transform.position;
		Vector3 b = currentTarget.position + Offset;
		b.z += zOffset;
		if (lerpSpeed > Mathf.Epsilon)
		{
			b = Vector3.Lerp(vector, b, lerpSpeed * Time.deltaTime);
		}
		if ((followAxes & FollowAxes.X) == FollowAxes.X)
		{
			vector.x = b.x;
		}
		if ((followAxes & FollowAxes.Y) == FollowAxes.Y)
		{
			vector.y = b.y;
		}
		if ((followAxes & FollowAxes.Z) == FollowAxes.Z)
		{
			vector.z = b.z;
		}
		if (lerpFromInitialPosition < 1f - Mathf.Epsilon)
		{
			vector = Vector3.Lerp(initialPosition, vector, lerpFromInitialPosition);
		}
		if (useRigidbody)
		{
			if ((bool)body)
			{
				body.MovePosition(vector);
			}
		}
		else
		{
			base.transform.position = vector;
		}
	}

	public void ForceFollowUpdate()
	{
		DoUpdate();
	}

	public void UpdateOrderUpdate()
	{
		DoUpdate();
	}

	public void ClearTargetOnDisable()
	{
		shouldClearTargetOnDisable = true;
	}
}
