using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HpSlider : MonoBehaviour{
    private C_Health cHealth;

    private Slider slider;
    
    [SerializeField] private Vector3 offset = Vector3.up * 10f;

    public bool isActive = false;
    
    // Start is called before the first frame update
    void Awake(){
        cHealth = transform.parent.GetComponent<C_Health>();
        slider = transform.GetChild(0).GetComponent<Slider>();
        slider.maxValue = cHealth.GetMaxHp();
        slider.value = cHealth.GetCurrentHp();
        transform.position = cHealth.gameObject.transform.position + offset;
    }
  

    public void UpdateHpSlider(){
        
        // Update the value of the slider to reflect the selectable's current HP
        slider.value = cHealth.GetCurrentHp();

        // Calculate the HP percentage (as a value between 0 and 1)
        float hpPercentage = cHealth.GetCurrentHp() / cHealth.GetMaxHp();

        // Set the color of the slider based on the HP percentage
        slider.fillRect.GetComponentInChildren<Image>().color = Color.Lerp(Color.red, Color.green, hpPercentage);
        
        slider.gameObject.SetActive(isActive);
    }
    
    public void Activate(bool active){
        isActive = active;
        slider.gameObject.SetActive(isActive);
    }
    
}
