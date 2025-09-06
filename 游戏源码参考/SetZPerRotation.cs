using TeamCherry.SharedUtils;
using UnityEngine;

[ExecuteInEditMode]
public class SetZPerRotation : MonoBehaviour
{
	[SerializeField]
	private Transform rotationSource;

	[SerializeField]
	private Transform zTarget;

	[SerializeField]
	private float defaultZ;

	[SerializeField]
	private float altZ;

	[SerializeField]
	private MinMaxFloat altZRange;

	private void Reset()
	{
		rotationSource = base.transform;
		zTarget = base.transform;
	}

	private void OnValidate()
	{
		if (altZRange.Start < 0f)
		{
			altZRange.Start = 0f;
		}
		if (altZRange.End > 360f)
		{
			altZRange.End = 360f;
		}
		if (altZRange.Start > altZRange.End)
		{
			altZRange.Start = altZRange.End;
		}
	}

	private void OnEnable()
	{
		if (!rotationSource || !zTarget)
		{
			base.enabled = false;
		}
	}

	private void LateUpdate()
	{
		float num;
		for (num = rotationSource.localEulerAngles.y; num < 0f; num += 360f)
		{
		}
		float num2 = (altZRange.IsInRange(num) ? altZ : defaultZ);
		Vector3 localPosition = zTarget.localPosition;
		if (localPosition.z != num2)
		{
			localPosition.z = num2;
			zTarget.localPosition = localPosition;
		}
	}
}
