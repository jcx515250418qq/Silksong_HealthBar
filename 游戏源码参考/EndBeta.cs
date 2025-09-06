using UnityEngine;

public class EndBeta : MonoBehaviour
{
	private GameManager gm;

	private bool isActive;

	private void Awake()
	{
		gm = GameManager.instance;
		isActive = true;
	}

	private void Start()
	{
	}

	private void Update()
	{
	}

	private void OnTriggerEnter2D(Collider2D otherCollider)
	{
		if (otherCollider.gameObject.tag == "Player" && isActive)
		{
			isActive = false;
		}
	}

	public void Reactivate()
	{
		isActive = true;
	}
}
