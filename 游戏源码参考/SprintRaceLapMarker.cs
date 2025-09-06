using UnityEngine;
using UnityEngine.Events;

public class SprintRaceLapMarker : MonoBehaviour
{
	public delegate bool HeroEnteredCorrectDelegate(bool canDisqualify);

	[SerializeField]
	private TriggerEnterEvent trigger;

	[SerializeField]
	private bool isEndMarker;

	[SerializeField]
	private HazardRespawnMarker hazardRespawnMarker;

	[Space]
	public UnityEvent OnHeroEnteredIncorrect;

	public UnityEvent OnHeroEnteredCorrect;

	public UnityEvent OnHeroEnteredAny;

	private SprintRaceController controller;

	private HeroEnteredCorrectDelegate heroEnteredCorrectFunc;

	private void OnEnable()
	{
		if ((bool)trigger)
		{
			trigger.OnTriggerEntered += OnTriggerEntered;
		}
	}

	private void OnDisable()
	{
		if ((bool)trigger)
		{
			trigger.OnTriggerEntered -= OnTriggerEntered;
		}
	}

	private void OnTriggerEntered(Collider2D col, GameObject sender)
	{
		if (!controller)
		{
			return;
		}
		HeroController component = col.GetComponent<HeroController>();
		SplineRunner component2 = col.GetComponent<SplineRunner>();
		if ((!component && !component2) || !controller.IsTracking || (isEndMarker && (bool)component && !controller.IsNextLapMarkerEnd))
		{
			return;
		}
		OnHeroEnteredAny.Invoke();
		if (!component)
		{
			return;
		}
		if (heroEnteredCorrectFunc != null && heroEnteredCorrectFunc(!isEndMarker))
		{
			OnHeroEnteredCorrect.Invoke();
			if ((bool)hazardRespawnMarker)
			{
				PlayerData.instance.SetHazardRespawn(hazardRespawnMarker);
			}
		}
		else
		{
			OnHeroEnteredIncorrect.Invoke();
		}
	}

	public void RegisterController(SprintRaceController newController, HeroEnteredCorrectDelegate newHeroEnteredCorrectFunc)
	{
		controller = newController;
		heroEnteredCorrectFunc = newHeroEnteredCorrectFunc;
	}
}
