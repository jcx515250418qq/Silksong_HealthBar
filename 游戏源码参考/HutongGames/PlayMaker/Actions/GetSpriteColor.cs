using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.SpriteRenderer)]
	[Tooltip("Gets the color of a sprite renderer")]
	public class GetSpriteColor : ComponentAction<SpriteRenderer>
	{
		[RequiredField]
		[CheckForComponent(typeof(SpriteRenderer))]
		[Tooltip("The GameObject with the SpriteRenderer component.")]
		public FsmOwnerDefault gameObject;

		[UIHint(UIHint.Variable)]
		[Tooltip("Get The Color of the SpriteRenderer component")]
		public FsmColor color;

		[UIHint(UIHint.Variable)]
		[Tooltip("Store the red channel in a float variable.")]
		public FsmFloat red;

		[UIHint(UIHint.Variable)]
		[Tooltip("Store the green channel in a float variable.")]
		public FsmFloat green;

		[UIHint(UIHint.Variable)]
		[Tooltip("Store the blue channel in a float variable.")]
		public FsmFloat blue;

		[UIHint(UIHint.Variable)]
		[Tooltip("Store the alpha channel in a float variable.")]
		public FsmFloat alpha;

		[Tooltip("Repeat every frame. Useful if the color variable is changing.")]
		public bool everyFrame;

		public override void Reset()
		{
			gameObject = null;
			color = null;
			red = new FsmFloat
			{
				UseVariable = true
			};
			green = new FsmFloat
			{
				UseVariable = true
			};
			blue = new FsmFloat
			{
				UseVariable = true
			};
			alpha = new FsmFloat
			{
				UseVariable = true
			};
			everyFrame = false;
		}

		public override void OnEnter()
		{
			GetColor();
			if (!everyFrame)
			{
				Finish();
			}
		}

		public override void OnUpdate()
		{
			GetColor();
		}

		private void GetColor()
		{
			if (UpdateCache(base.Fsm.GetOwnerDefaultTarget(gameObject)))
			{
				if (!color.IsNone)
				{
					color.Value = cachedComponent.color;
				}
				if (!red.IsNone)
				{
					red.Value = cachedComponent.color.r;
				}
				if (!green.IsNone)
				{
					green.Value = cachedComponent.color.g;
				}
				if (!blue.IsNone)
				{
					blue.Value = cachedComponent.color.b;
				}
				if (!alpha.IsNone)
				{
					alpha.Value = cachedComponent.color.a;
				}
			}
		}
	}
}
