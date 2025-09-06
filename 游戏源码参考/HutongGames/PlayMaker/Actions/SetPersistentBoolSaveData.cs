namespace HutongGames.PlayMaker.Actions
{
	public class SetPersistentBoolSaveData : FsmStateAction
	{
		public FsmString SceneName;

		public FsmString ID;

		public FsmBool SetValue;

		public override void Reset()
		{
			SceneName = null;
			ID = null;
			SetValue = null;
		}

		public override void OnEnter()
		{
			if (SceneData.instance.PersistentBools.TryGetValue(SceneName.Value, ID.Value, out var value))
			{
				value.Value = SetValue.Value;
			}
			else
			{
				value = new PersistentItemData<bool>
				{
					SceneName = SceneName.Value,
					ID = ID.Value,
					Value = SetValue.Value
				};
			}
			SceneData.instance.PersistentBools.SetValue(value);
			Finish();
		}
	}
}
