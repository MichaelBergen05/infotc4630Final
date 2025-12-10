using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;


[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(BoxCollider2D))]
public class LetterTile : MonoBehaviour
{
    [Header("Runtime")]
    public int row;
    public int col;
    public char letter;

    private SpriteRenderer _sr;

    [Header("Text")]
    [Tooltip("Assign the TextMeshPro / TextMeshProUGUI that shows the letter.")]
    public TMP_Text letterText;   // can be TextMeshPro or TextMeshProUGUI

    private bool _isSelected;

    [Header("Colors")]
    public Color normalColor = Color.white;
    public Color selectedColor = Color.yellow;

    private void Awake()
    {
        _sr = GetComponent<SpriteRenderer>();

        // If not assigned, try to find one on children
        if (letterText == null)
        {
            letterText = GetComponentInChildren<TMP_Text>();
        }

        UpdateVisual();
    }

    public void Init(int r, int c, char ch)
    {
        row = r;
        col = c;
        letter = ch;

        if (letterText == null)
        {
            letterText = GetComponentInChildren<TMP_Text>();
        }

        if (letterText != null)
        {
            letterText.text = letter.ToString().ToUpper();
        }
        else
        {
            Debug.LogError("LetterTile: No TMP_Text found on prefab or children!", this);
        }

        SetSelected(false);
    }

    public void SetSelected(bool selected)
    {
        _isSelected = selected;
        UpdateVisual();
    }

    public bool IsSelected => _isSelected;

    private void UpdateVisual()
    {
        if (_sr != null)
        {
            _sr.color = _isSelected ? selectedColor : normalColor;
        }
    }

    private void OnMouseDown()
    {
        Debug.Log($"Tile clicked: {letter} at ({row},{col})", this);

        if (SelectionManager.Instance != null)
        {
            SelectionManager.Instance.HandleTileClicked(this);
        }
        else
        {
            Debug.LogWarning("SelectionManager.Instance is null; did you add SelectionManager to the scene?");
            SetSelected(true);
        }
    }
}
