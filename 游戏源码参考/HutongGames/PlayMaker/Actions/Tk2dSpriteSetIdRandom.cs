using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("2D Toolkit/Sprite")]
	[Tooltip("Randomly set the sprite id of a sprite. \nNOTE: The Game Object must have a tk2dBaseSprite or derived component attached ( tk2dSprite, tk2dAnimatedSprite)")]
	public class Tk2dSpriteSetIdRandom : FsmStateAction
	{
		[RequiredField]
		[Tooltip("The Game Object to work with. NOTE: The Game Object must have a tk2dBaseSprite or derived component attached ( tk2dSprite, tk2dAnimatedSprite).")]
		[CheckForComponent(typeof(tk2dBaseSprite))]
		public FsmOwnerDefault gameObject;

		[CheckForComponent(typeof(tk2dSpriteCollection))]
		public FsmGameObject spriteCollection;

		private tk2dBaseSprite _sprite;

		private void _getSprite()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(gameObject);
			if (!(ownerDefaultTarget == null))
			{
				_sprite = ownerDefaultTarget.GetComponent<tk2dBaseSprite>();
			}
		}

		public override void Reset()
		{
			gameObject = null;
			spriteCollection = new FsmGameObject
			{
				UseVariable = true
			};
		}

		public override void OnEnter()
		{
			_getSprite();
			DoSetSpriteID();
			Finish();
		}

		private void DoSetSpriteID()
		{
			if (_sprite == null)
			{
				LogWarning("Missing tk2dBaseSprite component: " + _sprite.gameObject.name);
				return;
			}
			tk2dSpriteCollectionData collection = _sprite.Collection;
			if (!spriteCollection.IsNone)
			{
				GameObject value = spriteCollection.Value;
				if (value != null)
				{
					tk2dSpriteCollection component = value.GetComponent<tk2dSpriteCollection>();
					if (component != null)
					{
						collection = component.spriteCollection;
					}
				}
			}
			int newSpriteId = Random.Range(0, collection.Count + 1);
			_sprite.SetSprite(collection, newSpriteId);
		}
	}
}
