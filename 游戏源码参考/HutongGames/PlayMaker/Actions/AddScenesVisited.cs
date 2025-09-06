namespace HutongGames.PlayMaker.Actions
{
	public sealed class AddScenesVisited : FsmStateAction
	{
		[ArrayEditor(VariableType.String, "", 0, 0, 65536)]
		public FsmArray scenesArray;

		public FsmString[] scenesVisited;

		public FsmBool doMapUpdate;

		public FsmBool queueMapUpdate;

		public override void Reset()
		{
			scenesArray = null;
			scenesVisited = null;
			doMapUpdate = null;
			queueMapUpdate = null;
		}

		public override void OnEnter()
		{
			PlayerData instance = PlayerData.instance;
			bool flag = false;
			if (scenesArray != null && scenesArray.stringValues != null)
			{
				string[] stringValues = scenesArray.stringValues;
				foreach (string text in stringValues)
				{
					if (!string.IsNullOrEmpty(text) && instance.scenesVisited.Add(text))
					{
						flag = true;
					}
				}
			}
			if (scenesVisited != null)
			{
				for (int j = 0; j < scenesVisited.Length; j++)
				{
					FsmString fsmString = scenesVisited[j];
					if (fsmString != null)
					{
						string value = fsmString.Value;
						if (!string.IsNullOrEmpty(value) && instance.scenesVisited.Add(value))
						{
							flag = true;
						}
					}
				}
			}
			if (doMapUpdate.Value)
			{
				GameManager.instance.UpdateGameMap();
			}
			else if (queueMapUpdate.Value && flag)
			{
				instance.mapUpdateQueued = true;
			}
			Finish();
		}
	}
}
