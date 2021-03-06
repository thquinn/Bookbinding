﻿using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SwapUIScript : MonoBehaviour {
    static float FADE_SPEED = .2f;
    public GameScript gameScript;
    public TextMeshProUGUI titleDisplayText, scorePoints, help;
    public GameObject selector;
    CanvasGroup canvasGroup;

    // Use this for initialization
    void Start() {
        canvasGroup = GetComponent<CanvasGroup>();
    }

    // Update is called once per frame
    void Update() {
        float targetAlpha = (gameScript.IsCombining() || gameScript.gameOver) ? 0 : 1;
        if (gameScript.FirstIsReady() && canvasGroup.alpha != targetAlpha) {
            if (Mathf.Abs(canvasGroup.alpha - targetAlpha) <= FADE_SPEED) {
                canvasGroup.alpha = targetAlpha;
            }
            else if (canvasGroup.alpha < targetAlpha) {
                canvasGroup.alpha += FADE_SPEED;
            }
            else {
                canvasGroup.alpha -= FADE_SPEED;
            }
            selector.GetComponentInChildren<Renderer>().material.color = new Color(1, 0, 0, canvasGroup.alpha);
        }

        // Move selector.
        float selectorTargetX = gameScript.GetSelectorX();
        selector.transform.Translate((selectorTargetX - selector.transform.position.x) * .5f, 0, 0);

        titleDisplayText.SetText(gameScript.GetSelectedTitle());

        if (gameScript.score > 100) {
            help.enabled = false;
        }
    }

    public void SetScore(float iq) {
        scorePoints.text = iq.ToString("n2") + " IQ";
    }
}