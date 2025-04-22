using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_Tutorial : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] TMP_Text tutorialText;
    [SerializeField] GameObject buttons;
    [SerializeField] Button nextButton;
    [SerializeField] Button exitButton;

    private void OnEnable()
    {
        if (!Game.Tutorial.IsRunning)
        {
            UI.ShowTutorial(false);
            return;
        }

        nextButton.onClick.AddListener(OnNextPress);
        exitButton.onClick.AddListener(OnExitPress);

        UpdateTutorialUI();
    }

    private void OnDisable()
    {
        nextButton.onClick.RemoveListener(OnNextPress);
        exitButton.onClick.RemoveListener(OnExitPress);
    }


    public void OnPointerEnter(PointerEventData eventData)
    {
        UI.MouseOverUI = true;
        
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        UI.MouseOverUI = false;
    }

    private void OnNextPress()
    {
        UI.Sound.PlayButtonPressSound();
        Game.Tutorial.PartComplete();
    }

    private void OnExitPress()
    {
        Debug.Log("OnExitPress");
        UI.Sound.PlayButtonCancelSound();
        Game.Tutorial.ExitTutorial();
    }

    public void UpdateTutorialUI()
    {
        if (!Game.Tutorial.IsRunning)
        {
            UI.ShowTutorial(false);
            return;
        }
        tutorialText.text = Game.Tutorial.CurrentPart.TutorialText;
        buttons.SetActive(Game.Tutorial.CurrentPart.Type == Tutorial.TutorialPart.EType.ClickThrough);
        if (buttons.activeSelf) nextButton.gameObject.SetActive(Game.Tutorial.HasNextPart);
    }
}
