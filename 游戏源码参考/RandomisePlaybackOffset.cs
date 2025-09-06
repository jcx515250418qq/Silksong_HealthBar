using UnityEngine;

public class RandomisePlaybackOffset : StateMachineBehaviour
{
	[Range(0f, 1f)]
	[SerializeField]
	private float minOffset;

	[Range(0f, 1f)]
	[SerializeField]
	private float maxOffset = 1f;

	[SerializeField]
	private bool randomizeEveryEntry = true;

	private float lastOffset;

	private int lastFrame = -1;

	public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		if (lastFrame < Time.frameCount)
		{
			if (minOffset > maxOffset)
			{
				minOffset = maxOffset;
			}
			if (randomizeEveryEntry || stateInfo.normalizedTime == 0f)
			{
				lastOffset = Random.Range(minOffset, maxOffset);
			}
			lastFrame = Time.frameCount + 5;
			animator.Play(stateInfo.fullPathHash, layerIndex, lastOffset);
		}
	}
}
