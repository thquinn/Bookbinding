using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameScript : MonoBehaviour {
    public GameObject bookPrefab;
    public DictionaryScript dictionaryScript;
    public SwapUIScript swapUI;
    public CombineUIScript combineUI;
    List<BookScript> bookScripts;
    int selected, combineTarget;
    bool combining = false;
    float rightX;

    private const int NUM_STARTING_BOOKS = 4;

    bool leftLastFrame = false, rightLastFrame = false;

	void Start () {
        if (BookScript.fonts == null) {
            BookScript.fonts = Resources.LoadAll<TMP_FontAsset>("Fonts");
        }

        bookScripts = new List<BookScript>();
        selected = 0;
        combineTarget = -1;

        rightX = -7;
        while (bookScripts.Count < NUM_STARTING_BOOKS) {
            AddBook((NUM_STARTING_BOOKS - bookScripts.Count - 1) * .05f);
        }
	}
    void AddBook(float spawnTweenOffset) {
        GameObject book = Instantiate(bookPrefab);
        book.transform.Translate(rightX, 0, 0);
        BookScript bookScript = book.GetComponent<BookScript>();
        bookScript.SetTitle(dictionaryScript.RandomWord());
        bookScripts.Add(bookScript);
        float width = Random.Range(1, 1.5f);
        bookScript.SetScale(width, Random.Range(.66f, .9f));
        bookScript.SetJacketColor(Color.HSVToRGB(Random.value, Random.Range(.2f, .5f), Random.Range(.2f, .75f)));
        bookScript.SpawnTween(spawnTweenOffset);
        rightX += width;
    }

    void Update() {
        // Game logic.
        if (!combining && Input.GetButtonDown("Submit")) {
            AddBook(0);
        }

        // Swap mode.
        int maxSelect = bookScripts.Count - 1;
        while (maxSelect >= 0 && !bookScripts[maxSelect].IsReady()) {
            maxSelect--;
        }
        if (!combining) {
            if (Input.GetAxisRaw("Horizontal") < 0) {
                if (!leftLastFrame && selected > 0) {
                    if (Input.GetButton("Swap")) {
                        SwapBooks(selected - 1, selected);
                        selected--;
                    } else if (Input.GetButton("Combine")) {
                        combineTarget = selected - 1;
                        combining = true;
                        combineUI.SetBooks(bookScripts[selected], bookScripts[combineTarget]);
                    } else {
                        selected--;
                    }
                }
                leftLastFrame = true;
            }
            else {
                leftLastFrame = false;
            }
            if (Input.GetAxisRaw("Horizontal") > 0) {
                if (!rightLastFrame && selected < maxSelect) {
                    if (Input.GetButton("Swap")) {
                        SwapBooks(selected, selected + 1);
                        selected++;
                    } else if (Input.GetButton("Combine")) {
                        combineTarget = selected + 1;
                        combining = true;
                        combineUI.SetBooks(bookScripts[selected], bookScripts[combineTarget]);
                    } else {
                        selected++;
                    }
                }
                rightLastFrame = true;
            }
            else {
                rightLastFrame = false;
            }
        } else { // Combine mode.
            if (Input.GetButtonDown("Cancel")) {
                combining = false;
            } else if (Input.GetButtonDown("Submit")) {
                float shift = bookScripts[selected].model.transform.localScale.x + bookScripts[combineTarget].model.transform.localScale.x;
                for (int i = Mathf.Max(selected, combineTarget) + 1; i < bookScripts.Count; i++) {
                    bookScripts[i].Move(-shift, false);
                }
                rightX -= shift;
                Destroy(bookScripts[selected].gameObject);
                Destroy(bookScripts[combineTarget].gameObject);
                bookScripts.RemoveAt(Mathf.Max(selected, combineTarget));
                bookScripts.RemoveAt(Mathf.Min(selected, combineTarget));
                while (bookScripts.Count < 2) {
                    AddBook(0);
                }
                combining = false;
            }
        }
    }
    void SwapBooks(int one, int two) {
        BookScript bookOne = bookScripts[one];
        BookScript bookTwo = bookScripts[two];
        bookOne.Move(bookTwo.model.transform.localScale.x, one == selected);
        bookTwo.Move(-bookOne.model.transform.localScale.x, two == selected);
        bookScripts[one] = bookTwo;
        bookScripts[two] = bookOne;
    }

    public bool IsCombining() {
        return combining;
    }
    public float GetSelectorX() {
        float x = -7;
        for (int i = 0; i < selected; i++) {
            x += bookScripts[i].model.transform.localScale.x / 2;
            x += bookScripts[i + 1].model.transform.localScale.x / 2;
        }
        return x;
    }
    public string GetSelectedTitle() {
        return bookScripts[selected].title;
    }
}
