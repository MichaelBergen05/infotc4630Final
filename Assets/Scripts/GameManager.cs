using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;


public class GameManager : MonoBehaviour
{
    [Header("UI")]
    public TextMeshProUGUI feedbackText;
    public TextMeshProUGUI scoreText;

    private int _score = 0;

    public void OnConfirmWord()
    {
        string word = SelectionManager.Instance.GetCurrentWord();
        if (string.IsNullOrEmpty(word))
        {
            SetFeedback("No letters selected.");
            return;
        }

        if (WordValidator.IsValid(word))
        {
            int points = word.Length * 10; // simple scoring rule
            _score += points;
            UpdateScoreUI();

            // Copy the tiles so ClearSelection doesn't wipe our reference
            List<LetterTile> tiles = new List<LetterTile>(SelectionManager.Instance.GetCurrentTiles());

            SetFeedback($"Valid word: {word} (+{points})");

            // Clear selection visuals
            SelectionManager.Instance.ClearSelection();

            // Clear those tiles from the board and refill
            if (BoardManager.Instance != null)
            {
                BoardManager.Instance.ClearAndRefill(tiles);
            }
            else
            {
                Debug.LogError("GameManager: BoardManager.Instance is null!");
            }
        }
        else
        {
            SetFeedback($"Not a valid word: {word}");
            SelectionManager.Instance.ClearSelection();
        }
    }

    private void SetFeedback(string msg)
    {
        if (feedbackText != null)
        {
            feedbackText.text = msg;
        }
    }

    private void UpdateScoreUI()
    {
        if (scoreText != null)
        {
            scoreText.text = $"Score: {_score}";
        }
    }
}

