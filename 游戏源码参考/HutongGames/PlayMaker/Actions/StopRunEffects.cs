namespace HutongGames.PlayMaker.Actions
{
	public class StopRunEffects : FsmStateAction
	{
		[CheckForComponent(typeof(RunEffects))]
		public FsmGameObject SpawnedObject;

		public override void Reset()
		{
			SpawnedObject = null;
		}

		public override void OnEnter()
		{
			if ((bool)SpawnedObject.Value)
			{
				RunEffects component = SpawnedObject.Value.GetComponent<RunEffects>();
				if ((bool)component)
				{
					component.Stop();
				}
				SpawnedObject.Value = null;
			}
			Finish();
		}
	}
}
