using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor.Timeline.Actions;
using UnityEngine;

public class TextBlinkAnimation : MonoBehaviour {

    public float WaitTime = 0f;
    
    [SerializeField]private TextMeshProUGUI clickToPlayText;

    private void Start() {
        StartCoroutine(WaitAndBlink());
    }
    
    public IEnumerator WaitAndBlink() {
        float fontSize = clickToPlayText.fontSize;
        
        while (true) {
            fontSize = Mathf.Lerp(0, 32f, 0f);
            clickToPlayText.fontSize = fontSize;

            yield return new WaitForSeconds(WaitTime);

            fontSize = Mathf.Lerp(0, 32f, 1f);
            clickToPlayText.fontSize = fontSize;
            
            yield return new WaitForSeconds(WaitTime);
        }
    }
}
