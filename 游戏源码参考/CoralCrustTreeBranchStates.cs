using UnityEngine;

public sealed class CoralCrustTreeBranchStates : AnimatorActivatingStates
{
	[SerializeField]
	private JitterSelfForTime jitterSelf;

	[SerializeField]
	private ParticleSystem breakAnticParticle;

	[SerializeField]
	private GameObject breakAnticAudioGameObject;

	[SerializeField]
	private PlayParticleEffects breakParticle;

	[SerializeField]
	private PlayRandomAudioEvent breakAudioEventPlayer;

	[Space]
	[SerializeField]
	private ParticleSystemPool growParticle;

	[SerializeField]
	private PlayRandomAudioEvent growAudioEventPlayer;

	private static int lastPlayFrame;

	private static int playCount;

	private void OnGrowAnimationEvent()
	{
		growParticle.PlayParticles();
		jitterSelf.StartTimedJitter();
		growAudioEventPlayer.Play();
	}

	protected override void OnDeactivateWarning()
	{
		base.OnDeactivateWarning();
		jitterSelf.StartJitter();
		breakAnticParticle.Play(withChildren: true);
		breakAnticAudioGameObject.SetActive(value: true);
	}

	protected override void OnDeactivate()
	{
		if (Time.frameCount != lastPlayFrame)
		{
			lastPlayFrame = Time.frameCount;
			playCount = 0;
		}
		base.OnDeactivate();
		jitterSelf.StopJitter();
		breakParticle.PlayParticleSystems();
		breakAnticParticle.Stop(withChildren: true);
		breakAnticAudioGameObject.SetActive(value: false);
		if (playCount < 5 && CameraInfoCache.IsWithinBounds(base.transform.position, Vector2.one))
		{
			breakAudioEventPlayer.Play();
			playCount++;
		}
	}
}
