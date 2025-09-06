namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.StateMachine)]
	[Tooltip("Sets Event Data before sending an event. Get the Event Data, along with sender information, using the {{Get Event Info}} action.")]
	public class SetEventData : FsmStateAction
	{
		[Tooltip("Custom Game Object data.")]
		public FsmGameObject setGameObjectData;

		[Tooltip("Custom Int data.")]
		public FsmInt setIntData;

		[Tooltip("Custom Float data.")]
		public FsmFloat setFloatData;

		[Tooltip("Custom String data.")]
		public FsmString setStringData;

		[Tooltip("Custom Bool data.")]
		public FsmBool setBoolData;

		[Tooltip("Custom Vector2 data.")]
		public FsmVector2 setVector2Data;

		[Tooltip("Custom Vector3 data.")]
		public FsmVector3 setVector3Data;

		[Tooltip("Custom Rect data.")]
		public FsmRect setRectData;

		[Tooltip("Custom Quaternion data.")]
		public FsmQuaternion setQuaternionData;

		[Tooltip("Custom Color data.")]
		public FsmColor setColorData;

		[Tooltip("Custom Material data.")]
		public FsmMaterial setMaterialData;

		[Tooltip("Custom Texture data.")]
		public FsmTexture setTextureData;

		[Tooltip("Custom Object data.")]
		public FsmObject setObjectData;

		public bool everyFrame;

		public override void Reset()
		{
			setGameObjectData = new FsmGameObject
			{
				UseVariable = true
			};
			setIntData = new FsmInt
			{
				UseVariable = true
			};
			setFloatData = new FsmFloat
			{
				UseVariable = true
			};
			setStringData = new FsmString
			{
				UseVariable = true
			};
			setBoolData = new FsmBool
			{
				UseVariable = true
			};
			setVector2Data = new FsmVector2
			{
				UseVariable = true
			};
			setVector3Data = new FsmVector3
			{
				UseVariable = true
			};
			setRectData = new FsmRect
			{
				UseVariable = true
			};
			setQuaternionData = new FsmQuaternion
			{
				UseVariable = true
			};
			setColorData = new FsmColor
			{
				UseVariable = true
			};
			setMaterialData = new FsmMaterial
			{
				UseVariable = true
			};
			setTextureData = new FsmTexture
			{
				UseVariable = true
			};
			setObjectData = new FsmObject
			{
				UseVariable = true
			};
			everyFrame = false;
		}

		public override void OnEnter()
		{
			DoSetData();
			if (!everyFrame)
			{
				Finish();
			}
		}

		public override void OnUpdate()
		{
			DoSetData();
		}

		private void DoSetData()
		{
			Fsm.EventData.BoolData = setBoolData.Value;
			Fsm.EventData.IntData = setIntData.Value;
			Fsm.EventData.FloatData = setFloatData.Value;
			Fsm.EventData.Vector2Data = setVector2Data.Value;
			Fsm.EventData.Vector3Data = setVector3Data.Value;
			Fsm.EventData.StringData = setStringData.Value;
			Fsm.EventData.GameObjectData = setGameObjectData.Value;
			Fsm.EventData.RectData = setRectData.Value;
			Fsm.EventData.QuaternionData = setQuaternionData.Value;
			Fsm.EventData.ColorData = setColorData.Value;
			Fsm.EventData.MaterialData = setMaterialData.Value;
			Fsm.EventData.TextureData = setTextureData.Value;
			Fsm.EventData.ObjectData = setObjectData.Value;
		}
	}
}
