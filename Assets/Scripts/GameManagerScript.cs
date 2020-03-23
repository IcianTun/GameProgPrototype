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
        nextPlayerButton.onClick.AddListener(() => DisableBlackScreen());
    }
    public bool IsHoldingUnit()
    {
        return isHoldingUnit;
    }

    public void EndStep()
    {
        blackCover.SetActive(true);
    }

    public void DisableBlackScreen()
    {
        if (currentPlayer.playerColor == PlayerColor.Blue) // blue is 2nd player
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
            isHoldingUnit = true;
        }
    }

    public void HandleOnClickCell(HexCell cell)
    {
        if (phase == Phase.Deploy)
        {
            if (IsHoldingUnit() && (cell.unit == null))
            {
                GameObject newUnit = Instantiate(selectedUnitPrefab);
                Unit unit = cell.unit = newUnit.GetComponent<Unit>();
                unit.SetHexCell(cell);
                unit.SetPlayer(currentPlayer);  // TODO add to choosenCoordinate when 
                currentPlayer.unitList.Add(unit);
            }
        }
        if (phase == Phase.Move)
        {
            HandleMovePhase(cell);
        }
        if (phase == Phase.Action)
        {
            HandleActionPhase(cell);
        }
    }

    public void HandleMovePhase(HexCell cell)
    {
        if (cell.unit && cell.unit.player == currentPlayer && selectingUnitInBoard != cell.unit && cell.unit.choosenTargetCell == null)
        {
            selectingUnitInBoard = cell.unit;
            HexCell[] neighbors = cell.GetNeightbors();
            List<HexCell> neightborsList = new List<HexCell>(neighbors);
            int range = 1;
            while (range <= cell.unit.range)
            {
                List<HexCell> newList = new List<HexCell>();
                foreach (HexCell neighbor in neightborsList)
                {
                    if (neighbor)
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
        if (selectingUnitInBoard)
        {
            int distance = selectingUnitInBoard.hexCell.coordinates - cell.coordinates;
            if (distance > 0 && distance <= selectingUnitInBoard.moveRange)
            {
                selectingUnitInBoard.choosenTargetCell = cell;
                hexGrid.ResetColor();
            }
        }
    }

    void MergeAction()
    {
        bluePlayer.UnitsAction(phase);
        redPlayer.UnitsAction(phase);
    }

    public void HandleActionPhase(HexCell cell)
    {

    }
}
