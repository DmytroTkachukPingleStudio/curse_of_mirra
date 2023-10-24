using System;
using System.Collections;
using MoreMountains.Tools;
using MoreMountains.TopDownEngine;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LobbiesManager : LevelSelector
{
    [SerializeField]
    public PlayerNameHandler playerNameHandler;
    public static LobbiesManager Instance;

    void Start()
    {
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
        Instance = this;
        this.SetPlayerName();
    }

    public override void GoToLevel()
    {
        base.GoToLevel();
        gameObject.GetComponent<MMTouchButton>().DisableButton();
    }

    public void CreateLobby()
    {
        StartCoroutine(WaitForLobbyCreation());
    }

    public void ConnectToLobby(string idHash)
    {
        StartCoroutine(WaitForLobbyJoin(idHash));
    }

    public void Back()
    {
        LobbyConnection.Instance.Init();
        SceneManager.LoadScene("Lobbies");
    }

    public void Refresh()
    {
        LobbyConnection.Instance.Refresh();
        this.GetComponent<UIManager>().RefreshLobbiesList();
    }

    public void QuickGame()
    {
        LobbyConnection.Instance.QuickGame();
        StartCoroutine(Utils.WaitForGameCreation(this.LevelName));
    }

    public IEnumerator WaitForLobbyCreation()
    {
        LobbyConnection.Instance.CreateLobby();
        yield return new WaitUntil(
            () =>
                !string.IsNullOrEmpty(LobbyConnection.Instance.LobbySession)
                && LobbyConnection.Instance.playerId != UInt64.MaxValue
        );
        SceneManager.LoadScene("Lobby");
    }

    public void Reconnect()
    {
        LobbyConnection.Instance.Reconnect();
        SceneManager.LoadScene("CharacterSelection");
    }

    public IEnumerator WaitForLobbyJoin(string idHash)
    {
        LobbyConnection.Instance.ConnectToLobby(idHash);
        yield return new WaitUntil(() => LobbyConnection.Instance.playerId != UInt64.MaxValue);
        SceneManager.LoadScene("Lobby");
    }

    private void SetPlayerName()
    {
        Debug.Log("Player name is: " + LobbyConnection.Instance.playerName);
        if (LobbyConnection.Instance.playerName == "")
        {
            Debug.Log("Player name is empty");
            this.playerNameHandler.Show();
        }
    }
}
