using System.Linq;
using UnityEngine;

public class BattleSceneEnemy : MonoBehaviour, IInitialisable
{
	[SerializeField]
	private Component[] exclude;

	private PlayMakerFSM[] fsms;

	private AlertRange[] alertRanges;

	private LineOfSightDetector[] lineOfSightDetectors;

	private bool hasAwaken;

	private bool hasStarted;

	GameObject IInitialisable.gameObject => base.gameObject;

	public bool OnAwake()
	{
		if (hasAwaken)
		{
			return false;
		}
		hasAwaken = true;
		fsms = GetComponentsInChildren<PlayMakerFSM>(includeInactive: true).Except(exclude).OfType<PlayMakerFSM>().ToArray();
		alertRanges = GetComponentsInChildren<AlertRange>(includeInactive: true).Except(exclude).OfType<AlertRange>().ToArray();
		lineOfSightDetectors = GetComponentsInChildren<LineOfSightDetector>(includeInactive: true).Except(exclude).OfType<LineOfSightDetector>().ToArray();
		return true;
	}

	public bool OnStart()
	{
		OnAwake();
		if (hasStarted)
		{
			return false;
		}
		hasStarted = true;
		return true;
	}

	private void Awake()
	{
		OnAwake();
	}

	public void SetActive(bool value)
	{
		OnAwake();
		PlayMakerFSM[] array = fsms;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].enabled = value;
		}
		AlertRange[] array2 = alertRanges;
		for (int i = 0; i < array2.Length; i++)
		{
			array2[i].enabled = value;
		}
		LineOfSightDetector[] array3 = lineOfSightDetectors;
		for (int i = 0; i < array3.Length; i++)
		{
			array3[i].enabled = value;
		}
	}
}
