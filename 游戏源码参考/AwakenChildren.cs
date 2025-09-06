using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[DisallowMultipleComponent]
public sealed class AwakenChildren : MonoBehaviour
{
	[SerializeField]
	private List<GameObject> awakenTarget = new List<GameObject>();

	private bool didAwaken;

	private bool[] activeState;

	private void Awake()
	{
		awakenTarget.RemoveAll((GameObject o) => o == null);
		activeState = new bool[awakenTarget.Count];
		for (int i = 0; i < awakenTarget.Count; i++)
		{
			GameObject gameObject = awakenTarget[i];
			bool activeSelf = gameObject.activeSelf;
			activeState[i] = activeSelf;
			if (!activeSelf)
			{
				gameObject.SetActive(value: true);
				didAwaken = true;
			}
		}
	}

	private void Start()
	{
		if (!didAwaken)
		{
			return;
		}
		for (int i = 0; i < awakenTarget.Count; i++)
		{
			GameObject gameObject = awakenTarget[i];
			if (!activeState[i])
			{
				IInitialisable[] componentsInChildren = gameObject.GetComponentsInChildren<IInitialisable>(includeInactive: true);
				for (int j = 0; j < componentsInChildren.Length; j++)
				{
					componentsInChildren[j].OnStart();
				}
				gameObject.SetActive(value: false);
			}
		}
	}

	[ContextMenu("Gather FSMs")]
	private void GatherFSMs()
	{
		HashSet<GameObject> hashSet = new HashSet<GameObject>();
		PlayMakerFSM[] componentsInChildren = GetComponentsInChildren<PlayMakerFSM>(includeInactive: true);
		foreach (PlayMakerFSM playMakerFSM in componentsInChildren)
		{
			hashSet.Add(playMakerFSM.gameObject);
		}
		hashSet.Remove(base.gameObject);
		awakenTarget.RemoveAll((GameObject o) => o == null);
		awakenTarget = awakenTarget.Union(hashSet).ToList();
	}
}
