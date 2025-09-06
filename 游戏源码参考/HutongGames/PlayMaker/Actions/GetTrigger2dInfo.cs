namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Physics2D)]
	[Tooltip("Gets info on the last Trigger 2d event and store in variables.  See Unity and PlayMaker docs on Unity 2D physics.")]
	public class GetTrigger2dInfo : FsmStateAction
	{
		[UIHint(UIHint.Variable)]
		[Tooltip("Get the GameObject hit.")]
		public FsmGameObject gameObjectHit;

		[UIHint(UIHint.Variable)]
		[Tooltip("The number of separate shaped regions in the collider.")]
		public FsmInt shapeCount;

		[UIHint(UIHint.Variable)]
		[Tooltip("Useful for triggering different effects. Audio, particles...")]
		public FsmString physics2dMaterialName;

		public override void Reset()
		{
			gameObjectHit = null;
			shapeCount = null;
			physics2dMaterialName = null;
		}

		private void StoreTriggerInfo()
		{
			if (!(base.Fsm.TriggerCollider2D == null))
			{
				gameObjectHit.Value = base.Fsm.TriggerCollider2D.gameObject;
				shapeCount.Value = base.Fsm.TriggerCollider2D.shapeCount;
				physics2dMaterialName.Value = ((base.Fsm.TriggerCollider2D.sharedMaterial != null) ? base.Fsm.TriggerCollider2D.sharedMaterial.name : "");
			}
		}

		public override void OnEnter()
		{
			StoreTriggerInfo();
			Finish();
		}
	}
}
