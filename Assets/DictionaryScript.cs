using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

public class DictionaryScript : MonoBehaviour {
    public TextAsset wordsFile;
    public TextAsset sortedWords;
    public TextAsset titles;
    string[] dictionaryArray;
    HashSet<string> dictionarySet;
    Dictionary<string, List<string>> partsOfSpeech;
    List<TitleStructure> titleStructures;
    int totalTitleWeight;
    HashSet<string> usedWords;

    void Start() {
        dictionaryArray = wordsFile.text.Split('\n');
        dictionarySet = new HashSet<string>(dictionaryArray);
        partsOfSpeech = new Dictionary<string, List<string>>();
        foreach (string s in sortedWords.text.Split('\n')) {
            string[] tokens = s.Split('\t');
            char part = tokens[1][0]; // TODO: Many words can be multiple parts of speech. These are already in good.txt -- add logic to handle.
            string key = "%" + part;
            if (!partsOfSpeech.ContainsKey(key)) {
                partsOfSpeech.Add(key, new List<string>());
            }
            partsOfSpeech[key].Add(tokens[0]);
        }
        partsOfSpeech.Add("%?", new List<string>());
        partsOfSpeech.Add("%#", new List<string>());
        titleStructures = new List<TitleStructure>();
        totalTitleWeight = 0;
        foreach (string s in titles.text.Split('\n')) {
            string[] tokens = s.Split('\t');
            int weight = int.Parse(tokens[0]);
            titleStructures.Add(new TitleStructure(weight, tokens[1]));
            totalTitleWeight += weight;
        }
        usedWords = new HashSet<string>();
    }

    public bool IsRealWord(string word) {
        return dictionarySet.Contains(word);
    }

    public string RandomWord() {
        return dictionaryArray[Random.Range(0, dictionaryArray.Length)];
    }
    public string RandomNumberWord() {
        int r = Random.Range(0, 100);
        float selector = Random.value;
        if (selector < .05f) {
            r += 1000;
        }
        return NumToString.numeralToString(r).ToUpper();
    }
    public string WeightedRandomPartOfSpeech(string token) {
        string part = "";
        while (part.Length == 0 || usedWords.Contains(part)) {
            if (token == "%?") {
                part = RandomWord();
            } else if (token == "%#") {
                part = RandomNumberWord();
            } else {
                List<string> parts = partsOfSpeech[token];
                float power = 2;
                float selector = Random.Range(0, Mathf.Pow(parts.Count, power));
                int index = (int)Mathf.Floor(Mathf.Pow(selector, 1 / power));
                index = Mathf.Min(index, parts.Count);
                index = parts.Count - index - 1;
                part = parts[index];
            }
        }
        usedWords.Add(part);
        return part;
    }

    public string RandomTitle() {
        string title = "";
        while (title.Length == 0 || title.Length > 20) {
            int selector = Random.Range(0, totalTitleWeight);
            string structure = "SOMETHING BAD HAPPENED!";
            foreach (TitleStructure ts in titleStructures) {
                if (selector < ts.weight) {
                    structure = ts.structure;
                    break;
                }
                selector -= ts.weight;
            }

            foreach (string token in partsOfSpeech.Keys) {
                Regex regex = new Regex(Regex.Escape(token));
                while (structure.Contains(token)) {
                    structure = regex.Replace(structure, WeightedRandomPartOfSpeech(token), 1);
                }
            }
            title = structure.Trim();
        }
        return title;
    }
}

class TitleStructure {
    public int weight;
    public string structure;

    public TitleStructure(int weight, string structure) {
        this.weight = weight;
        this.structure = structure;
    }
}
class NumToString {
    static string[] ordersOfMagnitude = { "thousand", "million", "billion", "trillion", "quadrillion", "quintillion", "sextillion", "septillion", "octillion", "nonillion", "decillion", "undecillion", "duodecillion", "tredecillion", "quattuordecillion", "quindecillion", "sexdecillion", "septendecillion", "octodecillion", "novemdecillion", "vigintillion", "unvigintillion", "duovigintillion", "trevigintillion", "quattuorvigintillion", "quinvigintillion", "sexvigintillion", "septenvigintillion", "octovigintillion", "novemvigintillion", "trigintillion" };

    private static string oneDigitNumeralToString(long numeral) {
        if (numeral == 1)
            return "one";
        if (numeral == 2)
            return "two";
        if (numeral == 3)
            return "three";
        if (numeral == 4)
            return "four";
        if (numeral == 5)
            return "five";
        if (numeral == 6)
            return "six";
        if (numeral == 7)
            return "seven";
        if (numeral == 8)
            return "eight";
        if (numeral == 9)
            return "nine";
        else
            return "zero";
    }
    private static string twoDigitNumeralToString(long numeral) {
        string output = "";
        if (numeral / 10 == 0)
            return oneDigitNumeralToString(numeral);
        if (numeral == 10)
            return "ten";
        if (numeral == 11)
            return "eleven";
        if (numeral == 12)
            return "twelve";
        if (numeral == 13)
            return "thirteen";
        if (numeral == 14)
            return "fourteen";
        if (numeral == 15)
            return "fifteen";
        if (numeral == 16)
            return "sixteen";
        if (numeral == 17)
            return "seventeen";
        if (numeral == 18)
            return "eighteen";
        if (numeral == 19)
            return "nineteen";
        if (numeral / 10 == 2)
            output += "twenty";
        if (numeral / 10 == 3)
            output += "thirty";
        if (numeral / 10 == 4)
            output += "forty";
        if (numeral / 10 == 5)
            output += "fifty";
        if (numeral / 10 == 6)
            output += "sixty";
        if (numeral / 10 == 7)
            output += "seventy";
        if (numeral / 10 == 8)
            output += "eighty";
        if (numeral / 10 == 9)
            output += "ninety";
        if (numeral % 10 != 0)
            output += "-" + oneDigitNumeralToString(numeral % 10);
        return output;
    }
    private static string threeDigitNumeralToString(long numeral) {
        string output = "";
        if (numeral / 100 == 0)
            return twoDigitNumeralToString(numeral);
        output += oneDigitNumeralToString(numeral / 100) + " hundred";
        if (numeral % 100 != 0)
            output += " " + twoDigitNumeralToString(numeral % 100);
        return output;
    }
    public static string numeralToString(long numeral) {
        string output = "";
        bool negative = false;
        if (numeral < 0) {
            negative = true;
            numeral *= -1;
        }
        output += threeDigitNumeralToString(numeral % 1000);
        if (output == "zero" && numeral != 0)
            output = "";
        else if ((numeral % 1000) < 100 && (numeral / 1000) > 0)
            output = "and " + output;
        numeral /= 1000;
        int i = 0;
        while (numeral > 0) {
            if (numeral % 1000 != 0)
                output = threeDigitNumeralToString(numeral % 1000) + " " + ordersOfMagnitude[i] + " " + output;
            numeral /= 1000;
            i++;
        }
        if (negative)
            output = "negative " + output;
        return output.Trim();
    }
    public static string numeralToString(int numeral) {
        return numeralToString((long)numeral);
    }
}