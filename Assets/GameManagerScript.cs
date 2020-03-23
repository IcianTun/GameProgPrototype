using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum Phase
{
    Deploy,
    Move,
    Action
}

public enum PlayerTurn
{
    Blue,
    Red
}

public class GameManagerScript : MonoBehaviour {
    
    private static GameManagerScript _instance;
    public static GameManagerScript Instance { get { return _instance; } }

    public Button tankButton, lightButton, rangerButton, endTurnButton;
    

    public Phase phase;

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
        phase = Phase.Deploy;
    }

    void NextTurn()
    {

    }
}
