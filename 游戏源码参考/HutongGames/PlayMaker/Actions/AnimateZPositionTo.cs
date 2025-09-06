using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.AnimateVariables)]
	public class AnimateZPositionTo : EaseFsmAction
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
			tf = base.Fsm.GetOwnerDefaultTarget(GameObject).transform;
			if (!(tf == null))
			{
				if (!localSpace)
				{
					fromValue = tf.position.z;
				}
				else
				{
					fromValue = tf.localPosition.z;
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
					tf.position = new Vector3(tf.position.x, tf.position.y, resultFloats[0]);
				}
				else
				{
					tf.localPosition = new Vector3(tf.localPosition.x, tf.localPosition.y, resultFloats[0]);
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
					tf.position = new Vector3(tf.position.x, tf.position.y, reverse.IsNone ? ToValue.Value : (reverse.Value ? fromValue : ToValue.Value));
				}
				else
				{
					tf.localPosition = new Vector3(tf.localPosition.x, tf.localPosition.y, reverse.IsNone ? ToValue.Value : (reverse.Value ? fromValue : ToValue.Value));
				}
				finishInNextStep = true;
			}
		}
	}
}
