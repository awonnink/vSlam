using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DigiTime : MonoBehaviour {

    TextMeshPro tm;
	// Use this for initialization
	void Start () {
        tm = GetComponent<TextMeshPro>();

    }
	
	// Update is called once per frame
	void Update () {
        tm.text = System.DateTime.Now.ToString("hh:mm:ss");
	}
}
