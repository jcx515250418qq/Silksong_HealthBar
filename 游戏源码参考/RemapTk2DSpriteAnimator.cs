using System;
using System.Collections.Generic;
using UnityEngine;

public class RemapTk2DSpriteAnimator : MonoBehaviour
{
	[Serializable]
	private struct AnimationRemap
	{
		public string sourceClip;

		public string targetClip;
	}

	private enum MatchSyncTypes
	{
		Wrap = 0,
		Clamp = 1
	}

	[SerializeField]
	private tk2dSpriteAnimator sourceAnimator;

	[SerializeField]
	private tk2dSpriteAnimator targetAnimator;

	[SerializeField]
	private bool syncFrames = true;

	[SerializeField]
	[ModifiableProperty]
	[Conditional("syncFrames", true, false, false)]
	private MatchSyncTypes matchSyncType;

	[Space]
	[SerializeField]
	private List<AnimationRemap> animationsList = new List<AnimationRemap>();

	private readonly Dictionary<string, string> animations = new Dictionary<string, string>();

	private tk2dSprite targetSprite;

	private MeshRenderer targetRenderer;

	private bool shouldAnimate;

	private string lastSourceClip;

	private void Start()
	{
		shouldAnimate = true;
		if (!sourceAnimator)
		{
			shouldAnimate = false;
		}
		if ((bool)targetAnimator)
		{
			targetSprite = targetAnimator.GetComponent<tk2dSprite>();
			targetRenderer = targetAnimator.GetComponent<MeshRenderer>();
		}
		else
		{
			shouldAnimate = false;
		}
		if (syncFrames && targetSprite == null)
		{
			shouldAnimate = false;
		}
		foreach (AnimationRemap animations in animationsList)
		{
			this.animations[animations.sourceClip] = animations.targetClip;
		}
	}

	private void LateUpdate()
	{
		if (!shouldAnimate)
		{
			return;
		}
		string text = ((sourceAnimator.CurrentClip != null) ? sourceAnimator.CurrentClip.name : string.Empty);
		if (syncFrames)
		{
			if (targetAnimator.enabled)
			{
				targetAnimator.enabled = false;
			}
			if (!string.IsNullOrEmpty(text) && animations.ContainsKey(text))
			{
				if ((bool)targetRenderer)
				{
					targetRenderer.enabled = true;
				}
				MatchFrame(text);
			}
			else if ((bool)targetRenderer)
			{
				targetRenderer.enabled = false;
			}
			return;
		}
		if (!targetAnimator.enabled)
		{
			targetAnimator.enabled = true;
			lastSourceClip = null;
		}
		if (text != lastSourceClip)
		{
			lastSourceClip = text;
			if (!string.IsNullOrEmpty(text) && animations.ContainsKey(text))
			{
				targetAnimator.PlayFromFrame(animations[text], syncFrames ? sourceAnimator.CurrentFrame : 0);
			}
		}
	}

	private void MatchFrame(string sourceClipName)
	{
		tk2dSpriteAnimationClip clipByName = targetAnimator.GetClipByName(animations[sourceClipName]);
		if (clipByName == null)
		{
			Debug.LogError("targetAnimator does not have clip: " + sourceClipName, this);
			return;
		}
		int num = sourceAnimator.CurrentFrame;
		int num2 = clipByName.frames.Length;
		switch (matchSyncType)
		{
		case MatchSyncTypes.Wrap:
			num %= num2;
			break;
		case MatchSyncTypes.Clamp:
			if (num > num2 - 1)
			{
				num = num2 - 1;
			}
			break;
		default:
			throw new ArgumentOutOfRangeException();
		}
		tk2dSpriteAnimationFrame frame = clipByName.GetFrame(num);
		targetSprite.SetSprite(frame.spriteId);
	}
}
