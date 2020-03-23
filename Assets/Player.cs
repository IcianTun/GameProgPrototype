using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PlayerColor
{
    Blue,
    Red
}
public class Player : MonoBehaviour {

    public PlayerColor playerColor;

    public List<Unit> unitList;

    int production;

	// Use this for initialization
	void Start () {
        unitList = new List<Unit>();
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
