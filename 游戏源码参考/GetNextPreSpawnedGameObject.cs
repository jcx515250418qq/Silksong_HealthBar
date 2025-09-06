using HutongGames.PlayMaker;
using UnityEngine;

[ActionCategory("Hollow Knight")]
public class GetNextPreSpawnedGameObject : FsmStateAction
{
	[UIHint(UIHint.Variable)]
	[ArrayEditor(VariableType.GameObject, "", 0, 0, 65536)]
	public FsmArray storedArray;

	public FsmVector3 spawnPosition;

	[UIHint(UIHint.Variable)]
	public FsmGameObject storeObject;

	[UIHint(UIHint.Variable)]
	public FsmInt currentIndex;

	public override void Reset()
	{
		storedArray = null;
		spawnPosition = null;
		storeObject = null;
	}

	public override void OnEnter()
	{
		if (!currentIndex.IsNone && currentIndex.Value < storedArray.Length)
		{
			GameObject gameObject = (GameObject)storedArray.Values[currentIndex.Value];
			if (!spawnPosition.IsNone)
			{
				gameObject.transform.position = spawnPosition.Value;
			}
			gameObject.SetActive(value: true);
			storeObject.Value = gameObject;
			currentIndex.Value++;
		}
		Finish();
	}
}
