using System;
using UnityEngine;

public class InputDebugger : MonoBehaviour
{
	private void Update()
	{
		foreach (KeyCode value in Enum.GetValues(typeof(KeyCode)))
		{
			if (Input.GetKeyDown(value))
			{
				Debug.Log("KeyCode down: " + value);
			}
		}
	}
}
