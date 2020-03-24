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

    public Unit producingUnit;

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
                u.SetHexCell(u.choosenTargetCell);
                u.choosenTargetCell = null;
            }
        }
        if (phase == Phase.Move)
        {
            foreach(Unit u in unitList)
            {
                u.SetHexCell(u.choosenTargetCell);
                u.choosenTargetCell = null;
            }
        }
        if (phase == Phase.Action)
        {
            foreach (Unit myUnit in unitList)
            {

                foreach (Unit unitInTargetCell in myUnit.choosenTargetCell.unitList) {
                    if(unitInTargetCell.player != this)
                    {
                        unitInTargetCell.TakeDamage(myUnit.atk);
                    }
                }
                myUnit.choosenTargetCell = null;
            }
        }
    }

    public void DestroyDeadUnits()
    {
        foreach(Unit u in unitList)
        {
            if(u.hp <= 0)
            {
                Destroy(u.gameObject);
            }
        }
    }
}
