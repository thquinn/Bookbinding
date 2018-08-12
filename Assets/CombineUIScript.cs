using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;

public class CombineUIScript : MonoBehaviour {
    static float FADE_SPEED = .2f;
    public GameScript gameScript;
    public DictionaryScript dictionaryScript;
    public TextMeshProUGUI titlesDisplayText;
    public TMP_InputField inputField;
    public TextMeshProUGUI inputOverlay;
    CanvasGroup canvasGroup;

    BookScript book1, book2;

    // Use this for initialization
    void Start () {
        canvasGroup = GetComponent<CanvasGroup>();
        inputField.onValidateInput += ValidateInput;
        inputField.onValueChanged.AddListener(delegate { InputValueChanged(); });
    }

    // Update is called once per frame
    void Update () {
        float targetAlpha = gameScript.IsCombining() ? 1 : 0;
        if (canvasGroup.alpha != targetAlpha) {
            if (Mathf.Abs(canvasGroup.alpha - targetAlpha) <= FADE_SPEED) {
                canvasGroup.alpha = targetAlpha;
            }
            else if (canvasGroup.alpha < targetAlpha) {
                canvasGroup.alpha += FADE_SPEED;
            }
            else {
                canvasGroup.alpha -= FADE_SPEED;
            }
        }

        if (gameScript.IsCombining()) {
            inputField.ActivateInputField();
        } else {
            inputField.DeactivateInputField();
        }
    }

    private char ValidateInput(string text, int charIndex, char addedChar) {
        if (addedChar == '<' || addedChar == '>') {
            return '\0';
        }
        addedChar = Char.ToUpper(addedChar);
        return addedChar;
    }
    private void InputValueChanged() {
        int caret = inputField.caretPosition;
        string input = Regex.Replace(inputField.text, "<[^>]*>", "").ToUpper();
        int[] inputLetterCounts = new int[26];
        foreach (char c in input) {
            int i = (int)c - (int)'A';
            if (i < 0 || i >= 26) {
                continue;
            }
            inputLetterCounts[i]++;
        }
        string colorTitle1 = GetColorTitle(book1.title.ToUpper(), inputLetterCounts);
        string colorTitle2 = GetColorTitle(book2.title.ToUpper(), inputLetterCounts);
        titlesDisplayText.text = colorTitle1 + "<color=\"white\">\n" + colorTitle2;
        inputOverlay.text = GetColorAnagram(input, inputLetterCounts);
    }
    private string GetColorTitle(string title, int[] letterCounts) {
        StringBuilder sb = new StringBuilder();
        foreach (char c in title) {
            int charCode = (int)c - (int)'A';
            if (charCode < 0 || charCode >= 26) {
                sb.Append("<color=\"white\">");
            } else if (letterCounts[charCode] > 0) {
                sb.Append("<color=#808080>");
                letterCounts[charCode]--;
            } else {
                sb.Append("<color=\"white\">");
                letterCounts[charCode]--;
            }
            sb.Append(c);
        }
        return sb.ToString();
    }
    private string GetColorAnagram(string anagram, int[] letterCounts) {
        StringBuilder sb = new StringBuilder();
        for (int i = anagram.Length - 1; i >= 0; i--) {
            char c = anagram[i];
            sb.Insert(0, c);
            int charCode = (int)c - (int)'A';
            if (charCode < 0 || charCode >= 26) {
                continue;
            }
            if (letterCounts[charCode] > 0) {
                sb.Insert(0, "<color=#808080>");
                letterCounts[charCode]--;
            } else {
                sb.Insert(0, "<color=\"white\">");
            }
        }
        return sb.ToString();
    }

    public void SetBooks(BookScript book1, BookScript book2) {
        this.book1 = book1;
        this.book2 = book2;
        titlesDisplayText.text = book1.title.ToUpper() + '\n' + book2.title.ToUpper();
        inputField.text = "";
        inputOverlay.text = "";
    }
}
