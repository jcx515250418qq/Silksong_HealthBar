using UnityEngine;

public class CopyPosition : MonoBehaviour
{
	[SerializeField]
	private Transform copyTarget;

	[SerializeField]
	private bool useWorldSpace;

	[SerializeField]
	private bool everyFrame;

	private bool hasCopyTarget;

	private void Start()
	{
		hasCopyTarget = copyTarget;
		DoCopyPosition();
		if (!everyFrame)
		{
			base.enabled = false;
		}
	}

	private void Update()
	{
		DoCopyPosition();
	}

	private void DoCopyPosition()
	{
		if (hasCopyTarget)
		{
			if (useWorldSpace)
			{
				base.transform.position = copyTarget.position;
			}
			else
			{
				base.transform.localPosition = copyTarget.localPosition;
			}
		}
	}
}
