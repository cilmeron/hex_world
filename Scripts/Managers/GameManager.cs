using System;
using UnityEngine;
using UnityEngine.Events;
using System.IO;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [SerializeField] private bool floatingDamageText;
    [SerializeField] private GameObject damageTextPrefab;
    public Player player;
    public Player p1;
    public bool settingsread;
    public Player p2;

    private string settingsFilePath;

    // Default settings values
    private string serverHost = "pirotess.duckdns.org";
    private string playerName = "playername";
    private string serverPort = "8044";
    private string musicVolume = "100";
    private string soundVolume = "100";

    [SerializeField] private GameDifficulty _gameDifficulty = GameDifficulty.Easy;


    public enum GameDifficulty{
        Easy,
        Hard
    }
    
    void Awake()
    {
        player = p1;
        settingsread = false;
        // Ensure there is only one instance of the EventManager
        if (Instance == null)
        {
            Instance = this;
            //DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start(){
        settingsFilePath = Path.Combine(Application.persistentDataPath, "settings.ini");
        ReadOrCreateSettings();
        EventManager.Instance.deathEvent.AddListener(DeathListener);
    }

    private void DeathListener(C_Health c){
        if (c.Entity.GetPlayer().OwnedEntities.Count <= 0){
            Debug.Log(c.Entity.GetPlayer().nation + " lost the game");
            //Add Player won UI
        }
    }

    public void setSoundVolume(float val)
    {
        soundVolume = ""+(int)(val*100f);
        UpdateAndWriteSettingsToFile();
    }

    public void setPlayername(string name)
    {
        playerName = name;
        UpdateAndWriteSettingsToFile();
    }

    public void setMusicVolume(float val)
    {
        musicVolume = ""+(int)(val*100f);  
        UpdateAndWriteSettingsToFile();
    }

    public float getMusicVolume()
    {
        return float.Parse(musicVolume)/100f;
    }

    public float getSoundVolume()
    {
        return float.Parse(soundVolume)/100f;
    }

    private void ReadOrCreateSettings()
    {
        // If the settings file exists, read its content
        if (File.Exists(settingsFilePath))
        {
            ReadSettingsFromFile();
            settingsread = true;
        }
        // If the settings file doesn't exist, create it with default values
        else
        {
            CreateDefaultSettingsFile();
        }
    }
    private void ReadSettingsFromFile()
    {
        // Read all lines from the settings file
        string[] lines = File.ReadAllLines(settingsFilePath);

        // Parse each line and update the corresponding setting
        foreach (string line in lines)
        {
            string[] keyValue = line.Split('=');

            if (keyValue.Length == 2)
            {
                string key = keyValue[0].Trim();
                string value = keyValue[1].Trim();

                switch (key)
                {
                    case "server_host":
                        serverHost = value;
                        break;
                    case "playername":
                        playerName = value;
                        break;
                    case "server_port":
                        serverPort = value;
                        break;
                    case "music":
                        musicVolume = value;
                        break;
                    case "sound":
                        soundVolume = value;
                        break;
                }
            }
        }

        // Now you can use the settings values in your game
        Debug.Log("Settings loaded: " + "Host: " + serverHost + ", PlayerName: " + playerName +
                  ", Port: " + serverPort + ", Music: " + musicVolume + ", Sound: " + soundVolume);
    }

    private void CreateDefaultSettingsFile()
    {
        // Create a new settings file and write the default values to it
        string[] lines =
        {
            "server_host=" + serverHost,
            "playername=" + playerName,
            "server_port=" + serverPort,
            "music=" + musicVolume,
            "sound=" + soundVolume
        };

        File.WriteAllLines(settingsFilePath, lines);

        Debug.Log("Default settings file created.");
    }
    private void SetFloatingDamageText(C_Combat combatElement, int damage)
    {
        if (floatingDamageText)
        {
            GameObject damageTextObject = Instantiate(damageTextPrefab, combatElement.gameObject.transform.position, Quaternion.identity);
            damageTextObject.transform.SetParent(combatElement.gameObject.transform,false);
            Billboard billboard = damageTextObject.GetComponent<Billboard>();
            billboard.TMP.text = damage.ToString();
        }
    }

    public string getserverhost()
    {
        return serverHost;
    }
    public string getPlayername()
    {
        return playerName;
    }
    public string getServerPort()
    {
        return serverPort;
    }
    
    public bool FloatingDamageText{
        get => floatingDamageText;
        set => floatingDamageText = value;
    }
    public void UpdateAndWriteSettingsToFile()
    {
        // Create an array of strings with the updated values
        string[] lines =
        {
            "server_host=" + serverHost,
            "playername=" + playerName,
            "server_port=" + serverPort,
            "music=" + musicVolume,
            "sound=" + soundVolume
        };

        // Write the updated values to the settings file
        File.WriteAllLines(settingsFilePath, lines);

        Debug.Log("Settings updated and saved to file.");
    }

    public GameDifficulty GetGameDifficulty(){
        return _gameDifficulty;
    }
    
}