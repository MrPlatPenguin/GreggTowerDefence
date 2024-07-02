using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.SceneManagement;
using Unity.VisualScripting;
using System.Security.Cryptography;

public class GameManager : MonoBehaviour
{
    static GameManager _instance;
    public static bool IsNightTime { get; private set; }
    public static int DayNumber { get; private set; } = 1;
    public static float TimeSinceDayStart { get; private set; }
    public static bool IsGamePaused;

    [SerializeField] float _dayLength = 40;
    public static float DayLength { get => _instance._dayLength; }
    [SerializeField] float _nightLength = 20;
    public static float NightLength { get => _instance._nightLength; }

    [Range(0f, 1f)]
    [SerializeField] float _latestPointInNightSpawn = 0.75f;
    [SerializeField] int roundBettweenSlimeIncrease;
    [SerializeField] float _difficulty;
    [SerializeField] int _enemiesSpawnOnFirstDay;
    [SerializeField] Enemy enemyPrefab;
    [SerializeField] EnemySO[] enemyTypes;

    int _enemySpawnIndex;
    int _numEnemiesToSpawn;
    int _groupSize = 0;
    Vector2 _spawnLocation;
    bool _isGameOver;

    List<EnemySO> _nightWaveQueue = new List<EnemySO>();

    #region Events 
    event Action _onDayChange;
    public static event Action OnDayChange { add => _instance._onDayChange += value; remove => _instance._onDayChange -= value; }

    private Action _onDayFinish;
    public static event Action OnDayFinish { add => _instance._onDayFinish += value; remove => _instance._onDayFinish -= value; }

    private Action _onNightStart;
    public static event Action OnNightStart { add => _instance._onNightStart += value; remove => _instance._onNightStart -= value; }

    private Action _onGamePause;
    public static event Action OnGamePause { add => _instance._onGamePause += value; remove => _instance._onGamePause -= value; }

    private Action _onGameResume;
    public static event Action OnGameResume { add => _instance._onGameResume += value; remove => _instance._onGameResume -= value; }

    private Action _onFirstNight;
    public static event Action OnFirstNight { add => _instance._onFirstNight += value; remove => _instance._onFirstNight -= value; }

    public delegate void onGameOver(string deathMessage);
    private onGameOver _onGameOver;
    public static onGameOver OnGameOver { get => _instance._onGameOver; set => _instance._onGameOver = value; }
    #endregion

    private void Awake()
    {
        _instance = this;
    }

    public void InitializeGameManager()
    {
        AudioManager.ClearAudioPool();
        Enemy.CreateQuadTree();
        DayNumber = 1;
        IsNightTime = false;
        TimeSinceDayStart = 0;
        UnpauseGame();
    }

    private void Update()
    {
        if (Input.GetButtonDown("Pause"))
        {
            TogglePause();
        }
    }

    private void FixedUpdate()
    {
        TimeSinceDayStart += Time.fixedDeltaTime;

        if (IsNightTime)
            SpawnEnemies();

        if (!IsNightTime && TimeSinceDayStart > _dayLength)
            StartNight();

        else if (TimeSinceDayStart >= _dayLength + _nightLength)
            StartDay();
    }

    void StartDay()
    {
        IsNightTime = false;
        TimeSinceDayStart = 0;
        DayNumber++;
        _onDayChange?.Invoke();
        _onDayFinish?.Invoke();
        _onDayFinish = null;
        GameSaveAndLoad.SaveGame();
    }

    void StartNight()
    {
        GenerateEnemiesSpawnList();
        IsNightTime = true;
        _enemySpawnIndex = 0;
        _onNightStart?.Invoke();
        if (DayNumber == 1)
            _onFirstNight?.Invoke();
    }

    public static bool IsGameOver()
    {
        return _instance._isGameOver;
    }

