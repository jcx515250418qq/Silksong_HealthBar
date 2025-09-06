using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BounceBalloonCrane : UnlockablePropBase
{
	[SerializeField]
	private Transform hangChain;

	[SerializeField]
	private Vector2 hangChainRetractedPos;

	[Space]
	[SerializeField]
	private Vector2 beforeMoveOffset;

	[SerializeField]
	private AnimationCurve beforeMoveCurve;

	[SerializeField]
	private float beforeMoveDuration;

	[Space]
	[SerializeField]
	private float hangChainMoveSpeed;

	[SerializeField]
	private AnimationCurve hangChainMoveCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);

	[Space]
	[SerializeField]
	private Vector2 afterMoveOffset;

	[SerializeField]
	private AnimationCurve afterMoveCurve;

	[SerializeField]
	private float afterMoveDuration;

	[Space]
	[SerializeField]
	private CameraShakeTarget endCameraShake;

	[Space]
	[SerializeField]
	private LoopRotator[] gears;

	[SerializeField]
	private GameObject startEffectsParent;

	[SerializeField]
	private GameObject endEffectsParent;

	[Space]
	[SerializeField]
	private float balloonInflateWait;

	[Space]
	[SerializeField]
	private AudioSource chainLoopSource;

	[SerializeField]
	private AudioClip chainEndSound;

	private Vector2 hangChainEndPos;

	private Stack<Coroutine> moveRoutines;

	private BounceBalloonCraneAnchor[] anchors;

	private void OnDrawGizmosSelected()
	{
		if ((bool)hangChain && (bool)hangChain.parent)
		{
			Gizmos.DrawWireSphere(hangChain.parent.TransformPoint(hangChainRetractedPos), 0.1f);
		}
	}

	private void Awake()
	{
		if (gears == null)
		{
			gears = Array.Empty<LoopRotator>();
		}
		anchors = GetComponentsInChildren<BounceBalloonCraneAnchor>();
	}

	private void Start()
	{
		if ((bool)startEffectsParent)
		{
			startEffectsParent.SetActive(value: false);
		}
		if ((bool)endEffectsParent)
		{
			endEffectsParent.SetActive(value: false);
		}
		if ((bool)hangChain)
		{
			hangChainEndPos = hangChain.localPosition;
			hangChain.SetLocalPosition2D(hangChainRetractedPos);
			BounceBalloonCraneAnchor[] array = anchors;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].SetInactive();
			}
		}
	}

	public override void Open()
	{
		if ((bool)hangChain)
		{
			StopMoveRoutines();
			hangChain.SetLocalPosition2D(hangChainRetractedPos);
			BounceBalloonCraneAnchor[] array = anchors;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].SetInactive();
			}
			if (moveRoutines == null)
			{
				moveRoutines = new Stack<Coroutine>(4);
			}
			moveRoutines.Push(StartCoroutine(Move()));
		}
	}

	public override void Opened()
	{
		if ((bool)hangChain)
		{
			StopMoveRoutines();
			hangChain.SetLocalPosition2D(hangChainEndPos);
			LoopRotator[] array = gears;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].enabled = false;
			}
			BounceBalloonCraneAnchor[] array2 = anchors;
			for (int i = 0; i < array2.Length; i++)
			{
				array2[i].SetActive(isInstant: true);
			}
		}
	}

	private void StopMoveRoutines()
	{
		if (moveRoutines != null)
		{
			while (moveRoutines.Count > 0)
			{
				StopCoroutine(moveRoutines.Pop());
			}
		}
	}

	private IEnumerator Move()
	{
		LoopRotator[] array = gears;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].StartRotation();
		}
		if ((bool)chainLoopSource)
		{
			chainLoopSource.Play();
		}
		if ((bool)startEffectsParent)
		{
			startEffectsParent.SetActive(value: false);
			startEffectsParent.SetActive(value: true);
		}
		if (beforeMoveDuration > 0f)
		{
			Vector2 initialPos = hangChain.localPosition;
			Vector2 targetPos = initialPos + beforeMoveOffset;
			yield return moveRoutines.PushReturn(this.StartTimerRoutine(0f, beforeMoveDuration, delegate(float time)
			{
				hangChain.SetLocalPosition2D(Vector2.LerpUnclamped(initialPos, targetPos, beforeMoveCurve.Evaluate(time)));
			}));
		}
		if (hangChainMoveSpeed > 0f)
		{
			Vector2 initialPos2 = hangChain.localPosition;
			float duration = (hangChainEndPos - hangChainRetractedPos).magnitude / hangChainMoveSpeed;
			yield return moveRoutines.PushReturn(this.StartTimerRoutine(0f, duration, delegate(float time)
			{
				float t = hangChainMoveCurve.Evaluate(time);
				hangChain.SetLocalPosition2D(Vector2.LerpUnclamped(initialPos2, hangChainEndPos, t));
			}));
		}
		array = gears;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].StopRotation();
		}
		endCameraShake.DoShake(this);
		if ((bool)chainLoopSource)
		{
			chainLoopSource.Stop();
			if ((bool)chainEndSound)
			{
				chainLoopSource.PlayOneShot(chainEndSound);
			}
		}
		if ((bool)endEffectsParent)
		{
			endEffectsParent.SetActive(value: false);
			endEffectsParent.SetActive(value: true);
		}
		if (afterMoveDuration > 0f)
		{
			Vector2 initialPos3 = hangChain.localPosition;
			Vector2 targetPos2 = initialPos3 + afterMoveOffset;
			yield return moveRoutines.PushReturn(this.StartTimerRoutine(0f, afterMoveDuration, delegate(float time)
			{
				hangChain.SetLocalPosition2D(Vector2.LerpUnclamped(initialPos3, targetPos2, afterMoveCurve.Evaluate(time)));
			}));
		}
		if (balloonInflateWait > 0f)
		{
			yield return new WaitForSeconds(balloonInflateWait);
		}
		BounceBalloonCraneAnchor[] array2 = anchors;
		for (int i = 0; i < array2.Length; i++)
		{
			array2[i].SetActive(isInstant: false);
		}
	}
}
