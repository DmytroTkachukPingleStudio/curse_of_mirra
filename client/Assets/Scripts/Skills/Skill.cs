using System.Collections;
using MoreMountains.Tools;
using MoreMountains.TopDownEngine;
using UnityEngine;
using MoreMountains.Feedbacks;

public abstract class Skill : CharacterAbility
{
    [SerializeField]
    protected string skillId;

    [SerializeField]
    protected Action serverSkill;

    [SerializeField]
    protected bool blocksMovementOnExecute = true;

    [SerializeField]
    protected SkillInfo skillInfo;
    protected int skillLevel;

    public void SetSkill(Action serverSkill, SkillInfo skillInfo)
    {
        this.serverSkill = serverSkill;
        this.skillInfo = skillInfo;
        this.skillLevel = 0;
    }

    protected override void Start()
    {
        base.Start();

        if (blocksMovementOnExecute)
        {
            BlockingMovementStates = new CharacterStates.MovementStates[1];
            BlockingMovementStates[0] = CharacterStates.MovementStates.Attacking;
        }

        if (skillInfo.feedbackAnimation)
        {
            GameObject feedback = Instantiate(skillInfo.feedbackAnimation, _model.transform.parent);

            this.AbilityStartFeedbacks = feedback.GetComponent<MMF_Player>();
        }

        if (skillInfo)
        {
            _animator.SetFloat(skillId + "Speed", skillInfo.animationSpeedMultiplier);
        }
    }

    public void TryExecuteSkill()
    {
        if (AbilityAuthorized)
        {
            Vector3 direction = this.GetComponent<Character>()
                .GetComponent<CharacterOrientation3D>()
                .ForcedRotationDirection;
            RelativePosition relativePosition = new RelativePosition
            {
                X = direction.x,
                Y = direction.z
            };
            ExecuteSkill(relativePosition);
        }
    }

    public void TryExecuteSkill(Vector2 position)
    {
        if (AbilityAuthorized)
        {
            RelativePosition relativePosition = new RelativePosition
            {
                X = position.x,
                Y = position.y
            };
            ExecuteSkill(relativePosition);
        }
    }

    private void ExecuteSkill(RelativePosition relativePosition)
    {
        if (AbilityAuthorized)
        {
            SendActionToBackend(relativePosition);
        }
    }

    public void ExecuteFeedback()
    {
        _movement.ChangeState(CharacterStates.MovementStates.Attacking);
        _animator.SetBool(skillId, true);

        StartCoroutine(EndSkillFeedback());
    }

    private void SendActionToBackend(RelativePosition relativePosition)
    {
        ClientAction action = new ClientAction
        {
            Action = serverSkill,
            Position = relativePosition
        };
        SocketConnectionManager.Instance.SendAction(action);
    }

    private IEnumerator EndSkillFeedback()
    {
        if (skillInfo)
        {
            yield return new WaitForSeconds(skillInfo.blockMovementTime);
        }
        _movement.ChangeState(CharacterStates.MovementStates.Idle);
        _animator.SetBool(skillId, false);
    }
}
