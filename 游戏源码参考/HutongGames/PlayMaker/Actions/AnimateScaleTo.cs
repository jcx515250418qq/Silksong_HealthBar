using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.AnimateVariables)]
	public class AnimateScaleTo : EaseFsmAction
	{
		public FsmOwnerDefault Target;

		[RequiredField]
		public FsmVector3 ToLocalScale;

		private bool finishInNextStep;

		private Transform objectTransform;

		private Vector3 fromValue;

		public override void Reset()
		{
			base.Reset();
			Target = null;
			fromValue = new Vector3(0f, 0f, 0f);
			ToLocalScale = null;
			finishInNextStep = false;
		}

		public override void OnEnter()
		{
			base.OnEnter();
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(Target);
			if (!ownerDefaultTarget)
			{
				Finish();
				return;
			}
			objectTransform = ownerDefaultTarget.transform;
			fromValue = objectTransform.localScale;
			fromFloats = new float[3];
			fromFloats[0] = fromValue.x;
			fromFloats[1] = fromValue.y;
			fromFloats[2] = fromValue.z;
			toFloats = new float[3];
			toFloats[0] = ToLocalScale.Value.x;
			toFloats[1] = ToLocalScale.Value.y;
			toFloats[2] = ToLocalScale.Value.z;
			resultFloats = new float[3];
			finishInNextStep = false;
		}

		public override void OnUpdate()
		{
			base.OnUpdate();
			if (isRunning)
			{
				objectTransform.localScale = new Vector3(resultFloats[0], resultFloats[1], resultFloats[2]);
			}
			if (finishInNextStep)
			{
				Finish();
				if (finishEvent != null)
				{
					base.Fsm.Event(finishEvent);
				}
			}
			if (finishAction && !finishInNextStep)
			{
				objectTransform.localScale = new Vector3(reverse.IsNone ? ToLocalScale.Value.x : (reverse.Value ? fromValue.x : ToLocalScale.Value.x), reverse.IsNone ? ToLocalScale.Value.y : (reverse.Value ? fromValue.y : ToLocalScale.Value.y), reverse.IsNone ? ToLocalScale.Value.z : (reverse.Value ? fromValue.z : ToLocalScale.Value.z));
				finishInNextStep = true;
			}
		}
	}
}
