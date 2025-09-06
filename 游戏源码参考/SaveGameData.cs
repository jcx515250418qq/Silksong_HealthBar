using System;

[Serializable]
public class SaveGameData
{
	public PlayerData playerData;

	public SceneData sceneData;

	public SaveGameData()
	{
		playerData = new PlayerData();
		sceneData = new SceneData();
	}

	public SaveGameData(PlayerData playerData, SceneData sceneData)
	{
		this.playerData = playerData;
		this.sceneData = sceneData;
	}
}
