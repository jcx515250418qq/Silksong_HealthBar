using System;
using TeamCherry.SharedUtils;
using UnityEngine;

public class AudioSourceMovingVolume : MonoBehaviour, IUpdateBatchableLateUpdate
{
	[SerializeField]
	private Transform target;

	[SerializeField]
	private MinMaxFloat speedRange;

	[SerializeField]
	private AudioSource audioSource;

	[SerializeField]
	private MinMaxFloat volumeRange = new MinMaxFloat(0f, 1f);

	[SerializeField]
	private float lerpSpeed;

	private Vector2 previousLocalPos;

	private VectorCurveAnimator curveAnimator;

	private UpdateBatcher updateBatcher;

	public bool ShouldUpdate => true;

	private void OnEnable()
	{
		previousLocalPos = target.localPosition;
		audioSource.volume = 0f;
		curveAnimator = target.GetComponentInParent<VectorCurveAnimator>();
		if ((bool)curveAnimator)
		{
			VectorCurveAnimator vectorCurveAnimator = curveAnimator;
			vectorCurveAnimator.UpdatedPosition = (Action)Delegate.Combine(vectorCurveAnimator.UpdatedPosition, new Action(BatchedLateUpdate));
		}
		else
		{
			updateBatcher = GameManager.instance.GetComponent<UpdateBatcher>();
			updateBatcher.Add(this);
		}
	}

	private void OnDisable()
	{
		if ((bool)curveAnimator)
		{
			VectorCurveAnimator vectorCurveAnimator = curveAnimator;
			vectorCurveAnimator.UpdatedPosition = (Action)Delegate.Remove(vectorCurveAnimator.UpdatedPosition, new Action(BatchedLateUpdate));
			curveAnimator = null;
		}
		if ((bool)updateBatcher)
		{
			updateBatcher.Remove(this);
			updateBatcher = null;
		}
	}

	public void BatchedLateUpdate()
	{
		Vector2 vector = target.localPosition;
		Vector2 vector2 = vector - previousLocalPos;
		float num = Time.deltaTime;
		if (num == 0f)
		{
			num = Time.fixedDeltaTime;
		}
		float value = vector2.magnitude / num;
		float tBetween = speedRange.GetTBetween(value);
		float num2 = volumeRange.GetLerpedValue(tBetween);
		if (float.IsNaN(num2))
		{
			num2 = volumeRange.Start;
		}
		audioSource.volume = ((lerpSpeed <= Mathf.Epsilon) ? num2 : Mathf.Lerp(audioSource.volume, num2, num * lerpSpeed));
		previousLocalPos = vector;
	}
}
