using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	public class SetRandomAudioClipFromTable : FsmStateAction
	{
		[ObjectType(typeof(RandomAudioClipTable))]
		public FsmObject Table;

		[ObjectType(typeof(AudioSource))]
		public FsmGameObject AudioSource;

		public FsmBool AutoPlay;

		public FsmFloat delay;

		private float timer;

		public override void Reset()
		{
			Table = null;
			AudioSource = null;
			AutoPlay = null;
			timer = 0f;
		}

		public override void OnEnter()
		{
			timer = 0f;
			if (delay.Value == 0f)
			{
				DoSet();
			}
		}

		public override void OnUpdate()
		{
			if (timer >= delay.Value)
			{
				DoSet();
			}
			else
			{
				timer += Time.deltaTime;
			}
		}

		private void DoSet()
		{
			if (Table.Value == null)
			{
				Finish();
			}
			RandomAudioClipTable randomAudioClipTable = Table.Value as RandomAudioClipTable;
			GameObject value = AudioSource.Value;
			if ((bool)value)
			{
				AudioSource component = value.GetComponent<AudioSource>();
				if (randomAudioClipTable != null && component != null)
				{
					component.clip = randomAudioClipTable.SelectClip(forcePlay: true);
					component.volume = randomAudioClipTable.SelectVolume();
					component.pitch = randomAudioClipTable.SelectPitch();
					component.loop = true;
					if (AutoPlay.Value)
					{
						component.Play();
					}
				}
			}
			Finish();
		}
	}
}
