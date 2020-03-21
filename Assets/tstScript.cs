using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class tstScript : MonoBehaviour {

	// Use this for initialization
	void Start () {
        HexCoordinates a = new HexCoordinates(0, 0);
        HexCoordinates b = new HexCoordinates(0, 0);
        Debug.Log(a == b);
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
