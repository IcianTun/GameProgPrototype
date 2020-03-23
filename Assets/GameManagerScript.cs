using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Phase
{
    Deploy,
    Move,
    Action
}

public class GameManagerScript : MonoBehaviour {
    
    private static GameManagerScript _instance;

    public static GameManagerScript Instance { get { return _instance; } }

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }
    }

    void NextTurn()
    {

    }
}
