using UnityEngine;

[RequireComponent(typeof(Animator))]
public class AnimatorCullingLink : MonoBehaviour
{
	[SerializeField]
	private Renderer[] targets;

	private bool? wasVisible;

	private Animator animator;

	private void Awake()
	{
		animator = GetComponent<Animator>();
		animator.cullingMode = AnimatorCullingMode.AlwaysAnimate;
	}

	private void Update()
	{
		bool flag = false;
		Renderer[] array = targets;
		for (int i = 0; i < array.Length; i++)
		{
			if (array[i].isVisible)
			{
				flag = true;
			}
		}
		if (flag != wasVisible)
		{
			wasVisible = flag;
			animator.enabled = flag;
		}
	}
}
