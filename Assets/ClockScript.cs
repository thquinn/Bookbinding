using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClockScript : MonoBehaviour {
    public GameScript gameScript;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        transform.localRotation = Quaternion.Euler(0, 360 * gameScript.bookTimer, 0);
	}
}
