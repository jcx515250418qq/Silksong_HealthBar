using UnityEngine;

[RequireComponent(typeof(tk2dSpriteAnimator))]
public class PlayFromRandomFrame : MonoBehaviour
{
	[SerializeField]
	[ModifiableProperty]
	[Conditional("getFromCurrentClip", false, false, false)]
	private int frameCount;

	[SerializeField]
	private bool getFromCurrentClip;

	[SerializeField]
	private bool onEnable;

	private tk2dSpriteAnimator animator;

	private void Start()
	{
		DoRandomFrame();
	}

	private void OnEnable()
	{
		if (onEnable)
		{
			DoRandomFrame();
		}
	}

	private void DoRandomFrame()
	{
		animator = GetComponent<tk2dSpriteAnimator>();
		if (!animator)
		{
			return;
		}
		if (getFromCurrentClip)
		{
			tk2dSpriteAnimationClip tk2dSpriteAnimationClip2 = animator.CurrentClip ?? animator.DefaultClip;
			if (tk2dSpriteAnimationClip2 == null)
			{
				Debug.LogError("Clip is null", this);
				return;
			}
			frameCount = tk2dSpriteAnimationClip2.frames.Length;
		}
		int frame = Random.Range(0, frameCount);
		animator.PlayFromFrame(frame);
	}
}
