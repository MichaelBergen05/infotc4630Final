using UnityEngine;

public class TitleTileWave : MonoBehaviour
{
    public float amplitude = 0.08f;
    public float frequency = 1.5f;
    [HideInInspector] public float phaseOffset;

    private Vector3 startPos;

    void Start()
    {
        startPos = transform.localPosition;
    }

    void Update()
    {
        float t = Mathf.Clamp01(Time.timeSinceLevelLoad);
        float currentAmp = Mathf.Lerp(0f, amplitude, t);

        float y = Mathf.Sin(Time.time * frequency + phaseOffset) * currentAmp;
        transform.localPosition = startPos + Vector3.up * y;
    }
}