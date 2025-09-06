using System;
using JetBrains.Annotations;
using TeamCherry.SharedUtils;
using UnityEngine;
using UnityEngine.Serialization;

public class NoiseResponder : MonoBehaviour
{
	[SerializeField]
	[FormerlySerializedAs("IgnoreNeedolin")]
	private bool ignoreNeedolin;

	[SerializeField]
	private bool ignoreRange;

	[SerializeField]
	private NoiseMaker.Intensities minIntensity;

	[SerializeField]
	private MinMaxFloat respondDelay;

	[SerializeField]
	private float cooldown;

	[Space]
	[SerializeField]
	private PlayMakerFSM eventTarget;

	[SerializeField]
	[ModifiableProperty]
	[InspectorValidation("ValidateFsmEvent")]
	private string noisePreDelayFsmEvent;

	[SerializeField]
	[ModifiableProperty]
	[InspectorValidation("ValidateFsmEvent")]
	private string noiseStartedFsmEvent;

	[SerializeField]
	[ModifiableProperty]
	[InspectorValidation("ValidateFsmEvent")]
	private string noiseEndedFsmEvent;

	private bool wasNoiseMade;

	private double noiseRespondTime;

	private bool wasNoiseResponded;

	private double nextRespondTime;

	public event Action NoiseStarted;

	public event Action NoiseEnded;

	[UsedImplicitly]
	private bool? ValidateFsmEvent(string fsmEvent)
	{
		return eventTarget.IsEventValid(fsmEvent, isRequired: false);
	}

	private void OnEnable()
	{
		NoiseMaker.NoiseCreated += OnNoiseCreated;
	}

	private void OnDisable()
	{
		NoiseMaker.NoiseCreated -= OnNoiseCreated;
	}

	private void Update()
	{
		bool flag = wasNoiseMade && Time.timeAsDouble >= noiseRespondTime;
		if (!flag && !ignoreNeedolin && HeroPerformanceRegion.GetAffectedState(base.transform, ignoreRange) != 0)
		{
			flag = true;
		}
		if (flag && !wasNoiseResponded)
		{
			wasNoiseMade = false;
			wasNoiseResponded = true;
			OnNoiseStarted();
		}
		if (!flag && wasNoiseResponded)
		{
			wasNoiseResponded = false;
			OnNoiseEnded();
		}
	}

	private void OnNoiseCreated(Vector2 _, NoiseMaker.NoiseEventCheck isNoiseInRange, NoiseMaker.Intensities intensity, bool allowOffScreen)
	{
		if (intensity >= minIntensity && isNoiseInRange(base.transform.position) && !(Time.timeAsDouble < nextRespondTime))
		{
			nextRespondTime = Time.timeAsDouble + (double)cooldown;
			wasNoiseMade = true;
			noiseRespondTime = Time.timeAsDouble + (double)respondDelay.GetRandomValue();
			if ((bool)eventTarget && !string.IsNullOrEmpty(noisePreDelayFsmEvent))
			{
				eventTarget.SendEvent(noisePreDelayFsmEvent);
			}
		}
	}

	private void OnNoiseStarted()
	{
		if (this.NoiseStarted != null)
		{
			this.NoiseStarted();
		}
		if ((bool)eventTarget && !string.IsNullOrEmpty(noiseStartedFsmEvent))
		{
			eventTarget.SendEvent(noiseStartedFsmEvent);
		}
	}

	private void OnNoiseEnded()
	{
		if (this.NoiseEnded != null)
		{
			this.NoiseEnded();
		}
		if ((bool)eventTarget && !string.IsNullOrEmpty(noiseEndedFsmEvent))
		{
			eventTarget.SendEvent(noiseEndedFsmEvent);
		}
	}
}
