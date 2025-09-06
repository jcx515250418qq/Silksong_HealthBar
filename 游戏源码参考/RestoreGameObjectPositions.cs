using System.Collections.Generic;
using HutongGames.PlayMaker;
using UnityEngine;

[ActionCategory("Hollow Knight")]
public class RestoreGameObjectPositions : FsmStateAction
{
	private Dictionary<GameObject, Vector3> positions;

	public override void Reset()
	{
		base.Reset();
		positions = null;
	}

	public override void OnEnter()
	{
		base.OnEnter();
		if (positions == null)
		{
			positions = new Dictionary<GameObject, Vector3>(base.Owner.transform.childCount);
			foreach (Transform item in base.Owner.transform)
			{
				positions.Add(item.gameObject, item.localPosition);
			}
		}
		else
		{
			foreach (KeyValuePair<GameObject, Vector3> position in positions)
			{
				position.Key.transform.localPosition = position.Value;
			}
		}
		Finish();
	}
}
