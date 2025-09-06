namespace HutongGames.PlayMaker.Actions
{
	public abstract class BaseUpdateAction : FsmStateAction
	{
		public enum UpdateType
		{
			OnUpdate = 0,
			OnLateUpdate = 1,
			OnFixedUpdate = 2
		}

		[ActionSection("Update type")]
		[Tooltip("Repeat every frame.")]
		public bool everyFrame;

		[Tooltip("When to update the action.\nOnUpdate: The most common setting.\nOnLateUpdate: Update after everything else. Useful if dependent on another GameObect, e.g. following.\nOnFixedUpdate: Used to update physics e.g., GameObjects with RigidBody components.")]
		public UpdateType updateType;

		public abstract void OnActionUpdate();

		public override void Reset()
		{
			everyFrame = false;
			updateType = UpdateType.OnUpdate;
		}

		public override void OnPreprocess()
		{
			if (updateType == UpdateType.OnFixedUpdate)
			{
				base.Fsm.HandleFixedUpdate = true;
			}
			else if (updateType == UpdateType.OnLateUpdate)
			{
				base.Fsm.HandleLateUpdate = true;
			}
		}

		public override void OnUpdate()
		{
			if (updateType == UpdateType.OnUpdate)
			{
				OnActionUpdate();
			}
			if (!everyFrame)
			{
				Finish();
			}
		}

		public override void OnLateUpdate()
		{
			if (updateType == UpdateType.OnLateUpdate)
			{
				OnActionUpdate();
			}
			if (!everyFrame)
			{
				Finish();
			}
		}

		public override void OnFixedUpdate()
		{
			if (updateType == UpdateType.OnFixedUpdate)
			{
				OnActionUpdate();
			}
			if (!everyFrame)
			{
				Finish();
			}
		}
	}
}
