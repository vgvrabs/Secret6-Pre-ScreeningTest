using System;
using System.Text;
using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour {

    public GameObject GameUIPanel;
    public  GameObject MainMenuPanel;
    [SerializeField] private TextMeshProUGUI moveCountText;
    
    //private GameManager gameManager;

    private void OnEnable() {
        SingletonManager.Register(this);
    }

    private void OnDisable() {
        SingletonManager.Remove<UIManager>();
    }

    private void Start(){
        SetMoveCountText(0);
    }
    
    public void SetMoveCountText(int moveCount) {
        StringBuilder stringBuilder = new StringBuilder();
        stringBuilder.Append("Move Count:");
        stringBuilder.Append(moveCount);
        
        moveCountText.text = stringBuilder.ToString();
    }
}
