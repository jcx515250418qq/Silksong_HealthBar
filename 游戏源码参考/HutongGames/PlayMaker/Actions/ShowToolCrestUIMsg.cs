using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	public class ShowToolCrestUIMsg : FsmStateAction
	{
		[CheckForComponent(typeof(ToolCrestUIMsg))]
		[RequiredField]
		public FsmGameObject Prefab;

		[ObjectType(typeof(ToolCrest))]
		[RequiredField]
		public FsmObject Crest;

		public FsmEvent FinishEvent;

		public override void Reset()
		{
			Prefab = null;
			Crest = null;
			FinishEvent = null;
		}

		public override void OnEnter()
		{
			ToolCrest toolCrest = Crest.Value as ToolCrest;
			GameObject value = Prefab.Value;
			if (!toolCrest || !value)
			{
				Finish();
			}
			else
			{
				ToolCrestUIMsg.Spawn(toolCrest, value, OnCrestMsgEnd);
			}
		}

		private void OnCrestMsgEnd()
		{
			base.Fsm.Event(FinishEvent);
			Finish();
		}
	}
}
