using HutongGames.PlayMaker;

public class tk2dGetClipDuration : FSMUtility.GetComponentFsmStateAction<tk2dSpriteAnimator>
{
	[RequiredField]
	public FsmString ClipName;

	[UIHint(UIHint.Variable)]
	public FsmFloat StoreDuration;

	public override void Reset()
	{
		base.Reset();
		ClipName = null;
		StoreDuration = null;
	}

	protected override void DoAction(tk2dSpriteAnimator component)
	{
		tk2dSpriteAnimationClip clipByName = component.GetClipByName(ClipName.Value);
		if (clipByName != null)
		{
			StoreDuration.Value = clipByName.Duration;
		}
	}
}
