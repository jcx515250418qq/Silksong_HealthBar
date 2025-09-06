using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.AnimateVariables)]
	public class AnimateYPositionTo : EaseFsmAction
	{
		[RequiredField]
		public FsmOwnerDefault GameObject;

		[RequiredField]
		public FsmFloat ToValue;

		public bool localSpace;

		private bool finishInNextStep;

		private Transform tf;

		private float fromValue;

		public override void Reset()
		{
			base.Reset();
			fromValue = 0f;
			ToValue = null;
			finishInNextStep = false;
		}

		public override void OnEnter()
		{
			base.OnEnter();
			if (base.Fsm.GetOwnerDefaultTarget(GameObject) == null)
			{
				return;
			}
			tf = base.Fsm.GetOwnerDefaultTarget(GameObject).transform;
			if (!(tf == null))
			{
				if (!localSpace)
				{
					fromValue = tf.position.y;
				}
				else
				{
					fromValue = tf.localPosition.y;
				}
				fromFloats = new float[1];
				fromFloats[0] = fromValue;
				toFloats = new float[1];
				toFloats[0] = ToValue.Value;
				resultFloats = new float[1];
				finishInNextStep = false;
			}
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
				if (!localSpace)
				{
					tf.position = new Vector3(tf.position.x, resultFloats[0], tf.position.z);
				}
				else
				{
					tf.localPosition = new Vector3(tf.localPosition.x, resultFloats[0], tf.localPosition.z);
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
				if (!localSpace)
				{
					tf.position = new Vector3(tf.position.x, reverse.IsNone ? ToValue.Value : (reverse.Value ? fromValue : ToValue.Value), tf.position.z);
				}
				else
				{
					tf.localPosition = new Vector3(tf.localPosition.x, reverse.IsNone ? ToValue.Value : (reverse.Value ? fromValue : ToValue.Value), tf.localPosition.z);
				}
				finishInNextStep = true;
			}
			if (tf == null)
			{
				Finish();
			}
		}
	}
}
