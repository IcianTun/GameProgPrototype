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

    public Player player;

    public int hp;
    public int productionCost;

    public int atk;
    public int range;
    public int moveRange;

    public HexCell choosenTargetCell;

    public bool isUpgraded = false;

    public void TakeDamage(int damage)
    {
        if(unitType == UnitType.Tank && isUpgraded)
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

    public void SetHexCell(HexCell newHexCell)
    {
        hexCell = newHexCell;
        transform.position = newHexCell.transform.position + new Vector3(0,0.1f,0);;

    }

    public int getHP()
    {
        return hp;
    }

    public int getATK()
    {
        return atk;
    }

    public int getRange()
    {
        return range;
    }

    public int getMoveRange()
    {
        return moveRange;
    }

    public PlayerColor getPlayer()
    {
        return player.getColor();
    }

    public UnitType getType()
    {
        return unitType;
    }

    public void SetPlayer(Player player)
    {
        this.player = player;
    }
}
