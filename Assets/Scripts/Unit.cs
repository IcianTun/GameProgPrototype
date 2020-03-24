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

    }


}
