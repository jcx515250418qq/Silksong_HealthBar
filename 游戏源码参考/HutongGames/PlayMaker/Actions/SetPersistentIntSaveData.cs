namespace HutongGames.PlayMaker.Actions
{
	public class SetPersistentIntSaveData : FsmStateAction
	{
		public FsmString SceneName;

		public FsmString ID;

		public FsmInt SetValue;

		public override void Reset()
		{
			SceneName = null;
			ID = null;
			SetValue = null;
		}

		public override void OnEnter()
		{
			if (SceneData.instance.PersistentInts.TryGetValue(SceneName.Value, ID.Value, out var value))
			{
				value.Value = SetValue.Value;
			}
			else
			{
				value = new PersistentItemData<int>
				{
					SceneName = SceneName.Value,
					ID = ID.Value,
					Value = SetValue.Value
				};
			}
			SceneData.instance.PersistentInts.SetValue(value);
			Finish();
		}
	}
}
