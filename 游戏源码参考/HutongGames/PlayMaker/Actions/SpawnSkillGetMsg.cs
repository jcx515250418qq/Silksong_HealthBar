namespace HutongGames.PlayMaker.Actions
{
	public class SpawnSkillGetMsg : FsmStateAction
	{
		[RequiredField]
		[CheckForComponent(typeof(SkillGetMsg))]
		public FsmGameObject MsgPrefab;

		[RequiredField]
		[ObjectType(typeof(ToolItemSkill))]
		public FsmObject Skill;

		public override void Reset()
		{
			MsgPrefab = null;
			Skill = null;
		}

		public override void OnEnter()
		{
			SkillGetMsg.Spawn(MsgPrefab.Value.GetComponent<SkillGetMsg>(), Skill.Value as ToolItemSkill, base.Finish);
		}
	}
}
