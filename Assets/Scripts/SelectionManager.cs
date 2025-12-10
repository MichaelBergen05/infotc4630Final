using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SelectionManager : MonoBehaviour
{
    public static SelectionManager Instance { get; private set; }

    [Header("UI")]
    public TextMeshProUGUI currentWordText;

    private List<LetterTile> _selectedTiles = new List<LetterTile>();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public void HandleTileClicked(LetterTile tile)
    {
        // Case 1: tile is already last in path → deselect it (backtrack one step)
        if (_selectedTiles.Count > 0 && _selectedTiles[_selectedTiles.Count - 1] == tile)
        {
            RemoveLastTile();
            return;
        }

        // Case 2: tile is already elsewhere in the path → ignore (or handle special logic)
        if (_selectedTiles.Contains(tile))
        {
            // For now do nothing. We can allow “rewinding” later if you want.
            return;
        }

        // Case 3: first tile in path → always accept
        if (_selectedTiles.Count == 0)
        {
            AddTile(tile);
            return;
        }

        // Case 4: must be adjacent (including diagonal) to the last tile
        LetterTile last = _selectedTiles[_selectedTiles.Count - 1];
        if (AreAdjacent(last, tile))
        {
            AddTile(tile);
        }
        else
        {
            // Not adjacent → ignore
        }
    }

    private void AddTile(LetterTile tile)
    {
        _selectedTiles.Add(tile);
        tile.SetSelected(true);
        UpdateCurrentWordUI();
    }

    private void RemoveLastTile()
    {
        if (_selectedTiles.Count == 0) return;
        LetterTile last = _selectedTiles[_selectedTiles.Count - 1];
        last.SetSelected(false);
        _selectedTiles.RemoveAt(_selectedTiles.Count - 1);
        UpdateCurrentWordUI();
    }

    private bool AreAdjacent(LetterTile a, LetterTile b)
    {
        int dr = Mathf.Abs(a.row - b.row);
        int dc = Mathf.Abs(a.col - b.col);
        // Adjacent including diagonal = both dr and dc ≤ 1, and not the same tile
        return (dr <= 1 && dc <= 1 && (dr + dc) > 0);
    }

    public string GetCurrentWord()
    {
        System.Text.StringBuilder sb = new System.Text.StringBuilder();
        foreach (var t in _selectedTiles)
        {
            sb.Append(t.letter);
        }
        return sb.ToString();
    }

    public List<LetterTile> GetCurrentTiles()
    {
        return _selectedTiles;
    }

    public void ClearSelection()
    {
        foreach (var t in _selectedTiles)
        {
            if (t != null)
                t.SetSelected(false);
        }
        _selectedTiles.Clear();
        UpdateCurrentWordUI();
    }

    private void UpdateCurrentWordUI()
    {
        if (currentWordText != null)
        {
            currentWordText.text = GetCurrentWord();
        }
    }
}
