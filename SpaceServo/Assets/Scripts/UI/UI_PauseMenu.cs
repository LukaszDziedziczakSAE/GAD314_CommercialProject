using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UI_PauseMenu : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] Button resumeButton;
    [SerializeField] Button settingsButton;
    [SerializeField] Button surveyButton;
    [SerializeField] Button debugButton;
    [SerializeField] Button exitToMenuButton;

    private void OnEnable()
    {
        resumeButton.onClick.AddListener(OnResumeButtonPress);
        settingsButton.onClick.AddListener(OnSettingsButtonPress);
        surveyButton.onClick.AddListener(OnSurveyButtonPress);
        debugButton.onClick.AddListener(OnDebugButtonPress);
        exitToMenuButton.onClick.AddListener(OnExitToMenuButtonPress);
    }

    private void OnDisable()
    {
        resumeButton.onClick.RemoveListener(OnResumeButtonPress);
        settingsButton.onClick.RemoveListener(OnSettingsButtonPress);
        surveyButton.onClick.RemoveListener(OnSurveyButtonPress);
        debugButton.onClick.RemoveListener(OnDebugButtonPress);
        exitToMenuButton.onClick.RemoveListener(OnExitToMenuButtonPress);
    }


    private void OnResumeButtonPress()
    {
        Game.PauseGame(false);
    }

    private void OnSettingsButtonPress()
    {
        UI.ShowPauseMenu(false);
        UI.ShowSettings();
    }

    private void OnExitToMenuButtonPress()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(0);
    }

    private void OnSurveyButtonPress()
    {
        Application.OpenURL(Game.SurveyURL);
    }

    private void OnDebugButtonPress()
    {
        UI.ShowDebugInfo();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        UI.MouseOverUI = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        UI.MouseOverUI = false;
    }
}
