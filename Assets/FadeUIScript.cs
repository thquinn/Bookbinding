using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FadeUIScript : MonoBehaviour {
    static float FADE_SPEED = .1f;
    public bool fadeOut;
    public CanvasGroup canvasGroup;

    // Use this for initialization
    void Start () {
        canvasGroup.alpha = 1;
        fadeOut = false;
	}
	
	// Update is called once per frame
	void Update () {
        if (!fadeOut && canvasGroup.alpha > 0) {
            canvasGroup.alpha = Mathf.Max(0, canvasGroup.alpha - FADE_SPEED);
        }
        if (fadeOut) {
            canvasGroup.alpha += FADE_SPEED;
            if (canvasGroup.alpha == 1) {
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            }
        }
	}
}
