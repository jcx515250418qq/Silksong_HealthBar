using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	public class ShowExtraToolSlotUIMsg : FsmStateAction
	{
		[CheckForComponent(typeof(ExtraToolSlotUIMsg))]
		[RequiredField]
		public FsmGameObject Prefab;

		[ObjectType(typeof(ToolItemType))]
		public FsmEnum UnlockedSlotType;

		public FsmEvent FinishEvent;

		public override void Reset()
		{
			Prefab = null;
			UnlockedSlotType = null;
			FinishEvent = null;
		}

		public override void OnEnter()
		{
			GameObject value = Prefab.Value;
			if (!value)
			{
				Finish();
			}
			else
			{
				ExtraToolSlotUIMsg.Spawn((ToolItemType)(object)UnlockedSlotType.Value, value, OnCrestMsgEnd);
			}
		}

		private void OnCrestMsgEnd()
		{
			base.Fsm.Event(FinishEvent);
			Finish();
		}
	}
}
