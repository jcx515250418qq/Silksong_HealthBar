using UnityEngine;
using UnityEngine.Events;

public class StatusRecordComparisonResponse : MonoBehaviour
{
	[SerializeField]
	private string key;

	[SerializeField]
	private int compareTo;

	[Space]
	public UnityEvent OnIsEqual;

	public UnityEvent OnIsNotEqual;

	private bool doOnEnable;

	public string Key
	{
		get
		{
			return key;
		}
		set
		{
			key = value;
			OnEnable();
		}
	}

	public int CompareTo
	{
		get
		{
			return compareTo;
		}
		set
		{
			compareTo = value;
			OnEnable();
		}
	}

	private void Start()
	{
		DoResponse();
		doOnEnable = true;
	}

	private void OnEnable()
	{
		if (doOnEnable)
		{
			DoResponse();
		}
	}

	private void DoResponse()
	{
		if (!string.IsNullOrEmpty(key))
		{
			if (GameManager.instance.GetStatusRecordInt(key) == compareTo)
			{
				OnIsEqual.Invoke();
			}
			else
			{
				OnIsNotEqual.Invoke();
			}
		}
	}
}
