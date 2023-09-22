using UnityEngine;
using UnityEngine.EventSystems;

public class EntityDragHandler : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler, IPointerEnterHandler,IPointerExitHandler
{
    [SerializeField] private Vector3 startPosition;
    [SerializeField] private Vector3 offsetToMouse;
    [SerializeField] private Transform draggingTransform;
    [SerializeField] private Entity selectable;

    public void Start(){
        selectable = gameObject.GetComponent<Entity>();
    }
    
    public void OnBeginDrag(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Middle){
            if (selectable == null){
                return;
            }
            draggingTransform = transform;
            startPosition = draggingTransform.position;
            offsetToMouse = startPosition - Camera.main.ScreenToWorldPoint(eventData.position);
        }
    }
    
    public void OnDrag(PointerEventData eventData)
    {
        if (draggingTransform)
        {
            Vector3 newPosition = Camera.main.ScreenToWorldPoint(eventData.position) + offsetToMouse;
            draggingTransform.position = new Vector3(newPosition.x, newPosition.y, draggingTransform.position.z);
        }
    }
    
    public void OnEndDrag(PointerEventData eventData)
    {
        draggingTransform = null;
        startPosition = Vector3.zero;
        offsetToMouse = Vector3.zero;
    }
    
    public void OnPointerEnter(PointerEventData eventData)
    {
//        Debug.Log("On Hover Enter");
    }

    public void OnPointerExit(PointerEventData eventData)
    {
//        Debug.Log("On Hover Exit");
    }
    
}