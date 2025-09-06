using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.GameObject)]
	[Tooltip("Remove all components from a GameObject.")]
	public class RemoveAllComponents : FsmStateAction
	{
		[RequiredField]
		[Tooltip("The GameObject to destroy.")]
		public FsmGameObject gameObject;

		public override void Reset()
		{
			gameObject = null;
		}

		public override void OnEnter()
		{
			GameObject value = gameObject.Value;
			if (value != null)
			{
				MonoBehaviour[] components = value.GetComponents<MonoBehaviour>();
				foreach (MonoBehaviour monoBehaviour in components)
				{
					Debug.Log(monoBehaviour.name);
					if (monoBehaviour.name != "Play Maker FSM")
					{
						_ = monoBehaviour.name != "Persistent Bool Item";
					}
				}
			}
			Finish();
		}
	}
}
