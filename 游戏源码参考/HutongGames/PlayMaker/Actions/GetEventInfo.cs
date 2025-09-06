namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.StateMachine)]
	[Tooltip("Gets info on the last event that caused a state change. See also: {{Set Event Data}} action.")]
	[SeeAlso("{{SetEventData}}")]
	public class GetEventInfo : FsmStateAction
	{
		[UIHint(UIHint.Variable)]
		[Tooltip("The Game Object that sent the Event.")]
		public FsmGameObject sentByGameObject;

		[UIHint(UIHint.Variable)]
		[Tooltip("The name of the FSM that sent the Event.")]
		public FsmString fsmName;

		[UIHint(UIHint.Variable)]
		[Tooltip("Custom Bool data.")]
		public FsmBool getBoolData;

		[UIHint(UIHint.Variable)]
		[Tooltip("Custom Int data.")]
		public FsmInt getIntData;

		[UIHint(UIHint.Variable)]
		[Tooltip("Custom Float data.")]
		public FsmFloat getFloatData;

		[UIHint(UIHint.Variable)]
		[Tooltip("Custom Vector2 data.")]
		public FsmVector2 getVector2Data;

		[UIHint(UIHint.Variable)]
		[Tooltip("Custom Vector3 data.")]
		public FsmVector3 getVector3Data;

		[UIHint(UIHint.Variable)]
		[Tooltip("Custom String data.")]
		public FsmString getStringData;

		[UIHint(UIHint.Variable)]
		[Tooltip("Custom GameObject data.")]
		public FsmGameObject getGameObjectData;

		[UIHint(UIHint.Variable)]
		[Tooltip("Custom Rect data.")]
		public FsmRect getRectData;

		[UIHint(UIHint.Variable)]
		[Tooltip("Custom Quaternion data.")]
		public FsmQuaternion getQuaternionData;

		[UIHint(UIHint.Variable)]
		[Tooltip("Custom Material data.")]
		public FsmMaterial getMaterialData;

		[UIHint(UIHint.Variable)]
		[Tooltip("Custom Texture data.")]
		public FsmTexture getTextureData;

		[UIHint(UIHint.Variable)]
		[Tooltip("Custom Color data.")]
		public FsmColor getColorData;

		[UIHint(UIHint.Variable)]
		[Tooltip("Custom Object data.")]
		public FsmObject getObjectData;

		public override void Reset()
		{
			sentByGameObject = null;
			fsmName = null;
			getBoolData = null;
			getIntData = null;
			getFloatData = null;
			getVector2Data = null;
			getVector3Data = null;
			getStringData = null;
			getGameObjectData = null;
			getRectData = null;
			getQuaternionData = null;
			getMaterialData = null;
			getTextureData = null;
			getColorData = null;
			getObjectData = null;
		}

		public override void OnEnter()
		{
			if (Fsm.EventData.SentByGameObject != null)
			{
				sentByGameObject.Value = Fsm.EventData.SentByGameObject;
			}
			else if (Fsm.EventData.SentByFsm != null)
			{
				sentByGameObject.Value = Fsm.EventData.SentByFsm.GameObject;
				fsmName.Value = Fsm.EventData.SentByFsm.Name;
			}
			else
			{
				sentByGameObject.Value = null;
				fsmName.Value = "";
			}
			getBoolData.Value = Fsm.EventData.BoolData;
			getIntData.Value = Fsm.EventData.IntData;
			getFloatData.Value = Fsm.EventData.FloatData;
			getVector2Data.Value = Fsm.EventData.Vector2Data;
			getVector3Data.Value = Fsm.EventData.Vector3Data;
			getStringData.Value = Fsm.EventData.StringData;
			getGameObjectData.Value = Fsm.EventData.GameObjectData;
			getRectData.Value = Fsm.EventData.RectData;
			getQuaternionData.Value = Fsm.EventData.QuaternionData;
			getMaterialData.Value = Fsm.EventData.MaterialData;
			getTextureData.Value = Fsm.EventData.TextureData;
			getColorData.Value = Fsm.EventData.ColorData;
			getObjectData.Value = Fsm.EventData.ObjectData;
			Finish();
		}
	}
}
