using TeamCherry.SharedUtils;
using UnityEngine;

public class ScaleActivation : MonoBehaviour
{
	[SerializeField]
	private Vector3 fromScale = Vector3.zero;

	[SerializeField]
	private Vector3 toScale = Vector3.one;

	[SerializeField]
	private MinMaxFloat startDelay;

	[SerializeField]
	private MinMaxFloat endDelay;

	[SerializeField]
	private float scaleUpDuration;

	[SerializeField]
	private float scaleDownDuration;

	private Vector3 initialScale;

	private void Awake()
	{
		initialScale = base.transform.localScale;
	}

	public void Activate()
	{
		base.gameObject.SetActive(value: true);
		base.transform.localScale = initialScale.MultiplyElements(fromScale);
		base.transform.ScaleTo(this, initialScale.MultiplyElements(toScale), scaleUpDuration, startDelay.GetRandomValue());
	}

	public void Deactivate()
	{
		base.transform.ScaleTo(this, initialScale.MultiplyElements(fromScale), scaleDownDuration, endDelay.GetRandomValue(), dontTrack: false, isRealtime: false, delegate
		{
			base.gameObject.SetActive(value: false);
		});
	}
}
