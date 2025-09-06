using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("Hollow Knight")]
	[ActionTarget(typeof(GameObject), "gameObject", true)]
	[Tooltip("Creates a Game Object, usually using a Prefab.")]
	public class CreateUIMsgGetItem : FsmStateAction
	{
		[RequiredField]
		[Tooltip("GameObject to create. Usually a Prefab.")]
		public FsmGameObject gameObject;

		[UIHint(UIHint.Variable)]
		[Tooltip("Optionally store the created object.")]
		public FsmGameObject storeObject;

		[ObjectType(typeof(Sprite))]
		public FsmObject sprite;

		private bool hasSetSprite;

		public override void Reset()
		{
			gameObject = null;
			storeObject = null;
			sprite = new FsmObject
			{
				UseVariable = true
			};
		}

		public override void Awake()
		{
			GameObject value = this.gameObject.Value;
			if (!(value != null))
			{
				return;
			}
			GameObject gameObject = Object.Instantiate(value, Vector3.zero, Quaternion.identity);
			storeObject.Value = gameObject;
			Sprite sprite = this.sprite.Value as Sprite;
			if ((bool)sprite)
			{
				Transform transform = gameObject.transform.Find("Icon");
				if ((bool)transform)
				{
					SpriteRenderer component = transform.GetComponent<SpriteRenderer>();
					if ((bool)component)
					{
						component.sprite = sprite;
					}
					hasSetSprite = true;
				}
			}
			gameObject.SetActive(value: false);
		}

		public override void OnEnter()
		{
			if (storeObject.Value != null)
			{
				GameObject value = storeObject.Value;
				value.SetActive(value: true);
				if (!hasSetSprite)
				{
					Sprite sprite = this.sprite.Value as Sprite;
					if ((bool)sprite)
					{
						Transform transform = value.transform.Find("Icon");
						if ((bool)transform)
						{
							SpriteRenderer component = transform.GetComponent<SpriteRenderer>();
							if ((bool)component)
							{
								component.sprite = sprite;
							}
						}
					}
				}
				EventRegister.GetRegisterGuaranteed(base.Owner, "GET ITEM MSG END");
			}
			else
			{
				GameObject value2 = this.gameObject.Value;
				if (value2 != null)
				{
					GameObject gameObject = Object.Instantiate(value2, Vector3.zero, Quaternion.identity);
					storeObject.Value = gameObject;
					Sprite sprite2 = this.sprite.Value as Sprite;
					if ((bool)sprite2)
					{
						Transform transform2 = gameObject.transform.Find("Icon");
						if ((bool)transform2)
						{
							SpriteRenderer component2 = transform2.GetComponent<SpriteRenderer>();
							if ((bool)component2)
							{
								component2.sprite = sprite2;
							}
						}
					}
					EventRegister.GetRegisterGuaranteed(base.Owner, "GET ITEM MSG END");
				}
			}
			Finish();
		}
	}
}
