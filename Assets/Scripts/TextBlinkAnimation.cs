using System.Collections;
using TMPro;
using UnityEngine;

public class TextBlinkAnimation : MonoBehaviour {

    public float WaitTime = 0f;
    
    [SerializeField]private TextMeshProUGUI clickToPlayText;

    private void Start() { 
        StartCoroutine(WaitAndBlink());
    }

    private void OnEnable() {
        StartCoroutine(WaitAndBlink());
    }

    private void OnDisable() {
        StopCoroutine(WaitAndBlink());
    }

    public IEnumerator WaitAndBlink() {
        float fontSize = clickToPlayText.fontSize;
        
        while (true) {
            fontSize = Mathf.Lerp(0, 70f, 0f);
            clickToPlayText.fontSize = fontSize;

            yield return new WaitForSeconds(WaitTime);

            fontSize = Mathf.Lerp(0, 70f, 1f);
            clickToPlayText.fontSize = fontSize;
            
            yield return new WaitForSeconds(WaitTime);
        }
    }
}
