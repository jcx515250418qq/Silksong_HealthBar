using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.AnimateVariables)]
	public class AnimateRotationTo : EaseFsmAction
	{
		public FsmOwnerDefault gameObject;

		[RequiredField]
		public FsmFloat fromValue;

		[RequiredField]
		public FsmFloat toValue;

		public bool worldSpace;

		public bool negativeSpace;

		private Transform objectTransform;

		private bool finishInNextStep;

		public override void Reset()
		{
			base.Reset();
			fromValue = null;
			toValue = null;
			finishInNextStep = false;
		}

		public override void OnEnter()
		{
			base.OnEnter();
			objectTransform = base.Fsm.GetOwnerDefaultTarget(gameObject).transform;
			if (objectTransform == null)
			{
				return;
			}
			if (fromValue.IsNone)
			{
				fromValue.Value = objectTransform.localEulerAngles.z;
				if (negativeSpace)
				{
					while (fromValue.Value > 0f)
					{
						fromValue.Value -= 360f;
					}
				}
			}
			fromFloats = new float[1];
			fromFloats[0] = fromValue.Value;
			toFloats = new float[1];
			toFloats[0] = toValue.Value;
			resultFloats = new float[1];
			finishInNextStep = false;
		}

		public override void OnExit()
		{
			base.OnExit();
		}

		public override void OnUpdate()
		{
			base.OnUpdate();
			if (isRunning)
			{
				if (worldSpace)
				{
					objectTransform.eulerAngles = new Vector3(0f, 0f, resultFloats[0]);
				}
				else
				{
					objectTransform.localEulerAngles = new Vector3(0f, 0f, resultFloats[0]);
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
				if (worldSpace)
				{
					objectTransform.eulerAngles = new Vector3(0f, 0f, reverse.IsNone ? toValue.Value : (reverse.Value ? fromValue.Value : toValue.Value));
				}
				else
				{
					objectTransform.localEulerAngles = new Vector3(0f, 0f, reverse.IsNone ? toValue.Value : (reverse.Value ? fromValue.Value : toValue.Value));
				}
				finishInNextStep = true;
			}
		}
	}
}
