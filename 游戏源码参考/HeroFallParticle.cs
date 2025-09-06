using UnityEngine;

public sealed class HeroFallParticle : MonoBehaviour
{
	[SerializeField]
	private Animator animator;

	[SerializeField]
	private AnimatorHashCache offAnimation = new AnimatorHashCache("Off");

	[SerializeField]
	private AnimatorHashCache onAnimation = new AnimatorHashCache("On");

	private HeroController hc;

	private bool hasHC;

	private float threshold;

	private bool isOn;

	private void Start()
	{
		hc = HeroController.instance;
		hasHC = hc != null;
		base.enabled = hasHC;
	}

	private void LateUpdate()
	{
		bool flag = hc.current_velocity.y < hc.MAX_FALL_VELOCITY * 0.75f;
		if (flag != isOn)
		{
			isOn = flag;
			if (isOn)
			{
				animator.Play(onAnimation);
			}
			else
			{
				animator.Play(offAnimation);
			}
		}
	}
}
