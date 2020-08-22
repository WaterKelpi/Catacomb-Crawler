using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TitleScreenManager : MonoBehaviour
{
    public RectTransform titleText, controlsText;
    public Button controlsButton, titleButton;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void CloseGame() {
        Application.Quit();
    }

    public void GoToTop() {
        titleText.gameObject.SetActive(true);
        controlsText.gameObject.SetActive(false);
        controlsButton.gameObject.SetActive(true);
        titleButton.gameObject.SetActive(false);
    }

    public void GoToControls() {
        titleText.gameObject.SetActive(false);
        controlsText.gameObject.SetActive(true);
        controlsButton.gameObject.SetActive(false);
        titleButton.gameObject.SetActive(true);
    }


    public void StartGame() {
        SceneManager.LoadScene("Dungeon Example");
    }
}
