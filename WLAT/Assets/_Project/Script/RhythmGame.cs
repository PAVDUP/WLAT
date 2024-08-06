using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine.UI;
using Utils;

public class RhythmGame : MonoBehaviour
{
    public AudioSource audioSource;
    public float bpm = 120f;
    public int minMeasures = 1;
    public int maxMeasures = 4;
    public float threshold = 0.1f;

    private List<bool> _rhythm;
    private float _beatInterval;
    private float _nextBeatTime;
    private int _currentBeat;
    private bool _isPlaying;
    private int _totalBeats;

    #region ComputerRhythm
    
    public Transform computerBeatVisualizer;

    void GenerateRhythm()
    {
        int measures = Random.Range(minMeasures, maxMeasures + 1);
        _totalBeats = measures * 4;
        _rhythm = new List<bool>(_totalBeats);

        for (int i = 0; i < _totalBeats; i++)
        {
            _rhythm.Add(false);
        }

        for (int m = 0; m < measures; m++)
        {
            int beatIndex = m * 4 + Random.Range(0, 4);
            _rhythm[beatIndex] = true;
        }

        for (int i = 0; i < _totalBeats; i++)
        {
            if (_rhythm[i])
            {
                // 30% 확률로 비트를 true
                if (Random.Range(0, 100) < 30)
                {
                    _rhythm[i] = true;
                }
            }
        }
    }

    IEnumerator PlayComputerRhythm()
    {
        // Reset visualizer
        ResetVisualizer();
        
        yield return new WaitForSeconds(3f);
        
        for (int i = 0; i < _totalBeats; i++)
        {
            if (_rhythm[i])
            {
                audioSource.Play();
                VisualizeBeat(0f, computerBeatVisualizer);
            }
            else
            {
                VisualizeEmptyBeat(computerBeatVisualizer);
            }
            
            yield return new WaitForSeconds(_beatInterval);
        }

        yield return new WaitForSeconds(3f);
        StartGame();
    }
    
    #endregion

    #region PlayerRhythm
    
    public Transform playerBeatVisualizer;
    private bool _isPlayerInputChecked;
    private bool _isGameOver;
    
    void StartGame()
    {
        _currentBeat = 0;
        _nextBeatTime = Time.time;
        _minCheckTime = _nextBeatTime - _beatInterval * playerInputThreshold / 2;
        _maxCheckTime = _nextBeatTime + _beatInterval * playerInputThreshold / 2;
        _isPlaying = true;
    }

    void CheckBeat()
    {
        if (!_isPlayerInputChecked)
        {
            VisualizeEmptyBeat(playerBeatVisualizer);
        }
        
        if (!_isPlayerInputChecked && _rhythm[_currentBeat])
        {
            GameOver();
        }
        
        _currentBeat = (_currentBeat + 1);
        
        if (_currentBeat >= _totalBeats)
        {
            _isPlaying = false;

            if (!_isGameOver)
            {
                // !!!!! Re generate rhythm
                GenerateRhythm();
                StartCoroutine(PlayComputerRhythm());
            }
            else
            {
                OnGameOverEternal();
            }
        }
    }

    void CheckPlayerInput()
    {
        audioSource.Play();
        VisualizeBeat(0, playerBeatVisualizer);
        
        if (!_isPlayerInputChecked)
            _isPlayerInputChecked = true;
        else
        {
            _isGameOver = true;
            return;
        }
        
        if (Time.time >= _minCheckTime && Time.time <= _maxCheckTime)
        {
            if (!_rhythm[_currentBeat])
            {
                GameOver();
            }
        }
        else
        {
            GameOver();
        }
    }

    void GameOver()
    {
        _isGameOver = true;
        
        Debug.Log("Game Over!");
    }
    
    #endregion

    #region Common Methods
    
    [Header("Object Pool")]
    public SpriteRendererPoolManager objectPoolManager;
    public Sprite noonSprite;
    public Sprite nightSprite;
    public Sprite emptySprite;
    
    void VisualizeBeat(float delay, Transform beatVisualizer)
    {
        StartCoroutine(VisualizeBeatCoroutine(delay, beatVisualizer));
    }

    IEnumerator VisualizeBeatCoroutine(float delay, Transform beatVisualizer)
    {
        yield return new WaitForSeconds(delay);
        
        var beatObject = objectPoolManager.Pool.Get();
        _beatObjects.Add(beatObject);
        beatObject.transform.SetParent(beatVisualizer);
        
        if (beatVisualizer == playerBeatVisualizer)
        {
            beatObject.sprite = nightSprite;
        }
        else
        {
            beatObject.sprite = noonSprite;
        }
    }
    
    void VisualizeEmptyBeat(Transform beatVisualizer)
    {
        var beatObject = objectPoolManager.Pool.Get();
        _beatObjects.Add(beatObject);
        beatObject.transform.SetParent(beatVisualizer);
        beatObject.sprite = emptySprite;
    }

    #endregion

    #region Reset
    
    private List<Image> _beatObjects = new List<Image>();
    
    private void ResetVisualizer()
    {
        foreach (var beatObject in _beatObjects)
        {
            objectPoolManager.Pool.Release(beatObject);
        }
        
        _beatObjects.Clear();
    }

    #endregion
    
    #region GameOver Eternal
    
    [Header("GameOver")]
    public UnityEvent onGameOver;
    
    void OnGameOverEternal()
    {
        onGameOver.Invoke();
    }

    #endregion
    
    [Header("Check Player Input")]
    [Range(0,1)] public float playerInputThreshold = 1f;
    private float _minCheckTime;
    private float _maxCheckTime;
    
    void Start()
    {
        _beatInterval = 60f / bpm;
        GenerateRhythm();
        StartCoroutine(PlayComputerRhythm());
    }

    void Update()
    {
        if (_isPlaying)
        {
            if (Time.time >= _maxCheckTime)
            {
                CheckBeat();
                _nextBeatTime += _beatInterval;
                _minCheckTime = _nextBeatTime - _beatInterval * playerInputThreshold / 2;
                _maxCheckTime = _nextBeatTime + _beatInterval * playerInputThreshold / 2;
                _isPlayerInputChecked = false;
            }

            if (Input.GetKeyDown(KeyCode.Space))
            {
                CheckPlayerInput();
            }
        }
    }
}