using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Vector3)]
	[Tooltip("Sets the XYZ channels of a Vector3 Variable. To leave any channel unchanged, set variable to 'None'.")]
	public class SetVector3XYZ : FsmStateAction
	{
		[RequiredField]
		[UIHint(UIHint.Variable)]
		[Tooltip("Vector3 variable to set.")]
		public FsmVector3 vector3Variable;

		[Tooltip("Set using another Vector3 variable and/or individual channels below.")]
		[UIHint(UIHint.Variable)]
		public FsmVector3 vector3Value;

		[Tooltip("Set X channel.")]
		public FsmFloat x;

		[Tooltip("Set Y channel.")]
		public FsmFloat y;

		[Tooltip("Set Z channel.")]
		public FsmFloat z;

		[Tooltip("Repeat every frame.")]
		public bool everyFrame;

		public override void Reset()
		{
			vector3Variable = null;
			vector3Value = null;
			x = new FsmFloat
			{
				UseVariable = true
			};
			y = new FsmFloat
			{
				UseVariable = true
			};
			z = new FsmFloat
			{
				UseVariable = true
			};
			everyFrame = false;
		}

		public override void OnEnter()
		{
			DoSetVector3XYZ();
			if (!everyFrame)
			{
				Finish();
			}
		}

		public override void OnUpdate()
		{
			DoSetVector3XYZ();
		}

		private void DoSetVector3XYZ()
		{
			if (vector3Variable != null)
			{
				Vector3 value = vector3Variable.Value;
				if (!vector3Value.IsNone)
				{
					value = vector3Value.Value;
				}
				if (!x.IsNone)
				{
					value.x = x.Value;
				}
				if (!y.IsNone)
				{
					value.y = y.Value;
				}
				if (!z.IsNone)
				{
					value.z = z.Value;
				}
				vector3Variable.Value = value;
			}
		}
	}
}
