using TeamCherry.SharedUtils;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Logic)]
	[Tooltip("Sends Events based on the value of a Boolean Variable.")]
	public class PlayerDataBoolTest : FsmStateAction
	{
		[RequiredField]
		public FsmString boolName;

		[Tooltip("Event to send if the Bool variable is True.")]
		public FsmEvent isTrue;

		[Tooltip("Event to send if the Bool variable is False.")]
		public FsmEvent isFalse;

		private bool boolCheck;

		public override void Reset()
		{
			boolName = null;
			isTrue = null;
			isFalse = null;
		}

		public override void OnEnter()
		{
			GameManager instance = GameManager.instance;
			if (instance == null)
			{
				Debug.Log("GameManager could not be found");
				return;
			}
			if (VariableExtensions.VariableExists<bool, PlayerData>(boolName.Value))
			{
				boolCheck = instance.GetPlayerDataBool(boolName.Value);
				base.Fsm.Event(boolCheck ? isTrue : isFalse);
			}
			Finish();
		}
	}
}
