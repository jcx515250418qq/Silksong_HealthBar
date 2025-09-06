using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("iTween")]
	[Tooltip("Pause an iTween action.")]
	public class iTweenPause : FsmStateAction
	{
		[RequiredField]
		public FsmOwnerDefault gameObject;

		public iTweenFSMType iTweenType;

		public bool includeChildren;

		public bool inScene;

		public override void Reset()
		{
			iTweenType = iTweenFSMType.all;
			includeChildren = false;
			inScene = false;
		}

		public override void OnEnter()
		{
			base.OnEnter();
			DoiTween();
			Finish();
		}

		private void DoiTween()
		{
			if (iTweenType == iTweenFSMType.all)
			{
				iTween.Pause();
				return;
			}
			if (inScene)
			{
				iTween.Pause(Enum.GetName(typeof(iTweenFSMType), iTweenType));
				return;
			}
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(gameObject);
			if (!(ownerDefaultTarget == null))
			{
				iTween.Pause(ownerDefaultTarget, Enum.GetName(typeof(iTweenFSMType), iTweenType), includeChildren);
			}
		}
	}
}
