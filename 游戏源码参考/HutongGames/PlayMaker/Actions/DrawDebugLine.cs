using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Debug)]
	[Tooltip("Draws a line from a Start point to an End point. Specify the points as Game Objects or Vector3 world positions. If both are specified, position is used as a local offset from the Object's position.\n\nNotes:\n- Enable/disable Gizmos in the Game View toolbar.\n- Set how long debug lines are visible for in Preferences > Debugging.")]
	public class DrawDebugLine : FsmStateAction
	{
		[Tooltip("Draw line from a GameObject.")]
		public FsmGameObject fromObject;

		[Tooltip("Draw line from a world position, or local offset from GameObject if provided.")]
		public FsmVector3 fromPosition;

		[Tooltip("Draw line to a GameObject.")]
		public FsmGameObject toObject;

		[Tooltip("Draw line to a world position, or local offset from GameObject if provided.")]
		public FsmVector3 toPosition;

		[Tooltip("The color of the line.")]
		public FsmColor color;

		public override void Awake()
		{
			base.BlocksFinish = false;
		}

		public override void Reset()
		{
			fromObject = new FsmGameObject
			{
				UseVariable = true
			};
			fromPosition = new FsmVector3
			{
				UseVariable = true
			};
			toObject = new FsmGameObject
			{
				UseVariable = true
			};
			toPosition = new FsmVector3
			{
				UseVariable = true
			};
			color = Color.white;
		}

		public override void OnUpdate()
		{
			Vector3 position = ActionHelpers.GetPosition(fromObject, fromPosition);
			Vector3 position2 = ActionHelpers.GetPosition(toObject, toPosition);
			Debug.DrawLine(position, position2, color.Value);
		}

		public override void OnExit()
		{
			Vector3 position = ActionHelpers.GetPosition(fromObject, fromPosition);
			Vector3 position2 = ActionHelpers.GetPosition(toObject, toPosition);
			Debug.DrawLine(position, position2, color.Value, PlayMakerPrefs.DebugLinesDuration);
		}
	}
}
