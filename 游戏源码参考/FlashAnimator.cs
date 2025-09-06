using UnityEngine;

public class FlashAnimator : BaseAnimator
{
	[SerializeField]
	private Renderer renderer;

	private MaterialPropertyBlock block;

	[SerializeField]
	private float maxAmount = 1f;

	[SerializeField]
	private Color flashColour = Color.white;

	[SerializeField]
	private AnimationCurve curve = AnimationCurve.Linear(0f, 1f, 1f, 0f);

	[SerializeField]
	private float duration = 0.2f;

	private Coroutine flashRoutine;

	private void Awake()
	{
		block = new MaterialPropertyBlock();
	}

	public override void StartAnimation()
	{
		if ((bool)renderer)
		{
			if (flashRoutine != null)
			{
				StopCoroutine(flashRoutine);
				ResetFlash();
			}
			SetRendererProperty("_FlashColor", flashColour);
			flashRoutine = this.StartTimerRoutine(0f, duration, delegate(float time)
			{
				SetRendererProperty("_FlashAmount", Mathf.Lerp(0f, maxAmount, curve.Evaluate(time)));
			}, null, ResetFlash);
		}
	}

	private void ResetFlash()
	{
		SetRendererProperty("_FlashAmount", 0f);
	}

	private void SetRendererProperty(string property, float value)
	{
		renderer.GetPropertyBlock(block);
		block.SetFloat(property, value);
		renderer.SetPropertyBlock(block);
	}

	private void SetRendererProperty(string property, Color value)
	{
		renderer.GetPropertyBlock(block);
		block.SetColor(property, value);
		renderer.SetPropertyBlock(block);
	}
}
