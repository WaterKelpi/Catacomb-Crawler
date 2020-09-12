using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TitleScreenManager : MonoBehaviour
{
    public RectTransform titleText, controlsText;
    public Button controlsButton, titleButton;
    public Toggle isFullscreen;

    public RectTransform mainPanel, controlsPanel, settingsPanel;


    // Start is called before the first frame update
    void Start()
    {
        OpenPanel(mainPanel);
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

    public void ToggleFullscreen() {
        Screen.fullScreen = isFullscreen.isOn;
    }

    public void OpenPanel(RectTransform panelToOpen) {
        mainPanel.gameObject.SetActive(false);
        controlsPanel.gameObject.SetActive(false);
        settingsPanel.gameObject.SetActive(false);
        panelToOpen.gameObject.SetActive(true);
    }
}
