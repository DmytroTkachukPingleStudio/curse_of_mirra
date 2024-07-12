using System.Collections;
using System.Collections.Generic;
using MoreMountains.TopDownEngine;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public enum UIControls
{
    Skill1,
    Skill2,
    Skill3
}

public enum UIIndicatorType
{
    Cone,
    Area,
    AreaWithDirection,
    Arrow,
    ShortArrow,
    WideArrow,
    TripleArrows,
    None
}

public enum UIType
{
    Tap,
    Area,
    Direction,
    Select
}

public class CustomInputManager : InputManager
{
    [SerializeField]
    Image Skill1Icon;

    [SerializeField]
    Image Skill2Icon;

    [SerializeField]
    Image Skill3Icon;

    [SerializeField]
    CustomMMTouchButton Skill1;

    [SerializeField]
    CustomMMTouchButton Skill2;

    [SerializeField]
    CustomMMTouchButton Skill3;

    [SerializeField]
    GameObject Skill1CooldownContainer;

    [SerializeField]
    GameObject Skill2CooldownContainer;

    [SerializeField]
    GameObject Skill3CooldownContainer;

    [SerializeField]
    GameObject UIControlsWrapper;

    Dictionary<UIControls, CustomMMTouchButton> mobileButtons;
    Dictionary<UIControls, GameObject> buttonsCooldown;
    private SkillIndicatorsManager skillIndicatorsManager;
    private CustomMMTouchJoystick activeJoystick;
    private Vector3 initialLeftJoystickPosition;
    private bool disarmed = false;

    private float currentSkillRadius = 0;
    private bool activeJoystickStatus = false;

    private bool canceled = false;
    private GameObject _player;

    Color32 characterSkillColor;

    public Material material;

    protected override void Start()
    {
        base.Start();
        mobileButtons = new Dictionary<UIControls, CustomMMTouchButton>();
        mobileButtons.Add(UIControls.Skill1, Skill1);
        mobileButtons.Add(UIControls.Skill2, Skill2);
        mobileButtons.Add(UIControls.Skill3, Skill3);

        // TODO: this could be refactored implementing a button parent linking button and cooldown text
        // or extending CustomMMTouchButton and linking its cooldown text
        buttonsCooldown = new Dictionary<UIControls, GameObject>();
        buttonsCooldown.Add(UIControls.Skill1, Skill1CooldownContainer);
        buttonsCooldown.Add(UIControls.Skill2, Skill2CooldownContainer);
        buttonsCooldown.Add(UIControls.Skill3, Skill3CooldownContainer);

        UIControlsWrapper.GetComponent<CanvasGroup>().alpha = 0;
    }

    public void Setup()
    {
        _player = Utils.GetPlayer(GameServerConnectionManager.Instance.playerId);
        skillIndicatorsManager = _player.GetComponentInChildren<SkillIndicatorsManager>();
    }

    public void InitializeInputSprite(CoMCharacter characterInfo)
    {
        Skill1Icon.sprite = characterInfo.skillsInfo[0].skillSprite;
        Skill2Icon.sprite = characterInfo.skillsInfo[1].skillSprite;
        Skill3Icon.sprite = characterInfo.skillsInfo[2].skillSprite;
        characterSkillColor = characterInfo.InputFeedbackColor;
    }

    public IEnumerator ShowInputs()
    {
        yield return new WaitForSeconds(.1f);

        UIControlsWrapper.GetComponent<CanvasGroup>().alpha = 1;
    }

    public void AssignSkillToInput(UIControls trigger, UIType triggerType, Skill skill)
    {
        CustomMMTouchButton button = mobileButtons[trigger];
        CustomMMTouchJoystick joystick = button.GetComponent<CustomMMTouchJoystick>();

        switch (triggerType)
        {
            case UIType.Tap:
                // button.ButtonReleased.AddListener(skill.TryExecuteSkill);
                if (joystick)
                {
                    joystick.enabled = false;
                }
                MapTapInputEvents(button, skill);
                break;

            case UIType.Area:
                if (joystick)
                {
                    joystick.enabled = true;
                }
                MapAreaInputEvents(joystick, skill);
                break;

            case UIType.Direction:
                if (joystick)
                {
                    joystick.enabled = true;
                }
                MapDirectionInputEvents(button, skill);
                break;
        }
    }

