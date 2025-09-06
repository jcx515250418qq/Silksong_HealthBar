using System;
using UnityEngine;

[Serializable]
public class AnimatorHashCache
{
	[SerializeField]
	private string name;

	private int cachedHash;

	private bool valid;

	public int Hash
	{
		get
		{
			Update();
			return cachedHash;
		}
	}

	public string Name => name;

	public AnimatorHashCache(string name)
	{
		this.name = name;
		cachedHash = Animator.StringToHash(name);
		valid = true;
	}

	public void Update()
	{
		if (!valid)
		{
			cachedHash = Animator.StringToHash(name);
			valid = true;
		}
	}

	public void Dirty()
	{
		valid = false;
	}

	public static implicit operator int(AnimatorHashCache cache)
	{
		return cache?.Hash ?? 0;
	}
}
