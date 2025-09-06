namespace HutongGames.PlayMaker.Actions
{
	public sealed class AddScenesMapped : FsmStateAction
	{
		[ArrayEditor(VariableType.String, "", 0, 0, 65536)]
		public FsmArray scenesArray;

		public FsmString[] scenesMapped;

		public FsmBool requireQuill;

		public FsmBool doMapUpdate;

		public FsmBool queueMapUpdate;

		public override void Reset()
		{
			scenesArray = null;
			scenesMapped = null;
			requireQuill = null;
			doMapUpdate = null;
			queueMapUpdate = null;
		}

		public override void OnEnter()
		{
			PlayerData instance = PlayerData.instance;
			if (requireQuill.Value && (!instance.hasQuill || instance.QuillState == 0))
			{
				Finish();
				return;
			}
			bool flag = false;
			if (scenesArray != null && scenesArray.stringValues != null)
			{
				string[] stringValues = scenesArray.stringValues;
				foreach (string text in stringValues)
				{
					if (!string.IsNullOrEmpty(text) && instance.scenesMapped.Add(text))
					{
						flag = true;
					}
				}
			}
			if (scenesMapped != null)
			{
				for (int j = 0; j < scenesMapped.Length; j++)
				{
					FsmString fsmString = scenesMapped[j];
					if (fsmString != null)
					{
						string value = fsmString.Value;
						if (!string.IsNullOrEmpty(value) && instance.scenesMapped.Add(value))
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
