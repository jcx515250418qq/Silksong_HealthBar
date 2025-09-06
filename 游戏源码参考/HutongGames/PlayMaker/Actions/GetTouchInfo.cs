using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Device)]
	[Tooltip("Gets info on a touch event.")]
	public class GetTouchInfo : FsmStateAction
	{
		[Tooltip("Filter by a Finger ID. You can store a Finger ID in other Touch actions, e.g., Touch Event.")]
		public FsmInt fingerId;

		[Tooltip("If true, all screen coordinates are returned normalized (0-1), otherwise in pixels.")]
		public FsmBool normalize;

		[UIHint(UIHint.Variable)]
		[Tooltip("Store the position of the touch in a Vector3 Variable.")]
		public FsmVector3 storePosition;

		[UIHint(UIHint.Variable)]
		[Tooltip("Store the X position \u00a0in a Float Variable.")]
		public FsmFloat storeX;

		[UIHint(UIHint.Variable)]
		[Tooltip("Store the Y position \u00a0in a Float Variable.")]
		public FsmFloat storeY;

		[UIHint(UIHint.Variable)]
		[Tooltip("Store the movement of the touch in a Vector3 Variable.")]
		public FsmVector3 storeDeltaPosition;

		[UIHint(UIHint.Variable)]
		[Tooltip("Store the X movement in a Float Variable.")]
		public FsmFloat storeDeltaX;

		[UIHint(UIHint.Variable)]
		[Tooltip("Store the Y movement in a Float Variable.")]
		public FsmFloat storeDeltaY;

		[UIHint(UIHint.Variable)]
		[Tooltip("Store the time between touch events in a Float Variable.")]
		public FsmFloat storeDeltaTime;

		[UIHint(UIHint.Variable)]
		[Tooltip("Store the number of tap count of the touch (e.g. 2 = double tap).")]
		public FsmInt storeTapCount;

		[Tooltip("Repeat every frame.")]
		public bool everyFrame = true;

		private float screenWidth;

		private float screenHeight;

		public override void Reset()
		{
			fingerId = new FsmInt
			{
				UseVariable = true
			};
			normalize = true;
			storePosition = null;
			storeDeltaPosition = null;
			storeDeltaTime = null;
			storeTapCount = null;
			everyFrame = true;
		}

		public override void OnEnter()
		{
			screenWidth = Screen.width;
			screenHeight = Screen.height;
			DoGetTouchInfo();
			if (!everyFrame)
			{
				Finish();
			}
		}

		public override void OnUpdate()
		{
			DoGetTouchInfo();
		}

		private void DoGetTouchInfo()
		{
			if (Input.touchCount <= 0)
			{
				return;
			}
			Touch[] touches = Input.touches;
			for (int i = 0; i < touches.Length; i++)
			{
				Touch touch = touches[i];
				if (fingerId.IsNone || touch.fingerId == fingerId.Value)
				{
					float num = ((!normalize.Value) ? touch.position.x : (touch.position.x / screenWidth));
					float num2 = ((!normalize.Value) ? touch.position.y : (touch.position.y / screenHeight));
					if (!storePosition.IsNone)
					{
						storePosition.Value = new Vector3(num, num2, 0f);
					}
					storeX.Value = num;
					storeY.Value = num2;
					float num3 = ((!normalize.Value) ? touch.deltaPosition.x : (touch.deltaPosition.x / screenWidth));
					float num4 = ((!normalize.Value) ? touch.deltaPosition.y : (touch.deltaPosition.y / screenHeight));
					if (!storeDeltaPosition.IsNone)
					{
						storeDeltaPosition.Value = new Vector3(num3, num4, 0f);
					}
					storeDeltaX.Value = num3;
					storeDeltaY.Value = num4;
					storeDeltaTime.Value = touch.deltaTime;
					storeTapCount.Value = touch.tapCount;
				}
			}
		}
	}
}
