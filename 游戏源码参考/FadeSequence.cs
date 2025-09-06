using UnityEngine;

public class FadeSequence : SkippableSequence
{
	[SerializeField]
	private SkippableSequence childSequence;

	private float fade;

	private float fadeByController;

	[SerializeField]
	private float fadeDelay;

	private float timer;

	[SerializeField]
	private float fadeRate;

	public override bool IsSkipped => childSequence.IsSkipped;

	public float FadeRate => fadeRate;

	public override float FadeByController
	{
		get
		{
			return fadeByController;
		}
		set
		{
			fadeByController = value;
			UpdateFade();
		}
	}

	public override bool IsPlaying
	{
		get
		{
			if (!childSequence.IsPlaying)
			{
				return fade > Mathf.Epsilon;
			}
			return true;
		}
	}

	protected void Awake()
	{
		fade = 0f;
		timer = 0f;
		fadeByController = 1f;
	}

	public override void Begin()
	{
		fade = 0f;
		timer = 0f;
		UpdateFade();
		childSequence.Begin();
	}

	protected void Update()
	{
		if (childSequence.IsPlaying)
		{
			timer += Time.deltaTime;
		}
		fade = Mathf.MoveTowards(target: (!(timer >= fadeDelay) || !childSequence.IsPlaying) ? 0f : 1f, current: fade, maxDelta: fadeRate * Time.unscaledDeltaTime);
		UpdateFade();
	}

	public override void Skip()
	{
		childSequence.Skip();
	}

	private void UpdateFade()
	{
		childSequence.FadeByController = Mathf.Clamp01(fade * fadeByController);
	}
}
