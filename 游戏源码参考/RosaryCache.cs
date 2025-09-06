using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

public class RosaryCache : MonoBehaviour, IHitResponder
{
	[Serializable]
	protected class HitState
	{
		[FormerlySerializedAs("OnHit")]
		public UnityEvent OnActivated;

		private bool isActivated;

		public void Activate()
		{
			if (!isActivated)
			{
				isActivated = true;
				OnActivated?.Invoke();
			}
		}

		public HitState Duplicate()
		{
			return new HitState
			{
				OnActivated = new UnityEvent()
			};
		}
	}

	[SerializeField]
	private CameraShakeTarget hitCameraShake;

	[SerializeField]
	private GameObject hitEffectPrefab;

	[SerializeField]
	private bool clampHitPos;

	[SerializeField]
	private Vector2 clampHitPosMin;

	[SerializeField]
	private Vector2 clampHitPosMax;

	[SerializeField]
	private AudioEventRandom hitSound;

	[SerializeField]
	private AudioEventRandom emptyHitSound;

	[Space]
	[SerializeField]
	private AudioEventRandom touchSound;

	[SerializeField]
	private AudioEventRandom emptyTouchSound;

	[Space]
	[SerializeField]
	private PersistentIntItem persistent;

	[SerializeField]
	private bool deactivateCompleted;

	[SerializeField]
	private int startState = -1;

	[SerializeField]
	protected List<HitState> hitStates = new List<HitState>();

	[FormerlySerializedAs("canHitLastState")]
	[SerializeField]
	private bool canHitAfterLastState;

	[SerializeField]
	private FlingUtils.Config[] flingOnStateChange;

	[SerializeField]
	private Vector2 flingSpawnPos;

	[SerializeField]
	private Breakable breaker;

	[Space]
	public UnityEvent OnHit;

	public UnityEvent OnHitLeft;

	public UnityEvent OnHitRight;

	protected int State { get; private set; }

	protected int StateCount
	{
		get
		{
			if (hitStates == null)
			{
				return 0;
			}
			return hitStates.Count;
		}
	}

	protected virtual int HitCount => StateCount;

	protected bool IsReactivating { get; private set; }

	private void OnDrawGizmosSelected()
	{
		Gizmos.matrix = base.transform.localToWorldMatrix;
		Gizmos.DrawWireSphere(flingSpawnPos, 0.1f);
		if (clampHitPos)
		{
			Bounds bounds = default(Bounds);
			bounds.min = clampHitPosMin;
			bounds.max = clampHitPosMax;
			Bounds bounds2 = bounds;
			Gizmos.DrawWireCube(bounds2.center, bounds2.size);
		}
	}

	protected virtual void Awake()
	{
		State = startState;
		if ((bool)persistent)
		{
			persistent.OnGetSaveState += delegate(out int value)
			{
				value = State;
			};
			persistent.OnSetSaveState += delegate(int value)
			{
				State = value;
				IsReactivating = true;
				for (int i = 0; i <= Mathf.Min(State, hitStates.Count - 1); i++)
				{
					hitStates[i].Activate();
				}
				IsReactivating = false;
				if (State >= hitStates.Count - 1)
				{
					SetCompletedReturning();
					if ((bool)breaker)
					{
						breaker.SetAlreadyBroken();
					}
				}
			};
		}
		GameObject ownerObj = base.gameObject;
		bool flag = false;
		if ((bool)hitEffectPrefab)
		{
			PersonalObjectPool.EnsurePooledInScene(ownerObj, hitEffectPrefab, 1, finished: false);
			flag = true;
		}
		if (flingOnStateChange.Length != 0)
		{
			for (int j = 0; j < flingOnStateChange.Length; j++)
			{
				FlingUtils.EnsurePersonalPool(flingOnStateChange[j], ownerObj);
			}
			flag = true;
		}
		ChainPushReaction component = GetComponent<ChainPushReaction>();
		if (component != null)
		{
			component.OnTouched += Touched;
		}
		if (flag)
		{
			PersonalObjectPool.EnsurePooledInSceneFinished(ownerObj);
		}
	}

