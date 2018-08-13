using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicScript : MonoBehaviour {
    public AudioSource audioSource;
    private const float FADE_SPEED = .0005f, MAX_VOL = .25f;

	void Awake () {
        if (GameObject.FindGameObjectsWithTag("Music").Length > 1) {
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(gameObject);
	}
	
	// Update is called once per frame
	void Update () {
		if (audioSource.volume < MAX_VOL) {
            audioSource.volume = Mathf.Min(MAX_VOL, audioSource.volume + FADE_SPEED);
        }
	}
}