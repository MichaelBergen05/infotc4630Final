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
    public GameManager gameManager;

    private SpriteRenderer _sr;

    [Header("Text")]
    [Tooltip("Assign the TextMeshPro that shows the letter.")]
    public TMP_Text letterText;  

    private bool _isSelected;

    [Header("Colors")]
    public Color normalColor = Color.white;
    public Color selectedColor = new Color(0.65f, 0.78f, 0.85f);

    [Header("Audio")]
    public AudioClip letterClickSound;
    public AudioClip selectSound;

    private AudioSource _audio;

    private void Awake()
    {
        _sr = GetComponent<SpriteRenderer>();

        if (letterText == null)
            letterText = GetComponentInChildren<TMP_Text>();

        _audio = GetComponent<AudioSource>();
        if (_audio == null)
            _audio = gameObject.AddComponent<AudioSource>();

        _audio.playOnAwake = false;
        _audio.volume = 0.15f; // keep this quiet

        UpdateVisual();

        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
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

        if (gameManager.Lost == true) return;

        if (SelectionManager.Instance != null)
        {
            PlaySelectSound();
            SelectionManager.Instance.HandleTileClicked(this);
        }
        else
        {
            Debug.LogWarning("SelectionManager.Instance is null; did you add SelectionManager to the scene?");
            SetSelected(true);
        }
    }

    public IEnumerator PlayScoreAnimation()
    {
        Vector3 startScale = transform.localScale;
        Quaternion startRot = transform.localRotation;

        float duration = 0.25f;
        float t = 0f;

        Quaternion targetRot = Quaternion.Euler(0, 0, Random.Range(-10f, 10f));

        Vector3 targetScale = startScale * 1.3f;

        bool soundPlayed = false;

        // Pop out
        while (t < 1f)
        {
            t += Time.deltaTime / duration;
            transform.localScale = Vector3.Lerp(startScale, targetScale, t);
            transform.localRotation = Quaternion.Lerp(startRot, targetRot, t);

            if (!soundPlayed && t >= 0.95f)
            {
                PlayLetterSound();
                soundPlayed = true;
            }

            yield return null;
        }

        yield return new WaitForSeconds(0.1f);

        // Settle back
        t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime / duration;
            transform.localScale = Vector3.Lerp(targetScale, startScale, t);
            transform.localRotation = Quaternion.Lerp(targetRot, startRot, t);
            yield return null;
        }
    }

    private void PlayLetterSound()
    {
        if (letterClickSound == null || _audio == null)
            return;

        _audio.pitch = Random.Range(0.95f, 1.05f);
        _audio.PlayOneShot(letterClickSound);
    }

    private void PlaySelectSound()
    {
        if (selectSound == null || _audio == null)
            return;

        _audio.pitch = Random.Range(0.95f, 1.05f);
        _audio.PlayOneShot(selectSound);
    }
}
