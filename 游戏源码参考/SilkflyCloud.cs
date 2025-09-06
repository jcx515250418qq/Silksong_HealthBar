using UnityEngine;

public class SilkflyCloud : MemoryOrbSource
{
	[Space]
	[SerializeField]
	private Transform orbsParent;

	private int appearingOrbIndex;

	public int OrbsCount => orbsParent.childCount;

	private void OnDrawGizmos()
	{
		if ((bool)orbsParent)
		{
			Vector3 from = base.transform.position;
			for (int i = 0; i < orbsParent.childCount; i++)
			{
				Vector3 position = orbsParent.GetChild(i).position;
				Gizmos.DrawLine(from, position);
				from = position;
			}
		}
	}

	private void Start()
	{
		for (int i = 0; i < orbsParent.childCount; i++)
		{
			orbsParent.GetChild(i).gameObject.SetActive(value: false);
		}
	}

	public void StartTrail()
	{
		appearingOrbIndex = 0;
	}

	public GameObject GetNextOrb()
	{
		Transform child = orbsParent.GetChild(appearingOrbIndex);
		appearingOrbIndex++;
		return child.gameObject;
	}

	public bool IsFinalOrb()
	{
		return appearingOrbIndex >= orbsParent.childCount;
	}

	public void StartTimeAlert()
	{
		for (int i = 0; i < orbsParent.childCount; i++)
		{
			orbsParent.GetChild(i).GetComponent<MemoryOrb>().StartTimeAlert();
		}
	}

	public void Dissipate()
	{
		for (int i = 0; i < orbsParent.childCount; i++)
		{
			orbsParent.GetChild(i).GetComponent<MemoryOrb>().Dissipate();
		}
	}

	public void ActivateUncollectedOrbs(ulong bitmask, int startIndex, out int postIndex)
	{
		for (int i = 0; i < orbsParent.childCount; i++)
		{
			bool flag = bitmask.IsBitSet(startIndex + i);
			orbsParent.GetChild(i).gameObject.SetActive(!flag);
		}
		postIndex = startIndex + orbsParent.childCount;
	}
}
