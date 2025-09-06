using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.GameObject)]
	public class SetHazardRespawn : FsmStateAction
	{
		[RequiredField]
		public FsmGameObject hazardRespawnMarker;

		public override void Reset()
		{
			hazardRespawnMarker = null;
		}

		public override void OnEnter()
		{
			PlayerData instance = PlayerData.instance;
			if (instance == null)
			{
				Debug.LogError("Player Data reference is null, please check this is being set correctly.");
			}
			HazardRespawnMarker component = hazardRespawnMarker.Value.GetComponent<HazardRespawnMarker>();
			if ((bool)component)
			{
				instance.SetHazardRespawn(component);
			}
			Finish();
		}
	}
}
