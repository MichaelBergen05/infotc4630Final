using UnityEngine;

public class TitleWaveController : MonoBehaviour
{
    public float phaseStep = 0.4f;

    void Start()
    {
        TitleTileWave[] tiles = GetComponentsInChildren<TitleTileWave>();

        int count = tiles.Length;

        for (int i = 0; i < count; i++)
        {
            // Reverse the order
            tiles[i].phaseOffset = (count - 1 - i) * phaseStep;
        }
    }
}