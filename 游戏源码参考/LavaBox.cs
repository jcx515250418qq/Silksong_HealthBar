using UnityEngine;

public class LavaBox : MonoBehaviour
{
	public GameObject lavaSplashSmallPrefab;

	public bool isLavaFall;

	private const float EFFECT_POS_Z = 0.003f;

	private float effectPos_y;

	private void Awake()
	{
		BoxCollider2D component = GetComponent<BoxCollider2D>();
		if (component == null)
		{
			Debug.Log($"{this} is missing box collider", this);
			base.enabled = false;
		}
		else
		{
			effectPos_y = base.transform.position.y + component.offset.y + component.size.y / 2f + 0.5f;
		}
	}

	private void OnTriggerEnter2D(Collider2D collision)
	{
		GameObject gameObject = collision.gameObject;
		if (LavaSurfaceSplasher.TrySplash(gameObject) || gameObject.layer == 18 || gameObject.layer == 19 || gameObject.layer == 26)
		{
			Vector3 vector = default(Vector3);
			ObjectPoolExtensions.Spawn(position: (!isLavaFall) ? new Vector3(gameObject.transform.position.x, effectPos_y, 0.003f) : new Vector3(gameObject.transform.position.x, gameObject.transform.position.y, 0.003f), prefab: lavaSplashSmallPrefab);
		}
	}
}
