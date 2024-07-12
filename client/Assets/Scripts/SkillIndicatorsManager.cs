using MoreMountains.Tools;
using UnityEngine;

public class SkillIndicatorsManager : MonoBehaviour
{
    public bool hasCone;
    [MMCondition("hasCone", true)]
    [SerializeField]
    public GameObject cone;
    [MMCondition("hasCone", true)]
    public SectorController cone_controller;
    public bool hasBasicArrow;
    [MMCondition("hasBasicArrow", true)]
    [SerializeField]
    public GameObject basicArrow;
    public bool hasShortArrow;
    [MMCondition("hasShortArrow", true)]
    [SerializeField]
    public GameObject shortArrow;
    public bool hasWideArrow;
    [MMCondition("hasWideArrow", true)]
    [SerializeField]
    public GameObject wideArrow;
    public bool hasTripleArrows;
    [MMCondition("hasTripleArrows", true)]
    [SerializeField]
    public GameObject tripleArrows;
    public bool hasAOE;
    [MMCondition("hasAOE", true)]
    [SerializeField]
    public GameObject aoe;
    public bool hasAOEWithDirection;
    [MMCondition("hasAOEWithDirection", true)]
    [SerializeField]
    public GameObject aoeDirection;
    public bool hasRangeArea;
    [MMCondition("hasRangeArea", true)]
    [SerializeField]
    GameObject rangeArea;
    private Vector3 initialPosition;

    UIIndicatorType activeIndicator = UIIndicatorType.None;

    public float fov = 90f;
    public float skillAngle;
    public int rayCount = 50;
    public float angleIncrease;
    private float hitbox, skillRangeArea, arrowWidth;

    void Awake()
    {
        hitbox = (Utils.GetGamePlayer(GameServerConnectionManager.Instance.playerId).Radius / 100) * 2;
    }

    public void InitIndicator(Skill skill, Color32 color)
    {
        // TODO: Add the spread aoe (angle) depending of the skill.json
        skillRangeArea = skill.GetSkillRange();
        arrowWidth = skill.GetArroWidth();
        skillAngle = skill.GetAngle();
        fov = skill.GetIndicatorAngle();
        activeIndicator = skill.GetIndicatorType();
        initialPosition = transform.localPosition;

        SetArrows(skill);
        SetConeIndicator();

        if (activeIndicator == UIIndicatorType.Area)
        {
            SetAoeIndicator(skill, aoe);
        }
        if (activeIndicator == UIIndicatorType.AreaWithDirection)
        {
            SetAoeIndicator(skill, aoeDirection);
        }
        if (hasRangeArea)
        {
            rangeArea.transform.localScale = new Vector3(skillRangeArea, skillRangeArea, skillRangeArea);
        }
    }

    private void SetAoeIndicator(Skill skill, GameObject aoeIndicator)
    {
        float areaRadius = skill.GetSkillAreaRadius();
        if (skill.GetSkillInfo().usesHitboxAsArea)
        {
            aoeIndicator.transform.localScale = new Vector3(hitbox, 0, hitbox);
        }
        else if (areaRadius == 0)
        {
            aoeIndicator.transform.localScale = new Vector3(skillRangeArea, 0, skillRangeArea);
        }
        else
        {
            aoeIndicator.transform.localScale = new Vector3(areaRadius, 0, areaRadius);
        }
    }

    private void SetArrows(Skill skill)
    {
        if (activeIndicator == UIIndicatorType.Arrow)
        {
            SetArrowTransformValues(skill, basicArrow);
        }
        if (activeIndicator == UIIndicatorType.TripleArrows)
        {
            SetArrowTransformValues(skill, tripleArrows);
        }
        if (activeIndicator == UIIndicatorType.WideArrow)
        {
            SetArrowTransformValues(skill, wideArrow);
        }
        if (activeIndicator == UIIndicatorType.ShortArrow)
        {
            SetArrowTransformValues(skill, shortArrow);
        }
    }

    private void SetArrowTransformValues(Skill skill, GameObject arrow)
    {
        float scaleY = 1;
        if (activeIndicator == UIIndicatorType.ShortArrow)
        {
            skillRangeArea = skillRangeArea + skill.GetSkillOffset() - scaleY;
            arrow.transform.localPosition = new Vector3(0, -scaleY * 2, 0);
        }
        else
        {
            arrow.transform.localPosition = new Vector3(0, -scaleY / 2, 0);
        }
        arrow.transform.localScale = new Vector3(arrowWidth, scaleY, skillRangeArea);
    }

    public void Rotate(float x, float y, Skill skill)
    {
        var result = Mathf.Atan(x / y) * Mathf.Rad2Deg;
        float xRotation = 180;
        float zRotation = -180;
        if (y >= 0)
        {
            result += 180f;
            xRotation = 180;
            zRotation = -180f;
        }

        if (skill.GetUIType() == UIType.Direction)
        {
            transform.rotation = Quaternion.Euler(
                90f,
                result,
                activeIndicator == UIIndicatorType.Cone
                    ? -(180 - fov) / 2
                    : 0
            );
        }
        if (skill.GetIndicatorType() == UIIndicatorType.AreaWithDirection)
        {
            aoeDirection.transform.rotation = Quaternion.Euler(xRotation, result - 180, zRotation);
        }
    }

