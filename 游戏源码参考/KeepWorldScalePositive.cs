using UnityEngine;

public class KeepWorldScalePositive : MonoBehaviour, IUpdateBatchableUpdate
{
	[SerializeField]
	private bool x = true;

	[SerializeField]
	private bool y = true;

	[SerializeField]
	private bool everyFrame = true;

	private UpdateBatcher updateBatcher;

	public bool ShouldUpdate => everyFrame;

	private void OnEnable()
	{
		if (everyFrame)
		{
			updateBatcher = GameManager.instance.GetComponent<UpdateBatcher>();
			updateBatcher.Add(this);
		}
		else
		{
			UpdateScale();
		}
	}

	private void OnDisable()
	{
		if (updateBatcher != null)
		{
			updateBatcher.Remove(this);
			updateBatcher = null;
		}
	}

	public void BatchedUpdate()
	{
		UpdateScale();
	}

	private void UpdateScale()
	{
		Transform obj = base.transform;
		Vector3 localScale = obj.localScale;
		if (x && base.transform.lossyScale.x < 0f)
		{
			localScale.x = 0f - localScale.x;
		}
		if (y && base.transform.lossyScale.y < 0f)
		{
			localScale.y = 0f - localScale.y;
		}
		obj.localScale = localScale;
	}
}
