using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DictionaryScript : MonoBehaviour {
    public TextAsset wordsFile;
    public TextAsset sortedWords;
    string[] dictionaryArray;
    HashSet<string> dictionarySet;
    Dictionary<string, List<string>> partsOfSpeech;

    void Start () {
        dictionaryArray = wordsFile.text.Split('\n');
        dictionarySet = new HashSet<string>(dictionaryArray);
    }

    public string RandomWord() {
        return dictionaryArray[Random.Range(0, dictionaryArray.Length)];
    }
}