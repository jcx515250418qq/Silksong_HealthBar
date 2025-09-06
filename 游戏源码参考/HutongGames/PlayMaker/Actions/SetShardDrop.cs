using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("Hollow Knight")]
	public class SetShardDrop : FsmStateAction
	{
		[UIHint(UIHint.Variable)]
		public FsmOwnerDefault target;

		public FsmInt shards;

		public override void Reset()
		{
			target = new FsmOwnerDefault();
			shards = new FsmInt();
		}

		public override void OnEnter()
		{
			GameObject safe = target.GetSafe(this);
			if (safe != null)
			{
				HealthManager component = safe.GetComponent<HealthManager>();
				if (component != null && !shards.IsNone)
				{
					component.SetShellShards(shards.Value);
				}
			}
			Finish();
		}
	}
}
