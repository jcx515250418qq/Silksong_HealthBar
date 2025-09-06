using System.Collections.Generic;
using UnityEngine;

public class AreaTitle : ManagerSingleton<AreaTitle>
{
	[SerializeField]
	private List<GameObject> ensureAwake = new List<GameObject>();

	private bool[] enabledStates;

	public bool Initialised { get; private set; }

	protected override void Awake()
	{
		base.Awake();
		if (ManagerSingleton<AreaTitle>.UnsafeInstance == this)
		{
			PlayMakerGlobals.Instance.Variables.FindFsmGameObject("AreaTitle").Value = base.gameObject;
		}
		ensureAwake.RemoveAll((GameObject o) => o == null);
		enabledStates = new bool[ensureAwake.Count];
		for (int i = 0; i < ensureAwake.Count; i++)
		{
			GameObject gameObject = ensureAwake[i];
			bool activeSelf = gameObject.activeSelf;
			enabledStates[i] = activeSelf;
			if (!activeSelf)
			{
				gameObject.SetActive(value: true);
			}
		}
	}

	private void Start()
	{
		for (int i = 0; i < ensureAwake.Count; i++)
		{
			GameObject gameObject = ensureAwake[i];
			if (!enabledStates[i])
			{
				gameObject.SetActive(value: false);
			}
		}
		Initialised = true;
		base.gameObject.SetActive(value: false);
	}
}
