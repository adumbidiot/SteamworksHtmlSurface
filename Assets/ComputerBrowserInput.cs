using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ComputerBrowserInput : MonoBehaviour, IEventSystemHandler, IPointerClickHandler
{
    GraphicRaycaster raycaster;
    
    EventSystem m_EventSystem;
    
    // Start is called before the first frame update
    void Start()
    {
         raycaster = GetComponent<GraphicRaycaster>();
         
         m_EventSystem = GetComponent<EventSystem>();
    }

    // Update is called once per frame
    void Update()
    {        
         if (Input.GetKeyDown(KeyCode.Mouse0))
         {
             PointerEventData pointerData = new PointerEventData(m_EventSystem);
             List<RaycastResult> results = new List<RaycastResult>();
             pointerData.position = Input.mousePosition;
             raycaster.Raycast(pointerData, results);

             foreach (RaycastResult result in results)
             {
                 Debug.Log("Hit " + result.gameObject.name);
                 RectTransform rect = result.gameObject.GetComponent<RectTransform>();
                 
                 if(rect != null){
                    if(RectTransformUtility.ScreenPointToWorldPointInRectangle((RectTransform)this.transform, result.screenPosition, raycaster.eventCamera, out var worldPosition)) {
                        Debug.Log("worldPosition " + worldPosition);
                        
                        Vector3[] worldCorners = new Vector3[4];
                        rect.GetWorldCorners(worldCorners);
                        Debug.Log("Top Right: " + worldCorners[1]);
                        
                        Vector3 scaledWorldPosition = Vector3.zero;
                        scaledWorldPosition.x = worldPosition.x / rect.rect.width + rect.pivot.x;
                        scaledWorldPosition.y = worldPosition.y / rect.rect.height + rect.pivot.y;
                        Debug.Log("scaledWorldPosition: " + scaledWorldPosition);
                        
                        break;
                    }
                 }
             }
         }
    }
    
    public void OnPointerEnter(PointerEventData eventData) {
        Debug.Log("OnPointerEnter");
    }
    
    public void OnPointerClick(PointerEventData pointerEventData) {
        Debug.Log("OnPointerClick");
    }
}
