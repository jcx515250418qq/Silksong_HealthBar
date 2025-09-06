using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	public class AddSpawnedToQueueGroup : FsmStateAction
	{
		[RequiredField]
		[ObjectType(typeof(SpawnQueueGroup))]
		public FsmObject Group;

		[RequiredField]
		public FsmOwnerDefault Target;

		public override void Reset()
		{
			Group = null;
			Target = null;
		}

		public override void OnEnter()
		{
			GameObject safe = Target.GetSafe(this);
			SpawnQueueGroup spawnQueueGroup = Group.Value as SpawnQueueGroup;
			if (spawnQueueGroup != null)
			{
				spawnQueueGroup.AddSpawned(safe);
			}
			Finish();
		}
	}
}
