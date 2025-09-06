using UnityEngine;

public class CaravanMusicControl : MonoBehaviour
{
	[SerializeField]
	private TrackTriggerObjects enterRange;

	[SerializeField]
	private TrackTriggerObjects exitRange;

	private HeroController hc;

	private bool wasInside;

	private static int _insideCount;

	private void Awake()
	{
		enterRange.InsideStateChanged += OnEnterRangeInsideStateChanged;
		exitRange.InsideStateChanged += OnExitRangeInsideStateChanged;
	}

	private void Start()
	{
		hc = HeroController.instance;
		if (hc.isHeroInPosition && enterRange.IsInside)
		{
			SetInside(value: true);
		}
	}

	private void OnDestroy()
	{
		if (wasInside)
		{
			SetInside(value: false);
		}
	}

	private void OnEnterRangeInsideStateChanged(bool isInside)
	{
		if ((bool)hc && hc.isHeroInPosition && isInside)
		{
			SetInside(value: true);
		}
	}

	private void OnExitRangeInsideStateChanged(bool isInside)
	{
		if ((bool)hc && hc.isHeroInPosition && !isInside)
		{
			SetInside(value: false);
		}
	}

	private void SetInside(bool value)
	{
		if (value == wasInside)
		{
			return;
		}
		if (value)
		{
			_insideCount++;
			if (_insideCount == 1)
			{
				EventRegister.SendEvent(EventRegisterEvents.FleaMusicStart);
			}
		}
		else
		{
			_insideCount--;
			if (_insideCount <= 0)
			{
				EventRegister.SendEvent(EventRegisterEvents.FleaMusicStop);
			}
		}
		wasInside = value;
	}
}
