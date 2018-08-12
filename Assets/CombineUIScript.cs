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
    public TextMeshProUGUI redWarn, greenWarn;
    public bool isRed, isGreen, isValid;
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
        isRed = false;
        isGreen = false;
        isValid = false;
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
        bool isAllZero = true;
        foreach (int i in inputLetterCounts) {
            if (i != 0) {
                isAllZero = false;
                break;
            }
        }
        redWarn.gameObject.SetActive(isRed);
        greenWarn.gameObject.SetActive(isGreen);
        isValid = !isRed && !isGreen && isAllZero;
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
        // Grey out unavailable letters.
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
        string tagged = sb.ToString();

        // Highlight invalid and already-used words.
        HashSet<string> usedWords = new HashSet<string>();
        Regex nonAlpha = new Regex("[^A-Z]");
        foreach (string usedWord in (book1.title + " " + book2.title).Split(' ')) {
            usedWords.Add(nonAlpha.Replace(usedWord, ""));
        }
        string[] rawTokens = inputField.text.Split(' ');
        string[] coloredTokens = tagged.Split(' ');
        Debug.Assert(rawTokens.Length == coloredTokens.Length);
        string[] finishedTokens = new string[rawTokens.Length];
        for (int i = 0; i < rawTokens.Length; i++) {
            string word = rawTokens[i];
            if (word.Length == 0) {
                continue;
            }
            string alphaWord = nonAlpha.Replace(word, "");
            if (!gameScript.dictionaryScript.IsRealWord(alphaWord)) {
                finishedTokens[i] = "<mark=#ff000020>" + coloredTokens[i] + "</mark>";
                isRed = true;
            } else if (usedWords.Contains(alphaWord)) {
                finishedTokens[i] = "<mark=#ffff0020>" + coloredTokens[i] + "</mark>";
                isGreen = true;
            } else {
                finishedTokens[i] = coloredTokens[i];
                usedWords.Add(alphaWord);
            }
        }
        return string.Join(" ", finishedTokens);
    }

    public void SetBooks(BookScript book1, BookScript book2) {
        isRed = false;
        isGreen = false;
        isValid = false;
        this.book1 = book1;
        this.book2 = book2;
        titlesDisplayText.text = book1.title.ToUpper() + '\n' + book2.title.ToUpper();
        inputField.text = "";
        inputOverlay.text = "";
    }
}
