using TeamCherry.NestedFadeGroup;
using UnityEngine;

public class FrostEffectFader : MonoBehaviour
{
	[SerializeField]
	private NestedFadeGroupBase fadeGroup;

	[SerializeField]
	private AnimationCurve fadeCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);

	[SerializeField]
	private PlayParticleEffects particles;

	[SerializeField]
	[Range(0f, 1f)]
	private float particleThreshold;

	[SerializeField]
	private tk2dSprite tk2dSprite;

	[SerializeField]
	private Gradient spriteGradient;

	private bool isAboveThreshold;

	private float lastFrostAmount = -100f;

	private void OnEnable()
	{
		HeroController silentInstance = HeroController.SilentInstance;
		if ((bool)silentInstance)
		{
			silentInstance.FrostAmountUpdated += OnFrostAmountUpdated;
		}
	}

	private void OnDisable()
	{
		HeroController silentInstance = HeroController.SilentInstance;
		if ((bool)silentInstance)
		{
			silentInstance.FrostAmountUpdated -= OnFrostAmountUpdated;
		}
	}

	private void OnFrostAmountUpdated(float frostAmount)
	{
		if (lastFrostAmount == frostAmount)
		{
			return;
		}
		lastFrostAmount = frostAmount;
		if ((bool)fadeGroup)
		{
			fadeGroup.AlphaSelf = fadeCurve.Evaluate(frostAmount);
		}
		if ((bool)tk2dSprite)
		{
			tk2dSprite.color = spriteGradient.Evaluate(frostAmount);
		}
		bool flag = isAboveThreshold;
		isAboveThreshold = frostAmount >= particleThreshold;
		if (!particles)
		{
			return;
		}
		if (isAboveThreshold)
		{
			if (!flag)
			{
				particles.PlayParticleSystems();
			}
		}
		else if (flag)
		{
			particles.StopParticleSystems();
		}
	}
}
