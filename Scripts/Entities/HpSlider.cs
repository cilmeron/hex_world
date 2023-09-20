using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HpSlider : MonoBehaviour{
    private ICombatElement combatElement;

    private Slider slider;
    
    [SerializeField] private Vector3 offset = Vector3.up * 10f;

    public bool isActive = false;
    
    // Start is called before the first frame update
    void Start(){
        combatElement = transform.parent.GetComponent<ICombatElement>();
        slider = transform.GetChild(0).GetComponent<Slider>();
        slider.maxValue = combatElement.GetMaxHP();
        slider.value = combatElement.GetCurrentHP();
        transform.position = combatElement.GetGameObject().transform.position + offset;
    }

    // Update is called once per frame
    void Update(){
        slider.gameObject.SetActive(isActive);
        if (isActive){
            // Update the value of the slider to reflect the selectable's current HP
            slider.value = combatElement.GetCurrentHP();

            // Calculate the HP percentage (as a value between 0 and 1)
            float hpPercentage = combatElement.GetCurrentHP() / combatElement.GetMaxHP();

            // Set the color of the slider based on the HP percentage
            slider.fillRect.GetComponentInChildren<Image>().color = Color.Lerp(Color.red, Color.green, hpPercentage);
            
            
        }
    }

    public bool IsActive{
        get => isActive;
        set => isActive = value;
    }
}
