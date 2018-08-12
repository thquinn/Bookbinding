using System.Collections.Generic;
using System.Globalization;
using System.Threading;
using TMPro;
using UnityEngine;

public class BookScript : MonoBehaviour {
    public static TMP_FontAsset[] fonts;
    static TextInfo textInfo = new CultureInfo("en-US", false).TextInfo;
    static HashSet<string> noAllCapsFontNames = new HashSet<string>() { "AlexBrush-Regular SDF", "Allura-Regular SDF", "Arizonia-Regular SDF", "blackjack SDF", "DancingScript-Regular SDF", "GrandHotel-Regular SDF", "GreatVibes-Regular SDF", "Lobster_1", "Pacifico SDF", "Sofia-Regular SDF" };
    public GameObject tweenParent, model, canvas, textMesh;
    public string title;
    public HashSet<string> usedWords = new HashSet<string>();
    bool ready;
    float xTweenStart, xTweenTime = 1, xTweenSpeed;
    bool zTweenFront;

    void Start () {
        ready = false;
	}
    public bool IsReady() {
        return ready;
    }
	
	void Update () {
        if (xTweenTime < 1) {
            xTweenTime = Mathf.Min(1, xTweenTime + xTweenSpeed);
            if (xTweenTime == 1) {
                ready = true;
            }
            float x = EasingFunction.EaseInOutQuad(xTweenStart, 0, xTweenTime);
            float z = EasingFunction.EaseInOutQuad(0, 1, xTweenTime);
            if (z > .5) {
                z = 1 - z;
            }
            z *= 2;
            float y = zTweenFront ? z : 0;
            float xRot = zTweenFront ? z * -5 : 0;
            z *= zTweenFront ? -7.5f : 0;
            tweenParent.transform.localPosition = new Vector3(x, y, z);
            tweenParent.transform.localRotation = Quaternion.Euler(xRot, 0, 0);
        }
	}

    public void SetTitle(string title) {
        this.title = title;
        foreach (string s in title.ToUpper().Split(' ')) {
            usedWords.Add(s);
        }
        TMP_FontAsset font = fonts[Random.Range(0, fonts.Length)];
        TextMeshProUGUI ugui = textMesh.GetComponent<TextMeshProUGUI>();
        bool canBeAllCaps = !noAllCapsFontNames.Contains(font.name);
        string capitalizedTitle = (Random.value < .166 && canBeAllCaps) ? title.ToUpper() : textInfo.ToTitleCase(title.ToLower());
        ugui.SetText(capitalizedTitle);
        ugui.font = font;
    }
    public void SetScale(float x, float y) {
        model.transform.localScale = new Vector3(x, y, 1);
        gameObject.transform.Translate((x - 1) / 2f, -4 * (1 - y), 0);

        RectTransform rectTransform = canvas.GetComponent<RectTransform>();
        float canvasWidth = rectTransform.sizeDelta.x * y;
        rectTransform.sizeDelta = new Vector2(canvasWidth, rectTransform.sizeDelta.y);
        textMesh.GetComponent<RectTransform>().sizeDelta = rectTransform.sizeDelta;
    }
    public void SetJacketColor(Color color) {
        foreach (Renderer renderer in model.GetComponentsInChildren<Renderer>()) {
            if (renderer.material.name.StartsWith("PaperMaterial")) {
                continue;
            }
            renderer.material.color = color;
        }
        // TODO
        // - Also set text color in here, based on jacket color.
        // - Font, capitalization rules, smallcaps, text style
    }

    public void Move(float x, bool front) {
        transform.Translate(x, 0, 0);
        SwapTween(-x, .0667f, front);
    }
    public void SpawnTween(float offset) {
        xTweenTime = offset;
        xTweenStart = 30;
        tweenParent.transform.localPosition = new Vector3(30, 0, 0);
        xTweenSpeed = .01f;
        zTweenFront = true;
        ready = false;
    }
    private void SwapTween(float x, float speed, bool front) {
        if (xTweenTime < 1) {
            tweenParent.transform.localPosition = Vector3.zero;
        }
        xTweenTime = 0;
        xTweenStart = x;
        tweenParent.transform.localPosition = new Vector3(x, 0, 0);
        xTweenSpeed = speed;
        zTweenFront = front;
    }
}