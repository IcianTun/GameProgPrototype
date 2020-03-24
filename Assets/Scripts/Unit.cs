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

    public int maxHP;

    public int productionCost;

    public int atk;
    public int range;
    public int moveRange;

    public int upgradeTurnCount = 0;
    public int dodgeTurnCount = 0;

    [Header("For show")]
    public int hp;
    public HexCell choosenTargetCell;
    public bool isUpgraded = false;

    private void Start()
    {
        hp = maxHP;
    }

    public void TakeDamage(int damage)
    {
        if(unitType == UnitType.Tank && isUpgraded)
        {
            damage -= 1;
            damage = Mathf.Max(damage, 0);
        }
        if(dodgeTurnCount >= 2)
        {
            damage = 0;
            dodgeTurnCount = 0;
        }
           
        hp -= damage;
        if (hp < 0)
        {
            Destroy(gameObject);
        }
    }

    public void MoveToHexCell(HexCell newHexCell)
    {
        if (hexCell)
        {
            hexCell.unitList.Remove(this);
        }
        hexCell = newHexCell;
        newHexCell.unitList.Add(this);
        transform.position = newHexCell.transform.position + new Vector3(0,0.1f,0);

    }

    public void Upgrade()
    {
        // change stats
        if (unitType == UnitType.Tank)
        {
            maxHP = 8;
            atk = 4;
            range = 1;
            moveRange = 1;
        }
        if (unitType == UnitType.Light)
        {
            maxHP = 6;
            atk = 4;
            range = 1;
            moveRange = 3;
        }
        if (unitType == UnitType.Ranger)
        {
            maxHP = 6;
            atk = 4;
            range = 3;
            moveRange = 1;
        }
        // fully heal
        hp = maxHP;
        isUpgraded = true;
    }


}
