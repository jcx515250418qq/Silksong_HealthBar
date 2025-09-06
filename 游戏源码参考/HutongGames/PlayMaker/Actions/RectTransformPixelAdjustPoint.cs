using UnityEngine;
using UnityEngine.UI;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("RectTransform")]
	[Tooltip("Convert a given point in screen space into a pixel correct point.")]
	public class RectTransformPixelAdjustPoint : BaseUpdateAction
	{
		[RequiredField]
		[CheckForComponent(typeof(RectTransform))]
		[Tooltip("The GameObject target.")]
		public FsmOwnerDefault gameObject;

		[RequiredField]
		[CheckForComponent(typeof(Canvas))]
		[Tooltip("The canvas. Leave to none to use the canvas of the gameObject")]
		public FsmGameObject canvas;

		[Tooltip("The screen position.")]
		public FsmVector2 screenPoint;

		[ActionSection("Result")]
		[RequiredField]
		[Tooltip("Pixel adjusted point from the screen position.")]
		[UIHint(UIHint.Variable)]
		public FsmVector2 pixelPoint;

		private RectTransform _rt;

		private Canvas _canvas;

		public override void Reset()
		{
			base.Reset();
			gameObject = null;
			canvas = new FsmGameObject
			{
				UseVariable = true
			};
			screenPoint = null;
			pixelPoint = null;
		}

		public override void OnEnter()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(gameObject);
			if (ownerDefaultTarget != null)
			{
				_rt = ownerDefaultTarget.GetComponent<RectTransform>();
			}
			GameObject value = canvas.Value;
			if (value != null)
			{
				_canvas = value.GetComponent<Canvas>();
			}
			if (_canvas == null && ownerDefaultTarget != null)
			{
				Graphic component = ownerDefaultTarget.GetComponent<Graphic>();
				if (component != null)
				{
					_canvas = component.canvas;
				}
			}
			DoAction();
			if (!everyFrame)
			{
				Finish();
			}
		}

		public override void OnActionUpdate()
		{
			DoAction();
		}

		private void DoAction()
		{
			pixelPoint.Value = RectTransformUtility.PixelAdjustPoint(screenPoint.Value, _rt, _canvas);
		}
	}
}
