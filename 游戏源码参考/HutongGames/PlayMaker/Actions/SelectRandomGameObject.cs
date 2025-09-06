namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.GameObject)]
	[Tooltip("Selects a Random Game Object from an array of Game Objects.")]
	public class SelectRandomGameObject : FsmStateAction
	{
		[CompoundArray("Game Objects", "Game Object", "Weight")]
		[Tooltip("A possible GameObject choice.")]
		public FsmGameObject[] gameObjects;

		[HasFloatSlider(0f, 1f)]
		[Tooltip("The relative probability of this GameObject being picked. E.g. a weight of 0.5 is half as likely to be picked as a weight of 1.")]
		public FsmFloat[] weights;

		[RequiredField]
		[UIHint(UIHint.Variable)]
		[Tooltip("Store the selected GameObject in a GameObject Variable.")]
		public FsmGameObject storeGameObject;

		public override void Reset()
		{
			gameObjects = new FsmGameObject[3];
			weights = new FsmFloat[3] { 1f, 1f, 1f };
			storeGameObject = null;
		}

		public override void OnEnter()
		{
			DoSelectRandomGameObject();
			Finish();
		}

		private void DoSelectRandomGameObject()
		{
			if (gameObjects != null && gameObjects.Length != 0 && storeGameObject != null)
			{
				int randomWeightedIndex = ActionHelpers.GetRandomWeightedIndex(weights);
				if (randomWeightedIndex != -1)
				{
					storeGameObject.Value = gameObjects[randomWeightedIndex].Value;
				}
			}
		}
	}
}
