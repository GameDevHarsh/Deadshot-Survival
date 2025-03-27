using UnityEngine;
using UnityEngine.UI;

public class MainMenuUiManager : MonoBehaviour
{
    [SerializeField] private GameObject missionPanel;
    [SerializeField] private GameObject mainMenuPanel;
    [SerializeField] private GameObject optionMenuPanel;
    [SerializeField] private GameObject ExitMenuPanel;
    [SerializeField] private Slider sfxSlider;
    [SerializeField] private Slider musicSlider;
    [SerializeField] private AudioSource sfxAudioSource;
    [SerializeField] private AudioSource musicAudioSource;
    private void OnEnable()
    {
        sfxSlider.value = sfxAudioSource.volume;
        musicSlider.value = musicAudioSource.volume;
        missionPanel.SetActive(false);
        mainMenuPanel.SetActive(true);
        optionMenuPanel.SetActive(false);
        ExitMenuPanel.SetActive(false);
    }

    public void OnStartButtonClicked()
    {
        missionPanel.SetActive(true);
        mainMenuPanel.SetActive(false);
    }
    public void OnExitButtonClicked()
    {
        missionPanel.SetActive(false);
        mainMenuPanel.SetActive(false);
        ExitMenuPanel.SetActive(true);
    }
    public void OnMissionCloseButtonClicked()
    {
        missionPanel.SetActive(false);
        mainMenuPanel.SetActive(true);
    }
    public void OnOptionCloseButtonClicked()
    {
        optionMenuPanel.SetActive(false);
        mainMenuPanel.SetActive(true);
    }
    public void OnOptionButtonClicked()
    {
        mainMenuPanel.SetActive(false);
        optionMenuPanel.SetActive(true);
    }
    public void OnYesButtonClicked()
    {
        Application.Quit();
    }
    public void OnNoButtonClicked()
    {
        ExitMenuPanel.SetActive(false);
        mainMenuPanel.SetActive(true);
    }
    private void Update()
    {
        sfxAudioSource.volume = sfxSlider.value;
        musicAudioSource.volume = musicSlider.value;
    }

}
