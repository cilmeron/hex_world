using System;
using UnityEngine;
using UnityEngine.Events;


public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [SerializeField] private bool floatingDamageText;
    [SerializeField] private GameObject damageTextPrefab;
    public Player player;

    void Awake()
    {
        // Ensure there is only one instance of the EventManager
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start(){
        EventManager.Instance.damageEvent.AddListener(SetFloatingDamageText);
    }
    
    private void SetFloatingDamageText(Entity entity, int damage)
    {
        if (floatingDamageText)
        {
            GameObject damageTextObject = Instantiate(damageTextPrefab, entity.transform.position, Quaternion.identity);
            damageTextObject.transform.parent = entity.transform;
            Billboard billboard = damageTextObject.GetComponent<Billboard>();
            billboard.TMP.text = damage.ToString();
        }
    }

    
    public bool FloatingDamageText{
        get => floatingDamageText;
        set => floatingDamageText = value;
    }
}