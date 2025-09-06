using System;
using TeamCherry.SharedUtils;
using UnityEngine;

[Serializable]
public struct PlayerDataIntOperation
{
	private enum Operation
	{
		Add = 0,
		Subtract = 1,
		Multiply = 2,
		Set = 3
	}

	[SerializeField]
	[PlayerDataField(typeof(int), true)]
	private string variableName;

	[SerializeField]
	private Operation operation;

	[SerializeField]
	public int number;

	public void Execute()
	{
		int num = PlayerData.instance.GetVariable<int>(variableName);
		switch (operation)
		{
		case Operation.Add:
			num += number;
			break;
		case Operation.Subtract:
			num -= number;
			break;
		case Operation.Multiply:
			num *= number;
			break;
		case Operation.Set:
			num = number;
			break;
		}
		PlayerData.instance.SetVariable(variableName, num);
	}
}
