using System;
using System.Collections.Generic;
using HutongGames.PlayMaker;
using UnityEngine;

public static class PlayMakerValidator
{
	[Flags]
	public enum FixMode
	{
		None = 0,
		FixFsmExecutionStack = 1,
		FixGlobalVariables = 2,
		All = 3
	}

	public static List<string> ValidatePlayMakerState(FixMode tryFixIssues = FixMode.All)
	{
		List<string> errorMessages = new List<string>();
		ValidateFsmExecutionStack(ref errorMessages, tryFixIssues.HasFlag(FixMode.FixFsmExecutionStack));
		if (tryFixIssues.HasFlag(FixMode.FixGlobalVariables))
		{
			FixGlobalVariables(null);
		}
		return errorMessages;
	}

	private static void ValidateFsmExecutionStack(ref List<string> errorMessages, bool tryFixIssues = true)
	{
		if (FsmExecutionStack.ExecutingFsm == null)
		{
			return;
		}
		errorMessages.Add("FsmExecutionStack is not empty! FsmExecutionStack should be empty when validating the PlayMaker state. Non empty stack may result in major memory leaks in the runtime. The next messages contain the names of leaked FSMs. Those are the names of the FSM, not the GameObject that triggered the execution.");
		while (FsmExecutionStack.ExecutingFsm != null)
		{
			Fsm executingFsm = FsmExecutionStack.ExecutingFsm;
			_ = executingFsm.Name;
			string text = ((executingFsm.OwnerObject != null) ? executingFsm.OwnerObject.name : "(no object)");
			errorMessages.Add("Leaked FSM on the FsmExecutionStack. Name:" + (executingFsm.Name ?? "(no name)") + ", Owner: " + text + ". " + (tryFixIssues ? "(Issue automatically fixed)" : ""));
			if (tryFixIssues)
			{
				executingFsm.Stop();
				FsmExecutionStack.PopFsm();
				continue;
			}
			break;
		}
	}

	public static void FixGlobalVariables(List<NamedVariable> fixedVariables)
	{
		PlayMakerGlobals instance = PlayMakerGlobals.Instance;
		if (instance == null)
		{
			return;
		}
		FsmObject[] objectVariables = instance.Variables.ObjectVariables;
		if (objectVariables != null)
		{
			FsmObject[] array = objectVariables;
			foreach (FsmObject fsmObject in array)
			{
				if (fsmObject != null && fsmObject.Value.IsLeakedManagedShellObject())
				{
					fsmObject.Value = null;
					fixedVariables?.Add(fsmObject);
				}
			}
		}
		FsmGameObject[] gameObjectVariables = instance.Variables.GameObjectVariables;
		if (gameObjectVariables == null)
		{
			return;
		}
		FsmGameObject[] array2 = gameObjectVariables;
		foreach (FsmGameObject fsmGameObject in array2)
		{
			if (fsmGameObject != null && fsmGameObject.Value.IsLeakedManagedShellObject())
			{
				fsmGameObject.Value = null;
				fixedVariables?.Add(fsmGameObject);
			}
		}
	}

	private static bool IsLeakedManagedShellObject(this UnityEngine.Object obj)
	{
		if (obj == null)
		{
			return (object)obj != null;
		}
		return false;
	}
}
