using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class TitleScreenManager : MonoBehaviour
{
    public RectTransform titleText, controlsText;
    public Button controlsButton, titleButton;
    public Toggle isFullscreen;
    public TMP_InputField seedField;

    public RectTransform mainPanel, controlsPanel, settingsPanel, newGamePanel;

    public int curSeed = 0;


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
        if (curSeed == 0) { curSeed = Random.Range(-2100000000, 21000000); } else { Random.InitState(curSeed); }
        PlayerPrefs.SetInt("curSeed", curSeed);
        SceneManager.LoadScene("Dungeon Example");
    }

    public void ToggleFullscreen() {
        Screen.fullScreen = isFullscreen.isOn;
    }

    public void OpenPanel(RectTransform panelToOpen) {
        mainPanel.gameObject.SetActive(false);
        controlsPanel.gameObject.SetActive(false);
        settingsPanel.gameObject.SetActive(false);
        newGamePanel.gameObject.SetActive(false);
        panelToOpen.gameObject.SetActive(true);
        
    }

    public void SetSeed() {
        if (seedField.text != "") {
            curSeed = seedField.text.GetHashCode();
        }
        else {
            curSeed = 0;
        }
    }
}
