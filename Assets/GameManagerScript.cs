using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Phase
{
    Deploy
}

public class GameManagerScript : MonoBehaviour {
    
    private static GameManagerScript _instance;

    public static GameManagerScript Instance { get { return _instance; } }



	void NextTurn()
    {

    }
}
