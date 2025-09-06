using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.GameObject)]
	[Tooltip("Gets a Random Game Object from the scene.\nOptionally filter by Tag.")]
	public class GetRandomObject : FsmStateAction
	{
		[UIHint(UIHint.Tag)]
		[Tooltip("Only select from Game Objects with this Tag.")]
		public FsmString withTag;

		[RequiredField]
		[UIHint(UIHint.Variable)]
		[Tooltip("Store the result in a GameObject Variable.")]
		public FsmGameObject storeResult;

		[Tooltip("Repeat every frame.")]
		public bool everyFrame;

		public override void Reset()
		{
			withTag = "Untagged";
			storeResult = null;
			everyFrame = false;
		}

		public override void OnEnter()
		{
			DoGetRandomObject();
			if (!everyFrame)
			{
				Finish();
			}
		}

		public override void OnUpdate()
		{
			DoGetRandomObject();
		}

		private void DoGetRandomObject()
		{
			GameObject[] array = ((!(withTag.Value != "Untagged")) ? ((GameObject[])Object.FindObjectsOfType(typeof(GameObject))) : GameObject.FindGameObjectsWithTag(withTag.Value));
			if (array.Length != 0)
			{
				storeResult.Value = array[Random.Range(0, array.Length)];
			}
			else
			{
				storeResult.Value = null;
			}
		}
	}
}
