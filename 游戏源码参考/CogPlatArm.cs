using UnityEngine;

public class CogPlatArm : MonoBehaviour
{
	[Header("Structure")]
	[SerializeField]
	private CogPlat[] platforms;

	[Header("Parameters")]
	[SerializeField]
	private float rotationOffset = 90f;

	[SerializeField]
	private AnimationCurve rotationCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);

	private float initialRotation;

	private float targetRotation;

	public void StartRotation()
	{
		if (base.isActiveAndEnabled)
		{
			initialRotation = base.transform.GetLocalRotation2D();
			targetRotation = initialRotation + rotationOffset;
			CogPlat[] array = platforms;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].StartRotation();
			}
		}
	}

	public void UpdateRotation(float time)
	{
		if (base.isActiveAndEnabled)
		{
			float rotation = Mathf.LerpUnclamped(initialRotation, targetRotation, rotationCurve.Evaluate(time));
			base.transform.SetLocalRotation2D(rotation);
			CogPlat[] array = platforms;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].UpdateRotation(time);
			}
		}
	}

	public void EndRotation()
	{
		if (base.isActiveAndEnabled)
		{
			base.transform.SetLocalRotation2D(targetRotation);
			CogPlat[] array = platforms;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].EndRotation();
			}
		}
	}
}
