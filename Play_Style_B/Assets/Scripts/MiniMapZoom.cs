using UnityEngine;

public class MiniMapZoom : MonoBehaviour
{
    private RectTransform rectTransform;
    private RectTransform child1RectTransform;
    private RectTransform child2RectTransform;
    public bool isPushed = false;
    private int zoomIn = 500;
    private int zoomOut = 150;

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        child1RectTransform = transform.GetChild(0).GetComponent<RectTransform>();
        //Mask
        child2RectTransform = transform.GetChild(0).GetChild(0).GetComponent<RectTransform>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.M))
        {
            if (isPushed)
            {
                rectTransform.anchoredPosition = new Vector2(-405, 195);
                rectTransform.sizeDelta = new Vector2(zoomOut, zoomOut);
                child1RectTransform.sizeDelta = new Vector2(zoomOut, zoomOut);
                child2RectTransform.sizeDelta = new Vector2(zoomOut, zoomOut);
                isPushed = false;
            }
            else
            {
                rectTransform.anchoredPosition = new Vector2(0, 0);
                rectTransform.sizeDelta = new Vector2(zoomIn, zoomIn);
                child1RectTransform.sizeDelta = new Vector2(zoomIn, zoomIn);
                child2RectTransform.sizeDelta = new Vector2(zoomIn, zoomIn);
                isPushed = true;
            }
        }
    }
}
