using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Threading;
using TMPro;
using UnityEngine;

public class BookScript : MonoBehaviour {
    public static TMP_FontAsset[] fonts;
    static HashSet<string> noAllCapsFontNames = new HashSet<string>() { "AlexBrush-Regular SDF", "Allura-Regular SDF", "Arizonia-Regular SDF", "DancingScript-Regular SDF", "GrandHotel-Regular SDF", "GreatVibes-Regular SDF", "JosephinSans-Regular SDF", "Lobster_1", "Pacifico SDF", "Sofia-Regular SDF", "Titilium-Regular SDF" };
    static float TILT_TWEEN_SPEED = 0.5f;
    public GameObject tiltParent, tweenParent, model, textMesh;
    public AudioSource audioSource;
    public AudioClip[] swapClips, thumpClips;
    public string title;
    public float tilt;
    bool ready;
    float xTweenStart, xTweenTime = 1, xTweenSpeed;
    bool zTweenFront;
    public bool isDead = false;
    float deadSpeed = 0, deadAccel = .025f;

    void Start () {
        ready = false;
	}
    public bool IsReady() {
        return ready;
    }
	
	void Update () {
        if (isDead) {
            deadSpeed += deadAccel;
            transform.Translate(0, 0, -deadSpeed);
            if (transform.position.z < -100) {
                Destroy(gameObject);
            }
            return;
        }

        // Tilt.
        if (tilt >= GameScript.MAX_TILT) { // Drop the book; the game is over.
            tilt = Mathf.Min(90, tilt + 3);
        }
        float actualTilt = tiltParent.transform.localRotation.eulerAngles.z;
        if (actualTilt != tilt) {
            float zRot = Mathf.Abs(tilt - actualTilt) < .5f ? tilt : actualTilt + (tilt - actualTilt) * TILT_TWEEN_SPEED;
            float zRadians = zRot * Mathf.Deg2Rad;
            float zSin = Mathf.Sin(zRadians);
            float z2Sin = Mathf.Sin(zRadians * 2);
            float width = model.transform.localScale.x, height = model.transform.localScale.y * 8;
            float xOff = (height / 2 - width / 2) * zSin + (height / 20) * z2Sin;
            float yOff = (width / 2 - height / 2) * zSin + (height / 5) * z2Sin;
            tiltParent.transform.localPosition = new Vector3(xOff, yOff, 0);
            tiltParent.transform.localRotation = Quaternion.Euler(new Vector3(0, 0, zRot));
        }
        // Tween.
        if (xTweenTime < 1) {
            xTweenTime = Mathf.Min(1, xTweenTime + xTweenSpeed);
            if (!ready && xTweenTime >= .9f) {
                audioSource.PlayOneShot(thumpClips[Random.Range(0, thumpClips.Length)]);
                ready = true;
            }
            float x = EasingFunction.EaseInOutQuad(xTweenStart, 0, xTweenTime);
            float z = EasingFunction.EaseInQuad(0, 1, xTweenTime);
            if (z > .5) {
                z = 1 - z;
            }
            z *= 2;
            float y = zTweenFront ? z / 2 : 0;
            float xRot = zTweenFront ? z * -3 : 0;
            z *= zTweenFront ? -5f : 2.5f;
            tweenParent.transform.localPosition = new Vector3(x, y, z);
            tweenParent.transform.localRotation = Quaternion.Euler(new Vector3(xRot, 0, 0));
        }
	}

    public void SetTitle(string title) {
        this.title = title;
        TMP_FontAsset font = fonts[Random.Range(0, fonts.Length)];
        TextMeshPro pro = textMesh.GetComponent<TextMeshPro>();
        bool canBeAllCaps = !noAllCapsFontNames.Contains(font.name);
        string capitalizedTitle = (Random.value < .166 && canBeAllCaps) ? title.ToUpper() : AsTitleCase(title);
        pro.SetText(capitalizedTitle);
        pro.font = font;
    }
    public void SetScale(float x, float y) {
        model.transform.localScale = new Vector3(x, y, 1);
        gameObject.transform.Translate((x - 1) / 2f, -4 * (1 - y), 0);

        RectTransform rectTransform = textMesh.GetComponent<RectTransform>();
        float canvasWidth = rectTransform.sizeDelta.x * y;
        canvasWidth *= Random.Range(.4f, 1f);
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
        audioSource.PlayOneShot(swapClips[Random.Range(0, swapClips.Length)]);
        audioSource.clip = thumpClips[Random.Range(0, thumpClips.Length)];
        audioSource.PlayDelayed(.2f);
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

    public static string AsTitleCase(string title) {
        string workingTitle = title;

        if (string.IsNullOrEmpty(workingTitle) == false) {
            List<string> artsAndPreps = new List<string>()
                { "a", "an", "and", "any", "at", "from", "into", "of", "on", "or", "some", "the", "to", };

            //Get the culture property of the thread.
            CultureInfo cultureInfo = Thread.CurrentThread.CurrentCulture;
            //Create TextInfo object.
            TextInfo textInfo = cultureInfo.TextInfo;

            //Convert to title case.
            workingTitle = textInfo.ToTitleCase(title.ToLower());

            List<string> tokens = new List<string>(workingTitle.Split(new char[] { ' ' }, System.StringSplitOptions.RemoveEmptyEntries));

            workingTitle = tokens[0];

            tokens.RemoveAt(0);


            foreach (string token in tokens) {
                workingTitle += artsAndPreps.Contains(token.ToLower()) ? " " + token.ToLower() : " " + token;
            }

            // Handle an "Out Of" but not in the start of the sentance
            workingTitle = Regex.Replace(workingTitle, @"(?!^Out)(Out\s+Of)", "out of");
        }

        return workingTitle;

    }
}