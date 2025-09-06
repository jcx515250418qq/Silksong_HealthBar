using UnityEngine;

public class LoopMovementTrack : MonoBehaviour
{
	[SerializeField]
	private Transform[] childrenToMove;

	[Space]
	[SerializeField]
	private Vector3 startPos;

	[SerializeField]
	private Vector3 endPos;

	[Space]
	[SerializeField]
	private Vector3 startScale = Vector3.one;

	[SerializeField]
	private Vector3 endScale = Vector3.one;

	[Space]
	[SerializeField]
	private float loopTime;

	[SerializeField]
	private AnimationCurve movementCurve;

	[SerializeField]
	private AnimationCurve scaleCurve;

	[SerializeField]
	private bool isReversed;

	private void OnValidate()
	{
		if (childrenToMove != null)
		{
			for (int i = 0; i < childrenToMove.Length; i++)
			{
				if (childrenToMove[i].parent != base.transform)
				{
					childrenToMove[i] = null;
					Debug.LogError("Assigned transform must be a child of this transform!", this);
				}
			}
		}
		if (loopTime < 0f)
		{
			loopTime = 0f;
		}
		UpdateChildren(0f);
	}

	private void OnDrawGizmosSelected()
	{
		Gizmos.matrix = base.transform.localToWorldMatrix;
		Gizmos.DrawWireSphere(startPos, 0.2f);
		Gizmos.DrawWireSphere(endPos, 0.2f);
	}

	private void Update()
	{
		if (!(loopTime <= 0f))
		{
			float timeOffset = Time.time / loopTime;
			UpdateChildren(timeOffset);
		}
	}

	private void UpdateChildren(float timeOffset)
	{
		if (childrenToMove == null || childrenToMove.Length == 0)
		{
			return;
		}
		int num = childrenToMove.Length;
		int num2 = num - 1;
		for (int i = 0; i < num; i++)
		{
			Transform obj = childrenToMove[i];
			float num3 = (float)i / (float)num2;
			float num4 = (timeOffset + num3) % 1f;
			if (isReversed)
			{
				num4 = 1f - num4;
			}
			obj.localPosition = Vector3.Lerp(startPos, endPos, movementCurve.Evaluate(num4));
			obj.localScale = Vector3.Lerp(startScale, endScale, scaleCurve.Evaluate(num4));
		}
	}
}
