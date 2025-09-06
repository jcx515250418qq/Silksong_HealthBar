using UnityEngine;

public class TownGrass : MonoBehaviour
{
	public GameObject[] debris;

	public GameObject nailEffectPrefab;

	public AudioClip[] cutSound;

	public AudioSource source;

	private void OnTriggerEnter2D(Collider2D collision)
	{
		if (!GrassCut.ShouldCut(collision))
		{
			return;
		}
		int num = (int)Mathf.Sign(collision.transform.position.x - base.transform.position.x);
		Vector3 position = (collision.transform.position + base.transform.position) / 2f;
		if ((bool)nailEffectPrefab)
		{
			GameObject obj = nailEffectPrefab.Spawn(position);
			Vector3 localScale = obj.transform.localScale;
			localScale.x = Mathf.Abs(localScale.x) * (float)(-num);
			obj.transform.localScale = localScale;
		}
		if (debris.Length != 0)
		{
			GameObject[] array = debris;
			foreach (GameObject obj2 in array)
			{
				obj2.SetActive(value: true);
				obj2.transform.SetParent(null, worldPositionStays: true);
			}
		}
		if ((bool)source && cutSound.Length != 0)
		{
			source.transform.SetParent(null, worldPositionStays: true);
			source.PlayOneShot(cutSound[Random.Range(0, cutSound.Length)]);
		}
		base.gameObject.SetActive(value: false);
	}
}
