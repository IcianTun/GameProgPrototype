using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum UnitType
{
    Tank,
    Light,
    Ranger
}

public class Unit : MonoBehaviour {

    public HexCell hexCell;

    public UnitType unitType;
    
    public int hp;
    public int productionCost;

    public int atk;
    public int range;
    public int moveRange;

    public bool isUpgraded = false;

    public void takeDamage(int damage)
    {
        if(unitType == UnitType.Tank)
        {
            damage -= 1;
            damage = Mathf.Max(damage, 0);
        }
           
        hp -= damage;
        if (hp < 0)
        {
            Destroy(gameObject);
        }
    }

    public void setHexCell(HexCell newHexCell)
    {
        hexCell = newHexCell;
        transform.position = newHexCell.transform.position;
    }
}
