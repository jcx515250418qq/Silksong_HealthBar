using System;
using TeamCherry.SharedUtils;
using UnityEngine;
using UnityEngine.Events;

public class PlayerDataStringResponse : MonoBehaviour
{
	[Serializable]
	private class UnityStringEvent : UnityEvent<string>
	{
	}

	[SerializeField]
	[PlayerDataField(typeof(string), true)]
	private string fieldName;

	[Space]
	[SerializeField]
	private UnityStringEvent OnValue;

	private bool hasStarted;

	private void Start()
	{
		hasStarted = true;
		OnEnable();
	}

	private void OnEnable()
	{
		if (hasStarted)
		{
			Evaluate();
		}
	}

	private void Evaluate()
	{
		string variable = PlayerData.instance.GetVariable<string>(fieldName);
		OnValue.Invoke(variable);
	}
}
