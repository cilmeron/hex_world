using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HpSlider : MonoBehaviour{
    private Entity entity;

    private Slider slider;
    
    [SerializeField] private Vector3 offset = Vector3.up * 10f;

    public bool isActive = false;
    
    // Start is called before the first frame update
    void Start(){
        entity = transform.parent.GetComponent<Entity>();
        slider = transform.GetChild(0).GetComponent<Slider>();
        slider.maxValue = entity.MAXHp;
        slider.value = entity.CurrentHp;
        transform.position = entity.transform.position + offset;
    }

    // Update is called once per frame
    void Update(){
        slider.gameObject.SetActive(isActive);
        if (isActive){
            // Update the value of the slider to reflect the entity's current HP
            slider.value = entity.CurrentHp;

            // Calculate the HP percentage (as a value between 0 and 1)
            float hpPercentage = (float)entity.CurrentHp / (float)entity.MAXHp;

            // Set the color of the slider based on the HP percentage
            slider.fillRect.GetComponentInChildren<Image>().color = Color.Lerp(Color.red, Color.green, hpPercentage);
            
            transform.rotation = Quaternion.LookRotation(transform.position - Camera.main.transform.position);
        }
    }

    public bool IsActive{
        get => isActive;
        set => isActive = value;
    }
}
