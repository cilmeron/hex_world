using System;
using UnityEngine;
using UnityEngine.Events;
using System.IO;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [SerializeField] private bool floatingDamageText;
    [SerializeField] private GameObject damageTextPrefab;
    public Player player;
    public Player playerViking;
    public bool settingsread;
    public AudioSource MusicManager;
    public AudioSource SoundManager;
    public GameObject p1prefab;
    public NetworkManager networkManager;
    public GameObject p2prefab;
    public Dictionary<int, GameObject> opponents;
    public Dictionary<int, GameObject> myunits;
    private int units = 0;
    public Player playerSamurai;
    public GameObject p1pos;
    public GameObject p2pos;
    public Camera maincam;
    public Slider soundv;
    public Slider musicv;

    private string settingsFilePath;

    // Default settings values
    private string serverHost = "pirotess.duckdns.org";
    private string playerName = "playername";
    private string serverPort = "8044";
    private string musicVolume = "100";
    private string soundVolume = "100";
    private int numstartunits = 10;

    [SerializeField] private GameDifficulty _gameDifficulty = GameDifficulty.Easy;

    public void PlaceOpponent(int UID, Vector3 pos)
    {
        GameObject placeable;
        if (player == playerViking)
        {
            placeable = p2prefab;
        }
        else 
        {
            placeable = p1prefab;
        }
        GameObject placedunit = Instantiate(placeable, pos, Quaternion.identity);
        if (player == playerViking)
            placedunit.GetComponent<Unit>().SetPlayer(playerSamurai);
        else
            placedunit.GetComponent<Unit>().SetPlayer(playerViking);
        placedunit.GetComponent<Unit>().ID = UID;
        opponents.Add(UID, placedunit);
    }
    
    public void CheckOrCreateOwnUnit(int UID, Vector3 pos)
    {
        Debug.Log("Size of myunits:"+myunits.Count);
        if (!myunits.ContainsKey(UID))
        {
            GameObject placeable;
            if (player == playerViking)
            {
                placeable = p1prefab;
            }
            else 
            {
                placeable = p2prefab;
            }
            GameObject placedunit = Instantiate(placeable, pos, Quaternion.identity);
            placedunit.GetComponent<Unit>().SetPlayer(player);
            placedunit.GetComponent<Unit>().ID = UID;
            myunits.Add(UID, placedunit);
        }
    }
    public void MoveOpponent(int UID, Vector3 pos)    
    {
        GameObject outg;
        opponents.TryGetValue(UID, out outg);
        Debug.Log(outg);
        if (outg != null)
        {
            outg.GetComponent<git.Scripts.Components.C_Moveable>().SetMoveToPosition(pos,true);
        }
    }

    public Player GetPlayerWithNation(Player.Nations nation){
        switch(nation){
            case Player.Nations.VIKINGS:
                return playerViking; 
            case Player.Nations.SAMURAI:
                return playerSamurai;
                
        }
        return null;
    }
    
    public void AddEntityToPlayer(Entity e){
        Player p = GetPlayerWithNation(e.GetNation());
        p.AddOwnership(e);
    }
    
    
    public void setplayer1(bool reconnect)
    {
        player = playerViking;
        Vector3 place = p1pos.transform.position;
        maincam.transform.position = new Vector3(place.x, maincam.GetComponent<CameraManager>().altitude, place.z);
        if (!reconnect)
            SpawnStartUnits(p1pos.transform.position, playerViking, p1prefab);
    }

    public void setplayer2(bool reconnect)
    {
        player = playerSamurai;
        Vector3 place = p2pos.transform.position;
        maincam.transform.position = new Vector3(place.x, maincam.GetComponent<CameraManager>().altitude, place.z);
        if (!reconnect)
            SpawnStartUnits(p2pos.transform.position, playerSamurai, p2prefab);
    }

    void SpawnStartUnits(Vector3 pos, Player p, GameObject prefab)
    {
        for (int i = 0; i < numstartunits; i++)
        {
            float angle = UnityEngine.Random.Range(0f, 360f);
            float radians = angle * Mathf.Deg2Rad;
            float x = pos.x + 4f * Mathf.Cos(radians);
            float z = pos.z + 4f * Mathf.Sin(radians);
            Vector3 placepos = new Vector3(x, prefab.transform.position.y, z);
            GameObject placedunit = Instantiate(prefab, placepos, Quaternion.identity);
            placedunit.GetComponent<Unit>().SetPlayer(p);
            int ID = units++;
            placedunit.GetComponent<Unit>().ID = ID;
            myunits.Add(ID, placedunit);
            if (networkManager != null)
            {
               networkManager.SendMsg("C:"+playerName+":"+placepos.x+","+placepos.y+","+placepos.z+":"+ID);
            }
        }
    }
    public enum GameDifficulty{
        Easy,
        Hard
    }
    
    void Awake()
    {
        player = playerViking;
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
        opponents = new Dictionary<int, GameObject>();
        myunits = new Dictionary<int, GameObject>();
    }

    void Start(){
        settingsFilePath = Path.Combine(Path.GetDirectoryName(Application.dataPath), "settings.ini");
        ReadOrCreateSettings();
        if (EventManager.Instance != null)
            EventManager.Instance.deathEvent.AddListener(DeathListener);
    }

    private void DeathListener(C_Health c){
        Player p = null;
        if(c.Entity.GetNation() == playerViking.nation)
        {
            p = playerViking;
        }else if (c.Entity.GetNation() == playerSamurai.nation){
            p = playerSamurai;
        }
        if (p.OwnedEntities.Count <= 0){
            Debug.Log(c.Entity.GetNation() + " lost the game");
            //Add Player won UI
        }
    }

    public void setSoundVolume(float val)
    {
        soundVolume = ""+(int)(val*100f);
        UpdateAndWriteSettingsToFile();        
        if (SoundManager != null)
        {
            if (soundVolume == "0")
                SoundManager.mute = true;
            else
            {
                SoundManager.mute = false;
                SoundManager.volume = float.Parse(soundVolume)/100f;
            }
        }
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
        if (MusicManager != null)
        {
            if (musicVolume == "0")
                MusicManager.mute = true;
            else
            {
                MusicManager.mute = false;
                MusicManager.volume = float.Parse(musicVolume)/100f;
            }
        }
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
            if (MusicManager != null)
            {
                if (musicVolume == "0")
                    MusicManager.mute = true;
                else
                    MusicManager.volume = float.Parse(musicVolume)/100f;
            }
            if (SoundManager != null)
            {
                if (soundVolume == "0")
                    SoundManager.mute = true;
                else
                    SoundManager.volume = float.Parse(soundVolume)/100f;
            }
        }
        // If the settings file doesn't exist, create it with default values
        else
        {
            CreateDefaultSettingsFile();
        }
        soundv.value = getSoundVolume();
        musicv.value = getMusicVolume();
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
    public void QuitGame()
    {
        Debug.Log("QUIT!");
        networkManager.SendMsg("Q:"+networkManager.playername);
        SceneManager.LoadScene("StartMenu");
    }
    
}