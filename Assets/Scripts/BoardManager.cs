using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardManager : MonoBehaviour
{
    public static BoardManager Instance { get; private set; }

    [Header("Board Settings")]
    public int rows = 8;
    public int cols = 8;
    public float tileSpacing = 1.1f;

    [Header("Prefabs & Assets")]
    public LetterTile tilePrefab;

    private LetterTile[,] _tiles;

    // Cache offsets so we can reuse them
    private float _xOffset;
    private float _yOffset;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Start()
    {
        GenerateBoard();
    }

    private void GenerateBoard()
    {
        _tiles = new LetterTile[rows, cols];

        // Compute offsets once
        _xOffset = -(cols - 1) * tileSpacing * 0.5f;
        _yOffset = -(rows - 1) * tileSpacing * 0.5f;

        for (int r = 0; r < rows; r++)
        {
            for (int c = 0; c < cols; c++)
            {
                Vector3 pos = GetWorldPosition(r, c);
                LetterTile tile = Instantiate(tilePrefab, pos, Quaternion.identity, transform);
                char randomLetter = GetRandomLetter();
                tile.Init(r, c, randomLetter);

                _tiles[r, c] = tile;
            }
        }
    }

    private Vector3 GetWorldPosition(int r, int c)
    {
        return new Vector3(c * tileSpacing + _xOffset,
                           r * tileSpacing + _yOffset,
                           0f);
    }

    private char GetRandomLetter()
    {
        // Basic A–Z random letter (you can later weight this by frequency)
        int index = Random.Range(0, 26); // 0–25
        return (char)('A' + index);
    }

    public LetterTile GetTile(int r, int c)
    {
        if (r < 0 || r >= rows || c < 0 || c >= cols) return null;
        return _tiles[r, c];
    }

    /// <summary>
    /// Clear the given tiles, drop above tiles down, and spawn new ones at the top.
    /// </summary>
    public void ClearAndRefill(List<LetterTile> tilesToClear)
    {
        if (tilesToClear == null || tilesToClear.Count == 0) return;

        // 1) Mark cleared tiles as null and destroy their GameObjects
        foreach (LetterTile tile in tilesToClear)
        {
            if (tile == null) continue;

            int r = tile.row;
            int c = tile.col;

            if (r >= 0 && r < rows && c >= 0 && c < cols)
            {
                if (_tiles[r, c] == tile)
                {
                    _tiles[r, c] = null;
                }
            }

            Destroy(tile.gameObject);
        }

        // 2) For each column, drop tiles down to fill gaps
        for (int c = 0; c < cols; c++)
        {
            int writeRow = 0; // next row where we place a non-null tile

            // Move existing tiles down
            for (int r = 0; r < rows; r++)
            {
                if (_tiles[r, c] != null)
                {
                    if (r != writeRow)
                    {
                        LetterTile tile = _tiles[r, c];
                        _tiles[writeRow, c] = tile;
                        _tiles[r, c] = null;

                        tile.row = writeRow;
                        tile.transform.position = GetWorldPosition(writeRow, c);
                    }
                    writeRow++;
                }
            }

            // 3) Spawn new tiles above to fill remaining spaces at top
            for (int r = writeRow; r < rows; r++)
            {
                Vector3 pos = GetWorldPosition(r, c);

                LetterTile newTile = Instantiate(tilePrefab, pos, Quaternion.identity, transform);
                char letter = GetRandomLetter();
                newTile.Init(r, c, letter);

                _tiles[r, c] = newTile;
            }
        }
    }
}