    private void MapAreaInputEvents(CustomMMTouchJoystick joystick, Skill skill)
    {
        UnityEvent<CustomMMTouchJoystick> aoeEvent = new UnityEvent<CustomMMTouchJoystick>();
        aoeEvent.AddListener(ShowAimAoeSkill);
        joystick.newPointerDownEvent = aoeEvent;

        UnityEvent<Vector2, CustomMMTouchJoystick> aoeDragEvent =
            new UnityEvent<Vector2, CustomMMTouchJoystick>();
        aoeDragEvent.AddListener(AimAoeSkill);
        joystick.newDragEvent = aoeDragEvent;

        UnityEvent<Vector2, Skill> aoeRelease = new UnityEvent<Vector2, Skill>();
        aoeRelease.AddListener(ExecuteAoeSkill);
        joystick.skill = skill;
        joystick.newPointerUpEvent = aoeRelease;
    }

    private void MapTapInputEvents(CustomMMTouchButton button, Skill skill)
    {
        button.skill = skill;

        UnityEvent<Skill> aoeEvent = new UnityEvent<Skill>();
        aoeEvent.AddListener(ShowTapSkill);
        button.newPointerTapDown = aoeEvent;

        UnityEvent<Skill> tapRelease = new UnityEvent<Skill>();
        tapRelease.AddListener(ExecuteTapSkill);
        button.newPointerTapUp = tapRelease;
    }

    public void ShowTapSkill(Skill skill)
    {
        skillIndicatorsManager.InitIndicator(skill, characterSkillColor);
        skillIndicatorsManager.ActivateIndicator(skill.GetIndicatorType());
        skillIndicatorsManager.SetSkillRage(skill.GetUIType());
    }

    public void ShowAimAoeSkill(CustomMMTouchJoystick joystick)
    {
        skillIndicatorsManager.InitIndicator(joystick.skill, characterSkillColor);

        // FIXME: Using harcoded value for testing, Value should be set dinamically
        //TODO : Add the spread area (amgle) depeding of the skill.json
        activeJoystick = joystick;
    }

    public void AimAoeSkill(Vector2 aoePosition, CustomMMTouchJoystick joystick)
    {
        if (joystick.skill.GetIndicatorType() == UIIndicatorType.Area)
        {
            SetPositionToAOEIndicator(joystick.skill, aoePosition, skillIndicatorsManager.aoe);
        }
        if (joystick.skill.GetIndicatorType() == UIIndicatorType.AreaWithDirection)
        {
            SetPositionToAOEIndicator(joystick.skill, aoePosition, skillIndicatorsManager.aoeDirection);
        }
        activeJoystickStatus = canceled;

        skillIndicatorsManager.Rotate(aoePosition.x, aoePosition.y, joystick.skill);
    }
    private void SetPositionToAOEIndicator(Skill skill, Vector2 aoePosition, GameObject aoeIndicator)
    {
        //Multiply vector values according to the scale of the animation (in this case 12)
        float multiplier = skill.GetSkillRange();
        aoeIndicator.transform.localPosition = new Vector3(
                aoePosition.x * multiplier,
                0,
                aoePosition.y * multiplier
            );
    }

    public void ExecuteAoeSkill(Vector2 aoePosition, Skill skill)
    {
        skillIndicatorsManager.DeactivateIndicator();
        skillIndicatorsManager.UnsetSkillRage(skill.GetUIType());

        activeJoystick = null;
        EnableButtons();

        if (!canceled)
        {
            skill.TryExecuteSkill(aoePosition);
        }
    }

    public void ExecuteTapSkill(Skill skill)
    {
        if (!canceled)
        {
            skill.TryExecuteSkill();
        }

        skillIndicatorsManager.DeactivateIndicator();
        skillIndicatorsManager.UnsetSkillRage(skill.GetUIType());
    }

