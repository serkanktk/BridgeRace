using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement; // sahneler arasi gecis icin
using TMPro;
using System;




public class ButtonManagerScript : MonoBehaviour
{

    private bool pause;
    DataManager dataManager;
    private bool isSelectedMessageShown = false;
    private bool isUnlockedMessageShown = false;


    [Header("Pause Panel and Texts")]
    public GameObject pausePanel;
    public TextMeshProUGUI themeTwoText;
    public TextMeshProUGUI themeThreeText;
    public TextMeshProUGUI themeFourText;
    public TextMeshProUGUI themeFiveText;
    public TextMeshProUGUI themeSixText;
    public TextMeshProUGUI displayTextUnlocked;
    public TextMeshProUGUI displayTextSelected;


    private void SetText(TextMeshProUGUI text, int numberOfWinningsNeeded)
    {
        if (dataManager.totalWinnings < numberOfWinningsNeeded)
        {
            text.text = numberOfWinningsNeeded.ToString() + " Wins";
        }
        else
        {
            text.text = "Select";
        }
    }



    private void Start()
    {
        
       
        dataManager = FindObjectOfType<DataManager>();
        if (SceneManager.GetActiveScene().name == "SampleScene")
        {
            SetText(themeTwoText, 5);
            SetText(themeThreeText, 10);
            SetText(themeFourText, 20);
            SetText(themeFiveText, 40);
            SetText(themeSixText, 80);
        }
       
    }

    public void StartButtonOnClick()
    {
        Debug.Log("Start button is clicked");
        SceneManager.LoadScene(1);
    }
    // Nothing to change here
    // Just update the theme
    public void ThemeOneOnClick()
    {
        if (!isSelectedMessageShown && !isUnlockedMessageShown)
        {
            StartCoroutine(ShowMessageForDuration(displayTextSelected, "Selected", 1f, () => isSelectedMessageShown = false));
            if (!isSelectedMessageShown)
            {
                DataManager.Instance.buttonValue = 1;
            }

            isSelectedMessageShown = true;
        }
    }

    public void ThemeTwoOnClick()
    {
        ThemeOnDecision(2, 5);
    }

    public void ThemeThreeOnClick()
    {
        ThemeOnDecision(3, 10);
    }

    public void ThemeFourOnClick()
    {
        ThemeOnDecision(4, 20);
    }

    public void ThemeFiveOnClick()
    {
        ThemeOnDecision(5, 40);
    }

    public void ThemeSixOnClick()
    {
        ThemeOnDecision(6, 80);
    }

    // delete this later
    public void HomeButtonOnClick()
    {
        Time.timeScale = 1; // Resume the game by setting time scale to 1
        SceneManager.LoadScene(0);
    }

    public void PauseButtonOnClick()
    {
        Time.timeScale = 0; // Pause the game by setting time scale to 0
        pausePanel.SetActive(true);

    }

    public void ResumeButtonOnClick()
    {
        pausePanel.SetActive(false);
        Time.timeScale = 1; // Resume the game by setting time scale to 1
    }


    private void ThemeOnDecision(int buttonValue, int winningNumber)
    {
        if (dataManager.totalWinnings >= winningNumber)
        {
            if (!isSelectedMessageShown && !isUnlockedMessageShown)
            {
                StartCoroutine(ShowMessageForDuration(displayTextSelected, "Selected", 1f, () => isSelectedMessageShown = false));
                if (!isSelectedMessageShown)
                {
                    DataManager.Instance.buttonValue = buttonValue;
                }
                
                isSelectedMessageShown = true;
            }
        }
        else
        {
            if (!isUnlockedMessageShown && !isSelectedMessageShown)
            {
                StartCoroutine(ShowMessageForDuration(displayTextUnlocked, "Unlocked", 1f, () => isUnlockedMessageShown = false));
                if (!isUnlockedMessageShown)
                {
                    DataManager.Instance.buttonValue = 1;
                }
                isUnlockedMessageShown = true;
            }
        }
    }

    private IEnumerator ShowMessageForDuration(TextMeshProUGUI txt, string message, float duration, Action callback)
    {
        txt.text = message;
        yield return new WaitForSeconds(duration);
        txt.text = "";
        callback?.Invoke();
    }


}
