using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using TMPro;
using UnityEngine.Events;
using UnityEngine.UI;
using Utils;

public class RhythmGame : MonoBehaviour
{
    public bool mainGame = false;
    
    public AudioSource audioSource;
    public float bpm = 120f;
    [HideInInspector] public int[] measurePossibilities;
    [HideInInspector] public Vector2 beatPossibilities;
    public float threshold = 0.1f;

    private List<bool> _rhythm;
    private float _beatInterval;
    private float _nextBeatTime;
    private int _currentBeat;
    private bool _isPlaying;
    private int _totalBeats;

    #region ComputerRhythm
    
    [Header("ComputerRhythm")]
    public float computerWaitTime = 3f;
    public Transform computerBeatVisualizer;
    public UnityEvent onComputerInput;
    
    void GenerateRhythm()
    {
        int randomValue = Random.Range(0, 101);
        int measures = 0;
        int stackedValue = 0;
        
        for (int i = 0 ; i < measurePossibilities.Length; i++)
        {
            stackedValue += measurePossibilities[i];
            if (randomValue <= stackedValue)
            {
                measures = i + 1;
                break;
            }
        }
        
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

        var beatPossibility = Random.Range(beatPossibilities.x, beatPossibilities.y);
        
        for (int i = 0; i < _totalBeats; i++)
        {
            if (!_rhythm[i])
            {
                // 45% 확률로 비트를 true
                if (Random.Range(0, 100) < beatPossibility)
                {
                    _rhythm[i] = true;
                }
            }
        }

        _rhythm[0] = false;
    }

    IEnumerator PlayComputerRhythm()
    {
        // Reset visualizer
        ResetVisualizer();
        
        if (mainGame)
            timingText.text = computerWaitTime.ToString(CultureInfo.InvariantCulture);

        yield return new WaitForSeconds(computerWaitTime/3);
   
        if (mainGame)
            timingText.text = (computerWaitTime - computerWaitTime/3).ToString(CultureInfo.InvariantCulture);
        
        yield return new WaitForSeconds(computerWaitTime/3);
        
        if (mainGame)
            timingText.text = (computerWaitTime - 2*computerWaitTime/3).ToString(CultureInfo.InvariantCulture);
    
        yield return new WaitForSeconds(computerWaitTime/3);
        
        if (mainGame)
            timingText.text = "!!!";

        if (mainGame)
            StartCoroutine(StartBasicRhythm());
        
        for (int i = 0; i < _totalBeats; i++)
        {
            if (_rhythm[i])
            {
                audioSource.Play();
                onComputerInput.Invoke();
                VisualizeBeat(0, computerBeatVisualizer);
            }
            else
            {
                VisualizeEmptyBeat(computerBeatVisualizer);
            }
            
            yield return new WaitForSeconds(_beatInterval);
        }

        if (mainGame)
            timingText.text = playerWaitTime.ToString(CultureInfo.InvariantCulture);
        
        StartGame();
        
        yield return new WaitForSeconds(playerWaitTime/3);
   
        if (mainGame)
            timingText.text = (playerWaitTime - playerWaitTime/3).ToString(CultureInfo.InvariantCulture);
        
        yield return new WaitForSeconds(playerWaitTime/3);
        
        if (mainGame)
            timingText.text = (playerWaitTime - 2*playerWaitTime/3).ToString(CultureInfo.InvariantCulture);
        
        yield return new WaitForSeconds(playerWaitTime/3);
        
        if (mainGame)
        {
            timingText.text = "!!!";
        }
    }
    
    
    
    #endregion

    #region PlayerRhythm
    
    [Header("PlayerRhythm")]
    public float playerWaitTime = 3f;
    
    public Clock clock;
    public Transform playerBeatVisualizer;
    private bool _isPlayerInputChecked;
    private bool _isGameOver;
    public UnityEvent onPlayerInput;
    
    void StartGame()
    {
        _currentBeat = 0;
        _nextBeatTime = Time.time + playerWaitTime;
        _minCheckTime = _nextBeatTime - _beatInterval * playerInputThreshold / 2;
        _maxCheckTime = _nextBeatTime + _beatInterval * playerInputThreshold / 2;
        _isPlaying = true;
        StartCoroutine(StartBasicRhythmWithDelayed(playerWaitTime));
    }
    
    IEnumerator StartBasicRhythmWithDelayed(float time)
    {
        yield return new WaitForSeconds(time);
        
        _basicRhythmCoroutine = StartCoroutine(StartBasicRhythm());
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
            
            if (_basicRhythmCoroutine != null)
                StopCoroutine(_basicRhythmCoroutine);

            if (!_isGameOver)
            {
                levelController.Progress++;
                
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
        if (!clock.isDecrease)
            return;
        
        audioSource.Play();
        onPlayerInput.Invoke();
        VisualizeBeat(0, playerBeatVisualizer);
        
        if (!_isPlayerInputChecked)
            _isPlayerInputChecked = true;
        else
        {
            _isGameOver = true;
            return;
        }
        
        /*if (Time.time >= _minCheckTime && Time.time <= _maxCheckTime)
        {*/
            if (!_rhythm[_currentBeat])
            {
                GameOver();
            }/*
        }
        else
        {
            GameOver();
        }*/
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
    
    public void StartComputerPart()
    {
        levelController = FindObjectOfType<LevelController>();
        _beatInterval = 60f / bpm;
        GenerateRhythm();
        StartCoroutine(PlayComputerRhythm());
    }

    void Update()
    {
        if (_isPlaying)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                CheckPlayerInput();
            }
            
            if (Time.time >= _maxCheckTime)
            {
                CheckBeat();
                _nextBeatTime += _beatInterval;
                _minCheckTime = _nextBeatTime - _beatInterval * playerInputThreshold / 2;
                _maxCheckTime = _nextBeatTime + _beatInterval * playerInputThreshold / 2;
                _isPlayerInputChecked = false;
            }
        }
    }

    #region Show Timing in Main Game

    [Header("Main Game Features")]
    public LevelController levelController;
    public TextMeshProUGUI timingText;
    public AudioSource basicAudioSource;
    private Coroutine _basicRhythmCoroutine;
    
    IEnumerator StartBasicRhythm()
    {
        for (int i = 0; i < _totalBeats; i++)
        {
            basicAudioSource.Play();
            
            yield return new WaitForSeconds(_beatInterval);
        }
    }

    #endregion
}