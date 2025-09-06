using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("2D Toolkit/SpriteAnimator")]
	[Tooltip("Plays a sprite animation. \nNOTE: The Game Object must have a tk2dSpriteAnimator attached.")]
	public class Tk2dPlayAnimationChildrenRandom : FsmStateAction
	{
		[RequiredField]
		public FsmOwnerDefault gameObject;

		[RequiredField]
		[Tooltip("The clip name to play")]
		public string[] clipNames;

		public override void Reset()
		{
			gameObject = null;
			clipNames = null;
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
							string text = clipNames[Random.Range(0, clipNames.Length)];
							component.Play(text);
						}
					}
				}
			}
			Finish();
		}
	}
}
