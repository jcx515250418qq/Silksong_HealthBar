using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.GameObject)]
	[Tooltip("Prevent hitching at runtime by pre-building TK2D sprites on disabled objects.")]
	public class PreBuildTK2DSprites : FsmStateAction
	{
		[RequiredField]
		public FsmGameObject gameObject;

		public bool useChildren;

		public override void Reset()
		{
			gameObject = null;
			useChildren = true;
		}

		public override void OnEnter()
		{
			GameObject value = gameObject.Value;
			if (value != null)
			{
				tk2dSprite[] array = (useChildren ? value.GetComponentsInChildren<tk2dSprite>(includeInactive: true) : value.GetComponents<tk2dSprite>());
				for (int i = 0; i < array.Length; i++)
				{
					array[i].ForceBuild();
				}
			}
			Finish();
		}
	}
}
