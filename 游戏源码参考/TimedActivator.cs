using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TimedActivator : MonoBehaviour
{
	private class Activator
	{
		public ActivatingBase Activate;

		public float Distance;

		public float ActivateDelay;

		public float DeactivateDelay;

		public Animator Animator;

		public bool HasAnimatorParam;

		public float CurrentParamValue;

		public LastActivatorState lastState;
	}

	private struct Tracker
	{
		public Activator Activator;

		public float TimeLeft;
	}

	private enum LastActivatorState
	{
		NotActive = 0,
		Activate = 1,
		Warning = 2,
		Deactivate = 3
	}

	[SerializeField]
	private Transform activateObjectsParent;

	[SerializeField]
	private Transform interactiveObjectsParent;

	[SerializeField]
	private float delay;

	[SerializeField]
	private float deactivateDelay;

	[SerializeField]
	private bool deactivateOnReHit;

	[SerializeField]
	private float duration;

	[SerializeField]
	private AnimationCurve growCurve = AnimationCurve.Linear(1f, 1f, 1f, 1f);

	[SerializeField]
	private float deactivateWarningDuration;

	[SerializeField]
	private float distanceActivateDelay;

	[SerializeField]
	private float distanceDeactivateDelay;

	[SerializeField]
	private float interactiveParentDelay;

	[SerializeField]
	private bool deactivateOthers = true;

	[Space]
	public UnityEvent OnActivated;

	public UnityEvent OnActivatedDelay;

	public UnityEvent OnAlreadyActivated;

	public UnityEvent OnDeactivate;

	private ActivatingBase[] activateObjects;

	private ActivatingBase[] interactiveObjects;

	private Dictionary<Transform, Dictionary<int, List<Activator>>> activateSiblings;

	private List<Activator> allActivators;

	private readonly List<Tracker> activatingTrackers = new List<Tracker>();

	private readonly List<Tracker> warningTrackers = new List<Tracker>();

	private readonly List<Tracker> deactivatingTrackers = new List<Tracker>();

	private float durationLeft;

	private Coroutine activationRoutine;

	private static readonly int _growSpeedProp = Animator.StringToHash("Grow Speed");

	private static readonly List<TimedActivator> _timedActivators = new List<TimedActivator>();

	private bool interrupted;

	private void OnDrawGizmos()
	{
		if (!Application.isPlaying)
		{
			Awake();
			DoSetup(drawingGizmos: true);
		}
	}

	private void OnValidate()
	{
		if (duration < 0f)
		{
			duration = 0f;
		}
		if (deactivateWarningDuration > duration)
		{
			deactivateWarningDuration = duration;
		}
	}

	private void Awake()
	{
		OnValidate();
		activateObjects = (activateObjectsParent ? activateObjectsParent.GetComponentsInChildren<ActivatingBase>() : Array.Empty<ActivatingBase>());
		interactiveObjects = (interactiveObjectsParent ? interactiveObjectsParent.GetComponentsInChildren<ActivatingBase>() : Array.Empty<ActivatingBase>());
		UpdateSiblingLists();
	}

	private void OnEnable()
	{
		_timedActivators.Add(this);
	}

	private void Start()
	{
		DoSetup(drawingGizmos: false);
	}

	private void OnDisable()
	{
		_timedActivators.Remove(this);
	}

	private void DoSetup(bool drawingGizmos)
	{
		if (allActivators == null)
		{
			allActivators = new List<Activator>(activateObjects.Length);
		}
		else
		{
			allActivators.Clear();
		}
		Vector2 vector = base.transform.position;
		foreach (KeyValuePair<Transform, Dictionary<int, List<Activator>>> activateSibling in activateSiblings)
		{
			foreach (KeyValuePair<int, List<Activator>> item in activateSibling.Value)
			{
				List<Activator> value = item.Value;
				float num = 0f;
				for (int i = 0; i < value.Count; i++)
				{
					Activator activator = value[i];
					Vector2 a = activator.Activate.transform.position;
					Vector2 b;
					if (i == 0)
					{
						Activator activator2 = null;
						float num2 = Vector2.Distance(a, vector);
						if (item.Key > 0)
						{
							foreach (KeyValuePair<int, List<Activator>> item2 in activateSibling.Value)
							{
								if (item2.Key == item.Key)
								{
									continue;
								}
								foreach (Activator item3 in item2.Value)
								{
									float num3 = Vector2.Distance(a, item3.Activate.transform.position);
									if (!(num3 > num2))
									{
										activator2 = item3;
										num2 = num3;
									}
								}
							}
						}
						if (activator2 != null)
						{
							b = activator2.Activate.transform.position;
							num = activator2.Distance;
						}
						else
						{
							b = vector;
						}
					}
					else
					{
						b = value[i - 1].Activate.transform.position;
					}
					num = (activator.Distance = num + Vector2.Distance(a, b));
					activator.DeactivateDelay = interactiveParentDelay;
					AddActivator(activator);
				}
			}
		}
		ActivatingBase[] array = interactiveObjects;
		foreach (ActivatingBase activatingBase in array)
		{
			Vector2 a2 = activatingBase.transform.position;
			Activator activator3 = null;
			float num4 = float.MaxValue;
			foreach (KeyValuePair<Transform, Dictionary<int, List<Activator>>> activateSibling2 in activateSiblings)
			{
				foreach (KeyValuePair<int, List<Activator>> item4 in activateSibling2.Value)
				{
					foreach (Activator item5 in item4.Value)
					{
						float num5 = Vector2.Distance(a2, item5.Activate.transform.position);
						if (!(num5 > num4))
						{
							activator3 = item5;
							num4 = num5;
						}
					}
				}
			}
			float num6;
			if (activator3 == null)
			{
				_ = (Vector2)base.transform.position;
				num6 = 0f;
				num4 = Vector2.Distance(a2, vector);
			}
			else
			{
				_ = (Vector2)activator3.Activate.transform.position;
				num6 = activator3.Distance;
			}
			float distance = num6 + num4;
			AddActivator(new Activator
			{
				Activate = activatingBase,
				Distance = distance,
				ActivateDelay = interactiveParentDelay,
				DeactivateDelay = 0f - interactiveParentDelay
			});
		}
	}

	private void AddActivator(Activator activator)
	{
		Animator animator = (activator.Animator = activator.Activate.GetComponent<Animator>());
		activator.HasAnimatorParam = (bool)animator && animator.HasParameter(_growSpeedProp, AnimatorControllerParameterType.Float);
		if (activator.HasAnimatorParam)
		{
			activator.CurrentParamValue = animator.GetFloat(_growSpeedProp);
		}
		allActivators.Add(activator);
	}

	private void Update()
	{
		float num = growCurve.Evaluate(Time.time);
		float deltaTime = Time.deltaTime;
		for (int i = 0; i < allActivators.Count; i++)
		{
			Activator activator = allActivators[i];
			if (activator.HasAnimatorParam && Math.Abs(activator.CurrentParamValue - num) > float.Epsilon)
			{
				activator.Animator.SetFloat(_growSpeedProp, num);
				activator.CurrentParamValue = num;
			}
		}
		ProcessTrackerOnActivatingComplete(activatingTrackers, deltaTime);
		ProcessTrackerOnWarningComplete(warningTrackers, deltaTime);
		ProcessTrackerOnDeactivatingComplete(deactivatingTrackers, deltaTime);
	}

	private void ProcessTracker(List<Tracker> trackerList, float deltaTime, Action<Tracker> onTrackerComplete)
	{
		if (trackerList == null)
		{
			return;
		}
		bool flag = onTrackerComplete != null;
		int num = 0;
		for (int i = 0; i < trackerList.Count; i++)
		{
			Tracker tracker = trackerList[i];
			if (tracker.Activator.Activate.IsPaused)
			{
				trackerList[num] = tracker;
				num++;
				continue;
			}
			tracker.TimeLeft -= deltaTime;
			if (tracker.TimeLeft > 0f)
			{
				trackerList[num] = tracker;
				num++;
			}
			else if (flag)
			{
				onTrackerComplete(tracker);
			}
		}
		if (num < trackerList.Count)
		{
			trackerList.RemoveRange(num, trackerList.Count - num);
		}
	}

	private void ProcessTrackerOnActivatingComplete(List<Tracker> trackerList, float deltaTime)
	{
		if (trackerList == null)
		{
			return;
		}
		int num = 0;
		for (int i = 0; i < trackerList.Count; i++)
		{
			Tracker value = trackerList[i];
			if (value.Activator.Activate.IsPaused)
			{
				trackerList[num] = value;
				num++;
				continue;
			}
			value.TimeLeft -= deltaTime;
			if (value.TimeLeft > 0f)
			{
				trackerList[num] = value;
				num++;
			}
			else
			{
				Activator activator = value.Activator;
				activator.lastState = LastActivatorState.Activate;
				activator.Activate.SetActive(value: true);
			}
		}
		if (num < trackerList.Count)
		{
			trackerList.RemoveRange(num, trackerList.Count - num);
		}
	}

	private void ProcessTrackerOnWarningComplete(List<Tracker> trackerList, float deltaTime)
	{
		if (trackerList == null)
		{
			return;
		}
		int num = 0;
		for (int i = 0; i < trackerList.Count; i++)
		{
			Tracker value = trackerList[i];
			if (value.Activator.Activate.IsPaused)
			{
				trackerList[num] = value;
				num++;
				continue;
			}
			value.TimeLeft -= deltaTime;
			if (value.TimeLeft > 0f)
			{
				trackerList[num] = value;
				num++;
				continue;
			}
			Activator activator = value.Activator;
			if (activator.lastState == LastActivatorState.Deactivate || activator.lastState == LastActivatorState.Warning)
			{
				return;
			}
			activator.lastState = LastActivatorState.Warning;
			activator.Activate.DeactivateWarning();
		}
		if (num < trackerList.Count)
		{
			trackerList.RemoveRange(num, trackerList.Count - num);
		}
	}

	private void ProcessTrackerOnDeactivatingComplete(List<Tracker> trackerList, float deltaTime)
	{
		if (trackerList == null)
		{
			return;
		}
		int num = 0;
		for (int i = 0; i < trackerList.Count; i++)
		{
			Tracker value = trackerList[i];
			if (value.Activator.Activate.IsPaused)
			{
				trackerList[num] = value;
				num++;
				continue;
			}
			value.TimeLeft -= deltaTime;
			if (value.TimeLeft > 0f)
			{
				trackerList[num] = value;
				num++;
				continue;
			}
			Activator activator = value.Activator;
			if (activator.lastState == LastActivatorState.NotActive || activator.lastState == LastActivatorState.Deactivate)
			{
				return;
			}
			activator.lastState = LastActivatorState.Deactivate;
			activator.Activate.SetActive(value: false);
		}
		if (num < trackerList.Count)
		{
			trackerList.RemoveRange(num, trackerList.Count - num);
		}
	}

	public void StartActiveTimer()
	{
		durationLeft = duration;
		if (activationRoutine != null)
		{
			OnAlreadyActivated.Invoke();
			StopCoroutine(activationRoutine);
		}
		interrupted = false;
		activationRoutine = StartCoroutine(Activation());
	}

	private IEnumerator Activation()
	{
		if (!interrupted)
		{
			OnActivated.Invoke();
		}
		if (delay > 0f)
		{
			yield return new WaitForSeconds(delay);
		}
		if (!interrupted)
		{
			OnActivatedDelay.Invoke();
			StartActivation(setActive: true);
			interrupted = false;
		}
		if (deactivateDelay > 0f)
		{
			yield return new WaitForSeconds(deactivateDelay);
			OnDeactivate.Invoke();
		}
		bool hasWarned = false;
		while (durationLeft > 0f)
		{
			yield return null;
			durationLeft -= Time.deltaTime;
			if (!interrupted && !hasWarned && durationLeft <= deactivateWarningDuration)
			{
				hasWarned = true;
				SendDeactivateWarning();
			}
		}
		if (deactivateDelay <= 0f)
		{
			OnDeactivate.Invoke();
		}
		if (!interrupted)
		{
			StartActivation(setActive: false);
		}
		activationRoutine = null;
	}

	[ContextMenu("Test Activate", true)]
	[ContextMenu("Test Deactivate", true)]
	[ContextMenu("Test Deactivate Warning", true)]
	private bool CanDoContextMenu()
	{
		return Application.isPlaying;
	}

	[ContextMenu("Test Activate")]
	public void ForceActivate()
	{
		StartActivation(setActive: true);
	}

	[ContextMenu("Test Deactivate")]
	public void ForceDeactivate()
	{
		StartActivation(setActive: false);
	}

	private void StartActivation(bool setActive)
	{
		List<Tracker> list;
		float num;
		if (setActive)
		{
			if (deactivateOthers)
			{
				foreach (TimedActivator timedActivator in _timedActivators)
				{
					timedActivator.StartedActivating(timedActivator == this);
				}
			}
			list = activatingTrackers;
			num = distanceActivateDelay;
		}
		else
		{
			deactivatingTrackers.Clear();
			list = deactivatingTrackers;
			num = distanceDeactivateDelay;
		}
		foreach (Activator allActivator in allActivators)
		{
			float timeLeft = allActivator.Distance * num + (setActive ? allActivator.ActivateDelay : allActivator.DeactivateDelay);
			list.Add(new Tracker
			{
				Activator = allActivator,
				TimeLeft = timeLeft
			});
		}
	}

	[ContextMenu("Test Deactivate Warning")]
	public void SendDeactivateWarning()
	{
		warningTrackers.Clear();
		foreach (Activator allActivator in allActivators)
		{
			warningTrackers.Add(new Tracker
			{
				Activator = allActivator,
				TimeLeft = allActivator.Distance * distanceDeactivateDelay + allActivator.DeactivateDelay
			});
		}
	}

	private void StartedActivating(bool isFromHit)
	{
		interrupted = true;
		if (isFromHit && !deactivateOnReHit)
		{
			deactivatingTrackers.Clear();
			warningTrackers.Clear();
			activatingTrackers.Clear();
			return;
		}
		deactivatingTrackers.Clear();
		foreach (Activator allActivator in allActivators)
		{
			if (allActivator.Activate.IsActive && allActivator.lastState != 0 && allActivator.lastState != LastActivatorState.Deactivate)
			{
				deactivatingTrackers.Add(new Tracker
				{
					Activator = allActivator,
					TimeLeft = 0f
				});
			}
		}
		warningTrackers.Clear();
		activatingTrackers.Clear();
	}

	private void UpdateSiblingLists()
	{
		if (activateSiblings == null)
		{
			activateSiblings = new Dictionary<Transform, Dictionary<int, List<Activator>>>();
		}
		else
		{
			foreach (KeyValuePair<Transform, Dictionary<int, List<Activator>>> activateSibling in activateSiblings)
			{
				foreach (KeyValuePair<int, List<Activator>> item in activateSibling.Value)
				{
					item.Value.Clear();
				}
			}
		}
		ActivatingBase[] array = activateObjects;
		foreach (ActivatingBase activatingBase in array)
		{
			Transform parent = activatingBase.transform.parent;
			if (!activateSiblings.TryGetValue(parent, out var value))
			{
				value = (activateSiblings[parent] = new Dictionary<int, List<Activator>>());
			}
			if (value.TryGetValue(activatingBase.BranchIndex, out var value2))
			{
				value2.Capacity = activatingBase.transform.parent.childCount;
			}
			else
			{
				value2 = (value[activatingBase.BranchIndex] = new List<Activator>(activatingBase.transform.parent.childCount));
			}
			value2.Add(new Activator
			{
				Activate = activatingBase
			});
		}
	}
}
