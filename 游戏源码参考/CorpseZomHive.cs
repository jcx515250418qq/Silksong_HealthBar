using UnityEngine;

public class CorpseZomHive : CorpseChunker
{
	protected override void LandEffects()
	{
		base.LandEffects();
		GameObject gameObject = GameObject.FindWithTag("Extra Tag");
		if (!gameObject)
		{
			return;
		}
		for (int i = 0; i < 3; i++)
		{
			int index = Random.Range(0, gameObject.transform.childCount);
			Transform child = gameObject.transform.GetChild(index);
			if ((bool)child)
			{
				child.SetParent(null);
				child.position = base.transform.position;
				FSMUtility.SendEventToGameObject(child.gameObject, "SPAWN");
				FlingUtils.SelfConfig config = default(FlingUtils.SelfConfig);
				config.Object = child.gameObject;
				config.SpeedMin = 5f;
				config.SpeedMax = 10f;
				config.AngleMin = 0f;
				config.AngleMax = 180f;
				FlingUtils.FlingObject(config, base.transform, Vector3.zero);
			}
		}
	}
}
