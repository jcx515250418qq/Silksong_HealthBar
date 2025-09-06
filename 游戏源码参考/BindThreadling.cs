using UnityEngine;

public class BindThreadling : MonoBehaviour
{
	public Rigidbody2D rb;

	public Transform transform_Hornet;

	private void Start()
	{
		if (transform_Hornet == null)
		{
			transform_Hornet = GameManager.instance.hero_ctrl.transform;
		}
	}

	private void OnEnable()
	{
	}

	private void Update()
	{
	}
}
