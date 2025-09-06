using System.Collections;
using System.Collections.Generic;
using TeamCherry.NestedFadeGroup;
using TeamCherry.Splines;
using UnityEngine;

public class SplineDepress : MonoBehaviour
{
	[SerializeField]
	private SplineBase spline;

	[SerializeField]
	private Transform follower;

	[SerializeField]
	private Vector2 depressDistance;

	[SerializeField]
	private float releaseDuration;

	[SerializeField]
	private AnimationCurve releaseCurve;

	[Space]
	[SerializeField]
	private NestedFadeGroupFloatEvent walkCreakLoop;

	[SerializeField]
	private float walkCreakFadeUpTime;

	[SerializeField]
	private float walkCreakHoldTime;

	[SerializeField]
	private float walkCreakFadeDownTime;

	private Vector2 initialFollowerPos;

	private Coroutine followRoutine;

	private readonly List<Transform> touching = new List<Transform>();

	private void Awake()
	{
		spline.UpdateCondition = SplineBase.UpdateConditions.Manual;
		initialFollowerPos = follower.localPosition;
	}

	private void OnCollisionEnter2D(Collision2D other)
	{
		if (other.GetSafeContact().Normal.y >= 0f)
		{
			return;
		}
		touching.AddIfNotPresent(other.transform);
		if (touching.Count == 1)
		{
			if (followRoutine != null)
			{
				StopCoroutine(followRoutine);
			}
			followRoutine = StartCoroutine(FollowRoutine());
		}
	}

	private void OnCollisionExit2D(Collision2D other)
	{
		touching.Remove(other.transform);
	}

	private IEnumerator FollowRoutine()
	{
		follower.SetLocalPosition2D(initialFollowerPos + depressDistance);
		float walkCreakTimeLeft = walkCreakHoldTime;
		float previousCreakX = 0f;
		bool isCreaking = true;
		walkCreakLoop.AlphaSelf = 1f;
		do
		{
			Vector2 zero = Vector2.zero;
			foreach (Transform item in touching)
			{
				zero += (Vector2)item.position;
			}
			zero /= (float)touching.Count;
			follower.SetPositionX(zero.x);
			spline.UpdateSpline();
			if (Mathf.Abs(zero.x - previousCreakX) > 0.001f)
			{
				previousCreakX = zero.x;
				walkCreakTimeLeft = walkCreakHoldTime;
				if (!isCreaking)
				{
					isCreaking = true;
					walkCreakLoop.FadeToOne(walkCreakFadeUpTime);
				}
			}
			else if (isCreaking)
			{
				walkCreakTimeLeft -= Time.deltaTime;
				if (walkCreakTimeLeft < 0f)
				{
					isCreaking = false;
					walkCreakLoop.FadeToZero(walkCreakFadeDownTime);
				}
			}
			yield return null;
		}
		while (touching.Count != 0);
		walkCreakLoop.FadeToZero(walkCreakFadeDownTime);
		float elapsed = 0f;
		Vector2 fromPos = follower.localPosition;
		for (; elapsed < releaseDuration; elapsed += Time.deltaTime)
		{
			float t = releaseCurve.Evaluate(elapsed / releaseDuration);
			Vector2 position = Vector2.Lerp(fromPos, initialFollowerPos, t);
			follower.SetLocalPosition2D(position);
			spline.UpdateSpline();
			yield return null;
		}
		follower.SetLocalPosition2D(initialFollowerPos);
		spline.UpdateSpline();
		followRoutine = null;
	}
}
