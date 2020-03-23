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


public class GameManagerScript : MonoBehaviour {
    
    private static GameManagerScript _instance;
    public static GameManagerScript Instance { get { return _instance; } }

    public HexGrid hexGrid;
    public Color rangeColor = Color.yellow;

    public Button tankButton, lightButton, rangerButton, endStepButton;
    public Text currentPlayerText;
    public Text phaseText;
    public Text UnitInfoText;
    public Text gain;

    public Phase phase;

    //public PlayerColor playerTurn;
    public Player currentPlayer;

    int productionGain = 2;
    int productionMax = 6;

    public Player bluePlayer;
    public Player redPlayer;

    public GameObject tankPrefab;
    public GameObject lightPrefab;
    public GameObject rangerPrefab;

    GameObject selectedUnitPrefab;
    bool isHoldingUnit = false;

    public Unit selectingUnitInBoard;

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

    private void Info(Unit unit)
    {
        UnitInfoText.text = "Owner : " + unit.getPlayer()+"\nType : "+unit.getType()+"\nHP : " 
            + unit.getHP().ToString() + "\nATK : " + unit.getATK().ToString() + "\nRange : "+ 
            unit.getRange().ToString()+"\nMove : "+ unit.getMoveRange().ToString();
    }

    void Start()
    {
        phase = Phase.Deploy;
        phaseText.text = phase.ToString();
        currentPlayerText.text = "CurrentPlayer: Red";

        //playerTurn = PlayerColor.Red;
        currentPlayer = redPlayer;
        tankButton.onClick.AddListener(() => SetDeployUnit(UnitType.Tank));
        lightButton.onClick.AddListener(() => SetDeployUnit(UnitType.Light));
        rangerButton.onClick.AddListener(() => SetDeployUnit(UnitType.Ranger));
        endStepButton.onClick.AddListener(() => EndStep());
    }
    public bool IsHoldingUnit()
    {
        return isHoldingUnit;
    }

    public GameObject GetSelectedUnitPrefab()
    {
        return selectedUnitPrefab;
    }

    public void EndStep()
    {
        if(currentPlayer.playerColor == PlayerColor.Blue)
        {
            SwitchPhase();
        } 
        SwitchPlayerColor();
    }

    void SwitchPhase()
    {
        switch (phase)
        {
            case (Phase.Deploy):
                phase = Phase.Move;
                tankButton.interactable = false;
                lightButton.interactable = false;
                rangerButton.interactable = false;
                break;
            case (Phase.Move):
                phase = Phase.Action;
                break;
            case (Phase.Action):
                phase = Phase.Deploy;
                tankButton.interactable = true;
                lightButton.interactable = true;
                rangerButton.interactable = true;
                break;
        }
        phaseText.text = phase.ToString();

    }

    void SwitchPlayerColor()
    {
        switch (currentPlayer.playerColor)
        {
            case (PlayerColor.Blue):
                currentPlayerText.text = "CurrentPlayer: Red";
                currentPlayer = redPlayer;
                break;
            case (PlayerColor.Red):
                currentPlayerText.text = "CurrentPlayer: Blue";
                currentPlayer = bluePlayer;
                break;
        }
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
                Unit unit = cell.unit = newUnit.GetComponent<Unit>();
                cell.unit.SetHexCell(cell);
                unit.SetPlayer(currentPlayer);
                currentPlayer.unitList.Add(unit);
                Info(cell.unit);
            }
            //Debug.Log("touched cell " + cell.coordinates);
            //Debug.Log(cell.coordinates == coordinates); TRUE
        }
        if (phase == Phase.Move)
        {
            Info(cell.unit);
            if (cell.unit && cell.unit.player == currentPlayer )
            {
                selectingUnitInBoard = cell.unit;
                HexCell[] neighbors = cell.GetNeightbors();
                List<HexCell> neightborsList = new List<HexCell>(neighbors);
                int range = 1;
                while (range <= cell.unit.range)
                {
                    Debug.Log(range);
                    List<HexCell> newList = new List<HexCell>();
                    foreach (HexCell neighbor in neightborsList)
                    {
                        newList.AddRange(neighbor.GetNeightbors());
                        neighbor.color = rangeColor;
                        hexGrid.RenderCell();
                    }
                    neightborsList = newList;
                    range++;
                }

            }
            if (selectingUnitInBoard)
            {
                int distance = selectingUnitInBoard.hexCell.coordinates - cell.coordinates;
                if ( distance > 0 && distance <= selectingUnitInBoard.moveRange)
                {
                    selectingUnitInBoard.choosenTargetCell = cell;
                    hexGrid.ResetCorlor();
                    hexGrid.RenderCell();
                }
            }
        }
    }

}
