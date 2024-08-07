using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelController : MonoBehaviour
{
    private int _progress = 1;
    private int _level = 0;
    
    public int Progress
    {
        get => _progress;
        set
        {
            _progress = value;
            foreach (var rhythmGame in _rhythmGames)
            {
                rhythmGame.bpm += 5;
            }
            
            if (_progress % 5 == 0)
            {
                AddLevel();
            }
        }
    }
    
    public List<BarPossibility> barPossibilities = new List<BarPossibility>();
    public List<Vector2> beatPossibility = new List<Vector2>();
    public float originalMaxClockTime;
    public float minClockMaxTime = 7;
    private RhythmGame[] _rhythmGames;
    private Clock _clock;

    private void AddLevel()
    {
        _level++;
        
        var barPossibilityIndex = Mathf.Clamp(_level, 0, barPossibilities.Count - 1);
        var beatPossibilityIndex = Mathf.Clamp(_level, 0, beatPossibility.Count - 1);
        var minClockMax = Mathf.Clamp(originalMaxClockTime - _level, minClockMaxTime, originalMaxClockTime);
        
        foreach (var rhythmGame in _rhythmGames)
        {
            rhythmGame.measurePossibilities = barPossibilities[barPossibilityIndex].barPossibility;
            rhythmGame.beatPossibilities = beatPossibility[beatPossibilityIndex];
            _clock.maxTime = minClockMax;   
        }
    }

    private void Start()
    {
        _rhythmGames = FindObjectsOfType<RhythmGame>();
        _clock = FindObjectOfType<Clock>();
        originalMaxClockTime = _clock.maxTime;
    }
}

[Serializable]
public class BarPossibility
{
    public int[] barPossibility = new int[4];
}
