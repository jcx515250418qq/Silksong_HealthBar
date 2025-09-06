using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public sealed class tk2dSpriteInitialiser : MonoBehaviour, IInitialisable
{
	[SerializeField]
	private bool autoFind = true;

	[SerializeField]
	private List<tk2dSprite> sprites = new List<tk2dSprite>();

	private bool hasAwaken;

	private bool hasStarted;

	GameObject IInitialisable.gameObject => base.gameObject;

	public bool OnAwake()
	{
		if (hasAwaken)
		{
			return false;
		}
		hasAwaken = true;
		if (autoFind)
		{
			GatherSprites();
		}
		else
		{
			sprites.RemoveAll((tk2dSprite o) => o == null);
		}
		foreach (tk2dSprite sprite in sprites)
		{
			if (!sprite.gameObject.activeInHierarchy)
			{
				sprite.ForceBuild();
			}
		}
		return true;
	}

	public bool OnStart()
	{
		OnAwake();
		if (hasStarted)
		{
			return false;
		}
		hasStarted = true;
		return true;
	}

	private void Awake()
	{
		OnAwake();
	}

	[ContextMenu("GatherSprites")]
	private void GatherSprites()
	{
		sprites.RemoveAll((tk2dSprite o) => o == null);
		sprites = sprites.Union(base.gameObject.GetComponentsInChildren<tk2dSprite>(includeInactive: true)).ToList();
	}
}
