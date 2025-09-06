using UnityEngine;

public class Tk2dPlayRandomAnimationOnEnable : MonoBehaviour
{
	[SerializeField]
	private tk2dSpriteAnimator animator;

	[SerializeField]
	private string[] clipNames;

	private void Reset()
	{
		animator = GetComponent<tk2dSpriteAnimator>();
	}

	private void OnEnable()
	{
		Play();
	}

	public void Play()
	{
		string text = clipNames[Random.Range(0, clipNames.Length)];
		animator.Play(text);
		animator.PlayFromFrame(0);
	}
}
