using HutongGames.PlayMaker;

public class SetSceneAmbientLight : FsmStateAction
{
	public FsmFloat Intensity;

	public override void Reset()
	{
		Intensity = null;
	}

	public override void OnEnter()
	{
		SceneColorManager sceneColorManager = GameCameras.instance.sceneColorManager;
		if ((bool)sceneColorManager)
		{
			sceneColorManager.AmbientIntensityA = Intensity.Value;
		}
		Finish();
	}
}
