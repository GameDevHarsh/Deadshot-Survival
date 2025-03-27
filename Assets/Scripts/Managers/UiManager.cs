using UnityEngine;
using UnityEngine.UI;

public class UiManager : MonoBehaviour
{
    [SerializeField] private GameObject pauseMenuPanel;
    [SerializeField] private GameObject optionMenuPanel;
    [SerializeField] private GameObject exitMenuPanel;
    [SerializeField] private GameObject mainMenu;
    [SerializeField] private GameObject Game;
    [SerializeField] private GameObject background;
    [HideInInspector] public AudioSource sfxAudioSource;
    [HideInInspector] public AudioSource musicAudioSource;
    [SerializeField] private GameObject WinPanel;
    [SerializeField] private Slider sfxSlider;
    [SerializeField] private Slider musicSlider;
    [HideInInspector] public AudioClip mainMenuBGM;
    [SerializeField] private GameObject mainMenuBackground;
    [SerializeField] private GameObject WinMenuBackground;
    [SerializeField] private WaveManager waveManager;
    [SerializeField] private LevelManager levelManager;
    [SerializeField] private GameObject CompletedPanelButton;
    public bool isUIActive = false;
    // Start is called before the first frame update
    private void OnEnable()
    {
        sfxSlider.value = sfxAudioSource.volume;
        musicSlider.value = musicAudioSource.volume;
        pauseMenuPanel.SetActive(false);
        optionMenuPanel.SetActive(false);
        exitMenuPanel.SetActive(false);
        background.SetActive(false);
        
    }
    private void Update()
    {
        sfxAudioSource.volume = sfxSlider.value;
        musicAudioSource.volume = musicSlider.value;
        if (Input.GetKey(KeyCode.Escape))
        {
            pauseMenuPanel.SetActive(true);
            Cursor.lockState = CursorLockMode.None;
            background.SetActive(true);
            Time.timeScale = 0;
            isUIActive = true;
        }
    }
    public void OnResumeButtonClicked()
    {
        pauseMenuPanel.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
        Time.timeScale = 1;
        background.SetActive(false);
        isUIActive = false;
    }
    public void OnOptionButtonClicked()
    {
        optionMenuPanel.SetActive(true);
        pauseMenuPanel.SetActive(false);
    }
    public void OnCloseButtonClicked()
    {
        optionMenuPanel.SetActive(false);
        pauseMenuPanel.SetActive(true);
    }
    public void OnExitButtonClicked()
    {
        exitMenuPanel.SetActive(true);
        if (pauseMenuPanel.activeInHierarchy)
        {
            pauseMenuPanel.SetActive(false);
        }
        else
        {
            WinPanel.SetActive(false);
        }
        
    }
    public void OnYesButtonClicked()
    {
        mainMenu.SetActive(true);
        Game.SetActive(false);
        mainMenuBackground.SetActive(true);
        musicAudioSource.clip = mainMenuBGM;
        musicAudioSource.Play();
        sfxAudioSource.gameObject.SetActive(false);
        isUIActive = false;
        WinMenuBackground.SetActive(false);
    }
    public void OnNoButtonClicked()
    {
        exitMenuPanel.SetActive(false);
        if (!isUIActive)
        {
            WinPanel.SetActive(true);
        }
        else
        {
            pauseMenuPanel.SetActive(true);
        }
    }
    public void OnNextButtonClicked()
    {
        waveManager.gameObject.SetActive(true);
        if (waveManager.currentMissionNumber == 0)
        {
            levelManager.OnMission2ButtonClicked();
        }
        else if(waveManager.currentMissionNumber == 1)
        {
            levelManager.OnMission3ButtonClicked();
        }
        else
        {
            CompletedPanelButton.SetActive(true);
        }
    }
    public void OnRetryButtonClicked()
    {
        waveManager.gameObject.SetActive(true);
        if (waveManager.currentMissionNumber == 0)
        {
            levelManager.OnMission1ButtonClicked();
        }
        else if (waveManager.currentMissionNumber == 1)
        {
            levelManager.OnMission2ButtonClicked();
        }
        else if(waveManager.currentMissionNumber == 2)
        {
            levelManager.OnMission3ButtonClicked();
        }
    }
}
