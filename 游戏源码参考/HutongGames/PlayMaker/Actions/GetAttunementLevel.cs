using GlobalSettings;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Math)]
	public class GetAttunementLevel : FsmStateAction
	{
		[RequiredField]
		[UIHint(UIHint.Variable)]
		public FsmInt storeVariable;

		public override void Reset()
		{
			storeVariable = null;
		}

		public override void OnEnter()
		{
			GetLevel();
		}

		private void GetLevel()
		{
			int num = GameManager.instance.playerData.attunementLevel;
			if (Gameplay.MusicianCharmTool.IsEquipped)
			{
				num++;
			}
			storeVariable.Value = num;
		}
	}
}
