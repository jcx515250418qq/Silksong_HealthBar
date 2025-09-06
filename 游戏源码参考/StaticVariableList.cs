using System.Collections.Generic;
using UnityEngine;

public static class StaticVariableList
{
	private class StaticVariable
	{
		public object Value;

		public int SceneTransitionsLeft;
	}

	private static Dictionary<string, StaticVariable> _variables;

	private static List<string> _removeKeys;

	public static void Clear()
	{
		_variables?.Clear();
	}

	public static void ClearSceneTransitions()
	{
		if (_variables == null)
		{
			return;
		}
		if (_removeKeys == null)
		{
			_removeKeys = new List<string>();
		}
		foreach (KeyValuePair<string, StaticVariable> variable in _variables)
		{
			if (variable.Value.SceneTransitionsLeft > 0 && !(variable.Key == "doubleJumpNpcPresent"))
			{
				_removeKeys.Add(variable.Key);
			}
		}
		foreach (string removeKey in _removeKeys)
		{
			_variables.Remove(removeKey);
		}
		_removeKeys.Clear();
	}

	public static void SetValue(string variableName, object value, int sceneTransitionsLimit = 0)
	{
		if (_variables == null)
		{
			_variables = new Dictionary<string, StaticVariable>();
		}
		_variables[variableName] = new StaticVariable
		{
			Value = value,
			SceneTransitionsLeft = sceneTransitionsLimit
		};
	}

	public static T GetValue<T>(string variableName)
	{
		if (_variables != null && _variables.ContainsKey(variableName))
		{
			return (T)_variables[variableName].Value;
		}
		Debug.LogError("Attempt to get " + variableName + " from static variable list failed!");
		return default(T);
	}

	public static object GetValue(string variableName)
	{
		if (_variables != null && _variables.ContainsKey(variableName))
		{
			return _variables[variableName].Value;
		}
		Debug.LogError("Attempt to get " + variableName + " from static variable list failed!");
		return null;
	}

	public static T GetValue<T>(string variableName, T defaultValue)
	{
		if (_variables == null || !_variables.ContainsKey(variableName))
		{
			return defaultValue;
		}
		return (T)_variables[variableName].Value;
	}

	public static bool Exists(string variableName)
	{
		if (_variables != null)
		{
			return _variables.ContainsKey(variableName);
		}
		return false;
	}

	public static void ReportSceneTransition()
	{
		if (_variables == null)
		{
			return;
		}
		if (_removeKeys == null)
		{
			_removeKeys = new List<string>();
		}
		foreach (KeyValuePair<string, StaticVariable> variable in _variables)
		{
			StaticVariable value = variable.Value;
			if (value.SceneTransitionsLeft > 0)
			{
				value.SceneTransitionsLeft--;
				if (value.SceneTransitionsLeft == 0)
				{
					_removeKeys.Add(variable.Key);
				}
			}
		}
		foreach (string removeKey in _removeKeys)
		{
			_variables.Remove(removeKey);
		}
		_removeKeys.Clear();
	}
}
