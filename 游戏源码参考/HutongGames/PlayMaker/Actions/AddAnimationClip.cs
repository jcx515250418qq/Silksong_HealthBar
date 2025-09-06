using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Animation)]
	[Tooltip("Adds an Animation Clip to a Game Object. Optionally trim the clip.")]
	public class AddAnimationClip : FsmStateAction
	{
		[RequiredField]
		[CheckForComponent(typeof(Animation))]
		[Tooltip("The Game Object to add the Animation Clip to.")]
		public FsmOwnerDefault gameObject;

		[RequiredField]
		[ObjectType(typeof(AnimationClip))]
		[Tooltip("The animation clip to add. NOTE: Make sure the clip is compatible with the object's hierarchy.")]
		public FsmObject animationClip;

		[RequiredField]
		[Tooltip("Optionally give the animation a new name. Can be used by other Animation actions.")]
		public FsmString animationName;

		[ActionSection("Trimming")]
		[Tooltip("Optionally trim the animation by specifying a first and last frame.")]
		public FsmInt firstFrame;

		[Tooltip("Set the last frame of the trimmed animation. 0 means no trimming.")]
		public FsmInt lastFrame;

		[Tooltip("Add a frame at the end of the trimmed clip that's the same as the first frame so it loops nicely.")]
		public FsmBool addLoopFrame;

		public override void Reset()
		{
			gameObject = null;
			animationClip = null;
			animationName = "";
			firstFrame = 0;
			lastFrame = 0;
			addLoopFrame = false;
		}

		public override void OnEnter()
		{
			DoAddAnimationClip();
			Finish();
		}

		private void DoAddAnimationClip()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(gameObject);
			if (ownerDefaultTarget == null)
			{
				return;
			}
			AnimationClip animationClip = this.animationClip.Value as AnimationClip;
			if (!(animationClip == null))
			{
				Animation component = ownerDefaultTarget.GetComponent<Animation>();
				if (firstFrame.Value == 0 && lastFrame.Value == 0)
				{
					component.AddClip(animationClip, animationName.Value);
				}
				else
				{
					component.AddClip(animationClip, animationName.Value, firstFrame.Value, lastFrame.Value, addLoopFrame.Value);
				}
			}
		}
	}
}
