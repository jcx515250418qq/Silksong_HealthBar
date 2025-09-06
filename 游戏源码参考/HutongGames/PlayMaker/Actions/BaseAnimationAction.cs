using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	public abstract class BaseAnimationAction : ComponentAction<Animation>
	{
		public override void OnActionTargetInvoked(object targetObject)
		{
			AnimationClip animationClip = targetObject as AnimationClip;
			if (!(animationClip == null))
			{
				Animation component = base.Owner.GetComponent<Animation>();
				if (component != null)
				{
					component.AddClip(animationClip, animationClip.name);
				}
			}
		}
	}
}
