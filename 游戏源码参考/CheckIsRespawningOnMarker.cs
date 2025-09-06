using HutongGames.PlayMaker;
using UnityEngine;

public class CheckIsRespawningOnMarker : FSMUtility.CheckFsmStateAction
{
	public FsmOwnerDefault MarkerObject;

	public override bool IsTrue
	{
		get
		{
			GameObject safe = MarkerObject.GetSafe(this);
			if (!safe)
			{
				return false;
			}
			string text = safe.name;
			GameManager instance = GameManager.instance;
			PlayerData instance2 = PlayerData.instance;
			if (text != instance2.respawnMarkerName)
			{
				return false;
			}
			if (instance.GetSceneNameString() != instance2.respawnScene)
			{
				return false;
			}
			return true;
		}
	}

	public override void Reset()
	{
		base.Reset();
		MarkerObject = null;
	}
}
