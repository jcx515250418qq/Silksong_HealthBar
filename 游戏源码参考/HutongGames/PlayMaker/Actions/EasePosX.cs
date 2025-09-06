using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.AnimateVariables)]
	[Tooltip("Easing Animation - Float")]
	public class EasePosX : EaseFsmAction
	{
		[RequiredField]
		public FsmOwnerDefault GameObject;

		private float fromValue;

		[RequiredField]
		public FsmFloat toValue;

		public bool localSpace;

		private FsmFloat floatVariable;

		private bool finishInNextStep;

		private Transform tf;

		public override void Reset()
		{
			base.Reset();
			floatVariable = null;
			toValue = null;
			finishInNextStep = false;
		}

		public override void OnEnter()
		{
			base.OnEnter();
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(GameObject);
			if (!(ownerDefaultTarget == null))
			{
				tf = ownerDefaultTarget.transform;
				fromValue = tf.position.x;
				fromFloats = new float[1];
				fromFloats[0] = fromValue;
				toFloats = new float[1];
				toFloats[0] = toValue.Value;
				resultFloats = new float[1];
				finishInNextStep = false;
				floatVariable = fromValue;
			}
		}

		public override void OnExit()
		{
			base.OnExit();
		}

		public override void OnUpdate()
		{
			base.OnUpdate();
			if (!floatVariable.IsNone && isRunning)
			{
				floatVariable.Value = resultFloats[0];
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
				if (!floatVariable.IsNone)
				{
					floatVariable.Value = (reverse.IsNone ? toValue.Value : (reverse.Value ? fromValue : toValue.Value));
				}
				finishInNextStep = true;
			}
			if (localSpace)
			{
				tf.localPosition = new Vector3(floatVariable.Value, tf.localPosition.y, tf.localPosition.z);
			}
			else
			{
				tf.position = new Vector3(floatVariable.Value, tf.position.y, tf.position.z);
			}
		}
	}
}
