using UnityEngine;

public class SilkPusher : MonoBehaviour
{
	private PolygonCollider2D polyCollider;

	private bool touching;

	public float pushPerSecond;

	private void Awake()
	{
		polyCollider = GetComponent<PolygonCollider2D>();
	}

	private void OnEnable()
	{
		polyCollider.isTrigger = true;
	}

	private void OnTriggerStay2D(Collider2D collision)
	{
		collision.transform.Translate(pushPerSecond * Time.deltaTime, 0f, 0f);
	}

	private void OnTriggerExit2D(Collider2D collision)
	{
		polyCollider.isTrigger = false;
	}
}
