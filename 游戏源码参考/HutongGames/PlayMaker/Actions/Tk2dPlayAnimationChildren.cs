using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("2D Toolkit/SpriteAnimator")]
	[Tooltip("Plays a sprite animation. \nNOTE: The Game Object must have a tk2dSpriteAnimator attached.")]
	public class Tk2dPlayAnimationChildren : FsmStateAction
	{
		[RequiredField]
		public FsmOwnerDefault gameObject;

		[RequiredField]
		[Tooltip("The clip name to play")]
		public FsmString clipName;

		public override void Reset()
		{
			gameObject = null;
			clipName = null;
		}

		public override void OnEnter()
		{
			if (gameObject != null)
			{
				GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(gameObject);
				if (ownerDefaultTarget != null)
				{
					foreach (Transform item in ownerDefaultTarget.transform)
					{
						tk2dSpriteAnimator component = item.GetComponent<tk2dSpriteAnimator>();
						if ((bool)component)
						{
							component.Play(clipName.Value);
						}
					}
				}
			}
			Finish();
		}
	}
}
