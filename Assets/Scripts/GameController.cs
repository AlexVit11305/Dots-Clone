using UnityEngine;

/// <summary>
/// This is controller for base game logic
/// </summary>
/// 
public class GameController : MonoBehaviour
{
    [HideInInspector]
    public IDotConnectionController dotConnectionController = new DotConnectionController();
    
    //UI
    [SerializeField] 
    private GameUI gameUi;
    private SaveController _saveController = new SaveController();

    public static GameController Instance => _instance;
    private static GameController _instance;
    
    [HideInInspector]
    public GameSession currSession = null;
    
    public delegate void OnInfoUpdated(GameSession current);
    public event OnInfoUpdated InfoUpdated;
    
    public void Awake()
    {
        if (_instance == null)
            _instance = this;
        else
            Destroy(gameObject);
        
        GameData.savePath = Application.persistentDataPath + "/localSave.json";
        
    }

    void Start()
    {
        StartGame();
    }

    private void OnEnable()
    {
        //sign Ui to game events
        InfoUpdated += gameUi.UpdateInfo;
        //sign to Ui events
        gameUi.onClearSaveButtonClick += ClearSaves;
    }

    private void OnDisable()
    {
        //unsign Ui to game events
        InfoUpdated -= gameUi.UpdateInfo;
        //unsign to Ui events
        gameUi.onClearSaveButtonClick -= ClearSaves;
    }

    private void OnApplicationQuit()
    {
        SaveGame();
    }

    public void StartGame()
    {
        LoadGame();
    }

    private void LoadGame()
    {
        _saveController.Load(out currSession);
        if(currSession == null)
            currSession = new GameSession();
        InfoUpdated?.Invoke(currSession);
    }

    private void SaveGame()
    {
        if(currSession != null)
            _saveController.Save(currSession);
    }

    public void UpdateScore(int scores)
    {
        if(currSession == null)
            return;
        currSession.scores += scores;
        InfoUpdated?.Invoke(currSession);
    }

    private void ClearSaves()
    {
        _saveController.Clear();
        currSession.scores = 0;
        InfoUpdated?.Invoke(currSession);
    }
}
