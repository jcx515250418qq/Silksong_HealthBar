using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("Dialogue")]
	public class EndDialogue : FsmStateAction
	{
		public FsmBool ReturnControl;

		public FsmBool ReturnHUD;

		public FsmOwnerDefault Target;

		public FsmBool UseChildren;

		private int runningDialogues;

		public override void Reset()
		{
			ReturnControl = new FsmBool(true);
			ReturnHUD = new FsmBool(true);
			Target = null;
			UseChildren = null;
		}

		public override void OnEnter()
		{
			runningDialogues = 0;
			GameObject safe = Target.GetSafe(this);
			if ((bool)safe)
			{
				if (!UseChildren.Value)
				{
					PlayMakerNPC component = safe.GetComponent<PlayMakerNPC>();
					if (component != null)
					{
						component.CloseDialogueBox(ReturnControl.Value, ReturnHUD.Value, base.Finish);
					}
					else
					{
						Finish();
					}
					return;
				}
				PlayMakerNPC[] componentsInChildren = safe.GetComponentsInChildren<PlayMakerNPC>();
				if (componentsInChildren.Length == 0)
				{
					Finish();
					return;
				}
				PlayMakerNPC[] array = componentsInChildren;
				foreach (PlayMakerNPC playMakerNPC in array)
				{
					if (playMakerNPC.IsRunningDialogue)
					{
						runningDialogues++;
						playMakerNPC.CloseDialogueBox(ReturnControl.Value, ReturnHUD.Value, EndChild);
					}
				}
				if (runningDialogues <= 0)
				{
					Finish();
				}
			}
			else
			{
				Finish();
			}
		}

		private void EndChild()
		{
			runningDialogues--;
			if (runningDialogues <= 0)
			{
				Finish();
			}
		}
	}
}
