using UnityEngine;

public class DeactivateOnTK2DTargetAnimComplete : MonoBehaviour
{
	[SerializeField]
	private tk2dSpriteAnimator animator;

	[SerializeField]
	private GameObject disableGameObject;

	[SerializeField]
	private string targetClipName;

	private bool startedPlayingTargetClip;

	private void OnEnable()
	{
		startedPlayingTargetClip = false;
	}

	private void Update()
	{
		if (!startedPlayingTargetClip)
		{
			if (animator.IsPlaying(targetClipName))
			{
				startedPlayingTargetClip = true;
			}
		}
		else if (!animator.IsPlaying(targetClipName))
		{
			disableGameObject.SetActive(value: false);
		}
	}
}
