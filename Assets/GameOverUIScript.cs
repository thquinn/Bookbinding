using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameOverUIScript : MonoBehaviour {
    static float FADE_SPEED = .05f;
    public GameScript gameScript;
    public TextMeshProUGUI scoreText;
    CanvasGroup canvasGroup;

    // Use this for initialization
    void Start () {
        canvasGroup = GetComponent<CanvasGroup>();
    }
	
	// Update is called once per frame
	void Update () {
		if (gameScript.gameOver && canvasGroup.alpha < 1) {
            scoreText.text = "Your IQ is " + gameScript.score + ".";
            canvasGroup.alpha = Mathf.Min(1, canvasGroup.alpha + FADE_SPEED);
        }
	}
}
