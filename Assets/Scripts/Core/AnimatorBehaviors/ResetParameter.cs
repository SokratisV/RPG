using UnityEngine;
using UnityEngine.Animations;

public class ResetParameter : StateMachineBehaviour
{
	[SerializeField] private int defaultValue;
	private static readonly int Skill = Animator.StringToHash("skill");
	private static readonly int SkillAnimationExtraHash = Animator.StringToHash("skillExtra");

	public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex, AnimatorControllerPlayable controller)
	{
		animator.SetInteger(Skill, defaultValue);
		animator.ResetTrigger(SkillAnimationExtraHash);
	}
}