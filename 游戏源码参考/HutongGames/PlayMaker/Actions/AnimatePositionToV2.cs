using JetBrains.Annotations;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.AnimateVariables)]
	public class AnimatePositionToV2 : EaseFsmAction
	{
		public FsmOwnerDefault GameObject;

		public FsmVector3 ToValue;

		[HideIf("HasVectorValue")]
		public FsmFloat ToX;

		[HideIf("HasVectorValue")]
		public FsmFloat ToY;

		[HideIf("HasVectorValue")]
		public FsmFloat ToZ;

		public bool LocalSpace;

		private bool finishInNextStep;

		private Transform objectTransform;

		private Vector3 fromValue;

		private Vector3 toValue;

		[UsedImplicitly]
		public bool HasVectorValue()
		{
			return !ToValue.IsNone;
		}

		public override void Reset()
		{
			base.Reset();
			GameObject = null;
			ToValue = new FsmVector3
			{
				UseVariable = true
			};
			ToX = null;
			ToY = null;
			ToZ = null;
			LocalSpace = false;
		}

		public override void OnEnter()
		{
			base.OnEnter();
			objectTransform = base.Fsm.GetOwnerDefaultTarget(GameObject).transform;
			if (!(objectTransform == null))
			{
				fromValue = (LocalSpace ? objectTransform.localPosition : objectTransform.position);
				toValue = ((!ToValue.IsNone) ? ToValue.Value : new Vector3(ToX.Value, ToY.Value, ToZ.Value));
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
			base.OnUpdate();
			if (isRunning)
			{
				if (LocalSpace)
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
				if (LocalSpace)
				{
					objectTransform.localPosition = new Vector3(reverse.IsNone ? toValue.x : (reverse.Value ? fromValue.x : toValue.x), reverse.IsNone ? toValue.y : (reverse.Value ? fromValue.y : toValue.y), reverse.IsNone ? toValue.z : (reverse.Value ? fromValue.z : toValue.z));
				}
				else
				{
					objectTransform.position = new Vector3(reverse.IsNone ? toValue.x : (reverse.Value ? fromValue.x : toValue.x), reverse.IsNone ? toValue.y : (reverse.Value ? fromValue.y : toValue.y), reverse.IsNone ? toValue.z : (reverse.Value ? fromValue.z : toValue.z));
				}
				finishInNextStep = true;
			}
		}
	}
}
