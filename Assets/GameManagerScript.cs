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

    public PlayerTurn playerTurn;

    public GameObject tankPrefab;
    public GameObject lightPrefab;
    public GameObject rangerPrefab;

    GameObject selectedUnitPrefab;
    bool isHoldingUnit = false;

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

    void Start()
    {
        phase = Phase.Deploy;
        playerTurn = PlayerTurn.Red;
        tankButton.onClick.AddListener(() => SetDeployUnit(UnitType.Tank));
        lightButton.onClick.AddListener(() => SetDeployUnit(UnitType.Light));
        rangerButton.onClick.AddListener(() => SetDeployUnit(UnitType.Ranger));

    }

    void NextTurn()
    {

    }
    
    public bool IsHoldingUnit()
    {
        return isHoldingUnit;
    }

    public GameObject GetSelectedUnitPrefab()
    {
        return selectedUnitPrefab;
    }

    public void SetDeployUnit(UnitType unitType)
    {
        if(phase == Phase.Deploy) { 
            switch (unitType)
            {
                case (UnitType.Tank):
                    selectedUnitPrefab = tankPrefab;
                    break;
                case (UnitType.Light):
                    selectedUnitPrefab = lightPrefab;
                    break;
                case (UnitType.Ranger):
                    selectedUnitPrefab = rangerPrefab;
                    break;
            }
            isHoldingUnit = true;
        }
    }

    public void HandleOnClickCell(HexCell cell)
    {
        if (phase == Phase.Deploy)
        {
            if (IsHoldingUnit() && (cell.unit == null))
            {
                GameObject newUnit = Instantiate(GetSelectedUnitPrefab());
                cell.unit = newUnit.GetComponent<Unit>();
                cell.unit.SetHexCell(cell);
            }
            Debug.Log("touched cell " + cell.coordinates);
            //Debug.Log(cell.coordinates == coordinates); TRUE
        }
    }

}
