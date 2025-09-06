using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.AnimateVariables)]
	public class AnimatePositionTo : EaseFsmAction
	{
		public FsmOwnerDefault gameObject;

		[RequiredField]
		public FsmVector3 toValue;

		public bool localSpace;

		private bool finishInNextStep;

		private Transform objectTransform;

		private Vector3 fromValue;

		public override void Reset()
		{
			base.Reset();
			fromValue = new Vector3(0f, 0f, 0f);
			toValue = null;
			finishInNextStep = false;
			localSpace = false;
		}

		public override void OnEnter()
		{
			base.OnEnter();
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(gameObject);
			if (!ownerDefaultTarget)
			{
				Finish();
				return;
			}
			objectTransform = ownerDefaultTarget.transform;
			if (localSpace)
			{
				fromValue = objectTransform.localPosition;
			}
			else
			{
				fromValue = objectTransform.position;
			}
			fromFloats = new float[3];
			fromFloats[0] = fromValue.x;
			fromFloats[1] = fromValue.y;
			fromFloats[2] = fromValue.z;
			toFloats = new float[3];
			toFloats[0] = toValue.Value.x;
			toFloats[1] = toValue.Value.y;
			toFloats[2] = toValue.Value.z;
			resultFloats = new float[3];
			finishInNextStep = false;
		}

		public override void OnUpdate()
		{
			base.OnUpdate();
			if (isRunning)
			{
				if (localSpace)
				{
					objectTransform.localPosition = new Vector3(resultFloats[0], resultFloats[1], resultFloats[2]);
				}
				else
				{
					objectTransform.position = new Vector3(resultFloats[0], resultFloats[1], resultFloats[2]);
				}
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
				if (localSpace)
				{
					objectTransform.localPosition = new Vector3(reverse.IsNone ? toValue.Value.x : (reverse.Value ? fromValue.x : toValue.Value.x), reverse.IsNone ? toValue.Value.y : (reverse.Value ? fromValue.y : toValue.Value.y), reverse.IsNone ? toValue.Value.z : (reverse.Value ? fromValue.z : toValue.Value.z));
				}
				else
				{
					objectTransform.position = new Vector3(reverse.IsNone ? toValue.Value.x : (reverse.Value ? fromValue.x : toValue.Value.x), reverse.IsNone ? toValue.Value.y : (reverse.Value ? fromValue.y : toValue.Value.y), reverse.IsNone ? toValue.Value.z : (reverse.Value ? fromValue.z : toValue.Value.z));
				}
				finishInNextStep = true;
			}
		}
	}
}
