using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class SceneLintLogger : MonoBehaviour, ISceneLintUpgrader
{
	public interface ILogProvider
	{
		string GetSceneLintLog(string sceneName);
	}

	[SerializeField]
	private List<Object> logProviders;

	private void OnValidate()
	{
		if (logProviders == null)
		{
			return;
		}
		for (int num = logProviders.Count - 1; num >= 0; num--)
		{
			Object @object = logProviders[num];
			if (!(@object == null) && !(@object is ILogProvider))
			{
				logProviders[num] = null;
			}
		}
	}

	private void Awake()
	{
		if (logProviders == null)
		{
			return;
		}
		for (int num = logProviders.Count - 1; num >= 0; num--)
		{
			Object @object = logProviders[num];
			if (@object == null || !(@object is ILogProvider))
			{
				logProviders.RemoveAt(num);
			}
		}
	}

	public string OnSceneLintUpgrade(bool doUpgrade)
	{
		StringBuilder stringBuilder = null;
		string sceneName = base.gameObject.scene.name;
		foreach (ILogProvider item in EnumerateLogProviders())
		{
			string sceneLintLog = item.GetSceneLintLog(sceneName);
			if (!string.IsNullOrEmpty(sceneLintLog))
			{
				if (stringBuilder == null)
				{
					stringBuilder = new StringBuilder("SceneLintLogger: ");
					stringBuilder.Append(sceneLintLog);
				}
				else
				{
					stringBuilder.Append(", ");
					stringBuilder.Append(sceneLintLog);
				}
			}
		}
		return stringBuilder?.ToString();
	}

	public IEnumerable<ILogProvider> EnumerateLogProviders()
	{
		foreach (Object logProvider2 in logProviders)
		{
			if (logProvider2 is ILogProvider logProvider)
			{
				yield return logProvider;
			}
		}
	}
}
