using System;
using System.Collections;
using MoreMountains.Tools;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;

public class MainScreenManager : MonoBehaviour
{
    private const string BATTLE_SCENE_NAME = "Battle";
    private const string LOBBY_SCENE_NAME = "Lobby";
    private const string MAIN_SCENE_NAME = "MainScreen";
    private const string CHARACTER_INFO_SCENE_NAME = "CharacterInfo";
    private const string TITLE_SCENE_NAME = "TitleScreen";

    [SerializeField]
    UIModelManager modelManager;

    [SerializeField]
    TextMeshProUGUI playerName,
        currentPlayerName,
        trophiesAmount, 
        goldAmount, 
        gemsAmount,
        bountyRewardText;

    string characterNameToGo;
    public static int bountyReward;
    
    [SerializeField] GameObject playerNamePopUp;
    [SerializeField] CanvasGroup bountyRewardCG;

    [SerializeField] 
    RectTransform bountyRewardRT,
        goldContainerRT;

    void Start()
    {
        CharactersManager
            .Instance
            .SetGoToCharacter(ServerConnection.Instance.selectedCharacterName);
        characterNameToGo = ServerConnection.Instance.selectedCharacterName;
        modelManager.SetModel(characterNameToGo);
        playerName.text = PlayerPrefs.GetString("playerName");
        
        if (PlayerPrefs.GetString("playerName") == "")
        {
            ShowPlayerNamePopUp();
        }
        
        currentPlayerName.text = "Current name: " + PlayerPrefs.GetString("playerName");
        SetPlayerInfo();
    }

    public void GoToCharacteInfo()
    {
        Utils.GoToCharacterInfo(characterNameToGo, CHARACTER_INFO_SCENE_NAME);
    }

    public void ShowPlayerNamePopUp()
    {
        playerNamePopUp.SetActive(true);
        StartCoroutine(FadeIn(playerNamePopUp.GetComponent<CanvasGroup>(), 0.6f, 0f));
    }

    IEnumerator FadeIn(CanvasGroup element, float time, float delay)
    {
        yield return new WaitForSeconds(delay);

        for (float i = 0; i <= 1; i += Time.deltaTime / time)
        {
            element.alpha = i;
            yield return null;
        }
    }

    private void SetPlayerInfo() {
        StartCoroutine(
            ServerUtils.GetUserInformation(
                response =>
                {
                    SetCurrencies(response.currencies);
                },
                error =>
                {
                    Errors.Instance.HandleNetworkError("Error", error);
                }
            )
        );
    }

    private void SetCurrencies(ServerUtils.Currency[] currencies) {
        foreach (var currency in currencies) {
            switch (currency.currency.name) {
                case "Gold": 
                    MaybeShowBountyRewardAnimation(currency.amount);
                    break;
                default:
                    gemsAmount.text = currency.amount + "";
                    break;
            }
        }
    }

    private void MaybeShowBountyRewardAnimation(int totalGold) {
        if (bountyReward > 0) {
            BountyRewardAnimation(totalGold);
        }
        else {
            goldAmount.text = totalGold + "";
        }
    }

    public void BountyRewardAnimation(int totalGold)
    {
        goldAmount.text = (totalGold - bountyReward).ToString();
        bountyRewardText.text = "+" + bountyReward;

        Vector3[] path = new Vector3[]
        {
            bountyRewardRT.position,
            bountyRewardRT.position + new Vector3(-0.8f, 6f, 0f),
            bountyRewardRT.position + new Vector3(0f, 9f, 0f),
            bountyRewardRT.position + new Vector3(1.5f, 12f, 0f),
            goldContainerRT.position + new Vector3(0f, -1.5f, 0f)
        };

        Sequence mainSequence = DOTween.Sequence();

        mainSequence.Append(bountyRewardRT.DOPath(path, 1.2f, PathType.CatmullRom)
            .SetEase(Ease.InOutQuad));

        mainSequence.Append(bountyRewardRT.DOScale(1.3f, 0.5f).SetEase(Ease.OutQuad))
            .Append(bountyRewardRT.DOScale(1f, 0.5f).SetEase(Ease.InQuad))
            .Append(bountyRewardCG.DOFade(0f, 0.7f));

        Sequence goldIncrementSequence = DOTween.Sequence();
        goldIncrementSequence.AppendInterval(1.5f);

        for (int i = 0; i < bountyReward; i++)
        {
            goldIncrementSequence.AppendCallback(() =>
            {
                goldAmount.text = (int.Parse(goldAmount.text) + 1).ToString();
            });
            goldIncrementSequence.AppendInterval(0.05f);
        }

        goldIncrementSequence.OnComplete(() => bountyReward = 0);

        mainSequence.Play();
        goldIncrementSequence.Play();
    }

}
