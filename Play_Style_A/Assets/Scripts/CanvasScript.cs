using UnityEngine;
using System.Collections;
using UnityEngine.UIElements;

public class ObjectPlacer : MonoBehaviour 
{
    public Vector2 screenClickPos; //Vector3→Vector2
    public Vector2 canvasClickPos;
    private GameObject preNavDestination;
    [SerializeField] private GameObject placedObject;
    [SerializeField] private Vector3 destinationPos;
    private RectTransform canvasRectTransform;

    
    public GameObject targetObject;
    
    private MiniMapZoom miniMapZoom;
    //terrain
    Terrain terrain;

    
    public int width;
    public int height;

    private void Start()
    {
        
        width = Screen.width;
        height = Screen.height;

        
        miniMapZoom = targetObject.GetComponent<MiniMapZoom>();
        canvasRectTransform = this.GetComponent<RectTransform>();
        terrain = Terrain.activeTerrain;
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0) && miniMapZoom.isPushed == true)
        {
            
            preNavDestination = GameObject.Find("NavDestination(Clone)");
            GameObject.Destroy(preNavDestination);

            
            screenClickPos = Input.mousePosition;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRectTransform, screenClickPos, null, out canvasClickPos);

            
            destinationPos.x = 1024.0f / 2.0f + canvasClickPos.x * (1024.0f / 500.0f);
            destinationPos.y = 300.0f;
            destinationPos.z = 1024.0f / 2.0f + canvasClickPos.y * (1024.0f / 500.0f);

            float h = terrain.terrainData.GetInterpolatedHeight(destinationPos.x / terrain.terrainData.size.x, destinationPos.z / terrain.terrainData.size.z);
            destinationPos.y = h + 5;

            
            if (destinationPos.x >= 0 && destinationPos.x <= 1024 && destinationPos.z >= 0 && destinationPos.z <= 1024)
            {
                Instantiate(placedObject, destinationPos, Quaternion.identity);
            }
        }
    }
}
