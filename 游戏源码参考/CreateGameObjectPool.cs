using System.Collections.Generic;
using HutongGames.PlayMaker;
using UnityEngine;

public class CreateGameObjectPool : FsmStateAction
{
	public FsmGameObject prefab;

	public FsmInt amount;

	public FsmBool useExisting;

	public override void Reset()
	{
		prefab = null;
		amount = null;
		useExisting = new FsmBool(true);
	}

	public override void OnEnter()
	{
		if ((bool)prefab.Value)
		{
			int num = amount.Value;
			if (useExisting.Value)
			{
				List<GameObject> pooled = ObjectPool.GetPooled(prefab.Value, null, appendList: false);
				num -= pooled.Count;
			}
			if (num > 0)
			{
				ObjectPool.CreatePool(prefab.Value, num);
			}
		}
		Finish();
	}
}
