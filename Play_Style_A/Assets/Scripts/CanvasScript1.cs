using UnityEngine;
using System.Collections;

public class ObjectPlacerCopied : MonoBehaviour 
{
    public Vector2 screenClickPos; //Vector3→Vector2
    public Vector2 canvasClickPos;
    private GameObject preNavDestination;
    [SerializeField] private GameObject placedObject;
    [SerializeField] private Vector3 destinationPos;
    private RectTransform canvasRectTransform;

    
    public GameObject targetObject;
    
    private MiniMapZoom miniMapZoom;
    //canvas
    //public GameObject canvas;

    
    public int width;
    public int height;

    private void Start()
    {
        
        width = Screen.width;
        height = Screen.height;

        
        miniMapZoom = targetObject.GetComponent<MiniMapZoom>();
        canvasRectTransform = this.GetComponent<RectTransform>();
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0) && miniMapZoom.isPushed == true)
        {
            
            preNavDestination = GameObject.Find("NavDestination(Clone)");
            GameObject.Destroy(preNavDestination);

            //float horizontalAdjust = 190.0f * (width / 738.0f);
            //float verticalAdjust = 30.0f * (width / 738.0f);
            //float sizeAdjust = 356.0f * (width / 738.0f);

            
            screenClickPos = Input.mousePosition;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRectTransform, screenClickPos, null, out canvasClickPos);
            
            destinationPos.x = 1024.0f / 2.0f + canvasClickPos.x * (1024.0f / 500.0f);
            destinationPos.y = 300.0f;
            destinationPos.z = 1024.0f / 2.0f + canvasClickPos.y * (1024.0f / 500.0f);
            //destinationPos.x = (screenClickPos.x - horizontalAdjust) * (1024.0f / sizeAdjust);
            //destinationPos.y = 300.0f;
            //destinationPos.z = (screenClickPos.y - verticalAdjust) * (1024.0f / sizeAdjust);

            
            if (destinationPos.x >= 0 && destinationPos.x <= 1024 && destinationPos.z >= 0 && destinationPos.z <= 1024)
            {
                //Instantiate(placedObject, destinationPos, Quaternion.identity, transform);
                Instantiate(placedObject, destinationPos, Quaternion.identity);
            }
        }
    }
}
