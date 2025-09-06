using System.Collections.Generic;
using JetBrains.Annotations;
using TeamCherry.SharedUtils;
using UnityEngine;

public class CrossSceneWalker : MonoBehaviour
{
	private const float MAX_WALK_TIME = 68.5f;

	private const float MIN_PAUSE_TIME = 60f;

	private const float MAX_PAUSE_TIME = 120f;

	private static readonly int _blockedScene = "Belltown_Room_Fisher".GetHashCode();

	[SerializeField]
	private PlayerDataTest activeCondition;

	[Space]
	[SerializeField]
	private MinMaxFloat timerRange;

	[SerializeField]
	[ModifiableProperty]
	[InspectorValidation]
	private SplineWalker walker;

	private bool isInRange;

	private bool wasInRange;

	private bool isPaused;

	private bool hasTicked;

	private PlayerData pd;

	private static readonly List<CrossSceneWalker> _activeWalkers = new List<CrossSceneWalker>();

	private void OnValidate()
	{
		if (timerRange.End > 68.5f)
		{
			timerRange.End = 68.5f;
		}
		else if (timerRange.End < timerRange.Start)
		{
			timerRange.End = timerRange.Start;
		}
		if (timerRange.Start > timerRange.End)
		{
			timerRange.Start = timerRange.End;
		}
	}

	private void OnEnable()
	{
		_activeWalkers.AddIfNotPresent(this);
		if (!walker)
		{
			base.enabled = false;
		}
	}

	private void OnDisable()
	{
		_activeWalkers.Remove(this);
	}

	private void Start()
	{
		pd = PlayerData.instance;
		if (!activeCondition.IsFulfilled)
		{
			base.enabled = false;
		}
	}

	private void Update()
	{
		if (!hasTicked || isPaused)
		{
			return;
		}
		float fisherWalkerTimer = pd.FisherWalkerTimer;
		bool fisherWalkerDirection = pd.FisherWalkerDirection;
		float num = (fisherWalkerTimer - timerRange.Start) / (timerRange.End - timerRange.Start);
		wasInRange = isInRange;
		isInRange = num >= 0f && num <= 1f;
		if (isInRange && !wasInRange)
		{
			if (!fisherWalkerDirection)
			{
				walker.StartWalking(num, 1f);
			}
			else
			{
				walker.StartWalking(num, -1f);
			}
		}
	}

	public void PauseWalker()
	{
		walker.StopWalking();
		isPaused = true;
	}

	public void ResumeWalker()
	{
		isPaused = false;
		walker.ResumeWalking();
	}

	public static void Tick()
	{
		if (_activeWalkers.Exists((CrossSceneWalker a) => a.isPaused) || GameManager.instance.sceneNameHash == _blockedScene)
		{
			return;
		}
		PlayerData instance = PlayerData.instance;
		foreach (CrossSceneWalker activeWalker in _activeWalkers)
		{
			activeWalker.hasTicked = true;
		}
		float num = instance.FisherWalkerTimer;
		bool flag = instance.FisherWalkerDirection;
		if ((num >= 68.5f && !flag) || (num <= 0f && flag))
		{
			if (instance.FisherWalkerIdleTimeLeft <= 0f)
			{
				instance.FisherWalkerIdleTimeLeft = Random.Range(60f, 120f);
			}
			else
			{
				instance.FisherWalkerIdleTimeLeft -= Time.deltaTime;
				if (!(instance.FisherWalkerIdleTimeLeft <= 0f))
				{
					return;
				}
				flag = (instance.FisherWalkerDirection = !flag);
				num = (flag ? 68.5f : 0f);
			}
		}
		num = ((!flag) ? (num + Time.deltaTime) : (num - Time.deltaTime));
		instance.FisherWalkerTimer = num;
	}

	[UsedImplicitly]
	public static bool IsHome()
	{
		return PlayerData.instance.FisherWalkerTimer <= 0f;
	}

	[UsedImplicitly]
	public static void ResetIdleTime()
	{
		PlayerData instance = PlayerData.instance;
		if (!(instance.FisherWalkerTimer > 0f) && !(instance.FisherWalkerTimer < 120f))
		{
			instance.FisherWalkerIdleTimeLeft = Random.Range(60f, 120f);
		}
	}
}
