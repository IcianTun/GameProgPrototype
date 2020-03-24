using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class HexGrid : MonoBehaviour
{

    public int width = 6;
    public int height = 6;

    public HexCell cellPrefab;
    public Text cellLabelPrefab;

    Color whiteColor = Color.white;
    Color blueColor = Color.blue;
    Color redColor = Color.red;
    Color greyColor = Color.grey;
    Color objColor = Color.green;

    HexCell[] cells;

    Canvas gridCanvas;
    HexMesh hexMesh;

    public int index;
    public HexCell cellAtIndex;

    public List<HexCell> objectiveCells;

    void Awake()
    {
        gridCanvas = GetComponentInChildren<Canvas>();
        hexMesh = GetComponentInChildren<HexMesh>();
        cells = new HexCell[height * width];
        objectiveCells = new List<HexCell>();
        //for (int z = 0, i = 0; z < height; z++)
        //{
        //    for (int x = 0; x < width; x++)
        //    {
        //        CreateCell(x, z, i++);
        //    }
        //}


        for (int z = 0; z < height; z++)
        {
            int w;
            if (z % 2 == 1) w = width - 1;
            else w = width;
            for (int x = 0; x < w; x++)
            {
                CreateCell(x, z, z * width + x);
            }
        }
    }

    void Start()
    {
        hexMesh.Triangulate(cells);
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (!EventSystem.current.IsPointerOverGameObject()) { 
                HandleInput();
            }
            else
            {
                //Debug.Log("mousedOver");
            }
        }
        cellAtIndex = cells[index];
    }

    void HandleInput()
    {
        Ray inputRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(inputRay, out hit))
        {
            TouchCell(hit.point);
        }
    }

    void TouchCell(Vector3 position)
    {
        position = transform.InverseTransformPoint(position);
        HexCoordinates coordinate = HexCoordinates.FromPosition(position);
        HexCell cell = GetCell(coordinate.X, coordinate.Z);
        GameManagerScript.Instance.HandleOnClickCell(cell);
    }

    public void RenderCell()
    {
        hexMesh.Triangulate(cells);
    }
    public void ResetColor()
    {
        foreach (HexCell cell in cells)
        {
            if (cell)
                cell.color = cell.defaultColor;
        }
        RenderCell();
    }

    public HexCell GetCell(int xCoordinate, int zCoordinate)
    {
        int index = xCoordinate + zCoordinate * width + zCoordinate / 2;
        return cells[index];
    }

    public void ResetCellsUnitList()
    {
        foreach (HexCell cell in cells)
        {
            if (cell)
                cell.ResetUnitList();
        }
    }

    void CreateCell(int x, int z, int i)
    {
        Vector3 position;
        position.x = (x + z * 0.5f - z / 2) * (HexMetrics.innerRadius * 2f);
        position.y = 0f;
        position.z = z * (HexMetrics.outerRadius * 1.5f);

        HexCell cell = cells[i] = Instantiate<HexCell>(cellPrefab);
        cell.transform.SetParent(transform, false);
        cell.transform.localPosition = position;
        cell.coordinates = HexCoordinates.FromOffsetCoordinates(x, z);
        cell.color = whiteColor;
        cell.defaultColor = whiteColor;
        if (x > 0)
        {
            cell.SetNeighbor(HexDirection.W, cells[i - 1]);
        }
        if (z > 0)
        {
            if ((z & 1) == 0)
            {
                cell.SetNeighbor(HexDirection.SE, cells[i - width]);
                if (x > 0)
                {
                    cell.SetNeighbor(HexDirection.SW, cells[i - width - 1]);
                }
            }
            else
            {
                cell.SetNeighbor(HexDirection.SW, cells[i - width]);
                if (x < width - 1)
                {
                    cell.SetNeighbor(HexDirection.SE, cells[i - width + 1]);
                }
            }
        }

        if (z % 2 == 1)
        {
            cell.color = greyColor;
            cell.defaultColor = greyColor;
        }

        if (z < 2)
        {
            cell.color = blueColor;
            cell.defaultColor = blueColor;
        }

        if (z > (height - 3))
        {
            cell.color = redColor;
            cell.defaultColor = redColor;
        }

        if (z == 6)
        {
            if (x >= 3 & x <= 5)
            {
                cell.color = objColor;
                cell.defaultColor = objColor;
                objectiveCells.Add(cell);
                cell.isObjectiveZone = true;
            }
        }
        if (z == 5 | z == 7)
        {
            if (x >= 3 & x <= 4)
            {
                cell.color = objColor;
                cell.defaultColor = objColor;
            }
        }


        Text label = Instantiate<Text>(cellLabelPrefab);
        label.rectTransform.SetParent(gridCanvas.transform, false);
        label.rectTransform.anchoredPosition =
            new Vector2(position.x, position.z);
        label.text = cell.coordinates.ToStringOnSeparateLines();
    }
}