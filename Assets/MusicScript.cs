using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicScript : MonoBehaviour {
    public AudioSource audioSource;
    private const float FADE_SPEED = .0025f, MAX_VOL = .25f;

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
		if (audioSource.volume < MAX_VOL) {
            audioSource.volume = Mathf.Min(MAX_VOL, audioSource.volume + FADE_SPEED);
        }
	}
}
