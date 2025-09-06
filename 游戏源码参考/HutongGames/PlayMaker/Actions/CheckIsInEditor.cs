using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Application)]
	public class CheckIsInEditor : FSMUtility.CheckFsmStateAction
	{
		public override bool IsTrue => Application.isEditor;
	}
}
