using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.GameObject)]
	[Tooltip("Activates/deactivates a Game Object. Use this to hide/show areas, or enable/disable many Behaviours at once.")]
	public class ActivateGameObject : FsmStateAction
	{
		[RequiredField]
		[Tooltip("The GameObject to activate/deactivate.")]
		public FsmOwnerDefault gameObject;

		[RequiredField]
		[Tooltip("Check to activate, uncheck to deactivate Game Object.")]
		public FsmBool activate;

		[Tooltip("Recursively activate/deactivate all children.")]
		public FsmBool recursive;

		[Tooltip("Reset the game objects when exiting this state. Useful if you want an object to be active only while this state is active.\nNote: Only applies to the last Game Object activated/deactivated (won't work if Game Object changes).")]
		public bool resetOnExit;

		[Tooltip("Repeat this action every frame. Useful if Activate changes over time.")]
		public bool everyFrame;

		private GameObject activatedGameObject;

		public override void Reset()
		{
			gameObject = null;
			activate = true;
			recursive = false;
			resetOnExit = false;
			everyFrame = false;
		}

		public override void OnEnter()
		{
			DoActivateGameObject();
			if (!everyFrame)
			{
				Finish();
			}
		}

		public override void OnUpdate()
		{
			DoActivateGameObject();
		}

		public override void OnExit()
		{
			if (!(activatedGameObject == null) && resetOnExit)
			{
				if (recursive.Value)
				{
					SetActiveRecursively(activatedGameObject, !activate.Value);
				}
				else
				{
					activatedGameObject.SetActive(!activate.Value);
				}
			}
		}

		private void DoActivateGameObject()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(gameObject);
			if (!(ownerDefaultTarget == null))
			{
				if (recursive.Value)
				{
					SetActiveRecursively(ownerDefaultTarget, activate.Value);
				}
				else
				{
					ownerDefaultTarget.SetActive(activate.Value);
				}
				activatedGameObject = ownerDefaultTarget;
			}
		}

		public void SetActiveRecursively(GameObject go, bool state)
		{
			go.SetActive(state);
			foreach (Transform item in go.transform)
			{
				SetActiveRecursively(item.gameObject, state);
			}
		}
	}
}
