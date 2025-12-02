using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Pooling;
using FillBoardLogic;
using Utility;
using BoardLogic;
using System.Collections;

public class SC_GameLogic : MonoBehaviour
{
    [SerializeField]
    private CoroutineRunner coroutineRunner;
    [SerializeField]
    private PoolerPreWarmer preWarmer;

    private Dictionary<string, GameObject> unityObjects;
    private int score = 0;
    private float displayScore = 0;
    private GameBoard gameBoard;
    private BoardFiller boardGemsFiller;
    private GlobalEnums.GameState currentState = GlobalEnums.GameState.move;
    public GlobalEnums.GameState CurrentState { get { return currentState; } }

    private GemMover gemMover;
    private GemSwapController gemSwaper;
    private InputController inputController;
    private MatchDetector matchDetector;
    private IMatchResolver matchResolver;

    private bool isResolving;
    private TMP_Text scoreTxt;

    #region MonoBehaviour

    private void Start()
    {
        preWarmer.DoPrewarm();
        Init();
        StartGame();
    }

    private void Update()
    {
        displayScore = Mathf.Lerp(displayScore, gameBoard.Score, SC_GameVariables.Instance.scoreSpeed * Time.deltaTime);
        scoreTxt.text = displayScore.ToString("0");
        inputController.HandleInput();
    }

    private void OnDestroy()
    {
        gemSwaper.OnBeginSwap -= OnBeginSwap;
        gemSwaper.OnSwapWithMatch -= OnSwapWithMatch;
        gemSwaper.OnSwapWithoutMatch -= OnSwapWithoutMatch;
    }

    #endregion

    #region Logic
    private void Init()
    {
        unityObjects = new Dictionary<string, GameObject>();
        GameObject[] _obj = GameObject.FindGameObjectsWithTag("UnityObject");
        foreach (GameObject g in _obj)
            unityObjects.Add(g.name,g);

        scoreTxt = unityObjects["Txt_Score"].GetComponent<TMP_Text>();

        gemMover = new GemMover(coroutineRunner);
        gameBoard = new GameBoard(7, 7);

        matchDetector = new MatchDetector(gameBoard);
        gemSwaper = new GemSwapController(gameBoard, gemMover, matchDetector, coroutineRunner);
        boardGemsFiller = new BoardFiller(gameBoard, coroutineRunner, SC_GameVariables.Instance, this, gemMover);
        inputController = new InputController(Camera.main, gameBoard, gemSwaper, this, SC_GameVariables.Instance);
        matchDetector = new MatchDetector(gameBoard);
        matchResolver = new MatchResolver(gameBoard, matchDetector, boardGemsFiller, SC_GameVariables.Instance, ScoreCheck);

        gemSwaper.OnBeginSwap += OnBeginSwap;
        gemSwaper.OnSwapWithMatch += OnSwapWithMatch;
        gemSwaper.OnSwapWithoutMatch += OnSwapWithoutMatch;

        Setup();
    }

    private void Setup()
    {
        boardGemsFiller.Init(unityObjects["GemsHolder"].transform);
        boardGemsFiller.FillBoard();
        coroutineRunner.RunCoroutine(ResolveAll());
    }

    public void StartGame()
    {
        scoreTxt.text = score.ToString("0");
    }

    private void OnBeginSwap()
    {
        SetState(GlobalEnums.GameState.wait);
    }

    private void OnSwapWithMatch(Vector2Int? initiator, MatchScanResult result)
    {
        coroutineRunner.RunCoroutine(ResolveAll(initiator));
        SetState(GlobalEnums.GameState.wait);
    }

    private void OnSwapWithoutMatch(Vector2Int from, Vector2Int to)
    {
        SetState(GlobalEnums.GameState.move);
    }

    public void SetGem(int _X,int _Y, SC_Gem _Gem)
    {
        gameBoard.SetGem(_X,_Y, _Gem);
    }
    public SC_Gem GetGem(int _X, int _Y)
    {
        return gameBoard.GetGem(_X, _Y);
    }
    public void SetState(GlobalEnums.GameState _CurrentState)
    {
        currentState = _CurrentState;
    }

    private IEnumerator ResolveAll(Vector2Int? initiator = null)
    {
        if (isResolving)
            yield break;

        isResolving = true;

        yield return matchResolver.ResolveAll(initiator);

        isResolving = false;
        SetState(GlobalEnums.GameState.move);
    }

    public void ScoreCheck(SC_Gem gemToCheck)
    {
        gameBoard.Score += gemToCheck.scoreValue;
    }

    #endregion
}
