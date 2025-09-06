using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Transform)]
	public class CheckOutOfCamera : FsmStateAction
	{
		[RequiredField]
		public FsmOwnerDefault gameObject;

		public FsmFloat margin;

		public FsmEvent outsideEvent;

		public FsmEvent insideEvent;

		public FsmBool insideBool;

		public FsmBool outsideBool;

		public bool everyFrame;

		private Transform targetTransform;

		private Transform camTransform;

		public override void Reset()
		{
			gameObject = null;
			margin = 6f;
			outsideEvent = null;
			insideEvent = null;
			insideBool = null;
			outsideBool = null;
			everyFrame = false;
		}

		public override void OnEnter()
		{
			targetTransform = base.Fsm.GetOwnerDefaultTarget(gameObject).transform;
			camTransform = GameCameras.instance.mainCamera.gameObject.transform;
			DoCheck();
			if (!everyFrame)
			{
				Finish();
			}
		}

		public override void OnUpdate()
		{
			DoCheck();
		}

		private void DoCheck()
		{
			float num = targetTransform.position.x - camTransform.transform.position.x;
			float num2 = targetTransform.position.y - camTransform.transform.position.y;
			if (num < 0f)
			{
				num *= -1f;
			}
			if (num2 < 0f)
			{
				num2 *= -1f;
			}
			if (num > 15f + margin.Value || num2 > 9f + margin.Value)
			{
				if (outsideEvent != null)
				{
					base.Fsm.Event(outsideEvent);
				}
				outsideBool.Value = true;
				insideBool.Value = false;
			}
			else
			{
				if (insideEvent != null)
				{
					base.Fsm.Event(insideEvent);
				}
				outsideBool.Value = false;
				insideBool.Value = true;
			}
		}
	}
}
