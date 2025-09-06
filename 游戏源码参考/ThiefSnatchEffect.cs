using UnityEngine;

public class ThiefSnatchEffect : MonoBehaviour
{
	[SerializeField]
	private GameObject rosariesDisplay;

	[SerializeField]
	private GameObject shardsDisplay;

	[SerializeField]
	private AudioEvent mainAudio = AudioEvent.Default;

	[SerializeField]
	private AudioEvent currencyAudio = AudioEvent.Default;

	public void Setup(Transform enemy, bool rosaries, bool shards)
	{
		if ((bool)rosariesDisplay)
		{
			rosariesDisplay.SetActive(rosaries);
		}
		if ((bool)shardsDisplay)
		{
			shardsDisplay.SetActive(shards);
		}
		Vector3 position = HeroController.instance.transform.position;
		Vector3 position2 = base.transform.position;
		if (Random.Range(0, 2) == 0)
		{
			Vector3 localScale = base.transform.localScale;
			localScale.y *= -1f;
			base.transform.localScale = localScale;
		}
		float x = position2.x - position.x;
		float num = Mathf.Atan2(position2.y - position.y, x);
		num *= 57.29578f;
		base.transform.localRotation = Quaternion.Euler(0f, 0f, num);
		mainAudio.SpawnAndPlayOneShot(position2);
		if (rosaries)
		{
			currencyAudio.SpawnAndPlayOneShot(position2);
		}
	}
}
