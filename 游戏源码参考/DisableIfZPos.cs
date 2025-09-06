using UnityEngine;

public class DisableIfZPos : MonoBehaviour
{
	[Tooltip("If further than this distance from the hero on Z, will be disabled.")]
	public float limitZ = 1.8f;

	private void Start()
	{
		if (Mathf.Abs(base.transform.position.z - 0.004f) > limitZ)
		{
			base.gameObject.SetActive(value: false);
		}
	}
}
