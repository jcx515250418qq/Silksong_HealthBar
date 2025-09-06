using TeamCherry.SharedUtils;
using UnityEngine;

public class ParticleEffectsLerpEmission : MonoBehaviour
{
	[SerializeField]
	private ParticleSystem[] particles;

	[Space]
	[SerializeField]
	private AnimationCurve emissionMultiplierCurve;

	[SerializeField]
	private MinMaxFloat emissionMultiplier;

	[SerializeField]
	private bool recycleOnEnd = true;

	private float[] initialMultipliers;

	private bool hasStarted;

	private float emissionDuration;

	private float elapsedDuration;

	public float TotalMultiplier { get; set; }

	private void Awake()
	{
		TotalMultiplier = 1f;
	}

	private void OnEnable()
	{
		if (initialMultipliers == null || initialMultipliers.Length != particles.Length)
		{
			initialMultipliers = new float[particles.Length];
		}
		for (int i = 0; i < initialMultipliers.Length; i++)
		{
			ParticleSystem.EmissionModule emission = particles[i].emission;
			initialMultipliers[i] = emission.rateOverTimeMultiplier;
		}
		SetEmissionMultiplier(1f);
		hasStarted = false;
	}

	private void OnDisable()
	{
		for (int i = 0; i < initialMultipliers.Length; i++)
		{
			ParticleSystem.EmissionModule emission = particles[i].emission;
			emission.rateOverTimeMultiplier = initialMultipliers[i];
		}
	}

	private void Update()
	{
		if (!hasStarted)
		{
			return;
		}
		if (ShouldRecycle())
		{
			base.gameObject.Recycle();
		}
		else if (elapsedDuration < emissionDuration)
		{
			elapsedDuration += Time.deltaTime;
			float value = elapsedDuration / emissionDuration;
			value = Mathf.Clamp01(value);
			SetEmissionMultiplier(value);
			if (value >= 1f)
			{
				Stop();
			}
		}
	}

	private bool ShouldRecycle()
	{
		if (!recycleOnEnd)
		{
			return false;
		}
		ParticleSystem[] array = particles;
		for (int i = 0; i < array.Length; i++)
		{
			if (array[i].IsAlive(withChildren: true))
			{
				return false;
			}
		}
		return true;
	}

	public void Play(float duration)
	{
		ParticleSystem[] array = particles;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].Play(withChildren: true);
		}
		SetEmissionMultiplier(0f);
		hasStarted = true;
		emissionDuration = duration;
		elapsedDuration = 0f;
	}

	public void Stop()
	{
		ParticleSystem[] array = particles;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].Stop(withChildren: true);
		}
	}

	public void SetEmissionMultiplier(float t)
	{
		t = emissionMultiplierCurve.Evaluate(t);
		for (int i = 0; i < particles.Length; i++)
		{
			ParticleSystem.EmissionModule emission = particles[i].emission;
			emission.rateOverTimeMultiplier = initialMultipliers[i] * emissionMultiplier.GetLerpedValue(t) * TotalMultiplier;
		}
	}
}
