using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("Hollow Knight")]
	public class PreloadVibration : FsmStateAction
	{
		[ObjectType(typeof(LowFidelityVibrations))]
		private FsmEnum lowFidelityVibration;

		[ObjectType(typeof(TextAsset))]
		private FsmObject highFidelityVibration;

		public override void Reset()
		{
			base.Reset();
			lowFidelityVibration = new FsmEnum
			{
				UseVariable = false
			};
			highFidelityVibration = new FsmObject
			{
				UseVariable = false
			};
		}

		public override void OnEnter()
		{
			base.OnEnter();
			Finish();
		}
	}
}
