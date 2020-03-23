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

    public int production = 8;

	// Use this for initialization
	void Start () {
        unitList = new List<Unit>();
		
	}

    public void UnitsAction()
    {

    }
}
