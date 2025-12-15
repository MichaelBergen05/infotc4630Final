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

    // A–Z frequencies in English (sum = 100)
    private static readonly float[] letterFrequencies = {
    8.167f, // A
    1.492f, // B
    2.782f, // C
    4.253f, // D
    12.702f,// E
    2.228f, // F
    2.015f, // G
    6.094f, // H
    6.966f, // I
    0.153f, // J
    0.772f, // K
    4.025f, // L
    2.406f, // M
    6.749f, // N
    7.507f, // O
    1.929f, // P
    0.095f, // Q
    5.987f, // R
    6.327f, // S
    9.056f, // T
    2.758f, // U
    0.978f, // V
    2.360f, // W
    0.150f, // X
    1.974f, // Y
    0.074f  // Z
    };

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
        _xOffset = -(cols - 1) * tileSpacing * 0.95f;
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
        float total = 0f;
        foreach (float freq in letterFrequencies)
            total += freq;

        float roll = Random.Range(0f, total);
        float cumulative = 0f;

        for (int i = 0; i < letterFrequencies.Length; i++)
        {
            cumulative += letterFrequencies[i];
            if (roll <= cumulative)
            {
                return (char)('A' + i);
            }
        }

        // Fallback (should never hit)
        return 'E';
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
    public void ClearAndRefillAll()
    {
        for (int r = 0; r < rows; r++)
        {
            for (int c = 0; c < cols; c++)
            {
                if (_tiles[r, c] != null)
                {
                    Destroy(_tiles[r, c].gameObject);
                    _tiles[r, c] = null;
                }
            }
        }

        GenerateBoard();
    }
}
