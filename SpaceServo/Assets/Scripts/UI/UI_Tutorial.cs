using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class UI_Tutorial : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] TMP_Text tutorialText;
    [SerializeField] GameObject buttons;

    private void OnEnable()
    {
        if (!Game.Tutorial.IsRunning)
        {
            UI.ShowTutorial(false);
            return;
        }
        UpdateTutorialUI();
    }


    public void OnPointerEnter(PointerEventData eventData)
    {
        UI.MouseOverUI = true;
        
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        UI.MouseOverUI = false;
    }

    public void OnNextPress()
    {
        UI.Sound.PlayButtonPressSound();
        Game.Tutorial.PartComplete();
    }

    public void OnExitPress()
    {
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
    }
}
