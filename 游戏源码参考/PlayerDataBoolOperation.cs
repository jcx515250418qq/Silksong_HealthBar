using System;
using TeamCherry.SharedUtils;
using UnityEngine;

[Serializable]
public struct PlayerDataBoolOperation
{
	private enum Operation
	{
		Set = 0,
		Flip = 1
	}

	[SerializeField]
	[PlayerDataField(typeof(bool), true)]
	private string variableName;

	[SerializeField]
	private Operation operation;

	[SerializeField]
	[ModifiableProperty]
	[Conditional("IsSetOperation", true, true, true)]
	public bool value;

	private bool IsSetOperation()
	{
		return operation == Operation.Set;
	}

	public void Execute()
	{
		bool flag = PlayerData.instance.GetVariable<bool>(variableName);
		switch (operation)
		{
		case Operation.Set:
			flag = value;
			break;
		case Operation.Flip:
			flag = !flag;
			break;
		}
		PlayerData.instance.SetVariable(variableName, flag);
	}
}
