using System.Collections;
using UnityEngine;

public class AcidCorpseSplash : MonoBehaviour
{
	public TriggerEnterEvent corpseDetector;

	[Space]
	public GameObject acidSplashPrefab;

	public GameObject acidSteamPrefab;

	public ParticleSystem sporeCloudPrefab;

	public ParticleSystem bubCloudPrefab;

	[Space]
	public AudioSource audioPlayerPefab;

	public AudioEvent splashSound;

	private void Start()
	{
		if ((bool)corpseDetector)
		{
			corpseDetector.OnTriggerEntered += delegate(Collider2D col, GameObject sender)
			{
				StartCoroutine(CorpseSplash(col.gameObject));
			};
		}
	}

	private IEnumerator CorpseSplash(GameObject corpseObject)
	{
		Corpse component = corpseObject.GetComponent<Corpse>();
		if (!component)
		{
			yield break;
		}
		component.Acid();
		Rigidbody2D body = corpseObject.GetComponent<Rigidbody2D>();
		splashSound.SpawnAndPlayOneShot(audioPlayerPefab, corpseObject.transform.position);
		Vector3 position = corpseObject.transform.position;
		if ((bool)corpseDetector)
		{
			BoxCollider2D component2 = corpseDetector.GetComponent<BoxCollider2D>();
			if ((bool)component2)
			{
				position.y = component2.bounds.max.y;
			}
		}
		if ((bool)acidSplashPrefab)
		{
			Object.Instantiate(acidSplashPrefab, position + new Vector3(0f, 0f, -0.1f), acidSplashPrefab.transform.rotation);
		}
		if ((bool)acidSteamPrefab)
		{
			Object.Instantiate(acidSteamPrefab, position, acidSteamPrefab.transform.rotation);
		}
		ParticleSystem acidBubble = null;
		if ((bool)bubCloudPrefab)
		{
			acidBubble = Object.Instantiate(bubCloudPrefab, position, bubCloudPrefab.transform.rotation);
			if ((bool)acidBubble)
			{
				acidBubble.Play();
			}
		}
		ParticleSystem acidSpore = null;
		if ((bool)sporeCloudPrefab)
		{
			acidSpore = Object.Instantiate(sporeCloudPrefab, position, sporeCloudPrefab.transform.rotation);
			if ((bool)acidSpore)
			{
				acidSpore.Play();
			}
		}
		for (float elapsed = 0f; elapsed <= 0.5f; elapsed += Time.fixedDeltaTime)
		{
			if ((bool)body)
			{
				body.linearVelocity *= 0.1f;
			}
			yield return new WaitForFixedUpdate();
		}
		if ((bool)body)
		{
			body.isKinematic = true;
		}
		if ((bool)corpseObject)
		{
			tk2dSprite rend = corpseObject.GetComponent<tk2dSprite>();
			if ((bool)rend)
			{
				float elapsed = 0f;
				for (float fadeTime = 1f; elapsed <= fadeTime; elapsed += Time.deltaTime)
				{
					rend.color = Color.Lerp(Color.white, Color.clear, elapsed / fadeTime);
					yield return null;
				}
			}
		}
		if ((bool)acidBubble)
		{
			acidBubble.Stop();
		}
		if ((bool)acidSpore)
		{
			acidSpore.Stop();
		}
	}
}
