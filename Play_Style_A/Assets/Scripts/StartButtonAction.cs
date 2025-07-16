using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StartButtonAction : MonoBehaviour
{
    public Button button;

    // Start is called before the first frame update
    void Start()
    {
        button.onClick.AddListener(SceneSwitcher);
    }

    void SceneSwitcher()
    {
        SceneManager.LoadScene("Prospect");
    }
}
