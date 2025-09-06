using UnityEngine;

public class ReduceParticleEffects : MonoBehaviour
{
	private GameManager gm;

	private ParticleSystem emitter;

	private float emissionRateHigh;

	private float emissionRateLow;

	private int maxParticlesHigh;

	private int maxParticlesLow;

	private bool init;

	private void Start()
	{
		gm = GameManager.instance;
		gm.RefreshParticleLevel += SetEmission;
		emitter = GetComponent<ParticleSystem>();
		emissionRateHigh = ((emitter != null) ? emitter.emission.rateOverTimeMultiplier : 1f);
		emissionRateLow = emissionRateHigh / 2f;
		maxParticlesHigh = ((emitter != null) ? emitter.main.maxParticles : 20);
		maxParticlesLow = maxParticlesHigh / 2;
		SetEmission();
		init = true;
	}

	private void SetEmission()
	{
		if (emitter != null)
		{
			if (gm.gameSettings.particleEffectsLevel == 0)
			{
				ParticleSystem.EmissionModule emission = emitter.emission;
				emission.rateOverTimeMultiplier = emissionRateLow;
				ParticleSystem.MainModule main = emitter.main;
				main.maxParticles = maxParticlesLow;
			}
			else
			{
				ParticleSystem.EmissionModule emission2 = emitter.emission;
				emission2.rateOverTimeMultiplier = emissionRateHigh;
				ParticleSystem.MainModule main2 = emitter.main;
				main2.maxParticles = maxParticlesHigh;
			}
		}
	}

	private void OnEnable()
	{
		if (init)
		{
			gm.RefreshParticleLevel += SetEmission;
		}
	}

	private void OnDisable()
	{
		if (init)
		{
			gm.RefreshParticleLevel -= SetEmission;
		}
	}
}
