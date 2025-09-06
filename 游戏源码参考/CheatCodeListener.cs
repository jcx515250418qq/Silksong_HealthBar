using System;
using System.Collections.Generic;
using UnityEngine;

public class CheatCodeListener : MonoBehaviour
{
	private enum InputAction
	{
		Up = 0,
		Down = 1,
		Left = 2,
		Right = 3
	}

	private class CheatCode
	{
		public InputAction[] Sequence;

		public Action<CheatCodeListener> Response;
	}

	[SerializeField]
	private GameObject permadeathUnlockEffect;

	private readonly CheatCode[] cheatCodes = new CheatCode[1]
	{
		new CheatCode
		{
			Sequence = new InputAction[8]
			{
				InputAction.Up,
				InputAction.Down,
				InputAction.Up,
				InputAction.Down,
				InputAction.Left,
				InputAction.Right,
				InputAction.Left,
				InputAction.Right
			},
			Response = delegate(CheatCodeListener listener)
			{
				if (GameManager.instance.GetStatusRecordInt("RecPermadeathMode") == 0)
				{
					PermadeathUnlock.Unlock();
					UnityEngine.Object.Instantiate(listener.permadeathUnlockEffect);
				}
			}
		}
	};

	private readonly List<InputAction> currentSequence = new List<InputAction>();

	private HeroActions ia;

	private void Awake()
	{
		ia = ManagerSingleton<InputHandler>.Instance.inputActions;
	}

	private void OnEnable()
	{
		currentSequence.Clear();
	}

	private void Update()
	{
		int count = currentSequence.Count;
		if (ia.Up.WasPressed)
		{
			currentSequence.Add(InputAction.Up);
		}
		if (ia.Down.WasPressed)
		{
			currentSequence.Add(InputAction.Down);
		}
		if (ia.Left.WasPressed)
		{
			currentSequence.Add(InputAction.Left);
		}
		if (ia.Right.WasPressed)
		{
			currentSequence.Add(InputAction.Right);
		}
		if (currentSequence.Count <= count)
		{
			return;
		}
		CheatCode cheatCode = null;
		CheatCode[] array = cheatCodes;
		foreach (CheatCode cheatCode2 in array)
		{
			if (cheatCode2.Sequence.Length != currentSequence.Count)
			{
				continue;
			}
			bool flag = false;
			for (int j = 0; j < cheatCode2.Sequence.Length; j++)
			{
				if (cheatCode2.Sequence[j] != currentSequence[j])
				{
					flag = true;
					break;
				}
			}
			if (!flag)
			{
				cheatCode = cheatCode2;
				break;
			}
		}
		if (cheatCode != null)
		{
			cheatCode.Response(this);
			currentSequence.Clear();
		}
	}
}
