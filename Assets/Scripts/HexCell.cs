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

    public Unit unit;

    public HexCell GetNeighbor(HexDirection direction)
    {
        return neighbors[(int)direction];
    }
    public void SetNeighbor(HexDirection direction, HexCell cell)
    {
        neighbors[(int) direction] = cell;
        cell.neighbors[(int)direction.Opposite()] = this;
    }

}
