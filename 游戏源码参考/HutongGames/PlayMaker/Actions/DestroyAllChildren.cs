using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.GameObject)]
	[Tooltip("Destroy all children on a GameObject.")]
	public class DestroyAllChildren : FsmStateAction
	{
		[RequiredField]
		public FsmGameObject gameObject;

		public FsmBool disable;

		public override void Reset()
		{
			gameObject = null;
			disable = new FsmBool(false);
		}

		public override void OnEnter()
		{
			GameObject value = gameObject.Value;
			if (value != null)
			{
				foreach (Transform item in value.transform)
				{
					if (disable.Value)
					{
						item.gameObject.SetActive(value: false);
					}
					else
					{
						Object.Destroy(item.gameObject);
					}
				}
			}
			Finish();
		}
	}
}
