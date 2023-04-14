using UnityEngine;

[CreateAssetMenu(fileName = "AISettings", menuName = "AI/AISettings")]
public class AISettings : ScriptableObject
{
    [Range(0, 20)]
    public int skillLevel;

    [Range(1, 20)]
    public int depth;

    [Range(0, 500)]
    public int maxError;

    [Range(0,100)]
    public int matchProbability;

    [Range(0, 5000)]
    public int minThinkingTime;

    [Range(0,100)]
    public int decay;

    public bool useBook;
}