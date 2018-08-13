using UnityEngine;

[CreateAssetMenu(fileName = "LevelData", menuName = "Level Data")]
public class LevelDataSO : ScriptableObject
{
    [SerializeField] public LevelData[] data;
}

[System.Serializable]
public struct LevelData
{
    public Difficulty difficulty;
    public int MomHP;
    public float glitchSpeed;
    public float levelDuration;
    public float percentInitialSpacesFilled;
    public float clearPercent;

    public int smallWaves;
    public float smallTimeBetweenWaves;
    public int smallUnitsPerWave;
    public float smallMomChance;

    public int bigWaves;
    public float bigTimeBetweenWaves;

    public int bigWaveSurges;
    public float bigWaveTimeBetweenSurges;
    public int bigWaveUnitsPerSurge;
    public float bigWaveMomChance;
}
