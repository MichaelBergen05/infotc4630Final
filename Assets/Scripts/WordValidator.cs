using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WordValidator : MonoBehaviour 
{
    public static WordValidator Instance { get; private set; }

    [Header("Dictionary File")]
    public TextAsset wordList;

    private string[] _words; // sorted, uppercase

    [Header("Rules")]
    [Tooltip("Minimum length for a word to be considered valid.")]
    public int minWordLength = 3;

    private void Awake()
    {
        // Singleton pattern
        if (Instance != null && Instance != this)
        {
            Debug.LogWarning("Multiple WordValidator instances detected. Destroying this one.", this);
            Destroy(gameObject);
            return;
        }
        Instance = this;

        LoadWordList();
    }

    private void LoadWordList()
    {
        if (wordList == null)
        {
            Debug.LogError("WordValidator: No wordList TextAsset assigned in the Inspector!");
            return;
        }

        // Get raw text
        string temp = wordList.text;

        // Split on both \n and \r, remove empty lines
        string[] rawLines = temp.Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);

        // Normalize: trim + uppercase
        for (int i = 0; i < rawLines.Length; i++)
        {
            rawLines[i] = rawLines[i].Trim().ToUpperInvariant();
        }

        // Optionally sort to guarantee binary search works,
        // in case the file isn't already sorted.
        Array.Sort(rawLines, StringComparer.Ordinal);

        _words = rawLines;

        Debug.Log($"WordValidator: Loaded {_words.Length} words from TextAsset.");
    }

    /// <summary>
    /// Static API used by GameManager: WordValidator.IsValid(word)
    /// </summary>
    public static bool IsValid(string word)
    {
        if (Instance == null)
        {
            Debug.LogError("WordValidator.IsValid called but no WordValidator instance exists in the scene.");
            return false;
        }

        return Instance.IsWord(word);
    }

    /// <summary>
    /// Instance method that uses binary search on the _words array.
    /// </summary>
    private bool IsWord(string word)
    {
        if (_words == null || _words.Length == 0)
            return false;

        if (string.IsNullOrWhiteSpace(word))
            return false;

        string key = word.Trim().ToUpperInvariant();

        if (key.Length < minWordLength)
            return false;

        int low = 0;
        int high = _words.Length - 1;

        while (low <= high)
        {
            int mid = low + (high - low) / 2;

            int cmp = string.Compare(_words[mid], key, StringComparison.Ordinal);
            if (cmp == 0)
            {
                return true;
            }
            if (cmp < 0)
            {
                low = mid + 1;
            }
            else
            {
                high = mid - 1;
            }
        }

        return false;
    }
}
