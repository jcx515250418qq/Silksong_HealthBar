using UnityEngine;
using UnityEngine.Events;

public class PersistentIntItemResponder : MonoBehaviour
{
	[SerializeField]
	private PersistentIntItem persistent;

	[SerializeField]
	private Extensions.IntTest test;

	[SerializeField]
	private int testValue;

	[Space]
	public UnityEvent OnSuccess;

	public UnityEvent OnFailure;

	private void Awake()
	{
		if (!persistent)
		{
			return;
		}
		persistent.OnSetSaveState += delegate(int value)
		{
			if (value.Test(test, testValue))
			{
				OnSuccess.Invoke();
			}
			else
			{
				OnFailure.Invoke();
			}
		};
	}
}
