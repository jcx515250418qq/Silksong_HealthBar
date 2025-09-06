using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.GUILayout)]
	[Tooltip("Close a GUILayout group started with {{GUILayoutBeginArea}}.")]
	public class GUILayoutEndArea : FsmStateAction
	{
		public override void Reset()
		{
		}

		public override void OnGUI()
		{
			GUILayout.EndArea();
		}
	}
}
