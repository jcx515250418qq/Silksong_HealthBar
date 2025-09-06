using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.UnityObject)]
	[ActionTarget(typeof(Component), "targetProperty", false)]
	[ActionTarget(typeof(GameObject), "targetProperty", false)]
	[Tooltip("Sets the value of any public property or field on the targeted Unity Object. E.g., Drag and drop any component attached to a Game Object to access its properties.")]
	public class SetPropertyV2 : FsmStateAction
	{
		[Tooltip("Target Property. See below for more details.")]
		public FsmProperty TargetProperty;

		public FsmBool EveryFrame;

		public FsmBool ResetOnExit;

		private object originalValue;

		public override void Reset()
		{
			TargetProperty = new FsmProperty
			{
				setProperty = true
			};
			EveryFrame = false;
			ResetOnExit = false;
		}

		public override void OnEnter()
		{
			if (ResetOnExit.Value)
			{
				object rawValue = TargetProperty.GetVariable().RawValue;
				TargetProperty.GetValue();
				originalValue = TargetProperty.GetVariable().RawValue;
				TargetProperty.GetVariable().RawValue = rawValue;
			}
			TargetProperty.SetValue();
			if (!EveryFrame.Value && !ResetOnExit.Value)
			{
				Finish();
			}
		}

		public override void OnUpdate()
		{
			if (EveryFrame.Value)
			{
				TargetProperty.SetValue();
			}
		}

		public override void OnExit()
		{
			if (ResetOnExit.Value)
			{
				object rawValue = TargetProperty.GetVariable().RawValue;
				TargetProperty.GetVariable().RawValue = originalValue;
				TargetProperty.SetValue();
				TargetProperty.GetVariable().RawValue = rawValue;
			}
		}
	}
}
