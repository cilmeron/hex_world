using System;
using UnityEngine;
using UnityEngine.Events;


public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [SerializeField] private bool floatingDamageText;
    [SerializeField] private GameObject damageTextPrefab;
    public Player player;
    [SerializeField] private GameDifficulty _gameDifficulty = GameDifficulty.Easy;


    public enum GameDifficulty{
        Easy,
        Hard
    }
    
    void Awake()
    {
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
        EventManager.Instance.deathEvent.AddListener(DeathListener);
    }

    private void DeathListener(C_Health c){
        if (c.Entity.GetPlayer().OwnedEntities.Count <= 0){
            Debug.Log(c.Entity.GetPlayer().nation + " lost the game");
            //Add Player won UI
        }
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

    
    public bool FloatingDamageText{
        get => floatingDamageText;
        set => floatingDamageText = value;
    }

    public GameDifficulty GetGameDifficulty(){
        return _gameDifficulty;
    }
    
}