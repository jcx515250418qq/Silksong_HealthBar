using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.AnimateVariables)]
	public class AnimatePositionBy : EaseFsmAction
	{
		public FsmOwnerDefault gameObject;

		[RequiredField]
		public FsmVector3 shiftBy;

		private Vector3 fromValue;

		private Vector3 toValue;

		private bool finishInNextStep;

		private Transform objectTransform;

		private bool gotFromValue;

		public override void Reset()
		{
			base.Reset();
			shiftBy = null;
			finishInNextStep = false;
			gotFromValue = false;
		}

		public override void OnEnter()
		{
			base.OnEnter();
			if (shiftBy.Value.magnitude <= Mathf.Epsilon)
			{
				Finish();
				return;
			}
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(gameObject);
			if (!ownerDefaultTarget)
			{
				Finish();
				return;
			}
			objectTransform = ownerDefaultTarget.transform;
			if (!objectTransform)
			{
				Finish();
			}
			else if (delay.Value == 0f || delay.IsNone)
			{
				fromValue = objectTransform.position;
				toValue = objectTransform.position + shiftBy.Value;
				fromFloats = new float[3];
				fromFloats[0] = fromValue.x;
				fromFloats[1] = fromValue.y;
				fromFloats[2] = fromValue.z;
				toFloats = new float[3];
				toFloats[0] = toValue.x;
				toFloats[1] = toValue.y;
				toFloats[2] = toValue.z;
				resultFloats = new float[3];
				finishInNextStep = false;
			}
		}

		public override void OnUpdate()
		{
			if (isRunning)
			{
				if (!gotFromValue)
				{
					fromValue = objectTransform.position;
					toValue = objectTransform.position + shiftBy.Value;
					fromFloats = new float[3];
					fromFloats[0] = fromValue.x;
					fromFloats[1] = fromValue.y;
					fromFloats[2] = fromValue.z;
					toFloats = new float[3];
					toFloats[0] = toValue.x;
					toFloats[1] = toValue.y;
					toFloats[2] = toValue.z;
					resultFloats = new float[3];
					finishInNextStep = false;
					gotFromValue = true;
				}
				if (resultFloats[0] != 0f || resultFloats[1] != 0f || resultFloats[2] != 0f)
				{
					objectTransform.position = new Vector3(resultFloats[0], resultFloats[1], resultFloats[2]);
				}
			}
			if (finishInNextStep)
			{
				finished = true;
				Finish();
				if (finishEvent != null)
				{
					base.Fsm.Event(finishEvent);
				}
			}
			if (finishAction && !finishInNextStep)
			{
				objectTransform.position = new Vector3(reverse.IsNone ? toValue.x : (reverse.Value ? fromValue.x : toValue.x), reverse.IsNone ? toValue.y : (reverse.Value ? fromValue.y : toValue.y), reverse.IsNone ? toValue.z : (reverse.Value ? fromValue.z : toValue.z));
				finishInNextStep = true;
			}
			base.OnUpdate();
		}
	}
}
