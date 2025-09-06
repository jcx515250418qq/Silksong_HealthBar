using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	public class HeroBoxControl : FsmStateAction
	{
		[Serializable]
		public enum HeroBoxState
		{
			Off = 0,
			Normal = 1,
			Downspike = 2,
			DownDrill = 3,
			Scuttle = 4,
			Sprint = 5,
			Airdash = 6,
			ReaperDownSlash = 7,
			WallSlide = 8,
			ParryStance = 9,
			WallScramble = 10,
			Harpoon = 11
		}

		[ObjectType(typeof(HeroBoxState))]
		public FsmEnum heroBoxState;

		private HeroBox heroBox;

		protected virtual bool IsEveryFrame => false;

		public override void Reset()
		{
			heroBoxState = null;
		}

		public override void OnEnter()
		{
			bool flag = heroBox;
			if (!flag)
			{
				HeroController instance = HeroController.instance;
				if ((bool)instance)
				{
					heroBox = instance.heroBox;
					flag = heroBox;
				}
			}
			if (flag)
			{
				HeroBoxState boxState = (HeroBoxState)(object)heroBoxState.Value;
				SetBoxState(boxState);
			}
			else
			{
				Debug.LogError("Failed to find hero box.");
			}
			if (!IsEveryFrame)
			{
				Finish();
			}
		}

		protected void SetBoxState(HeroBoxState boxState)
		{
			switch (boxState)
			{
			case HeroBoxState.Off:
				heroBox.HeroBoxOff();
				break;
			case HeroBoxState.Normal:
				heroBox.HeroBoxNormal();
				break;
			case HeroBoxState.Downspike:
				heroBox.HeroBoxDownspike();
				break;
			case HeroBoxState.DownDrill:
				heroBox.HeroBoxDownDrill();
				break;
			case HeroBoxState.Scuttle:
				heroBox.HeroBoxScuttle();
				break;
			case HeroBoxState.Sprint:
				heroBox.HeroBoxSprint();
				break;
			case HeroBoxState.Airdash:
				heroBox.HeroBoxAirdash();
				break;
			case HeroBoxState.ReaperDownSlash:
				heroBox.HeroBoxReaperDownSlash();
				break;
			case HeroBoxState.WallSlide:
				heroBox.HeroBoxWallSlide();
				break;
			case HeroBoxState.ParryStance:
				heroBox.HeroBoxParryStance();
				break;
			case HeroBoxState.WallScramble:
				heroBox.HeroBoxWallScramble();
				break;
			case HeroBoxState.Harpoon:
				heroBox.HeroBoxHarpoon();
				break;
			default:
				heroBox.HeroBoxNormal();
				Debug.LogError($"Encountered unsupported box state {boxState}. Setting to normal.");
				break;
			}
		}
	}
}
