using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	public class SetNpcControlTalkAnim : FsmStateAction
	{
		[RequiredField]
		[CheckForComponent(typeof(NPCControlBase))]
		public FsmOwnerDefault Target;

		[ObjectType(typeof(HeroTalkAnimation.AnimationTypes))]
		public FsmEnum Anim;

		public override void Reset()
		{
			Target = null;
			Anim = null;
		}

		public override void OnEnter()
		{
			GameObject safe = Target.GetSafe(this);
			if ((bool)safe)
			{
				NPCControlBase component = safe.GetComponent<NPCControlBase>();
				if ((bool)component)
				{
					component.HeroAnimation = (HeroTalkAnimation.AnimationTypes)(object)Anim.Value;
				}
			}
			Finish();
		}
	}
}
