using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("2D Toolkit/Sprite")]
	[Tooltip("Set the sprite id of a sprite. Can use id or name. \nNOTE: The Game Object must have a tk2dBaseSprite or derived component attached ( tk2dSprite, tk2dAnimatedSprite)")]
	public class Tk2dSpriteSetId : FsmStateAction
	{
		[RequiredField]
		[Tooltip("The Game Object to work with. NOTE: The Game Object must have a tk2dBaseSprite or derived component attached ( tk2dSprite, tk2dAnimatedSprite).")]
		[CheckForComponent(typeof(tk2dBaseSprite))]
		public FsmOwnerDefault gameObject;

		[Tooltip("The sprite Id")]
		[UIHint(UIHint.FsmInt)]
		public FsmInt spriteID;

		[Tooltip("OR The sprite name ")]
		[UIHint(UIHint.FsmString)]
		public FsmString ORSpriteName;

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
			spriteID = null;
			ORSpriteName = null;
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
			int value2 = spriteID.Value;
			if (ORSpriteName.Value != "")
			{
				_sprite.SetSprite(collection, ORSpriteName.Value);
			}
			else if (value2 != _sprite.spriteId)
			{
				_sprite.SetSprite(collection, value2);
			}
		}
	}
}
