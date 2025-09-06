using GlobalSettings;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Color)]
	public class GetPoisonTintColour : FsmStateAction
	{
		[RequiredField]
		[UIHint(UIHint.Variable)]
		[Tooltip("The Color Variable in which to store poison tint.")]
		public FsmColor colorVariable;

		public override void Reset()
		{
			colorVariable = null;
		}

		public override void OnEnter()
		{
			DoSetColorValue();
			Finish();
		}

		private void DoSetColorValue()
		{
			if (colorVariable != null)
			{
				colorVariable.Value = Gameplay.PoisonPouchTintColour;
			}
		}
	}
}
