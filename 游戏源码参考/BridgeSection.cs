using System.Collections;
using UnityEngine;

public class BridgeSection : MonoBehaviour
{
	public tk2dSpriteAnimator sectionAnim;

	public tk2dSpriteAnimator fenceAnim;

	public MeshRenderer fenceRenderer;

	public AudioSource source;

	public AudioSource fenceSource;

	private void Awake()
	{
		base.transform.SetPositionZ(0.036f);
	}

	public void Open(BridgeLever lever, bool playAnim = true)
	{
		if (playAnim)
		{
			float num = Vector2.Distance(base.transform.position, lever.transform.position);
			StartCoroutine(Open(num * 0.1f + 0.25f));
			return;
		}
		sectionAnim.Play("Bridge Activated");
		fenceAnim.Play("Fence Activated");
		fenceRenderer.enabled = true;
		base.transform.SetPositionZ(0.001f);
	}

	private IEnumerator Open(float waitTime)
	{
		yield return new WaitForSeconds(waitTime);
		StartCoroutine(OpenFence());
		sectionAnim.Play("Bridge Rise");
		source.Play();
		base.transform.SetPositionZ(0.001f);
	}

	private IEnumerator OpenFence()
	{
		yield return new WaitForSeconds(2.5f);
		fenceRenderer.enabled = true;
		fenceAnim.Play("Fence Rise");
		fenceSource.Play();
	}
}
