using System.Collections;
using UnityEngine;

public abstract class ConditionalAnimation : MonoBehaviour
{
	public abstract bool CanPlayAnimation();

	public abstract void PlayAnimation();

	public abstract IEnumerator PlayAndWait();
}
