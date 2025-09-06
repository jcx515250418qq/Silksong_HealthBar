using UnityEngine;

public class SetRandomSpriteId : MonoBehaviour, IExternalDebris
{
	private tk2dSprite sprite;

	protected void Awake()
	{
		sprite = GetComponent<tk2dSprite>();
	}

	public void Init()
	{
		if (sprite != null)
		{
			tk2dSpriteCollectionData collection = sprite.Collection;
			if (collection != null)
			{
				sprite.SetSprite(collection, Random.Range(0, collection.Count));
			}
		}
	}

	void IExternalDebris.InitExternalDebris()
	{
		Init();
	}
}
