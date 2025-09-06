using UnityEngine;

public class CorpseHatcher : Corpse
{
	protected override void Smash()
	{
		if (!hitAcid)
		{
			BloodSpawner.SpawnBlood(base.transform.position, 40, 40, 15f, 20f, 75f, 105f, null);
			GameObject gameObject = GameObject.FindWithTag("Extra Tag");
			if ((bool)gameObject)
			{
				for (int i = 0; i < 2; i++)
				{
					int index = Random.Range(0, gameObject.transform.childCount);
					Transform child = gameObject.transform.GetChild(index);
					if ((bool)child)
					{
						child.SetParent(null);
						child.position = base.transform.position;
						FSMUtility.SendEventToGameObject(child.gameObject, "SPAWN");
					}
				}
			}
		}
		base.Smash();
	}
}
