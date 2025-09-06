using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.GameObject)]
	[Tooltip("Selects a Random Game Object from an array of Game Objects.")]
	public class SelectRandomGameObjectV2 : FsmStateAction
	{
		[CompoundArray("Game Objects", "Game Object", "Weight")]
		public FsmGameObject[] gameObjects;

		[HasFloatSlider(0f, 1f)]
		public FsmFloat[] weights;

		[RequiredField]
		[UIHint(UIHint.Variable)]
		public FsmGameObject storeGameObject;

		[UIHint(UIHint.Variable)]
		public FsmGameObject disallowedGameObject;

		public override void Reset()
		{
			gameObjects = new FsmGameObject[3];
			weights = new FsmFloat[3] { 1f, 1f, 1f };
			storeGameObject = null;
			disallowedGameObject = null;
		}

		public override void OnEnter()
		{
			DoSelectRandomGameObject();
			Finish();
		}

		private void DoSelectRandomGameObject()
		{
			if (gameObjects == null || gameObjects.Length == 0 || storeGameObject == null)
			{
				return;
			}
			int randomWeightedIndex = ActionHelpers.GetRandomWeightedIndex(weights);
			if (randomWeightedIndex == -1)
			{
				return;
			}
			GameObject value = gameObjects[randomWeightedIndex].Value;
			if (!disallowedGameObject.IsNone && disallowedGameObject.Value != null)
			{
				int num = 0;
				while (value == disallowedGameObject.Value && num < 100)
				{
					randomWeightedIndex = ActionHelpers.GetRandomWeightedIndex(weights);
					value = gameObjects[randomWeightedIndex].Value;
				}
			}
			storeGameObject.Value = value;
		}
	}
}
