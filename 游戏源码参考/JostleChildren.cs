using System.Collections.Generic;
using UnityEngine;

public class JostleChildren : MonoBehaviour
{
	[SerializeField]
	private Vector3 offsetMin;

	[SerializeField]
	private Vector3 offsetMax;

	[Space]
	[SerializeField]
	private Vector3 rotationOffsetMin;

	[SerializeField]
	private Vector3 rotationOffsetMax;

	[Space]
	[SerializeField]
	private AnimationCurve jostleCurve;

	[SerializeField]
	private float jostleDuration;

	[Space]
	[SerializeField]
	private CameraShakeTarget cameraShake;

	private List<Transform> children;

	private List<Vector3> initialPositions;

	private List<Vector3> targetPositions;

	private List<Vector3> initialEulerAngles;

	private List<Vector3> targetEulerAngles;

	private bool isReady;

	private Coroutine jostleRoutine;

	private void Start()
	{
		int childCount = base.transform.childCount;
		children = new List<Transform>(childCount);
		initialPositions = new List<Vector3>(childCount);
		targetPositions = new List<Vector3>(childCount);
		initialEulerAngles = new List<Vector3>(childCount);
		targetEulerAngles = new List<Vector3>(childCount);
		isReady = true;
	}

	[ContextMenu("Do Jostle")]
	public void DoJostle()
	{
		if (!isReady)
		{
			return;
		}
		if (jostleRoutine != null)
		{
			StopCoroutine(jostleRoutine);
			jostleRoutine = null;
			for (int i = 0; i < children.Count; i++)
			{
				children[i].localPosition = initialPositions[i];
			}
		}
		children.Clear();
		initialPositions.Clear();
		targetPositions.Clear();
		initialEulerAngles.Clear();
		targetEulerAngles.Clear();
		foreach (Transform item in base.transform)
		{
			children.Add(item);
			initialPositions.Add(item.localPosition);
			initialEulerAngles.Add(item.localEulerAngles);
			Vector3 randomVector3InRange = Helper.GetRandomVector3InRange(offsetMin, offsetMax);
			targetPositions.Add(item.localPosition + randomVector3InRange);
			Vector3 randomVector3InRange2 = Helper.GetRandomVector3InRange(rotationOffsetMin, rotationOffsetMax);
			targetEulerAngles.Add(item.localEulerAngles + randomVector3InRange2);
		}
		cameraShake.DoShake(this);
		jostleRoutine = this.StartTimerRoutine(0f, jostleDuration, delegate(float time)
		{
			time = jostleCurve.Evaluate(time);
			int count = children.Count;
			for (int j = 0; j < count; j++)
			{
				Transform obj = children[j];
				obj.localPosition = Vector3.LerpUnclamped(initialPositions[j], targetPositions[j], time);
				obj.localEulerAngles = Vector3.LerpUnclamped(initialEulerAngles[j], targetEulerAngles[j], time);
			}
		}, null, delegate
		{
			jostleRoutine = null;
		});
	}

	[ContextMenu("Do Jostle", true)]
	private bool CanTestJostle()
	{
		return Application.isPlaying;
	}
}
