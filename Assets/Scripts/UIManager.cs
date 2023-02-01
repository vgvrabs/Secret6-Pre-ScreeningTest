using System;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManager : MonoBehaviour {

    public GameObject GameUIPanel;
    public GameObject MainMenuPanel;
    public GameObject SettingsPanel;
    public GameObject EndGamePanel;
    public GameObject InfoPanel;
    public Toggle AutoSolveToggle;
    public Slider Slider;
    public TextMeshProUGUI BlockCountText;
    public TextMeshProUGUI TotalMovesText;
    public TextMeshProUGUI TotalTime;

    private GameManager gameManager;
    [SerializeField] private TextMeshProUGUI moveCountText;
    
    //private GameManager gameManager;

    private void OnEnable() {
        SingletonManager.Register(this);
    }

    private void OnDisable() {
        SingletonManager.Remove<UIManager>();
    }

    private void Start() {
        gameManager = SingletonManager.Get<GameManager>();
        MainMenuPanel.SetActive(true);
        SettingsPanel.SetActive(false);
        GameUIPanel.SetActive(false);
        EndGamePanel.SetActive(false);
        InfoPanel.SetActive(false);
        SetMoveCountText(0);
    }
    
    public void SetMoveCountText(int moveCount) {
        if(!moveCountText) return;
        
        StringBuilder stringBuilder = new StringBuilder();
        stringBuilder.Append("Move Count:");
        stringBuilder.Append(moveCount);
        
        moveCountText.text = stringBuilder.ToString();
    }

    public void OnClickSettingsButton() {
        if (!SettingsPanel) return;
        
        MainMenuPanel.SetActive(false);
        SettingsPanel.SetActive(true);
        AutoSolveToggle.isOn = gameManager.AutoSolve;
        Slider.value = gameManager.BlockCount;
    }

    public void OnClickExitSettingsButton() {
        if (!SettingsPanel) return; 
        
        SettingsPanel.SetActive(false);
        MainMenuPanel.SetActive(true);
    }

    public void OnClickAutoSolveButton() {
        gameManager.AutoSolve = AutoSolveToggle.isOn;
    }

    public void SetBlockCount() {
        if (!Slider) return;
        gameManager.BlockCount = (int)Slider.value;
        gameManager.BlockCount = Mathf.Clamp(gameManager.BlockCount, 1, 8);

        if (!BlockCountText) return;
        BlockCountText.text = gameManager.BlockCount.ToString();
    }

    public void OnClickInfoPanel() {
        if (!InfoPanel) return;
        InfoPanel.SetActive(true);
        MainMenuPanel.SetActive(false);
    }

    public void OnExitInfoPanel() {
        if (!InfoPanel) return;
        InfoPanel.SetActive(false);
        MainMenuPanel.SetActive(true);
    }

    public void SetEndGamePanel() {
        EndGamePanel.SetActive(true);
        GameUIPanel.SetActive(false);
        StringBuilder text = new();
        
        text.Append("Total Moves:");
        text.Append(gameManager.MoveCount);
        TotalMovesText.text = text.ToString();

        text.Clear();

        text.Append("Time Completed:");
        text.Append(Mathf.Round((Time.time - gameManager.TimeStarted) * 100f)/100f);
        text.Append("s");
        TotalTime.text = text.ToString();
    }

    public void OnClickReturnButton() {
        gameManager.PlayerData.IsAutoSolveOn = AutoSolveToggle.isOn;
        gameManager.PlayerData.SavedBlockCount = gameManager.BlockCount;
        SceneManager.LoadScene(0);
    }
}
