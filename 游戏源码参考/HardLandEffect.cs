using GlobalEnums;
using UnityEngine;

public class HardLandEffect : MonoBehaviour
{
	public GameObject dustObj;

	public GameObject grassObj;

	public GameObject boneObj;

	public GameObject spaObj;

	public GameObject metalObj;

	public GameObject peakPuffObj;

	public GameObject wetObj;

	public GameObject impactEffect;

	[Space]
	private double recycleTime;

	private void OnEnable()
	{
		dustObj.SetActive(value: false);
		dustObj.SetActiveChildren(value: true);
		grassObj.SetActive(value: false);
		grassObj.SetActiveChildren(value: true);
		boneObj.SetActive(value: false);
		boneObj.SetActiveChildren(value: true);
		spaObj.SetActive(value: false);
		spaObj.SetActiveChildren(value: true);
		metalObj.SetActive(value: false);
		metalObj.SetActiveChildren(value: true);
		wetObj.SetActive(value: false);
		wetObj.SetActiveChildren(value: true);
		peakPuffObj.SetActive(value: false);
		peakPuffObj.SetActiveChildren(value: true);
		GameCameras.instance.cameraShakeFSM.SendEvent("AverageShake");
		impactEffect.SetActive(value: true);
		switch (GameManager.instance.playerData.environmentType)
		{
		case EnvironmentTypes.Dust:
		case EnvironmentTypes.Wood:
			dustObj.SetActive(value: true);
			break;
		case EnvironmentTypes.Grass:
			grassObj.SetActive(value: true);
			break;
		case EnvironmentTypes.Bone:
			boneObj.SetActive(value: true);
			break;
		case EnvironmentTypes.PeakPuff:
			peakPuffObj.SetActive(value: true);
			break;
		case EnvironmentTypes.ShallowWater:
			spaObj.SetActive(value: true);
			break;
		case EnvironmentTypes.Metal:
		case EnvironmentTypes.ThinMetal:
			metalObj.SetActive(value: true);
			break;
		case EnvironmentTypes.Moss:
		case EnvironmentTypes.WetMetal:
		case EnvironmentTypes.WetWood:
		case EnvironmentTypes.RunningWater:
			wetObj.SetActive(value: true);
			break;
		}
		recycleTime = Time.timeAsDouble + 1.5;
	}

	private void Update()
	{
		if (Time.timeAsDouble > recycleTime)
		{
			base.gameObject.Recycle();
		}
	}
}
