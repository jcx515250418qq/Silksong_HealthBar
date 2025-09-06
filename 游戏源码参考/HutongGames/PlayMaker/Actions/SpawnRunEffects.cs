using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	public class SpawnRunEffects : FsmStateAction
	{
		public FsmOwnerDefault SpawnPoint;

		[CheckForComponent(typeof(RunEffects))]
		public FsmGameObject RunEffectsPrefab;

		[UIHint(UIHint.Variable)]
		public FsmGameObject StoreObject;

		public FsmBool DoSprintmasterEffect;

		public FsmBool StopOnExit;

		public override void Reset()
		{
			SpawnPoint = null;
			RunEffectsPrefab = null;
			StoreObject = null;
			StopOnExit = null;
		}

		public override void OnEnter()
		{
			if ((bool)RunEffectsPrefab.Value)
			{
				GameObject safe = SpawnPoint.GetSafe(this);
				GameObject gameObject = RunEffectsPrefab.Value.Spawn();
				StoreObject.Value = gameObject;
				if ((bool)safe)
				{
					gameObject.transform.SetParent(safe.transform, worldPositionStays: false);
				}
				RunEffects component = gameObject.GetComponent<RunEffects>();
				if ((bool)component)
				{
					component.StartEffect(isHero: true, DoSprintmasterEffect.Value);
				}
			}
			Finish();
		}

		public override void OnExit()
		{
			if (StopOnExit.Value && (bool)StoreObject.Value)
			{
				RunEffects component = StoreObject.Value.GetComponent<RunEffects>();
				if ((bool)component)
				{
					component.Stop();
				}
				StoreObject.Value = null;
			}
		}
	}
}
