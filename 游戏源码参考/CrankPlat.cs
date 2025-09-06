using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class CrankPlat : MonoBehaviour, HeroPlatformStick.IMoveHooks
{
	[SerializeField]
	private PersistentBoolItem persistent;

	[SerializeField]
	private bool returnOnHeroBelow;

	[Space]
	[SerializeField]
	private Transform crank;

	[SerializeField]
	private float distanceRotation;

	[SerializeField]
	private HitResponse crankHit;

	[SerializeField]
	private float crankInertZ;

	[SerializeField]
	private float crankEndRotation;

	[Space]
	[SerializeField]
	private Transform endPoint;

	[SerializeField]
	private float hitForce;

	[SerializeField]
	private float maxHitSpeed;

	[SerializeField]
	private float returnForce;

	[SerializeField]
	private float maxReturnSpeed;

	[Space]
	[SerializeField]
	private AudioEventRandom riseAudio;

	[SerializeField]
	private AudioEventRandom endAudio;

	[SerializeField]
	private AudioEventRandom fallAudio;

	[SerializeField]
	private AudioEventRandom returnedAudio;

	[SerializeField]
	private AudioSource playOnSource;

	[Space]
	public UnityEvent OnReturned;

	public UnityEvent OnCompleted;

	private bool isComplete;

	private Vector2 startPos;

	private Vector2 endPos;

	private float posT;

	private float speed;

	private Coroutine hitRoutine;

	private float crankHitDir;

	public event Action OnStartMove;

	public event Action OnStopMove;

	private void OnDrawGizmos()
	{
		if ((bool)endPoint)
		{
			Gizmos.DrawLine(base.transform.position, endPoint.position);
		}
	}

	private void Awake()
	{
		startPos = base.transform.position;
		endPos = endPoint.position;
		if ((bool)persistent)
		{
			persistent.OnGetSaveState += delegate(out bool value)
			{
				value = isComplete;
			};
			persistent.OnSetSaveState += delegate(bool value)
			{
				isComplete = value;
				if (isComplete)
				{
					SetComplete();
					UpdatePos();
				}
			};
		}
		if (!crankHit)
		{
			return;
		}
		crankHit.WasHitDirectional += delegate(HitInstance.HitDirection direction)
		{
			switch (direction)
			{
			case HitInstance.HitDirection.Left:
				crankHitDir = 1f;
				break;
			case HitInstance.HitDirection.Right:
				crankHitDir = -1f;
				break;
			default:
			{
				HeroController instance = HeroController.instance;
				crankHitDir = 0f - Mathf.Sign(base.transform.position.x - instance.transform.position.x);
				break;
			}
			}
			DoHit();
		};
	}

	public void DoHit()
	{
		if (!isComplete)
		{
			if (speed < 0f)
			{
				speed = 0f;
			}
			speed += hitForce;
			PlayAudioEventOnSource(riseAudio);
			if (hitRoutine == null)
			{
				hitRoutine = StartCoroutine(HitRoutine());
			}
		}
	}

	private void PlayAudioEventOnSource(AudioEventRandom audioEvent)
	{
		playOnSource.Stop();
		playOnSource.clip = audioEvent.GetClip();
		playOnSource.pitch = audioEvent.SelectPitch();
		playOnSource.volume = audioEvent.Volume;
		playOnSource.Play();
	}

	private IEnumerator HitRoutine()
	{
		this.OnStartMove?.Invoke();
		float distance = Vector2.Distance(startPos, endPos);
		bool flag;
		do
		{
			float deltaTime = Time.deltaTime;
			float num = Mathf.Clamp(speed, 0f - maxReturnSpeed, maxHitSpeed);
			posT += num / distance * deltaTime;
			float num2 = speed;
			speed -= returnForce * deltaTime;
			if (num2 >= 0f && speed < 0f)
			{
				PlayAudioEventOnSource(fallAudio);
			}
			UpdatePos();
			yield return null;
			flag = posT >= 1f - Mathf.Epsilon;
		}
		while (!flag && posT > Mathf.Epsilon);
		UpdatePos();
		playOnSource.Stop();
		if (flag)
		{
			PlayAudioEventOnSource(endAudio);
			SetComplete();
			OnCompleted.Invoke();
		}
		else
		{
			PlayAudioEventOnSource(returnedAudio);
			posT = 0f;
			OnReturned.Invoke();
		}
		this.OnStopMove?.Invoke();
		speed = 0f;
		if (flag && returnOnHeroBelow)
		{
			HeroController hc = HeroController.instance;
			Vector3 pos = base.transform.position;
			pos.y -= 5f;
			while (hc.transform.position.y > pos.y)
			{
				yield return null;
			}
			posT -= 0.01f;
			isComplete = false;
			SetCrankEnabled(value: true);
			hitRoutine = StartCoroutine(HitRoutine());
		}
		else
		{
			hitRoutine = null;
		}
	}

	private void UpdatePos()
	{
		Vector2 vector = Vector2.Lerp(startPos, endPos, posT);
		base.transform.SetPosition2D(vector);
		if (!isComplete && (bool)crank)
		{
			crank.SetRotation2D(Vector2.Distance(vector, startPos) * distanceRotation * crankHitDir);
		}
	}

	private void SetComplete()
	{
		posT = 1f;
		isComplete = true;
		if ((bool)crank)
		{
			crank.SetLocalPositionZ(crankInertZ);
			crank.SetLocalRotation2D(crankEndRotation);
		}
		SetCrankEnabled(value: false);
	}

	private void SetCrankEnabled(bool value)
	{
		if ((bool)crankHit)
		{
			Collider2D component = crankHit.GetComponent<Collider2D>();
			if ((bool)component)
			{
				component.enabled = value;
			}
		}
	}

	public void AddMoveHooks(Action onStartMove, Action onStopMove)
	{
		OnStartMove += onStartMove;
		OnStopMove += onStopMove;
	}
}
