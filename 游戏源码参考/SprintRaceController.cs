using System;
using TeamCherry.Splines;
using UnityEngine;

public class SprintRaceController : MonoBehaviour
{
	public delegate void RaceCompletedDelegate(bool didHeroWin);

	[SerializeField]
	private HermiteSplinePath path;

	[SerializeField]
	private SprintRaceLapMarker[] lapMarkers;

	[SerializeField]
	private SimpleCounter counter;

	[Space]
	[SerializeField]
	private string raceEndEvent;

	[SerializeField]
	private string raceEndCompleteEvent;

	[SerializeField]
	private int lapCount;

	[SerializeField]
	private float[] lapBaseSpeeds;

	[SerializeField]
	private float dashDownDuration;

	[Space]
	[SerializeField]
	private SavedItem reward;

	private int runnerLapsCompleted;

	private int heroLapsCompleted;

	private int nextLapMarkerIndex;

	private bool isCompleted;

	public float DashDownDuration => dashDownDuration;

	public bool IsTracking { get; private set; }

	public bool IsNextLapMarkerEnd => nextLapMarkerIndex == lapMarkers.Length - 1;

	public float TotalPathDistance
	{
		get
		{
			if (!path)
			{
				return 0f;
			}
			return path.TotalDistance;
		}
	}

	public int LapCount => lapCount;

	public SavedItem Reward => reward;

	public event RaceCompletedDelegate RaceCompleted;

	public event Action RaceDisqualified;

	private void OnValidate()
	{
		if (lapCount < 1)
		{
			lapCount = 1;
		}
		SyncLapCount();
	}

	private void SyncLapCount()
	{
		if (lapBaseSpeeds != null && lapBaseSpeeds.Length == lapCount)
		{
			return;
		}
		float[] array = lapBaseSpeeds;
		lapBaseSpeeds = new float[lapCount];
		if (array == null)
		{
			return;
		}
		for (int i = 0; i < lapBaseSpeeds.Length; i++)
		{
			int num = i;
			if (num >= array.Length)
			{
				num = array.Length - 1;
				if (num < 0)
				{
					continue;
				}
			}
			lapBaseSpeeds[i] = array[num];
		}
	}

	private void Awake()
	{
		OnValidate();
		if (lapMarkers == null)
		{
			return;
		}
		for (int i = 0; i < lapMarkers.Length; i++)
		{
			int capturedIndex = i;
			lapMarkers[i].RegisterController(this, delegate(bool canDisqualify)
			{
				ReportHeroLapMarkerHit(capturedIndex, canDisqualify, out var wasCorrect);
				return wasCorrect;
			});
		}
	}

	private void OnDisable()
	{
		if (IsTracking)
		{
			StopTracking();
		}
	}

	private void OnDrawGizmosSelected()
	{
		if (lapMarkers == null)
		{
			return;
		}
		for (int i = 0; i < lapMarkers.Length; i++)
		{
			SprintRaceLapMarker sprintRaceLapMarker = lapMarkers[i];
			if (!sprintRaceLapMarker)
			{
				continue;
			}
			Transform transform = sprintRaceLapMarker.transform;
			BoxCollider2D component = sprintRaceLapMarker.GetComponent<BoxCollider2D>();
			if ((bool)component)
			{
				Matrix4x4 matrix = Gizmos.matrix;
				Gizmos.matrix = transform.localToWorldMatrix;
				Gizmos.DrawWireCube(component.offset, component.size);
				Gizmos.matrix = matrix;
			}
			if (i != 0)
			{
				SprintRaceLapMarker sprintRaceLapMarker2 = lapMarkers[i - 1];
				if ((bool)sprintRaceLapMarker2)
				{
					Vector3 position = transform.position;
					Gizmos.DrawLine(sprintRaceLapMarker2.transform.position, position);
				}
			}
		}
	}

	public Vector2 GetPositionAlongSpline(float splineDistance)
	{
		if (!path)
		{
			return Vector2.zero;
		}
		return path.GetPositionAlongSpline(splineDistance);
	}

	public float GetDistanceAlongSpline(Vector2 position, bool getNext = false)
	{
		if (!path)
		{
			return 0f;
		}
		return path.GetDistanceAlongSpline(position, getNext);
	}

	public void BeginInRace()
	{
		counter.SetCap(lapCount);
		counter.SetCurrent(0);
		counter.Appear();
	}

	public void EndInRace()
	{
		counter.Disappear();
	}

	public void StartTracking()
	{
		runnerLapsCompleted = 0;
		heroLapsCompleted = 0;
		nextLapMarkerIndex = 0;
		IsTracking = true;
		isCompleted = false;
		HazardRespawnTrigger.IsSuppressed = true;
	}

	public void StopTracking()
	{
		IsTracking = false;
		HazardRespawnTrigger.IsSuppressed = false;
	}

	public void ReportRunnerLapCompleted(out bool isRaceComplete)
	{
		if (IsTracking)
		{
			runnerLapsCompleted++;
			CheckCompletion(isHero: false);
		}
		isRaceComplete = !IsTracking;
	}

	private void ReportHeroLapMarkerHit(int index, bool canDisqualify, out bool wasCorrect)
	{
		wasCorrect = false;
		if (!IsTracking || index == nextLapMarkerIndex - 1)
		{
			return;
		}
		if (index != nextLapMarkerIndex)
		{
			if (canDisqualify)
			{
				StopTracking();
				this.RaceDisqualified?.Invoke();
				if (!string.IsNullOrEmpty(raceEndEvent))
				{
					EventRegister.SendEvent(raceEndEvent);
				}
			}
			return;
		}
		nextLapMarkerIndex++;
		wasCorrect = true;
		if (nextLapMarkerIndex >= lapMarkers.Length)
		{
			heroLapsCompleted++;
			nextLapMarkerIndex = 0;
			counter.SetCurrent(heroLapsCompleted);
			CheckCompletion(isHero: true);
		}
	}

	private void CheckCompletion(bool isHero)
	{
		if (isHero)
		{
			if (heroLapsCompleted < lapCount)
			{
				return;
			}
			this.RaceCompleted?.Invoke(didHeroWin: true);
		}
		else
		{
			if (runnerLapsCompleted != lapCount)
			{
				return;
			}
			this.RaceCompleted?.Invoke(didHeroWin: false);
		}
		if (!isCompleted)
		{
			isCompleted = true;
			if (!string.IsNullOrEmpty(raceEndCompleteEvent))
			{
				EventRegister.SendEvent(raceEndCompleteEvent);
			}
			if (!string.IsNullOrEmpty(raceEndEvent))
			{
				EventRegister.SendEvent(raceEndEvent);
			}
		}
	}

	public void GetRaceInfo(out int runnerLaps, out int heroLaps, out float currentBaseSpeed)
	{
		runnerLaps = runnerLapsCompleted;
		heroLaps = heroLapsCompleted;
		currentBaseSpeed = lapBaseSpeeds[Mathf.Clamp(heroLaps, 0, lapBaseSpeeds.Length - 1)];
	}
}
