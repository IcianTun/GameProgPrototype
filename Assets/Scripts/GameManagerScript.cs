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


public class GameManagerScript : MonoBehaviour
{

    private static GameManagerScript _instance;
    public static GameManagerScript Instance { get { return _instance; } }

    [Header("Grid Things")]
    public HexGrid hexGrid;
    public Color rangeColor = Color.yellow;

    [Header("UI Things")]
    public Button tankButton;
    public Button lightButton, rangerButton, endStepButton, nextPlayerButton;
    public Text currentPlayerText;
    public Text phaseText;
    public Text unitInfoText;
    public GameObject blackCover;

    [Header("GameInfo")]
    public Phase phase;
    public Player currentPlayer;
    public int turnCount = 1;

    static int productionGain = 2;   // gain start at 2
    static int productionMax = 6;


    [Header("Game Setup")]
    public Player bluePlayer;
    public Player redPlayer;
    public GameObject tankPrefab;
    public GameObject lightPrefab;
    public GameObject rangerPrefab;

    GameObject selectedUnitPrefab;

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

    void Start()
    {
        phase = Phase.Deploy;
        phaseText.text = phase.ToString();
        currentPlayerText.color = Color.blue;
        currentPlayerText.text = "CurrentPlayer: Blue";

        //playerTurn = PlayerColor.Red;
        currentPlayer = bluePlayer;
        tankButton.onClick.AddListener(() => SetDeployUnit(UnitType.Tank));
        lightButton.onClick.AddListener(() => SetDeployUnit(UnitType.Light));
        rangerButton.onClick.AddListener(() => SetDeployUnit(UnitType.Ranger));
        endStepButton.onClick.AddListener(() => EndStep());
        nextPlayerButton.onClick.AddListener(() => DisableBlackScreen());
    }

    private void Update()
    {
        
    }

    public void EndStep()
    {
        blackCover.SetActive(true);
    }

