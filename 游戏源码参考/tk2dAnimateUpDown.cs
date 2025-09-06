using UnityEngine;

public class tk2dAnimateUpDown : MonoBehaviour
{
	[SerializeField]
	private tk2dSpriteAnimator animator;

	[Space]
	[SerializeField]
	private string upAnim;

	[SerializeField]
	private string downAnim;

	[SerializeField]
	private bool startUp;

	private MeshRenderer renderer;

	private bool hasStarted;

	private bool isAnimatingDown;

	private void Start()
	{
		if (!hasStarted)
		{
			hasStarted = true;
			if ((bool)animator)
			{
				renderer = animator.GetComponent<MeshRenderer>();
			}
			if (startUp)
			{
				isAnimatingDown = false;
				renderer.enabled = true;
				tk2dSpriteAnimationClip clipByName = animator.GetClipByName(upAnim);
				animator.PlayFromFrame(clipByName, clipByName.frames.Length - 1);
			}
			else
			{
				isAnimatingDown = true;
				renderer.enabled = false;
				tk2dSpriteAnimationClip clipByName2 = animator.GetClipByName(downAnim);
				animator.PlayFromFrame(clipByName2, clipByName2.frames.Length - 1);
			}
		}
	}

	[ContextMenu("Animate Up", true)]
	[ContextMenu("Animate Down", true)]
	private bool CanAnimate()
	{
		return Application.isPlaying;
	}

	[ContextMenu("Animate Up")]
	public void AnimateUp()
	{
		if (isAnimatingDown)
		{
			isAnimatingDown = false;
			PlayAnimation(upAnim);
		}
	}

	[ContextMenu("Animate Down")]
	public void AnimateDown()
	{
		if (!isAnimatingDown)
		{
			isAnimatingDown = true;
			PlayAnimation(downAnim);
		}
	}

	private void PlayAnimation(string animName)
	{
		Start();
		if ((bool)animator)
		{
			renderer.enabled = true;
			animator.Play(animName);
			animator.AnimationCompleted = OnAnimationComplete;
		}
	}

	private void OnAnimationComplete(tk2dSpriteAnimator animator, tk2dSpriteAnimationClip clip)
	{
		if (isAnimatingDown)
		{
			renderer.enabled = false;
		}
	}
}
