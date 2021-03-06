﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
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
    public Color enemyFoundColor = Color.magenta;

    [Header("UI Things")]
    public Button tankButton;
    public Button lightButton, rangerButton, endStepButton, nextPlayerButton;
    public Text currentPlayerText;
    public Text phaseText;
    public Text UnitInfoText;
    public Text UnitInfoText2;
    public Text ProductionText;
    public GameObject blackCover;
    public Text victoryInfoText;
    public Text TurnCountText;
    public GameObject endGameCover;
    public Text winText;
    public Button restartGameButton;

    [Header("GameInfo")]
    public Phase phase;
    public Player currentPlayer;
    public int turnCount = 1;

    static int productionGain = 2;   // gain start at 2
    static int productionMax = 6;
    public Player currentWinningPlayer;
    public int victoryCount;

    [Header("Game Setup")]
    public Player bluePlayer;
    public Player redPlayer;
    public GameObject tankPrefab;
    public GameObject lightPrefab;
    public GameObject rangerPrefab;

    GameObject selectedUnitPrefab;

    public Unit selectingUnitInBoard;

    public List<Unit> producedUnit;

    int notHasUpgradeCellTurnCount = 1;

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

    void Update()
    {
        if (!EventSystem.current.IsPointerOverGameObject())
        {
            Ray inputRay = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(inputRay, out hit))
            {
                HexCell hoveringCell = hexGrid.GetCell(hit.point);
                InfoText(hoveringCell.unitList);
            }

        }
        if (currentWinningPlayer && victoryCount >0) { 
            switch (currentWinningPlayer.playerColor)
            {
                case (PlayerColor.Blue):
                    victoryInfoText.color = Color.blue;
                    break;
                case (PlayerColor.Red):
                    victoryInfoText.color = Color.red;
                    break;
            }
            victoryInfoText.text = "VictoryPoint: "+victoryCount+"/3";
        }
        else
        {
            victoryInfoText.text = "";
        }
        GainText();
        TurnCountText.text = "Turn: " + turnCount;
    }

    private void GainText()
    {
        switch (currentPlayer.playerColor)
        {
            case (PlayerColor.Blue):
                ProductionText.color = Color.blue;
                break;
            case (PlayerColor.Red):
                ProductionText.color = Color.red;
                break;
        }
        ProductionText.text = "Production left: = " + currentPlayer.production + 
            (currentPlayer.productionNeeded == 0 ? "" : "("+currentPlayer.productionNeeded+")");
    }

    private void InfoText(List<Unit> unitList)
    {
        UnitInfoText.text = "";
        UnitInfoText2.text = "";
        int i = unitList.Count;
        if (i > 0)
        {
            Unit unit = unitList[0];
            switch (unit.player.playerColor)
            {
                case (PlayerColor.Blue):
                    UnitInfoText.color = Color.blue;
                    break;
                case (PlayerColor.Red):
                    UnitInfoText.color = Color.red;
                    break;
            }
            string star = (unit.player == currentPlayer && !unit.choosenTargetCell ? "*" : " ");
            UnitInfoText.text = star+unit.unitType.ToString() + (unit.isUpgraded ? "(U)" : "") + "\nHP : "
                + unit.hp.ToString() + "\nATK : " + unit.atk + "\nRange : " +
                unit.range.ToString() + "   Move : " + unit.moveRange;
        }
        if (i > 1)
        {
            Unit unit2 = unitList[1];
            switch (unit2.player.playerColor)
            {
                case (PlayerColor.Blue):
                    UnitInfoText2.color = Color.blue;
                    break;
                case (PlayerColor.Red):
                    UnitInfoText2.color = Color.red;
                    break;
            }
            string star2 = (unit2.player == currentPlayer && !unit2.choosenTargetCell ? "*" : " ");
            UnitInfoText2.text = star2+unit2.unitType.ToString() + (unit2.isUpgraded? "(U)" : "") + "\nHP : "
                + unit2.hp.ToString() + "\nATK : " + unit2.atk + "\nRange : " +
                unit2.range.ToString() + "   Move : " + unit2.moveRange;
        }

    }

    private void InfoClearText()
    {
        UnitInfoText.text = "";
        UnitInfoText2.text = "";
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
        restartGameButton.onClick.AddListener(() => RestartGame());
        producedUnit = new List<Unit>();
    }

    public void EndStep()
    {
        blackCover.SetActive(true);
    }

    public void DisableBlackScreen()
    {
        if (currentPlayer.playerColor == PlayerColor.Red) // red is 2nd player this is telling that we are ending phase
        {
            SwitchPlayerColor();
            InfoClearText();
            MergeAction();
            SwitchPhase();
        } else
        {
            SwitchPlayerColor();
            InfoClearText();
        }
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
                HandleEndTurn();
                break;
        }
        phaseText.text = phase.ToString();

    }

    void SwitchPlayerColor()
    {
        if (phase == Phase.Deploy)
        {
            HideDeployedUnit();

        }
        else
        {
            hexGrid.ResetColor();
        }
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
        selectingUnitInBoard = null;
    }

    void HandleEndTurn()
    {
        if (turnCount % 2 == 1 && productionGain != productionMax)
        {
            productionGain += 1;
        }
        bluePlayer.production += productionGain;
        redPlayer.production += productionGain;

        if (bluePlayer.producingUnit)
        {
            if(bluePlayer.production > bluePlayer.productionNeeded)
            {
                bluePlayer.production -= bluePlayer.productionNeeded;
                bluePlayer.productionNeeded = 0;
                bluePlayer.producingUnit.MoveToHexCell(bluePlayer.producingUnit.choosenTargetCell);
                bluePlayer.unitList.Add(bluePlayer.producingUnit);
                bluePlayer.producingUnit.choosenTargetCell = null;
                //bluePlayer.unitList.Add(bluePlayer.producingUnit);
                bluePlayer.producingUnit = null;
            }
        }
        if (redPlayer.producingUnit)
        {
            if (redPlayer.production > redPlayer.productionNeeded)
            {
                redPlayer.production -= redPlayer.productionNeeded;
                redPlayer.productionNeeded = 0;
                redPlayer.producingUnit.MoveToHexCell(redPlayer.producingUnit.choosenTargetCell);
                redPlayer.unitList.Add(redPlayer.producingUnit);
                redPlayer.producingUnit.choosenTargetCell = null;
                //redPlayer.unitList.Add(redPlayer.producingUnit);
                redPlayer.producingUnit = null;
            }
        }
        HandleUpgrade();
        HandleObjective();
        turnCount += 1;
        HandleSkillTurnCount();
        hexGrid.ResetColor();
    }

    void HandleSkillTurnCount()
    {
        foreach(Unit unit in bluePlayer.unitList)
        {
            if (unit.unitType == UnitType.Light && unit.isUpgraded)
            {
                unit.dodgeTurnCount++;
            }
            if (unit.isBinded)
            {
                unit.isBinded = false;
            }
        }
        foreach (Unit unit in redPlayer.unitList)
        {
            if (unit.unitType == UnitType.Light && unit.isUpgraded)
            {
                unit.dodgeTurnCount++;
            }
            if (unit.isBinded)
            {
                unit.isBinded = false;
            }
        }
    }

    void HandleUpgrade()
    {
        bool hasUpgradeCell = false;
        foreach(HexCell upgradeCell in hexGrid.upgradeCells)
        {
            if (upgradeCell.isUpgradeCell)
            {
                hasUpgradeCell = true;
            }
            if(upgradeCell.isUpgradeCell && upgradeCell.unitList.Count == 1)
            {
                Unit unitInUpgradeCell = upgradeCell.unitList[0];
                unitInUpgradeCell.upgradeTurnCount++;
                if (unitInUpgradeCell.upgradeTurnCount == 2)
                {
                    unitInUpgradeCell.Upgrade();
                    upgradeCell.isUpgradeCell = false;
                    hasUpgradeCell = false;
                }
            }
        }
        if (!hasUpgradeCell)
        {
            notHasUpgradeCellTurnCount += 1;
            if (notHasUpgradeCellTurnCount == 2)
            {
                hexGrid.upgradeCells[Random.Range(0, hexGrid.upgradeCells.Count)].isUpgradeCell = true;
                notHasUpgradeCellTurnCount = 0;
            }
        }
    }

    void HandleObjective()
    {
        bool isThereBlueUnit = false;
        bool isThereRedUnit = false;
        foreach(HexCell objCell in hexGrid.objectiveCells)
        {
            foreach(Unit u in objCell.unitList)
            {
                if (u.player == bluePlayer) isThereBlueUnit = true;
                if (u.player == redPlayer) isThereRedUnit = true;
            }
        }

        if (isThereBlueUnit && !isThereRedUnit)
        {
            if(currentWinningPlayer == bluePlayer)
            {
                victoryCount += 1;
            } else
            {
                victoryCount = 1;
                currentWinningPlayer = bluePlayer;
            }
        }
        if (!isThereBlueUnit && isThereRedUnit)
        {
            if (currentWinningPlayer == redPlayer)
            {
                victoryCount += 1;
            }
            else
            {
                victoryCount = 1;
                currentWinningPlayer = redPlayer;
            }
        }
        if (!isThereBlueUnit && !isThereRedUnit)
        {
            victoryCount = 0;
            currentWinningPlayer = null;
        }
        if(victoryCount == 3)
        {
            ShowGameWin();
        }
    }

    void ShowGameWin()
    {
        switch (currentWinningPlayer.playerColor)
        {
            case (PlayerColor.Blue):
                winText.color = Color.blue;
                winText.text = "Blue Player Win!";
                break;
            case (PlayerColor.Red):
                winText.color = Color.red;
                winText.text = "Red Player Win!";
                break;
        }
        endGameCover.SetActive(true);
    }

    void RestartGame()
    {
        SceneManager.LoadScene("TunScene");
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
        if (IsDeployZone(clickedCell) && selectedUnitPrefab && (!AlreadyDeployThere(clickedCell)) && !(IsMyUnitThere(clickedCell) ))
        {
            if (currentPlayer.production >= selectedUnitPrefab.GetComponent<Unit>().productionCost)
            {
                GameObject newUnitObj = Instantiate(selectedUnitPrefab);
                newUnitObj.transform.position = clickedCell.transform.position + new Vector3(0, 0.1f, 0);
                Unit unit = newUnitObj.GetComponent<Unit>();
                unit.player = currentPlayer;
                unit.choosenTargetCell = clickedCell;
                switch (currentPlayer.playerColor)
                {
                    case (PlayerColor.Blue):
                        unit.transform.Rotate(0.0f, 0.0f, 90.0f);
                        break;
                    case (PlayerColor.Red):
                        unit.transform.Rotate(0.0f, 0.0f, -90.0f);
                        break;
                }
                currentPlayer.unitList.Add(unit);
                currentPlayer.production -= unit.productionCost;
                producedUnit.Add(unit);
                var unitlist = new List<Unit> { unit };
                InfoText(unitlist);
            } else if (currentPlayer.production > 0) {
                GameObject newUnitObj = Instantiate(selectedUnitPrefab);
                newUnitObj.transform.position = clickedCell.transform.position + new Vector3(0, 0.1f, 0);
                Unit unit = newUnitObj.GetComponent<Unit>();
                unit.player = currentPlayer;
                unit.choosenTargetCell = clickedCell;

                switch (currentPlayer.playerColor)
                {
                    case (PlayerColor.Blue):
                        unit.transform.Rotate(0.0f, 0.0f, 90.0f);
                        break;
                    case (PlayerColor.Red):
                        unit.transform.Rotate(0.0f, 0.0f, -90.0f);
                        break;
                }

                currentPlayer.producingUnit = unit;     // Produce this unit next turn
                                                        // this unit not added to player.unitList and clickedCell.unitList
                currentPlayer.productionNeeded = unit.productionCost - currentPlayer.production;       
                currentPlayer.production = 0;
            }

        }
    }
    void HideDeployedUnit()
    {
        foreach (Unit u in producedUnit)
        {
            u.transform.position = u.choosenTargetCell.transform.position + new Vector3(0, -0.1f, 0);
        }
        if (currentPlayer.producingUnit)
        {
            currentPlayer.producingUnit.transform.position = currentPlayer.producingUnit.choosenTargetCell.transform.position + new Vector3(0, -0.1f, 0);
        }
        producedUnit.Clear();
    }

    bool AlreadyDeployThere(HexCell cell)
    {
        foreach (Unit u in producedUnit)
        {
            if(cell == u.choosenTargetCell)
            {
                return true;
            }
        }
        return false;
    }

    void HandleMovePhase(HexCell clickedCell)
    {
        List<Unit> unitList = clickedCell.unitList;
        foreach (Unit u in unitList)
        {
            InfoText(unitList);
            if (u.player == currentPlayer && u.choosenTargetCell == null)
            {
                hexGrid.ResetColor();
                selectingUnitInBoard = u;
                HexCell[] neighbors = clickedCell.GetNeightbors();
                List<HexCell> neightborsList = new List<HexCell>(neighbors);
                int r = 1;
                while (r <= u.moveRange)
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
                    r++;
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
        if (phase == Phase.Action)
        {
            bluePlayer.DestroyDeadUnits();
            redPlayer.DestroyDeadUnits();
        }
    }

    public void HandleActionPhase(HexCell clickedCell)
    {
        if (selectingUnitInBoard)       // Mode select target
        {
            int distance = selectingUnitInBoard.hexCell.coordinates - clickedCell.coordinates;
            if (distance <= selectingUnitInBoard.range && IsEnemyUnitThere(clickedCell))
            {
                selectingUnitInBoard.choosenTargetCell = clickedCell;
                hexGrid.ResetColor();
                selectingUnitInBoard = null;
            }
        }
        else
        {
            List<Unit> unitList = clickedCell.unitList;
            foreach (Unit u in unitList)
            {
                InfoText(unitList);
                if (u.player == currentPlayer && u.choosenTargetCell == null)
                {
                    selectingUnitInBoard = u;
                    HexCell[] neighbors = clickedCell.GetNeightbors();
                    if (IsEnemyUnitThere(clickedCell))
                    {
                        clickedCell.color = enemyFoundColor;
                    }
                    List<HexCell> neightborsList = new List<HexCell>(neighbors);
                    int r = 1;
                    while (r <= u.range)
                    {
                        List<HexCell> newList = new List<HexCell>();
                        foreach (HexCell neighbor in neightborsList)
                        {
                            if (neighbor)
                            {
                                foreach (HexCell neighborOfNeighbor in neighbor.GetNeightbors())
                                {
                                    if (neighborOfNeighbor)
                                        newList.Add(neighborOfNeighbor);
                                }
                                if (IsEnemyUnitThere(neighbor))
                                {
                                    neighbor.color = enemyFoundColor;
                                }
                            }

                        }
                        neightborsList = newList;
                        r++;
                    }
                    hexGrid.RenderCell();
                }
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
        foreach (Unit u in cell.unitList)
        {
            if (u.player != currentPlayer)
            {
                InfoText(cell.unitList);
                return true;
            }
        }
        return false;
    }

    bool IsMyUnitThere(HexCell cell)
    {
        foreach (Unit u in cell.unitList)
        {
            if (u.player == currentPlayer)
            {
                return true;
            }
        }
        return false;
    }



}
