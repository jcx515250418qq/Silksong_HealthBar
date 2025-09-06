using HutongGames.PlayMaker;
using UnityEngine;

public class ShowPromptMarker : FsmStateAction
{
	public FsmGameObject prefab;

	public FsmString labelName;

	[UIHint(UIHint.Variable)]
	public FsmGameObject spawnPoint;

	[UIHint(UIHint.Variable)]
	public FsmGameObject storeObject;

	public override void Reset()
	{
		prefab = new FsmGameObject();
		labelName = new FsmString();
		spawnPoint = new FsmGameObject();
		storeObject = new FsmGameObject();
	}

	public override void OnEnter()
	{
		if ((bool)prefab.Value && (bool)spawnPoint.Value)
		{
			GameObject gameObject = null;
			if ((bool)storeObject.Value)
			{
				gameObject = storeObject.Value;
			}
			else
			{
				gameObject = prefab.Value.Spawn();
				storeObject.Value = gameObject;
			}
			gameObject.transform.SetPosition2D(spawnPoint.Value.transform.position);
			PromptMarker component = gameObject.GetComponent<PromptMarker>();
			if ((bool)component)
			{
				component.SetLabel(labelName.Value);
				component.SetOwner(base.Owner);
				component.Show();
			}
		}
		Finish();
	}
}
