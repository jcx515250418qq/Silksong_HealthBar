using HutongGames.PlayMaker;
using UnityEngine;

[ActionCategory("Hollow Knight")]
public class PreSpawnGameObjects : FsmStateAction
{
	public FsmGameObject prefab;

	[UIHint(UIHint.Variable)]
	[ArrayEditor(VariableType.GameObject, "", 0, 0, 65536)]
	public FsmArray storeArray;

	public FsmInt spawnAmount;

	public FsmInt spawnAmountMultiplier;

	public override void Reset()
	{
		prefab = null;
		storeArray = null;
		spawnAmount = null;
		spawnAmountMultiplier = 1;
	}

	public override void OnEnter()
	{
		if ((bool)prefab.Value && !storeArray.IsNone && spawnAmount.Value > 0 && spawnAmountMultiplier.Value > 0)
		{
			int num = spawnAmount.Value * spawnAmountMultiplier.Value;
			storeArray.Resize(num);
			for (int i = 0; i < num; i++)
			{
				storeArray.Values[i] = Object.Instantiate(prefab.Value);
				((GameObject)storeArray.Values[i]).SetActive(value: false);
			}
		}
		Finish();
	}
}
