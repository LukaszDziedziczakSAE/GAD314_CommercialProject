using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_Settings : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] Button backButton;
    [SerializeField] Slider masterAudioSlider;
    [SerializeField] Slider musicSlider;
    [SerializeField] Slider sfxSlider;
    [SerializeField] Slider voiceSlider;
    [SerializeField] Slider uiSlider;
    [SerializeField] Slider scrollSpeedSlider;
    [SerializeField] AudioMixer audioMixer;

    private void OnEnable()
    {
        backButton.onClick.AddListener(OnBackButtonPress);
        masterAudioSlider.onValueChanged.AddListener(OnMasterAudioSliderChange);
        musicSlider.onValueChanged.AddListener(OnMusicSliderChange);
        sfxSlider.onValueChanged.AddListener(OnSfxSliderChange);
        voiceSlider.onValueChanged.AddListener(OnVoiceSliderChange);
        uiSlider.onValueChanged.AddListener(OnUISliderChange);
        scrollSpeedSlider.onValueChanged.AddListener(OnScrollSpeedSliderChange);

        if (audioMixer.GetFloat("Master", out float masterValue))
            masterAudioSlider.SetValueWithoutNotify(masterValue);

        if (audioMixer.GetFloat("Music", out float musicValue))
            musicSlider.SetValueWithoutNotify(musicValue);

        if (audioMixer.GetFloat("SFX", out float sfxValue))
            sfxSlider.SetValueWithoutNotify(sfxValue);

        if (audioMixer.GetFloat("Voice", out float voiceValue))
            voiceSlider.SetValueWithoutNotify(voiceValue);

        if (audioMixer.GetFloat("UI", out float uiValue))
            uiSlider.SetValueWithoutNotify(uiValue);

        scrollSpeedSlider.SetValueWithoutNotify(Game.CameraController.ScrollSpeed);
    }

    private void OnDisable()
    {
        backButton.onClick.RemoveListener(OnBackButtonPress);
        masterAudioSlider.onValueChanged.RemoveListener(OnMasterAudioSliderChange);
        musicSlider.onValueChanged.RemoveListener(OnMusicSliderChange);
        sfxSlider.onValueChanged.RemoveListener(OnSfxSliderChange);
        voiceSlider.onValueChanged.RemoveListener(OnVoiceSliderChange);
        uiSlider.onValueChanged.RemoveListener(OnUISliderChange);
        scrollSpeedSlider.onValueChanged.RemoveListener(OnScrollSpeedSliderChange);
    }

    private void OnBackButtonPress()
    {
        UI.ShowPauseMenu();
        UI.ShowSettings(false);
    }

    private void OnMasterAudioSliderChange(float newValue)
    {
        audioMixer.SetFloat("Master", newValue);
    }

    private void OnMusicSliderChange(float newValue)
    {
        audioMixer.SetFloat("Music", newValue);
    }

    private void OnSfxSliderChange(float newValue)
    {
        audioMixer.SetFloat("SFX", newValue);
    }

    private void OnVoiceSliderChange(float newValue)
    {
        audioMixer.SetFloat("Voice", newValue);
    }

    private void OnUISliderChange(float newValue)
    {
        audioMixer.SetFloat("UI", newValue);
    }

    private void OnScrollSpeedSliderChange(float newValue)
    {
        Game.CameraController.SetScrollSpeed(newValue);
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
