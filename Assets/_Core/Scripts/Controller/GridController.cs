using System;
using UnityEngine;
using UnityEngine.UI;

public class GridController : Singleton<GridController>
{
    [Header("Grid Components")]
    [SerializeField] private GridLayoutGroup gridLayoutGroup;
    [SerializeField] private RectTransform gridRect;
    [SerializeField] private Cell cell;

    [Header("Table Colors")]
    [SerializeField] private Color color1;
    [SerializeField] private Color color2;

    private const int gridSize = 8;

    private Cell[,] grid;

    public Cell[,] Grid { get => grid; set => grid = value; }

    private void Start()
    {
        CreateGrid();
    }

    private void CreateGrid()
    {
        float cellSize = gridRect.rect.width / gridSize;

        gridLayoutGroup.cellSize = new Vector2(cellSize, cellSize);

        grid = new Cell[gridSize, gridSize];

        for (sbyte y = 0; y < gridSize; y++)
        {
            for (sbyte x = 0; x < gridSize; x++)
            {
                Cell newCell = Instantiate(cell, gridLayoutGroup.transform);

                bool temp = (x + y) % 2 == 0;

                if (temp)
                    newCell.Init(color1, color2, x, y);
                else
                    newCell.Init(color2, color1, x, y);

                grid[x, y] = newCell;
            }
        }
    }

    public Cell GetCell(sbyte x, sbyte y)
    {
        return grid[x, y];
    }

    public Cell GetCell(string name)
    {
        sbyte x = (sbyte)(name[0] - 'a');
        sbyte y = (sbyte)(name[1] - '1');
        return grid[x, y];
    }

    internal void CleanBoard()
    {
        for (sbyte y = 0; y < gridSize; y++)
        {
            for (sbyte x = 0; x < gridSize; x++)
            {
                ChessPiece piece = grid[x, y].GetChessPiece();
                if (piece != null)
                    Destroy(piece.gameObject);
                grid[x, y].SetChessPiece(null);
            }
        }
    }
}
