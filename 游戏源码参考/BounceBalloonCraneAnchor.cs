using UnityEngine;

public class BounceBalloonCraneAnchor : MonoBehaviour
{
	[SerializeField]
	private SpriteRenderer fakeBalloon;

	[SerializeField]
	private BounceBalloon realBalloon;

	[SerializeField]
	private GameObject realBalloonJoiner;

	[Space]
	[SerializeField]
	private float fakeBalloonLerpTime;

	[SerializeField]
	private AnimationCurve fakeBalloonLerpCurveX = AnimationCurve.Linear(0f, 0f, 1f, 1f);

	[SerializeField]
	private AnimationCurve fakeBalloonLerpCurveY = AnimationCurve.Linear(0f, 0f, 1f, 1f);

	private Transform originalJoinerParent;

	private Vector3 originalJoinerPosition;

	private Vector3 originalFakeBalloonPos;

	private Coroutine moveRoutine;

	public void SetInactive()
	{
		ResetMove();
		if ((bool)fakeBalloon)
		{
			fakeBalloon.enabled = true;
		}
		if ((bool)realBalloon)
		{
			realBalloon.SetDeflated();
			realBalloon.gameObject.SetActive(value: false);
		}
		if ((bool)realBalloonJoiner)
		{
			realBalloonJoiner.SetActive(value: false);
			if ((bool)originalJoinerParent)
			{
				realBalloonJoiner.transform.SetParent(originalJoinerParent, worldPositionStays: true);
				realBalloonJoiner.transform.localPosition = originalJoinerPosition;
			}
		}
	}

	public void SetActive(bool isInstant)
	{
		ResetMove();
		Vector3 ropeScale = Vector3.zero;
		Transform rope = null;
		if ((bool)realBalloonJoiner)
		{
			realBalloonJoiner.SetActive(value: true);
			if ((bool)realBalloon)
			{
				if (originalJoinerParent == null)
				{
					originalJoinerPosition = realBalloonJoiner.transform.localPosition;
					originalJoinerParent = realBalloonJoiner.transform.parent;
				}
				realBalloonJoiner.transform.SetParent(realBalloon.transform, worldPositionStays: true);
			}
			rope = realBalloonJoiner.transform.GetChild(0);
			if ((bool)rope)
			{
				Transform transform = new GameObject("Pivot").transform;
				transform.SetParentReset(realBalloonJoiner.transform);
				rope.SetParent(transform, worldPositionStays: true);
				rope = transform;
				ropeScale = rope.localScale;
			}
		}
		if (isInstant || fakeBalloonLerpTime <= 0f || !fakeBalloon || !realBalloon)
		{
			if ((bool)realBalloon)
			{
				realBalloon.gameObject.SetActive(value: true);
				if (isInstant)
				{
					realBalloon.Opened();
				}
				else
				{
					realBalloon.Open();
				}
			}
			if ((bool)fakeBalloon)
			{
				fakeBalloon.enabled = false;
			}
			return;
		}
		Transform transform2 = fakeBalloon.transform;
		originalFakeBalloonPos = transform2.localPosition;
		Transform realBalloonTrans = realBalloon.transform;
		Vector3 fromPos = transform2.position;
		Vector3 toPos = realBalloonTrans.position;
		fakeBalloon.enabled = false;
		AmbientFloat floater = realBalloon.GetComponentInChildren<AmbientFloat>();
		if ((bool)floater)
		{
			floater.enabled = false;
		}
		realBalloonTrans.position = fromPos;
		realBalloon.gameObject.SetActive(value: true);
		realBalloon.Open();
		moveRoutine = this.StartTimerRoutine(0f, fakeBalloonLerpTime, delegate(float time)
		{
			float x = Mathf.LerpUnclamped(fromPos.x, toPos.x, fakeBalloonLerpCurveX.Evaluate(time));
			float y = Mathf.LerpUnclamped(fromPos.y, toPos.y, fakeBalloonLerpCurveY.Evaluate(time));
			float z = Mathf.LerpUnclamped(fromPos.z, toPos.z, time);
			realBalloonTrans.position = new Vector3(x, y, z);
			if ((bool)rope)
			{
				rope.transform.localScale = ropeScale * time;
			}
		}, null, delegate
		{
			realBalloonTrans.position = toPos;
			if ((bool)floater)
			{
				floater.enabled = true;
			}
		});
	}

	private void ResetMove()
	{
		if (moveRoutine != null)
		{
			StopCoroutine(moveRoutine);
			fakeBalloon.transform.localPosition = originalFakeBalloonPos;
		}
	}
}
