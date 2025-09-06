using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.UnityObject)]
	[ActionTarget(typeof(Component), "targetProperty", false)]
	[ActionTarget(typeof(GameObject), "targetProperty", false)]
	[Tooltip("Gets the value of any public property or field on the targeted Unity Object and stores it in a variable. E.g., Drag and drop any component attached to a Game Object to access its properties.")]
	public class GetProperty : FsmStateAction
	{
		[Tooltip("TargetObject:\nAny object derived from UnityEngine.Object. For example, you can drag a Component from the Unity Inspector into this field. HINT: Use\u00a0{{Lock}}\u00a0to lock the current\u00a0FSM selection if you need to drag a component from another GameObject.\nProperty:\nUse the property selection menu to select the property to get. Note: You can drill into the property, e.g., transform.localPosition.x.\nStore Result:\nStore the result in a variable.")]
		public FsmProperty targetProperty;

		[Tooltip("Repeat every frame. Useful if the property is changing over time.")]
		public bool everyFrame;

		public override void Reset()
		{
			targetProperty = new FsmProperty
			{
				setProperty = false
			};
			everyFrame = false;
		}

		public override void OnEnter()
		{
			targetProperty.GetValue();
			if (!everyFrame)
			{
				Finish();
			}
		}

		public override void OnUpdate()
		{
			targetProperty.GetValue();
		}
	}
}