    void SpawnEnemies()
    {
        // Check if we have already spawned all the enemies for this night phase
        if (_enemySpawnIndex >= _numEnemiesToSpawn)
            return;

        // Calculate the number of enemies that should have spawned by this point in the night
        float timeSinceNightStart = TimeSinceDayStart - _dayLength;
        float endSpawnTime = NightLength * _latestPointInNightSpawn;
        int targetSpawn = Mathf.CeilToInt(Mathf.Clamp01(timeSinceNightStart / endSpawnTime) * _numEnemiesToSpawn);

        // Return if the group spawn will tip over the targeted spawn by this point
        if (targetSpawn < _enemySpawnIndex + _groupSize)
            return;

        Vector2 spawnDirection = UnityEngine.Random.insideUnitCircle.normalized;
        float spawnDistance = MapGenerator.CellSize * (MapGenerator.HomeSize + 5);
        _spawnLocation = MapGenerator.MapCentre + (spawnDirection * spawnDistance);

        for (int i = 0; i < _groupSize; i++)
        {
            // Instantiate an enemy at the calculated location
            Vector2 individualSpawnOffset = UnityEngine.Random.insideUnitCircle * MapGenerator.CellSize;
            enemyPrefab.SpawnEnemy(_nightWaveQueue[0], _spawnLocation + individualSpawnOffset);

            // Removes the enemy from the queue
            _nightWaveQueue.RemoveAt(0);
            _enemySpawnIndex++;
        }

        // Randomises the group size for the next spawn group
        _groupSize = UnityEngine.Random.Range(1, DayNumber);
        _groupSize = Mathf.Clamp(_groupSize, 1, _nightWaveQueue.Count);
    }

    public static void GameOver(string deatMessage)
    {
        Time.timeScale = 0f;
        Debug.Log("Game Over");
        PlayerEvents.ResetEvents();
        StructureEvents.ResetEvents();
        _instance._isGameOver = true;

        _instance._onGameOver?.Invoke(deatMessage);
        GameSaveAndLoad.DeleteSave();
    }

    int NumEnemiesToSpawn()
    {
        float exponentBonus = MathF.Max(DayNumber - 20,0) * 0.045f;

        return Mathf.RoundToInt(_difficulty * (MathF.Pow(DayNumber, 2 + exponentBonus)) + _enemiesSpawnOnFirstDay);
    }

    void GenerateEnemiesSpawnList()
    {
        _nightWaveQueue.Clear();
        int targetDifficulty = NumEnemiesToSpawn();
        int spawnedDifficulty = 0;
        int remainingDifficulty;

        int enemyTier = Mathf.Clamp(Mathf.FloorToInt(DayNumber / roundBettweenSlimeIncrease), 0, enemyTypes.Length - 1);

        for (int i = enemyTier; i >= 0; i--)
        {
            remainingDifficulty = targetDifficulty - spawnedDifficulty;
            int halfDifficulty = Mathf.FloorToInt(remainingDifficulty * 0.5f);

            EnemySO enemy = enemyTypes[i];

            int overflow = halfDifficulty % enemy.Strength;

            int numberOfEnemies = ((halfDifficulty - overflow) / enemy.Strength);

            for (int ii = 0; ii < numberOfEnemies; ii++)
            {
                _nightWaveQueue.Add(enemyTypes[i]);
                spawnedDifficulty += enemy.Strength;
            }
        }

        remainingDifficulty = targetDifficulty - spawnedDifficulty;

        for (int i = 0; i < remainingDifficulty; i++)
        {
            _nightWaveQueue.Add(enemyTypes[0]);
        }

        _numEnemiesToSpawn = _nightWaveQueue.Count;
        _nightWaveQueue.Shuffle();
    }

    public static void SkipDay()
    {
        _instance.StartDay();
    }

    public static void SetDay(int day)
    {
        DayNumber = day;
        TimeSinceDayStart = 0;
        _instance._onDayChange?.Invoke();
    }

    public static void SkipToNight()
    {
        TimeSinceDayStart = DayLength;
        _instance.StartNight();
    }

    void TogglePause()
    {
        if (IsGamePaused)
            UnpauseGame();

        else if (!IsGamePaused)
            PauseGame();
    }

    public static void PauseGame()
    {
        if (IsGamePaused || IsGameOver())
            return;
        
        IsGamePaused = true;
        Time.timeScale = 0;
        _instance._onGamePause?.Invoke();
    }

    public static void UnpauseGame()
    {
        if (!IsGamePaused || Builder.GetBuildMode() != BuildMode.None || TipsPopUp.TooltipActive() || IsGameOver())
            return;
        IsGamePaused = false;
        Time.timeScale = 1;
        _instance._onGameResume?.Invoke();
    }

    public static int GetDayScore()
    {
        float dayPerc = Mathf.Clamp01(TimeSinceDayStart / (DayLength + NightLength));
        int score = Mathf.RoundToInt(((float)DayNumber + dayPerc - 1) * 450);
        return score;
    }

    private void OnDrawGizmosSelected()
    {
        Enemy.GetEnemyQuadTree().DrawQuad(Color.white);
    }
}