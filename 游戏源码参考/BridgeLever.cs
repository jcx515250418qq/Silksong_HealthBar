using System.Collections;
using UnityEngine;

public class BridgeLever : MonoBehaviour
{
	public string playerDataBool = "cityBridge1";

	public Collider2D bridgeCollider;

	[Space]
	public BridgeSection[] sections;

	[Space]
	public AudioSource audioPlayerPrefab;

	public AudioEvent switchSound;

	public GameObject strikeNailPrefab;

	public Transform hitOrigin;

	private tk2dSpriteAnimator anim;

	private AudioSource source;

	private bool activated;

	private void Awake()
	{
		anim = GetComponent<tk2dSpriteAnimator>();
		source = GetComponent<AudioSource>();
	}

	private void Start()
	{
		activated = GameManager.instance.playerData.GetBool(playerDataBool);
		if (activated)
		{
			bridgeCollider.enabled = true;
			BridgeSection[] array = sections;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].Open(this, playAnim: false);
			}
			anim.Play("Lever Activated");
		}
		else
		{
			bridgeCollider.enabled = false;
		}
	}

	private void OnTriggerEnter2D(Collider2D collision)
	{
		if (!activated && collision.tag == "Nail Attack")
		{
			activated = true;
			StartCoroutine(OpenBridge());
		}
	}

	private IEnumerator OpenBridge()
	{
		GameManager.instance.playerData.SetBool(playerDataBool, value: true);
		switchSound.SpawnAndPlayOneShot(audioPlayerPrefab, base.transform.position);
		GameManager.instance.FreezeMoment(1);
		GameCameras.instance.cameraShakeFSM.SendEvent("EnemyKillShake");
		if ((bool)strikeNailPrefab)
		{
			strikeNailPrefab.Spawn(hitOrigin.position);
		}
		anim.Play("Lever Hit");
		bridgeCollider.enabled = true;
		yield return new WaitForSeconds(0.1f);
		FSMUtility.SetBool(GameCameras.instance.cameraShakeFSM, "RumblingMed", value: true);
		PlayMakerFSM playMakerFSM = PlayMakerFSM.FindFsmOnGameObject(HeroController.instance.gameObject, "Roar and Wound States");
		if ((bool)playMakerFSM)
		{
			playMakerFSM.FsmVariables.FindFsmGameObject("Roar Object").Value = base.gameObject;
		}
		FSMUtility.SendEventToGameObject(HeroController.instance.gameObject, "ROAR ENTER");
		BridgeSection[] array = sections;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].Open(this);
		}
		source.Play();
		yield return new WaitForSeconds(3.25f);
		source.Stop();
		FSMUtility.SetBool(GameCameras.instance.cameraShakeFSM, "RumblingMed", value: false);
		GameCameras.instance.cameraShakeFSM.SendEvent("StopRumble");
		FSMUtility.SendEventToGameObject(HeroController.instance.gameObject, "ROAR EXIT");
	}
}
