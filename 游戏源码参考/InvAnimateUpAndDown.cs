using UnityEngine;

public class InvAnimateUpAndDown : MonoBehaviour
{
	public string upAnimation;

	public string downAnimation;

	public float upDelay;

	public int randomStartFrameSpriteMax;

	private tk2dSpriteAnimator spriteAnimator;

	private MeshRenderer meshRenderer;

	private float timer;

	private bool animatingDown;

	private bool readyingAnimUp;

	public bool IsLastAnimatedDown { get; private set; }

	private void Awake()
	{
		spriteAnimator = GetComponent<tk2dSpriteAnimator>();
		meshRenderer = GetComponent<MeshRenderer>();
	}

	private void Update()
	{
		if (animatingDown && !spriteAnimator.Playing)
		{
			meshRenderer.enabled = false;
			animatingDown = false;
		}
		if (timer > 0f)
		{
			timer -= Time.unscaledDeltaTime;
		}
		if (readyingAnimUp && timer <= 0f)
		{
			animatingDown = false;
			meshRenderer.enabled = true;
			if (randomStartFrameSpriteMax > 0)
			{
				int frame = Random.Range(0, randomStartFrameSpriteMax);
				spriteAnimator.PlayFromFrame(upAnimation, frame);
			}
			else
			{
				spriteAnimator.Play(upAnimation);
			}
			readyingAnimUp = false;
		}
	}

	public void Show()
	{
		if ((bool)meshRenderer)
		{
			meshRenderer.enabled = true;
		}
		IsLastAnimatedDown = false;
		if (!(spriteAnimator == null))
		{
			tk2dSpriteAnimationClip clipByName = spriteAnimator.GetClipByName(upAnimation);
			if (clipByName != null)
			{
				spriteAnimator.PlayFromFrame(clipByName, clipByName.frames.Length - 1);
			}
		}
	}

	public void AnimateUp()
	{
		readyingAnimUp = true;
		timer = upDelay;
		IsLastAnimatedDown = false;
	}

	public void Hide()
	{
		if ((bool)meshRenderer)
		{
			meshRenderer.enabled = false;
		}
		IsLastAnimatedDown = true;
	}

	public void AnimateDown()
	{
		spriteAnimator.Play(downAnimation);
		animatingDown = true;
		IsLastAnimatedDown = true;
	}

	public void ReplayUpAnim()
	{
		meshRenderer.enabled = true;
		spriteAnimator.PlayFromFrame(0);
	}
}
