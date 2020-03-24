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

    public int productionNeeded;        // for unit > cost

    public Unit producingUnit;

    public PlayerColor getColor()
    {
        return playerColor;
    }

    // Use this for initialization
    void Start () {
        unitList = new List<Unit>();
		
	}

    public void UnitsAction(Phase phase)
    {
        if (phase == Phase.Deploy)
        {
            foreach (Unit u in unitList)
            {
                if (u.choosenTargetCell)
                {
                    u.MoveToHexCell(u.choosenTargetCell);
                    u.choosenTargetCell = null;
                }
            }
        }
        if (phase == Phase.Move)
        {
            foreach(Unit u in unitList)
            {
                if (u.choosenTargetCell) { 
                    u.MoveToHexCell(u.choosenTargetCell);
                    u.choosenTargetCell = null;
                }
            }
        }
        if (phase == Phase.Action)
        {
            foreach (Unit myUnit in unitList)
            {
                if (myUnit.choosenTargetCell)
                {
                    foreach (Unit unitInTargetCell in myUnit.choosenTargetCell.unitList)
                    {
                        if (unitInTargetCell.player != this)
                        {
                            unitInTargetCell.TakeDamage(myUnit.atk);
                        }
                    }
                    myUnit.choosenTargetCell = null;
                }
            }
        }
    }

    public void DestroyDeadUnits()
    {
        for(int i = unitList.Count-1; i>= 0; i--)
        {
            if(unitList[i].hp <= 0)
            {
                unitList[i].hexCell.unitList.Remove(unitList[i]);
                Destroy(unitList[i].gameObject);
                unitList.Remove(unitList[i]);
            }
        }


    }
}
