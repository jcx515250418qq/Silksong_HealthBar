using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.GUILayout)]
	[Tooltip("Inserts a space in the current layout group.")]
	public class GUILayoutSpace : FsmStateAction
	{
		[Tooltip("Size of space in pixels.")]
		public FsmFloat space;

		public override void Reset()
		{
			space = 10f;
		}

		public override void OnGUI()
		{
			GUILayout.Space(space.Value);
		}
	}
}
