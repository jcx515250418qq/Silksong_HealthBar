using System;
using System.Collections.Generic;
using UnityEngine;

public sealed class ToolGameObjectActivator : MonoBehaviour
{
	[Serializable]
	private struct ToolTest
	{
		public ToolItem tool;

		public bool expectedUnlockedState;

		public bool IsFulfilled
		{
			get
			{
				if (tool == null)
				{
					return false;
				}
				return tool.IsUnlocked == expectedUnlockedState;
			}
		}
	}

	[Tooltip("Activated if tests pass, deactivated if tests fail.")]
	[SerializeField]
	private GameObject activateGameObject;

	[Tooltip("Deactivated if tests pass, activated if tests fail")]
	[SerializeField]
	private GameObject deactivateGameObject;

	[SerializeField]
	private List<ToolTest> toolTests = new List<ToolTest>();

	private void OnEnable()
	{
		Evaulate();
	}

	private void Evaulate()
	{
		toolTests.RemoveAll((ToolTest o) => o.tool == null);
		bool flag = false;
		foreach (ToolTest toolTest in toolTests)
		{
			if (!toolTest.IsFulfilled)
			{
				flag = false;
				break;
			}
			flag = true;
		}
		if ((bool)activateGameObject)
		{
			activateGameObject.SetActive(flag);
		}
		if ((bool)deactivateGameObject)
		{
			deactivateGameObject.SetActive(!flag);
		}
	}
}