    public void SetConeIndicator()
    {
        if (activeIndicator == UIIndicatorType.Cone)
        {
            cone_controller.SetSectorDegree(fov);

            float coneIndicatorAngle = 0;
            angleIncrease = fov / rayCount;
            Mesh mesh = new Mesh();
            Vector3 origin = Vector3.zero;

            Vector3[] vertices = new Vector3[rayCount + 1 + 1];
            Vector2[] uv = new Vector2[vertices.Length];
            int[] triangles = new int[rayCount * 3];

            vertices[0] = origin;
            int vertexIndex = 1;
            int trianglesIndex = 0;

            for (int i = 0; i < rayCount; i++)
            {
                Vector3 vertex = origin + GetVectorFromAngle(coneIndicatorAngle) * skillRangeArea;
                vertices[vertexIndex] = vertex;

                if (i > 0)
                {
                    triangles[trianglesIndex + 0] = 0;
                    triangles[trianglesIndex + 1] = vertexIndex - 1;
                    triangles[trianglesIndex + 2] = vertexIndex;
                    trianglesIndex += 3;
                }
                vertexIndex++;
                coneIndicatorAngle -= angleIncrease;
            }

            triangles[0] = 0;
            triangles[1] = 1;
            triangles[2] = 2;

            mesh.vertices = vertices;
            mesh.uv = uv;
            mesh.triangles = triangles;
            cone.GetComponent<MeshFilter>().mesh = mesh;
        }
    }

    private Vector3 GetBisectorDirection()
    {
        Mesh coneMesh = cone.GetComponent<MeshFilter>().mesh;
        Vector3[] coneVertices = coneMesh.vertices;

        // Calculate the vertices of the triangle
        Vector3 vertexA = cone.transform.TransformPoint(coneVertices[0]);
        Vector3 vertexB = cone.transform.TransformPoint(coneVertices[1]);
        Vector3 vertexC = cone.transform.TransformPoint(coneVertices[coneVertices.Length - 2]); // That is the last vertex

        // Calculate the sides of the triangle
        Vector3 sideAB = vertexB - vertexA;
        Vector3 sideAC = vertexC - vertexA;

        // return the bisector direction of the triangle's vertex angle
        return (sideAB.normalized + sideAC.normalized).normalized;
    }

    public Vector3 GetVectorFromAngle(float angle)
    {
        float angleRad = angle * (Mathf.PI / 180f);
        return new Vector3(Mathf.Cos(angleRad), Mathf.Sin(angleRad));
    }
    public void SetSkillRage(UIType uiType)
    {
        switch (uiType)
        {
            case UIType.Area:
                rangeArea.SetActive(true);
                break;
        }
    }
    public void UnsetSkillRage(UIType uiType)
    {
        switch (uiType)
        {
            case UIType.Area:
                rangeArea.SetActive(false);
                break;
        }
    }

    public void ActivateIndicator(UIIndicatorType indicatorType)
    {
        switch (indicatorType)
        {
            case UIIndicatorType.Cone:
                cone_controller.gameObject.SetActive(true);
                cone.SetActive(true);
                break;
            case UIIndicatorType.Arrow:
                basicArrow.SetActive(true);
                break;
            case UIIndicatorType.ShortArrow:
                shortArrow.SetActive(true);
                break;
            case UIIndicatorType.WideArrow:
                wideArrow.SetActive(true);
                break;
            case UIIndicatorType.TripleArrows:
                tripleArrows.SetActive(true);
                break;
            case UIIndicatorType.Area:
                aoe.SetActive(true);
                break;
            case UIIndicatorType.AreaWithDirection:
                aoeDirection.SetActive(true);
                break;
        }
    }

    public void DeactivateIndicator()
    {
        switch (activeIndicator)
        {
            case UIIndicatorType.Cone:
                cone_controller.gameObject.SetActive(false);
                cone.SetActive(false);
                break;
            case UIIndicatorType.Arrow:
                basicArrow.SetActive(false);
                break;
            case UIIndicatorType.ShortArrow:
                shortArrow.SetActive(false);
                break;
            case UIIndicatorType.WideArrow:
                wideArrow.SetActive(false);
                break;
            case UIIndicatorType.TripleArrows:
                tripleArrows.SetActive(false);
                break;
            case UIIndicatorType.Area:
                aoe.SetActive(false);
                break;
            case UIIndicatorType.AreaWithDirection:
                aoeDirection.SetActive(false);
                break;
        }
        Reset();
    }

    private void Reset()
    {
        transform.rotation = Quaternion.Euler(new Vector3(0f, 0, 0));
        transform.localPosition = initialPosition;
        if (hasAOE)
        {
            aoe.transform.localPosition = initialPosition;

        }
        if (hasAOEWithDirection)
        {
            aoeDirection.transform.localPosition = initialPosition;
        }
    }
}
