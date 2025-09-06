using UnityEngine;

public class SlopePlat : MonoBehaviour
{
	public float heroXLeft;

	public float heroXRight;

	public float platYLeft;

	public float platYRight;

	private GameObject hero;

	private void Start()
	{
		hero = GameObject.FindWithTag("Player");
	}

	private void Update()
	{
		float x = hero.transform.position.x;
		if (x <= heroXLeft)
		{
			base.transform.localPosition = new Vector3(base.transform.localPosition.x, platYLeft, base.transform.localPosition.z);
			return;
		}
		if (x >= heroXRight)
		{
			base.transform.localPosition = new Vector3(base.transform.localPosition.x, platYRight, base.transform.localPosition.z);
			return;
		}
		float t = Mathf.InverseLerp(heroXLeft, heroXRight, x);
		base.transform.localPosition = new Vector3(base.transform.localPosition.x, Mathf.Lerp(platYLeft, platYRight, t), base.transform.localPosition.z);
	}
}
