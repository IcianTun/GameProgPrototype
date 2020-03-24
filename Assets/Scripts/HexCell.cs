using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum HexDirection
{
    NE, E, SE, SW, W, NW
}
public static class HexDirectionExtensions
{

    public static HexDirection Opposite(this HexDirection direction)
    {
        return (int)direction < 3 ? (direction + 3) : (direction - 3);
    }
}

public class HexCell : MonoBehaviour {

    [SerializeField]
    HexCell[] neighbors;

    public HexCoordinates coordinates;
    public Color color;
    public Color defaultColor;

    public List<Unit> unitList;

    public bool isObjectiveZone = false;
    public bool isUpgradeCell = false;

    void Start()
    {
        unitList = new List<Unit>();
    }

    public HexCell GetNeighbor(HexDirection direction)
    {
        return neighbors[(int)direction];
    }

    public HexCell[] GetNeightbors()
    {
        return neighbors;
    }

    public void SetNeighbor(HexDirection direction, HexCell cell)
    {
        if (cell) { 
            neighbors[(int) direction] = cell;
            cell.neighbors[(int)direction.Opposite()] = this;
        }
    }

    public void ResetUnitList()
    {
        unitList.Clear();
    }

}
