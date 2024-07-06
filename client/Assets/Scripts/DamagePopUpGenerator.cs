using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DamagePopUpGenerator : MonoBehaviour
{
    public float timeDestroy = 1f;
    [SerializeField] GameObject damagePopUpPrefab;
    Color redColor;
    Color yellowColor;
    Color whiteColor = Color.white;

    public void GenerateDamagePopUp(Vector3 positon, string text, string damageType) {
        Vector3 randomness = new Vector3(Random.Range(0f, 0.5f), 0f, 0f);
        var damagePopUp = Instantiate(damagePopUpPrefab, transform.position + randomness, Quaternion.identity, transform);
        TextMeshProUGUI damagePopUpText = damagePopUp.GetComponent<TextMeshProUGUI>();
        SetTextColor(damagePopUpText, damageType);
        damagePopUpText.text = text;
        Destroy(damagePopUp, timeDestroy);
    }

    private void SetTextColor(TextMeshProUGUI damagePopUpText, string damageType) {
        ColorUtility.TryParseHtmlString("#FF3F35", out redColor);
        ColorUtility.TryParseHtmlString("#FFC956", out yellowColor);

        switch (damageType) {
            case "light": {
                damagePopUpText.color = whiteColor;
                break;
            }
            case "medium": {
                damagePopUpText.color = yellowColor;
                break;
            }
            case "heavy": {
                damagePopUpText.color = redColor;
                break;
            }
        }

    }
}
