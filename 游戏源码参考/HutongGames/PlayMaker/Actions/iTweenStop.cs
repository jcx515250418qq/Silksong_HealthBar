using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("iTween")]
	[Tooltip("Stop an iTween action.")]
	public class iTweenStop : FsmStateAction
	{
		[RequiredField]
		public FsmOwnerDefault gameObject;

		public FsmString id;

		public iTweenFSMType iTweenType;

		public bool includeChildren;

		public bool inScene;

		public override void Reset()
		{
			id = new FsmString
			{
				UseVariable = true
			};
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
			if (id.IsNone)
			{
				if (iTweenType == iTweenFSMType.all)
				{
					iTween.Stop();
					return;
				}
				if (inScene)
				{
					iTween.Stop(Enum.GetName(typeof(iTweenFSMType), iTweenType));
					return;
				}
				GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(gameObject);
				if (!(ownerDefaultTarget == null))
				{
					iTween.Stop(ownerDefaultTarget, Enum.GetName(typeof(iTweenFSMType), iTweenType), includeChildren);
				}
			}
			else
			{
				iTween.StopByName(id.Value);
			}
		}
	}
}
