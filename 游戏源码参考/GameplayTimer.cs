using GlobalEnums;
using UnityEngine;

public class GameplayTimer : MonoBehaviour
{
	[SerializeField]
	private Animator[] pauseAnimators;

	[SerializeField]
	private ParticleSystem[] pauseParticleSystems;

	private float timeLeft;

	private bool wasPaused;

	private GameManager gm;

	public bool IsTimerComplete => timeLeft <= 0f;

	private void OnEnable()
	{
		gm = GameManager.instance;
	}

	private void Update()
	{
		if (timeLeft <= 0f)
		{
			return;
		}
		if (gm.GameState != GameState.PLAYING)
		{
			if (!wasPaused)
			{
				SetPaused(paused: true);
			}
			return;
		}
		if (wasPaused)
		{
			SetPaused(paused: false);
		}
		timeLeft -= Time.deltaTime;
	}

	private void SetPaused(bool paused)
	{
		Animator[] array = pauseAnimators;
		foreach (Animator animator in array)
		{
			if ((bool)animator)
			{
				animator.speed = ((!paused) ? 1 : 0);
			}
		}
		ParticleSystem[] array2 = pauseParticleSystems;
		foreach (ParticleSystem particleSystem in array2)
		{
			if (paused && particleSystem.isPlaying)
			{
				particleSystem.Pause(withChildren: true);
			}
			else if (particleSystem.isPaused)
			{
				particleSystem.Play(withChildren: true);
			}
		}
		wasPaused = paused;
	}

	public void StartTimer(float time)
	{
		timeLeft = time;
	}
}
