using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.GameObject)]
	[Tooltip("Activates/deactivates a Game Object. Use this to hide/show areas, or enable/disable many Behaviours at once.")]
	public class ActivateGameObjectDelay : FsmStateAction
	{
		[RequiredField]
		[Tooltip("The GameObject to activate/deactivate.")]
		public FsmOwnerDefault gameObject;

		[RequiredField]
		[Tooltip("Check to activate, uncheck to deactivate Game Object.")]
		public FsmBool activate;

		[Tooltip("Reset the game objects when exiting this state. Useful if you want an object to be active only while this state is active.\nNote: Only applies to the last Game Object activated/deactivated (won't work if Game Object changes).")]
		public bool resetOnExit;

		public FsmFloat delay;

		private float timer;

		private GameObject activatedGameObject;

		private bool initialActiveState;

		public override void Reset()
		{
			gameObject = null;
			activate = true;
			resetOnExit = false;
			delay = null;
			timer = 0f;
		}

		public override void OnEnter()
		{
			if (delay.Value <= 0f)
			{
				DoActivateGameObject();
			}
			else
			{
				timer = delay.Value;
			}
		}

		public override void OnUpdate()
		{
			if (timer > 0f)
			{
				timer -= Time.deltaTime;
			}
			else
			{
				DoActivateGameObject();
			}
		}

		public override void OnExit()
		{
			if (!(activatedGameObject == null) && resetOnExit)
			{
				activatedGameObject.SetActive(initialActiveState);
			}
		}

		private void DoActivateGameObject()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(gameObject);
			if (!(ownerDefaultTarget == null))
			{
				initialActiveState = ownerDefaultTarget.activeSelf;
				ownerDefaultTarget.SetActive(activate.Value);
				activatedGameObject = ownerDefaultTarget;
				Finish();
			}
		}
	}
}
