using UnityEngine;

public class HitResponseAnimator : MonoBehaviour
{
	[SerializeField]
	private HitResponse hitResponse;

	[SerializeField]
	private Animator animator;

	[SerializeField]
	[ArrayForEnum(typeof(HitInstance.HitDirection))]
	private string[] hitAnimations;

	[SerializeField]
	private int playOnLayer;

	private void OnValidate()
	{
		ArrayForEnumAttribute.EnsureArraySize(ref hitAnimations, typeof(HitInstance.HitDirection));
	}

	private void Awake()
	{
		OnValidate();
		hitResponse.WasHitDirectional += OnWasHitDirectional;
	}

	private void OnWasHitDirectional(HitInstance.HitDirection hitDirection)
	{
		string text = hitAnimations[(int)hitDirection];
		if (string.IsNullOrEmpty(text) && (hitDirection == HitInstance.HitDirection.Up || hitDirection == HitInstance.HitDirection.Down))
		{
			Vector3 position = HeroController.instance.transform.position;
			Vector3 position2 = base.transform.position;
			text = ((position.x < position2.x) ? hitAnimations[1] : hitAnimations[0]);
		}
		animator.Play(text, playOnLayer, 0f);
	}
}
