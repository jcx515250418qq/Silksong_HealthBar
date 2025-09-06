namespace HutongGames.PlayMaker.Actions
{
	public sealed class ConchProjectileHelperAction : FsmStateAction
	{
		[ObjectType(typeof(ConchProjectileCollision))]
		public FsmOwnerDefault target;

		public FsmVector2 direction;

		private ConchProjectileCollision conchProjectileCollision;

		public override void Reset()
		{
			target = null;
		}

		public override void OnEnter()
		{
			conchProjectileCollision = target.GetSafe<ConchProjectileCollision>(this);
			if (conchProjectileCollision != null)
			{
				conchProjectileCollision.SetDirection(direction.Value);
			}
		}

		public override void OnExit()
		{
			if (conchProjectileCollision != null)
			{
				conchProjectileCollision.StateExited();
			}
		}
	}
}
