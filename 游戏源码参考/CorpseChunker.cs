using TeamCherry.SharedUtils;
using UnityEngine;

public class CorpseChunker : Corpse
{
	[Header("Chunker Variables")]
	[SerializeField]
	private bool instantChunker;

	[Space]
	[SerializeField]
	private GameObject effects;

	[SerializeField]
	private GameObject chunks;

	[SerializeField]
	private bool keepMeshRendererActive;

	protected override bool DoLandEffectsInstantly => instantChunker;

	protected override void LandEffects()
	{
		base.LandEffects();
		if ((bool)body)
		{
			body.linearVelocity = Vector2.zero;
		}
		splatAudioClipTable.SpawnAndPlayOneShot(audioPlayerPrefab, base.transform.position);
		BloodSpawner.SpawnBlood(base.transform.position, 30, 30, 5f, 30f, 60f, 120f, null);
		GameCameras instance = GameCameras.instance;
		if ((bool)instance)
		{
			instance.cameraShakeFSM.SendEvent("EnemyKillShake");
		}
		if ((bool)effects)
		{
			effects.SetActive(value: true);
		}
		if ((bool)chunks)
		{
			chunks.SetActive(value: true);
			chunks.transform.SetParent(null, worldPositionStays: true);
			FlingUtils.ChildrenConfig config = default(FlingUtils.ChildrenConfig);
			config.Parent = chunks;
			config.SpeedMin = 15f;
			config.SpeedMax = 20f;
			config.AngleMin = 60f;
			config.AngleMax = 120f;
			config.OriginVariationX = 0f;
			config.OriginVariationY = 0f;
			FlingUtils.FlingChildren(config, base.transform, Vector3.zero, new MinMaxFloat(0f, 0.001f));
		}
		if ((bool)meshRenderer && !keepMeshRendererActive)
		{
			meshRenderer.enabled = false;
		}
	}
}
