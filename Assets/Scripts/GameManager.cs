using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;


public class GameManager : MonoBehaviour
{
    [Header("UI")]
    public TextMeshProUGUI feedbackText;
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI movesText;
    public TextMeshProUGUI levelText;
    public GameObject LosePanel;
    public GameObject ConfirmButton;
    public GameObject AnnoyingParchment;
    public bool Lost = false;

    [Header("Audio")]
    public AudioClip wordScoreSound;
    public AudioClip failSound;

    private AudioSource _audio;

    [Header("Level System")]
    public LevelData[] levels;

    private int _currentLevelIndex = 0;
    private int _movesRemaining;

    private int _score = 0;

    private void Start()
    {
        StartLevel(0);

        _audio = GetComponent<AudioSource>();
        if (_audio == null)
            _audio = gameObject.AddComponent<AudioSource>();

        _audio.playOnAwake = false;
        _audio.volume = 0.15f; // keep this quiet
    }

    private void StartLevel(int levelIndex)
    {
        _currentLevelIndex = levelIndex;
        _score = 0;

        LevelData level = levels[_currentLevelIndex];
        _movesRemaining = level.maxMoves;

        UpdateScoreUI();
        UpdateMovesUI();
        UpdateLevelUI();
        if(_currentLevelIndex == 5)
            SetFeedback($"Level ???: Reach {level.targetScore} points");
        else
            SetFeedback($"Level {levelIndex + 1}: Reach {level.targetScore} points");
    }

    public void OnConfirmWord()
    {
        string word = SelectionManager.Instance.GetCurrentWord();
        if (string.IsNullOrEmpty(word))
        {
            SetFeedback("No letters selected.");
            return;
        }

        _movesRemaining--;
        UpdateMovesUI();

        if (WordValidator.IsValid(word))
        {
            int points = CalculateWordScore(word);
            _score += points;
            

            CheckLevelState();

            List<LetterTile> tiles =
                new List<LetterTile>(SelectionManager.Instance.GetCurrentTiles());

            SetFeedback($"Valid word: {word} (+{points})");

            SelectionManager.Instance.ClearSelection();

            StartCoroutine(AnimateAndClearTiles(tiles));
        }
        else
        {
            PlayFailSound();
            SetFeedback($"Not a valid word: {word}");
            SelectionManager.Instance.ClearSelection();
            CheckLevelState();
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
            LevelData level = levels[_currentLevelIndex];
            scoreText.text = $"Score: {_score} / {level.targetScore}";
        }
    }

    private void CheckLevelState()
    {
        LevelData level = levels[_currentLevelIndex];

        if (_score >= level.targetScore)
        {
            SetFeedback("Level Complete!");
            StartCoroutine(AdvanceLevel());
        }
        else if (_movesRemaining <= 0)
        {
            OnLose();
        }
    }

    private IEnumerator AdvanceLevel()
    {
        yield return new WaitForSeconds(1.5f);

        if (_currentLevelIndex + 1 < levels.Length)
        {
            StartLevel(_currentLevelIndex + 1);
            BoardManager.Instance.ClearAndRefillAll(); // optional helper
        }
        else
        {
            SetFeedback("You win!");
            // Later: load end screen
        }
    }

    private IEnumerator RestartLevel()
    {
        yield return new WaitForSeconds(1.5f);
        StartLevel(_currentLevelIndex);
        BoardManager.Instance.ClearAndRefillAll(); // optional helper
    }

    private void UpdateMovesUI()
    {
        if (movesText != null)
            movesText.text = $"Moves: {_movesRemaining}";
    }

    private void UpdateLevelUI()
    {
        if (levelText != null && _currentLevelIndex != 5)
            levelText.text = $"Level {_currentLevelIndex + 1}";
        else
            levelText.text = $"Level ???";
    }

    public void MenuButton()
    {
        SceneManager.LoadScene("_Menu");
    }

    public void OnLose()
    {
        PlayFailSound();
        Lost = true;
        AnnoyingParchment.SetActive(false);
        ConfirmButton.SetActive(false);
        LosePanel.SetActive(true);
    }

    public void Retry()
    {
        SceneManager.LoadScene("_Level_Mode");
    }

    private int GetLetterValue(char c)
    {
        c = char.ToUpperInvariant(c);

        // Common letters
        if ("EAIONRTLSU".Contains(c))
            return 10;

        // Mid-frequency letters
        if ("DGBCMPFHVWY".Contains(c))
            return 20;

        // Rare letters (K J X Q Z)
        return 30;
    }

    private float GetLengthMultiplier(int length)
    {
        if (length >= 10) return 3.2f;
        if (length == 9) return 2.6f;
        if (length == 8) return 2.1f;
        if (length == 7) return 1.7f;
        if (length == 6) return 1.4f;
        if (length == 5) return 1.2f;
        if (length == 4) return 1.1f;
        return 1.0f;
    }

    private int CalculateWordScore(string word)
    {
        int baseScore = 0;

        foreach (char c in word)
        {
            baseScore += GetLetterValue(c);
        }

        float multiplier = GetLengthMultiplier(word.Length);
        return Mathf.RoundToInt(baseScore * multiplier);
    }

    private IEnumerator AnimateAndClearTiles(List<LetterTile> tiles)
    {
        // Play animation on each tile
        foreach (LetterTile tile in tiles)
        {
            if (tile != null)
                tile.StartCoroutine(tile.PlayScoreAnimation());

            yield return new WaitForSeconds(0.15f);
        }

        // Optional: play ONE sound here
        // audioSource.PlayOneShot(scoreSound);

        // Wait for animation to finish
        yield return new WaitForSeconds(.04f + (tiles.Count * .16f));

        PlayScoreSound();
        UpdateScoreUI();

        if (BoardManager.Instance != null)
        {
            BoardManager.Instance.ClearAndRefill(tiles);
        }
    }

    private void PlayScoreSound()
    {
        if (wordScoreSound == null || _audio == null)
            return;

        _audio.pitch = Random.Range(0.95f, 1.05f);
        _audio.PlayOneShot(wordScoreSound);
    }

    private void PlayFailSound()
    {
        if (failSound == null || _audio == null)
            return;

        _audio.pitch = Random.Range(0.95f, 1.05f);
        _audio.PlayOneShot(failSound);
    }
}