    private void MapDirectionInputEvents(CustomMMTouchButton button, Skill skill)
    {
        CustomMMTouchJoystick joystick = button.GetComponent<CustomMMTouchJoystick>();
        UnityEvent<CustomMMTouchJoystick> directionEvent = new UnityEvent<CustomMMTouchJoystick>();
        directionEvent.AddListener(ShowAimDirectionSkill);
        joystick.newPointerDownEvent = directionEvent;

        UnityEvent<Vector2, CustomMMTouchJoystick> directionDragEvent =
            new UnityEvent<Vector2, CustomMMTouchJoystick>();
        directionDragEvent.AddListener(AimDirectionSkill);
        joystick.newDragEvent = directionDragEvent;

        UnityEvent<Vector2, Skill> directionRelease = new UnityEvent<Vector2, Skill>();
        directionRelease.AddListener(ExecuteDirectionSkill);
        joystick.skill = skill;
        joystick.newPointerUpEvent = directionRelease;

        button.skill = skill;

        UnityEvent<Skill> aoeEvent = new UnityEvent<Skill>();
        aoeEvent.AddListener(ShowAimDirectionTargetsSkill);
        button.newPointerTapDown = aoeEvent;
    }

    private void ShowAimDirectionSkill(CustomMMTouchJoystick joystick)
    {
        skillIndicatorsManager.InitIndicator(joystick.skill, characterSkillColor);

        activeJoystick = joystick;
    }

    private void ShowAimDirectionTargetsSkill(Skill skill)
    {
        skillIndicatorsManager.InitIndicator(skill, characterSkillColor);
    }

    private void AimDirectionSkill(Vector2 direction, CustomMMTouchJoystick joystick)
    {
        if (!canceled)
        {
            skillIndicatorsManager.Rotate(direction.x, direction.y, joystick.skill);
            skillIndicatorsManager.ActivateIndicator(joystick.skill.GetIndicatorType());
            skillIndicatorsManager.SetSkillRage(joystick.skill.GetUIType());
        }
        activeJoystickStatus = canceled;
    }

    private void ExecuteDirectionSkill(Vector2 direction, Skill skill)
    {
        skillIndicatorsManager.DeactivateIndicator();
        skillIndicatorsManager.UnsetSkillRage(skill.GetUIType());

        activeJoystick = null;
        EnableButtons();

        if (!canceled)
        {
            skill.TryExecuteSkill(direction);
        }
    }

    public void CheckSkillCooldown(UIControls control, float cooldown, bool useCooldown)
    {
        CustomMMTouchButton button = mobileButtons[control];
        GameObject cooldownContainer = buttonsCooldown[control];
        TMP_Text cooldownText = cooldownContainer.GetComponentInChildren<TMP_Text>();
        if (useCooldown)
        {
            if (cooldown > 0f)
            {
                button.DisableButton();
                cooldownContainer.SetActive(true);
                cooldownText.text = ((ulong)cooldown + 1).ToString();
            }
            else
            {
                button.EnableButton();
                cooldownContainer.SetActive(false);
            }
        }
        else
        {
            cooldownContainer.gameObject.SetActive(false);
            button.EnableButton();
        }
    }

    private void DisableButtons()
    {
        foreach (var (key, button) in mobileButtons)
        {
            if (button != activeJoystick)
            {
                button.GetComponent<CustomMMTouchButton>().Interactable = false;
            }
        }
    }

    private void EnableButtons()
    {
        foreach (var (key, button) in mobileButtons)
        {
            button.GetComponent<CustomMMTouchButton>().Interactable = true;
        }
    }

    public void SetCanceled(bool cancelValue, bool dragged, UIIndicatorType indicatorType, UIType uiType)
    {
        canceled = cancelValue;
        if (skillIndicatorsManager && cancelValue && !dragged)
        {
            skillIndicatorsManager.DeactivateIndicator();
            skillIndicatorsManager.UnsetSkillRage(uiType);
        }
        else if (skillIndicatorsManager && !cancelValue && dragged)
        {
            skillIndicatorsManager.ActivateIndicator(indicatorType);
            skillIndicatorsManager.SetSkillRage(uiType);
        }
    }

    private bool IsSamePlayer(GameObject player)
    {
        return player.name == _player.name;
    }
}
