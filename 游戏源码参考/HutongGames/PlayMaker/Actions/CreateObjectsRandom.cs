using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	public class CreateObjectsRandom : FsmStateAction
	{
		[ArrayEditor(typeof(GameObject), "", 0, 0, 65536)]
		public FsmArray SpawnPool;

		public FsmInt AmountMin;

		public FsmInt AmountMax;

		public FsmGameObject SpawnPoint;

		public FsmFloat xSpread;

		public FsmFloat ySpread;

		public FsmBool Activated;

		[ArrayEditor(typeof(GameObject), "", 0, 0, 65536)]
		[UIHint(UIHint.Variable)]
		public FsmArray StoreArray;

		public override void Reset()
		{
			SpawnPool = null;
			xSpread = null;
			ySpread = null;
			AmountMin = null;
			AmountMax = null;
			SpawnPoint = null;
			Activated = true;
			StoreArray = null;
		}

		public override void OnEnter()
		{
			int length = SpawnPool.Length;
			int num = Random.Range(AmountMin.Value, AmountMax.Value + 1);
			StoreArray.Resize(num);
			for (int i = 0; i < num; i++)
			{
				int index = Random.Range(0, length);
				GameObject gameObject = SpawnPool.Get(index) as GameObject;
				if ((bool)gameObject)
				{
					GameObject gameObject2 = Object.Instantiate(gameObject);
					if (SpawnPoint.Value != null)
					{
						gameObject2.transform.position = SpawnPoint.Value.transform.position;
					}
					gameObject2.transform.position = new Vector3(gameObject2.transform.position.x + Random.Range(0f - xSpread.Value, xSpread.Value), gameObject2.transform.position.y + Random.Range(0f - ySpread.Value, ySpread.Value), gameObject2.transform.position.z);
					gameObject2.SetActive(Activated.Value);
					StoreArray.Set(i, gameObject2);
				}
			}
			Finish();
		}
	}
}
