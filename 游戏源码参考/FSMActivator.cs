using System.Collections;
using UnityEngine;

public class FSMActivator : MonoBehaviour
{
	[HideInInspector]
	public bool activateStaggered = true;

	private PlayMakerFSM[] fsms;

	private tk2dSpriteAnimator spriteAnim;

	private bool activated;

	private void Awake()
	{
		fsms = GetComponents<PlayMakerFSM>();
		spriteAnim = GetComponent<tk2dSpriteAnimator>();
	}

	public void Activate()
	{
		if (activateStaggered)
		{
			StartCoroutine(ActivateStaggered());
		}
		else
		{
			if (activated)
			{
				return;
			}
			if (fsms.Length != 0)
			{
				for (int i = 0; i < fsms.Length; i++)
				{
					fsms[i].enabled = true;
				}
			}
			if (spriteAnim != null)
			{
				spriteAnim.Play();
			}
			activated = true;
		}
	}

	public IEnumerator ActivateStaggered()
	{
		if (activated)
		{
			yield break;
		}
		if (fsms.Length != 0)
		{
			activated = true;
			for (int i = 0; i < fsms.Length; i++)
			{
				fsms[i].enabled = true;
				yield return null;
			}
		}
		if (spriteAnim != null)
		{
			activated = true;
			spriteAnim.Play();
		}
	}

	public void Deactivate()
	{
		if (activated && fsms.Length != 0)
		{
			for (int i = 0; i < fsms.Length; i++)
			{
				fsms[i].enabled = false;
			}
		}
	}

	public IEnumerator DeactivateStaggered()
	{
		if (activated && fsms.Length != 0)
		{
			for (int i = 0; i < fsms.Length; i++)
			{
				fsms[i].enabled = false;
				yield return null;
			}
		}
	}
}
