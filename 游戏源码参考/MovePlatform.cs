using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovePlatform : UnlockablePropBase
{
	[SerializeField]
	private Vector2 beforeMoveOffset;

	[SerializeField]
	private AnimationCurve beforeMoveCurve;

	[SerializeField]
	private float beforeMoveDuration;

	[Space]
	[SerializeField]
	private Vector2 inactivePos;

	[SerializeField]
	private Vector2 activePos;

	[SerializeField]
	private float moveSpeed = 10f;

	[Space]
	[SerializeField]
	private Vector2 afterMoveOffset;

	[SerializeField]
	private AnimationCurve afterMoveCurve;

	[SerializeField]
	private float afterMoveDuration;

	[Space]
	[SerializeField]
	private GameObject[] activateWhileMoving;

	[SerializeField]
	private GameObject[] activateOnMove;

	[SerializeField]
	private Transform gearParent;

	[SerializeField]
	private PlayMakerFSM swayPlatFsm;

	[Space]
	[SerializeField]
	private AudioClip moveStartClip;

	[SerializeField]
	private AudioSource moveAudioSource;

	[SerializeField]
	private AudioClip moveEndClip;

	private bool activated;

	private LoopRotator[] gears;

	private Stack<Coroutine> moveRoutines;

	private void Reset()
	{
		activePos = (inactivePos = base.transform.position) + Vector2.down;
	}

	private void OnDrawGizmosSelected()
	{
		float z = base.transform.position.z;
		Gizmos.color = Color.yellow;
		Gizmos.DrawWireSphere(inactivePos.ToVector3(z), 0.2f);
		Gizmos.color = Color.green;
		Gizmos.DrawWireSphere(activePos.ToVector3(z), 0.2f);
	}

	private void Start()
	{
		if ((bool)gearParent)
		{
			gears = gearParent.GetComponentsInChildren<LoopRotator>();
			gearParent.SetParent(null, worldPositionStays: true);
		}
		else
		{
			gears = new LoopRotator[0];
		}
		activateWhileMoving.SetAllActive(value: false);
		activateOnMove.SetAllActive(value: false);
		SetCanSway(value: false);
	}

	private void SetCanSway(bool value)
	{
		if ((bool)swayPlatFsm)
		{
			bool flag = !value;
			swayPlatFsm.FsmVariables.FindFsmBool("Dont Move").Value = flag;
			if (flag)
			{
				swayPlatFsm.SendEvent("STOP MOVE");
			}
		}
	}

	public override void Open()
	{
		if (!activated)
		{
			activated = true;
			moveRoutines = new Stack<Coroutine>(4);
			moveRoutines.Push(StartCoroutine(Move()));
		}
	}

	public override void Opened()
	{
		activated = true;
		if (moveRoutines != null)
		{
			while (moveRoutines.Count > 0)
			{
				StopCoroutine(moveRoutines.Pop());
			}
		}
		base.transform.SetPosition2D(activePos);
		LoopRotator[] array = gears;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].enabled = false;
		}
		SetCanSway(value: true);
	}

	private IEnumerator Move()
	{
		activateOnMove.SetAllActive(value: true);
		activateWhileMoving.SetAllActive(value: true);
		LoopRotator[] array = gears;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].StartRotation();
		}
		if ((bool)moveAudioSource)
		{
			moveAudioSource.Play();
			if ((bool)moveStartClip)
			{
				moveAudioSource.PlayOneShot(moveStartClip);
			}
		}
		if (beforeMoveDuration > 0f)
		{
			Vector2 initialPos = base.transform.position;
			Vector2 targetPos = initialPos + beforeMoveOffset;
			yield return moveRoutines.PushReturn(this.StartTimerRoutine(0f, beforeMoveDuration, delegate(float time)
			{
				base.transform.SetPosition2D(Vector2.LerpUnclamped(initialPos, targetPos, beforeMoveCurve.Evaluate(time)));
			}));
		}
		if (moveSpeed > 0f)
		{
			Vector2 initialPos2 = base.transform.position;
			float duration = (activePos - inactivePos).magnitude / moveSpeed;
			yield return moveRoutines.PushReturn(this.StartTimerRoutine(0f, duration, delegate(float time)
			{
				base.transform.SetPosition2D(Vector2.LerpUnclamped(initialPos2, activePos, time));
			}));
		}
		array = gears;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].StopRotation();
		}
		if ((bool)moveAudioSource)
		{
			moveAudioSource.Stop();
			if ((bool)moveEndClip)
			{
				moveAudioSource.PlayOneShot(moveEndClip);
			}
		}
		if (afterMoveDuration > 0f)
		{
			Vector2 initialPos3 = base.transform.position;
			Vector2 targetPos2 = initialPos3 + afterMoveOffset;
			yield return moveRoutines.PushReturn(this.StartTimerRoutine(0f, afterMoveDuration, delegate(float time)
			{
				base.transform.SetPosition2D(Vector2.LerpUnclamped(initialPos3, targetPos2, afterMoveCurve.Evaluate(time)));
			}));
		}
		activateWhileMoving.SetAllActive(value: false);
		SetCanSway(value: true);
	}
}
