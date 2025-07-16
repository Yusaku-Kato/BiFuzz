using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class BackButtonAction : MonoBehaviour
{
    public Button button;

    // Start is called before the first frame update
    void Start()
    {
        button.onClick.AddListener(SceneSwitcher);
    }

    void SceneSwitcher()
    {
        
        int previousSceneIndex = SceneManager.GetActiveScene().buildIndex - 1;
        SceneManager.LoadScene(previousSceneIndex);
    }
}
