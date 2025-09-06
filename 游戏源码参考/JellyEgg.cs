using UnityEngine;

public class JellyEgg : MonoBehaviour
{
	public bool bomb;

	public GameObject explosionObject;

	public ParticleSystem popEffect;

	public GameObject strikeEffect;

	public MeshRenderer meshRenderer;

	public AudioSource audioSource;

	public VibrationData popVibration;

	public CircleCollider2D circleCollider;

	public GameObject falseShiny;

	public GameObject shinyItem;

	private void OnTriggerEnter2D(Collider2D otherCollider)
	{
		if (otherCollider.gameObject.tag == "Nail Attack" || otherCollider.gameObject.tag == "Hero Spell" || otherCollider.gameObject.tag == "HeroBox")
		{
			Burst();
		}
	}

	private void Burst()
	{
		meshRenderer.enabled = false;
		popEffect.Play();
		audioSource.Play();
		circleCollider.enabled = false;
		if (bomb)
		{
			explosionObject.Spawn(base.transform.position, base.transform.localRotation);
			return;
		}
		float num = Random.Range(1f, 1.5f);
		strikeEffect.transform.localScale = new Vector3(num, num, num);
		strikeEffect.transform.localEulerAngles = new Vector3(strikeEffect.transform.localEulerAngles.x, strikeEffect.transform.localEulerAngles.y, Random.Range(0f, 360f));
		strikeEffect.SetActive(value: true);
		if (falseShiny != null)
		{
			falseShiny.SetActive(value: false);
		}
		if (shinyItem != null)
		{
			shinyItem.SetActive(value: true);
		}
		VibrationManager.PlayVibrationClipOneShot(popVibration, null);
	}
}
