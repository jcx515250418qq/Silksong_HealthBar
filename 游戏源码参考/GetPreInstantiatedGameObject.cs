using HutongGames.PlayMaker;
using UnityEngine;

public class GetPreInstantiatedGameObject : FsmStateAction
{
	[RequiredField]
	public FsmOwnerDefault target;

	[RequiredField]
	[UIHint(UIHint.Variable)]
	public FsmGameObject storeGameObject;

	public override void Reset()
	{
		target = new FsmOwnerDefault();
	}

	public override void OnEnter()
	{
		GameObject safe = target.GetSafe(this);
		if ((bool)safe)
		{
			PreInstantiateGameObject component = safe.GetComponent<PreInstantiateGameObject>();
			if ((bool)component)
			{
				GameObject instantiatedGameObject = component.InstantiatedGameObject;
				if ((bool)instantiatedGameObject)
				{
					instantiatedGameObject.SetActive(value: true);
					storeGameObject.Value = instantiatedGameObject;
				}
			}
		}
		Finish();
	}
}