	public IHitResponder.HitResponse Hit(HitInstance damageInstance)
	{
		bool flag = true;
		AudioEventRandom audioEventRandom = hitSound;
		int state = State;
		if (hitStates.Count > 0)
		{
			State++;
			if (State >= hitStates.Count)
			{
				State = hitStates.Count - 1;
				if (!canHitAfterLastState)
				{
					return IHitResponder.Response.None;
				}
			}
			if (State >= HitCount)
			{
				flag = false;
				audioEventRandom = emptyHitSound;
			}
		}
		else
		{
			flag = false;
			audioEventRandom = emptyHitSound;
		}
		if (State != state)
		{
			DoFling();
			AttackTypes attackType = damageInstance.AttackType;
			if (attackType == AttackTypes.Explosion || attackType == AttackTypes.Heavy || attackType == AttackTypes.Lightning)
			{
				while (State < hitStates.Count - 1)
				{
					State++;
					DoFling();
				}
			}
		}
		Vector3 position = base.transform.position;
		float? y = GetHitSourceY(damageInstance.Source.transform.position.y);
		Vector3 vector = position.Where(null, y, null);
		if (clampHitPos)
		{
			Vector3 vector2 = base.transform.TransformPoint(clampHitPosMin);
			Vector3 vector3 = base.transform.TransformPoint(clampHitPosMax);
			if (vector.x < vector2.x)
			{
				vector.x = vector2.x;
			}
			else if (vector.x > vector3.x)
			{
				vector.x = vector3.x;
			}
			if (vector.y < vector2.y)
			{
				vector.y = vector2.y;
			}
			else if (vector.y > vector3.y)
			{
				vector.y = vector3.y;
			}
		}
		if ((bool)hitEffectPrefab)
		{
			hitEffectPrefab.Spawn(vector);
		}
		if (flag)
		{
			hitCameraShake.DoShake(this);
		}
		audioEventRandom.SpawnAndPlayOneShot(vector);
		RespondToHit(damageInstance, vector);
		OnHit.Invoke();
		float num;
		switch (damageInstance.GetActualHitDirection(base.transform, HitInstance.TargetType.Regular))
		{
		case HitInstance.HitDirection.Left:
			num = -1f;
			break;
		case HitInstance.HitDirection.Right:
			num = 1f;
			break;
		case HitInstance.HitDirection.Up:
		case HitInstance.HitDirection.Down:
			num = ((!(damageInstance.Source.transform.position.x > base.transform.position.x)) ? 1f : (-1f));
			break;
		default:
			throw new ArgumentOutOfRangeException();
		}
		if (num > 0f)
		{
			OnHitRight.Invoke();
		}
		else
		{
			OnHitLeft.Invoke();
		}
		if ((bool)breaker && State == hitStates.Count - 1)
		{
			breaker.BreakSelf();
		}
		return IHitResponder.Response.GenericHit;
		void DoFling()
		{
			((hitStates.Count > 0) ? hitStates[State] : null)?.Activate();
			FlingUtils.Config[] array = flingOnStateChange;
			for (int i = 0; i < array.Length; i++)
			{
				FlingUtils.SpawnAndFling(array[i], base.transform, flingSpawnPos);
			}
		}
	}

	public void Touched()
	{
		Touched(base.transform.position);
	}

	public void Touched(Vector3 touchPosition)
	{
		if (State >= HitCount)
		{
			emptyTouchSound.SpawnAndPlayOneShot(touchPosition);
		}
		else
		{
			touchSound.SpawnAndPlayOneShot(touchPosition);
		}
	}

	protected virtual float GetHitSourceY(float sourceHeight)
	{
		return sourceHeight;
	}

	protected virtual void RespondToHit(HitInstance damageInstance, Vector2 hitPos)
	{
	}

	protected virtual void SetCompletedReturning()
	{
		if (deactivateCompleted)
		{
			base.gameObject.SetActive(value: false);
		}
	}
}
