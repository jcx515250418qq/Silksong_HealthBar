using System.Collections.Generic;
using UnityEngine;

public class SongGateStringControl : MonoBehaviour
{
	[SerializeField]
	private List<SongGateString> strings;

	[SerializeField]
	private bool getChildren;

	private bool init;

	private void Awake()
	{
		Init();
		if (getChildren)
		{
			strings.AddRange(GetComponentsInChildren<SongGateString>(includeInactive: true));
		}
	}

	private void Init()
	{
		if (!init)
		{
			init = true;
			strings.RemoveAll((SongGateString o) => o == null);
		}
	}

	public void StrumStart()
	{
		Init();
		foreach (SongGateString @string in strings)
		{
			@string.StrumStart();
		}
	}

	public void StrumEnd()
	{
		Init();
		foreach (SongGateString @string in strings)
		{
			@string.StrumEnd();
		}
	}

	public void QuickStrum()
	{
		Init();
		foreach (SongGateString @string in strings)
		{
			@string.QuickStrum();
		}
	}
}