    public void DisableBlackScreen()
    {
        if (currentPlayer.playerColor == PlayerColor.Red) // red is 2nd player
        {
            MergeAction();
            SwitchPhase();
        }
        SwitchPlayerColor();
        blackCover.SetActive(false);
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
                if(turnCount%2 == 1 && productionGain != productionMax)
                {
                    productionGain += 1;
                }
                break;
        }
        phaseText.text = phase.ToString();

    }

    void SwitchPlayerColor()
    {
        switch (currentPlayer.playerColor)
        {
            case (PlayerColor.Blue):
                currentPlayerText.color = Color.red;
                currentPlayerText.text = "CurrentPlayer: Red";
                currentPlayer = redPlayer;
                break;
            case (PlayerColor.Red):
                currentPlayerText.color = Color.blue;
                currentPlayerText.text = "CurrentPlayer: Blue";
                currentPlayer = bluePlayer;
                break;
        }
        selectedUnitPrefab = null;
    }

    public void SetDeployUnit(UnitType unitType)
    {
        if (phase == Phase.Deploy)
        {
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
        }
    }

    public void HandleOnClickCell(HexCell clickedCell)
    {
        if (phase == Phase.Deploy)
        {
            HandleDeployPhase(clickedCell);
        }
        if (phase == Phase.Move)
        {
            HandleMovePhase(clickedCell);
        }
        if (phase == Phase.Action)
        {
            HandleActionPhase(clickedCell);
        }
    }

    void HandleDeployPhase(HexCell clickedCell)
    {
        if (IsDeployZone(clickedCell) && selectedUnitPrefab && (clickedCell.unitList.Count == 0) )
        {
            if (currentPlayer.production >= selectedUnitPrefab.GetComponent<Unit>().productionCost)
            {
                GameObject newUnit = Instantiate(selectedUnitPrefab);
                Unit unit = newUnit.GetComponent<Unit>();
                clickedCell.unitList.Add(unit);
                //unit.SetHexCell(clickedCell);
                unit.SetPlayer(currentPlayer);                  // TODO add to choosenCoordinate approach
                currentPlayer.unitList.Add(unit);
                currentPlayer.production -= unit.productionCost;
            } else if(currentPlayer.production > 0){
                // Produce this unit next turn
                // currentPlayer.producingUnit = ;
            }

        }
    }

    void HandleMovePhase(HexCell clickedCell)
    {

        List<Unit> unitList = clickedCell.unitList;
        foreach (Unit u in unitList)
        {
            if(u.player == currentPlayer && u.choosenTargetCell == null)
            {
                selectingUnitInBoard = u;
                HexCell[] neighbors = clickedCell.GetNeightbors();
                List<HexCell> neightborsList = new List<HexCell>(neighbors);
                int range = 1;
                while (range <= u.moveRange)
                {
                    List<HexCell> newList = new List<HexCell>();
                    foreach (HexCell neighbor in neightborsList)
                    {
                        if (neighbor && CanMoveThere(neighbor))
                        {
                            newList.AddRange(neighbor.GetNeightbors());
                            neighbor.color = rangeColor;
                            hexGrid.RenderCell();
                        }
                    }
                    neightborsList = newList;
                    range++;
                }
            }
        }
        /*
        if (cell.unit && cell.unit.player == currentPlayer && selectingUnitInBoard != cell.unit && cell.unit.choosenTargetCell == null)
        {
            selectingUnitInBoard = cell.unit;
        }
        */
        if (selectingUnitInBoard)       // Mode select destination
        {

            int distance = selectingUnitInBoard.hexCell.coordinates - clickedCell.coordinates;
            if (distance > 0 && distance <= selectingUnitInBoard.moveRange && CanMoveThere(clickedCell))
            {
                selectingUnitInBoard.choosenTargetCell = clickedCell;
                hexGrid.ResetColor();
                selectingUnitInBoard = null;
            }
        }
    }

    bool CanMoveThere(HexCell cell)
    {
        bool canMoveThere = true;
        foreach (Unit u in currentPlayer.unitList)
        {
            if (u.choosenTargetCell == cell)
            {
                canMoveThere = false;
            }
        }
        return canMoveThere;
    }

    void MergeAction()
    {
        bluePlayer.UnitsAction(phase);
        redPlayer.UnitsAction(phase);
        if(phase == Phase.Action)
        {
            bluePlayer.DestroyDeadUnits();
            redPlayer.DestroyDeadUnits();
        }
    }

    public void HandleActionPhase(HexCell clickedCell)
    {
        List<Unit> unitList = clickedCell.unitList;
        foreach (Unit u in unitList)
        {
            if (u.player == currentPlayer && u.choosenTargetCell == null)
            {
                selectingUnitInBoard = u;
                HexCell[] neighbors = clickedCell.GetNeightbors();
                List<HexCell> neightborsList = new List<HexCell>(neighbors);
                int range = 1;
                while (range <= u.moveRange)
                {
                    List<HexCell> newList = new List<HexCell>();
                    foreach (HexCell neighbor in neightborsList)
                    {
                        if (neighbor && CanMoveThere(neighbor))
                        {
                            newList.AddRange(neighbor.GetNeightbors());
                            neighbor.color = rangeColor;
                            hexGrid.RenderCell();
                        }
                    }
                    neightborsList = newList;
                    range++;
                }
            }
        }
        if (selectingUnitInBoard)       // Mode select target
        {

            int distance = selectingUnitInBoard.hexCell.coordinates - clickedCell.coordinates;
            if (distance > 0 && distance <= selectingUnitInBoard.range && CanMoveThere(clickedCell) && IsEnemyUnitThere(clickedCell))
            {
                selectingUnitInBoard.choosenTargetCell = clickedCell;
                hexGrid.ResetColor();
                selectingUnitInBoard = null;
            }
        }

    }

    bool IsDeployZone(HexCell cell)
    {
        bool isInDeployZone = false;
        if (currentPlayer.playerColor == PlayerColor.Blue)
        {
            return cell.coordinates.Z < 2;
        }
        if (currentPlayer.playerColor == PlayerColor.Red)
        {
            return cell.coordinates.Z > (hexGrid.height - 3);
        }
        return isInDeployZone;
    }

    bool IsEnemyUnitThere(HexCell cell)
    {
        foreach(Unit u in cell.unitList)
        {
            if (u.player != currentPlayer)
            {
                return true;
            }
        }
        return false;
    }

}
