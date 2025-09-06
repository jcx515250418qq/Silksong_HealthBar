using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCFlyAround : MonoBehaviour
{
	private enum FlyingAnimStates
	{
		Inactive = 0,
		Flying = 1,
		Turning = 2
	}

	[SerializeField]
	private tk2dSpriteAnimator animator;

	[SerializeField]
	[InspectorValidation]
	private LookAnimNPC lookAnimNPC;

	[Space]
	[SerializeField]
	[ModifiableProperty]
	[InspectorValidation("ValidateAnimName")]
	private string flyAnim;

	[SerializeField]
	[ModifiableProperty]
	[InspectorValidation("ValidateAnimName")]
	private string turnAnim;

	[SerializeField]
	[ModifiableProperty]
	[InspectorValidation("ValidateAnimName")]
	private string flyEndAnim;

	[SerializeField]
	[ModifiableProperty]
	[InspectorValidation("ValidateAnimName")]
	private string talkAnim;

	[Space]
	[SerializeField]
	private AnimationCurve flyCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);

	[SerializeField]
	private float flySpeed = 10f;

	[Tooltip("Ignores new point if less than min distance away.")]
	[SerializeField]
	private float minFlyDistance = 1f;

	[Space]
	[SerializeField]
	private Transform talkFlyPointsParent;

	[SerializeField]
	private Vector2 minPointsPos = Vector2.negativeInfinity;

	[SerializeField]
	private Vector2 maxPointsPos = Vector2.positiveInfinity;

	[Space]
	[SerializeField]
	private PlayMakerFSM flyAudioFSM;

	private int nextFlyPointIndex;

	private List<Transform> talkingFlyPoints;

	private readonly List<Coroutine> flyRoutines = new List<Coroutine>();

	private bool isFlyingAroundTalking;

	private bool reActivateLookAnimNpc;

	private bool shouldStopFlyingAnimRoutine;

	private FlyingAnimStates flyingAnimState;

	public event Action ArrivedAtPoint;

	private void OnDrawGizmosSelected()
	{
		Vector2 vector = maxPointsPos - minPointsPos;
		Gizmos.DrawWireCube(minPointsPos + vector / 2f, vector);
	}

	private bool? ValidateAnimName(string animName)
	{
		return animator.IsAnimValid(animName, isRequired: false);
	}

	private void Awake()
	{
		if ((bool)talkFlyPointsParent)
		{
			talkingFlyPoints = new List<Transform>(talkFlyPointsParent.childCount);
			for (int i = 0; i < talkFlyPointsParent.childCount; i++)
			{
				talkingFlyPoints.Add(talkFlyPointsParent.GetChild(i));
			}
		}
		else
		{
			talkingFlyPoints = new List<Transform>(0);
		}
	}

	private float PlayAnim(string animName)
	{
		if (!animator)
		{
			return 0f;
		}
		if (string.IsNullOrEmpty(animName))
		{
			return 0f;
		}
		return animator.PlayAnimGetTime(animName);
	}

	private void StopFlyingRoutines()
	{
		foreach (Coroutine flyRoutine in flyRoutines)
		{
			if (flyRoutine != null)
			{
				StopCoroutine(flyRoutine);
			}
		}
		flyRoutines.Clear();
		flyingAnimState = FlyingAnimStates.Inactive;
		nextFlyPointIndex = 0;
	}

	public void StartFlyToPoint(Vector2 point)
	{
		if (!isFlyingAroundTalking)
		{
			StopFlyingRoutines();
			flyRoutines.Add(StartCoroutine(FlyToPoint(point, null, force: true)));
		}
	}

	private IEnumerator FlyToPoint(Vector2 targetPos, Func<bool> breakCondition = null, bool force = false)
	{
		bool wasLookAnimNpcActive = lookAnimNPC.State != LookAnimNPC.AnimState.Disabled;
		if (wasLookAnimNpcActive)
		{
			lookAnimNPC.Deactivate();
			reActivateLookAnimNpc = true;
		}
		shouldStopFlyingAnimRoutine = false;
		Coroutine animRoutine = StartCoroutine(FlyingAnimations());
		flyRoutines.Add(animRoutine);
		Vector2 startPos = base.transform.position;
		float distance = Vector2.Distance(startPos, targetPos);
		bool shouldMove = force || distance > minFlyDistance;
		if (shouldMove)
		{
			float duration = distance / flySpeed;
			if (distance > 0f && (bool)flyAudioFSM)
			{
				flyAudioFSM.SendEvent("FLY START");
			}
			for (float elapsed = 0f; elapsed < duration; elapsed += Time.deltaTime)
			{
				if (breakCondition != null && breakCondition())
				{
					break;
				}
				float t = flyCurve.Evaluate(elapsed / duration);
				Vector2 position = Vector2.LerpUnclamped(startPos, targetPos, t);
				base.transform.SetPosition2D(position);
				yield return null;
			}
		}
		if ((bool)flyAudioFSM)
		{
			flyAudioFSM.SendEvent("FLY STOP");
		}
		if (shouldMove)
		{
			base.transform.SetPosition2D(targetPos);
		}
		shouldStopFlyingAnimRoutine = true;
		while (flyingAnimState != 0)
		{
			yield return null;
		}
		StopCoroutine(animRoutine);
		if (distance >= minFlyDistance || (force && distance > 0.5f))
		{
			yield return new WaitForSeconds(PlayAnim(flyEndAnim));
		}
		PlayTalkOrFly();
		if (wasLookAnimNpcActive)
		{
			lookAnimNPC.Activate();
		}
		if (this.ArrivedAtPoint != null)
		{
			this.ArrivedAtPoint();
		}
	}

	private IEnumerator FlyingAnimations()
	{
		bool wasFacingLeft = lookAnimNPC.WasFacingLeft;
		while (!shouldStopFlyingAnimRoutine)
		{
			yield return null;
			PlayTalkOrFly();
			flyingAnimState = FlyingAnimStates.Flying;
			bool shouldFaceLeft = lookAnimNPC.ShouldFaceLeft();
			if (wasFacingLeft == shouldFaceLeft)
			{
				continue;
			}
			flyingAnimState = FlyingAnimStates.Turning;
			lookAnimNPC.ForceShouldTurnChecking = true;
			lookAnimNPC.Activate();
			yield return null;
			while (true)
			{
				LookAnimNPC.AnimState state = lookAnimNPC.State;
				if (state != LookAnimNPC.AnimState.TurningRight && state != LookAnimNPC.AnimState.TurningLeft)
				{
					break;
				}
				yield return null;
			}
			lookAnimNPC.ForceShouldTurnChecking = false;
			lookAnimNPC.Deactivate();
			wasFacingLeft = shouldFaceLeft;
		}
		flyingAnimState = FlyingAnimStates.Inactive;
	}

	private void PlayTalkOrFly()
	{
		if (lookAnimNPC.IsNPCTalking)
		{
			PlayAnim(talkAnim);
		}
		else
		{
			PlayAnim(flyAnim);
		}
	}

	private IEnumerator FlyAroundTalking()
	{
		while (!lookAnimNPC.IsNPCTalking)
		{
			yield return null;
		}
		LookAnimNPC.AnimState state = lookAnimNPC.State;
		bool wasLookAnimNpcActive = state != LookAnimNPC.AnimState.Disabled;
		if (wasLookAnimNpcActive)
		{
			while (state == LookAnimNPC.AnimState.TurningLeft || state == LookAnimNPC.AnimState.TurningRight)
			{
				yield return null;
				state = lookAnimNPC.State;
			}
			lookAnimNPC.Deactivate();
		}
		if (wasLookAnimNpcActive)
		{
			reActivateLookAnimNpc = true;
		}
		bool wasNpcTalking = false;
		while (true)
		{
			if (lookAnimNPC.IsNPCTalking)
			{
				if (!wasNpcTalking)
				{
					PlayAnim(talkAnim);
				}
			}
			else if (wasNpcTalking)
			{
				PlayAnim(flyAnim);
			}
			wasNpcTalking = lookAnimNPC.IsNPCTalking;
			int previousLineNumber = lookAnimNPC.CurrentLineNumber;
			if (talkingFlyPoints.Count > 0)
			{
				while (lookAnimNPC.CurrentLineNumber == previousLineNumber)
				{
					yield return null;
				}
			}
			else
			{
				while (lookAnimNPC.IsNPCTalking == wasNpcTalking)
				{
					yield return null;
				}
			}
			if (lookAnimNPC.IsNPCTalking && talkingFlyPoints.Count > 0)
			{
				Transform transform = talkingFlyPoints[nextFlyPointIndex];
				bool flag = true;
				for (int i = 0; i < talkingFlyPoints.Count; i++)
				{
					nextFlyPointIndex++;
					if (nextFlyPointIndex >= talkingFlyPoints.Count)
					{
						nextFlyPointIndex = 0;
					}
					transform = talkingFlyPoints[nextFlyPointIndex];
					Vector3 position = transform.position;
					if (position.x > minPointsPos.x && position.y > minPointsPos.y && position.x < maxPointsPos.x && position.y < maxPointsPos.y && flag)
					{
						flag = false;
						if (Vector2.Distance(position, base.transform.position) > minFlyDistance)
						{
							break;
						}
					}
				}
				Coroutine routine = StartCoroutine(FlyToPoint(transform.position, () => !lookAnimNPC.IsNPCTalking));
				flyRoutines.Add(routine);
				yield return routine;
				flyRoutines.Remove(routine);
			}
			yield return null;
		}
	}

	public void EnableTalkingFlyAround()
	{
		if (!isFlyingAroundTalking && (bool)lookAnimNPC)
		{
			isFlyingAroundTalking = true;
			StopFlyingRoutines();
			flyRoutines.Add(StartCoroutine(FlyAroundTalking()));
		}
	}

	public void DisableTalkingFlyAround()
	{
		if (isFlyingAroundTalking)
		{
			isFlyingAroundTalking = false;
			StopFlyingRoutines();
			if (reActivateLookAnimNpc)
			{
				lookAnimNPC.Activate();
				reActivateLookAnimNpc = false;
			}
		}
	}
}
