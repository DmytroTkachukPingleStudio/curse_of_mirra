using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New CoM Character", menuName = "CoM Character")]
public class CoMCharacter : ScriptableObject
{
    public new string name;
    public Sprite artWork;
    public Sprite selectedArtwork;
    public Sprite blockArtwork;
    public Sprite characterPlayer;
    public GameObject prefab;
    public Sprite skillBackground;
    public Color32 InputFeedbackColor;
    public List<SkillInfo> skillsInfo;
    public SkillInfo skillBasicInfo;
}
