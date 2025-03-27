using Core.Inputs;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;

public class LevelManager : MonoBehaviour
{
    [SerializeField] private List<Transform> PlayerPostions;
    [SerializeField] private GameObject player;
    [SerializeField] private GameObject Game;
    [SerializeField] private GameObject mainMenu;
    [SerializeField] private WaveManager waveManager;
    [SerializeField] private Button mission2Button;
    [SerializeField] private Button mission3Button;
    [SerializeField] private AudioSource musicAudioSource;
    [SerializeField] private AudioSource sfxAudioSource;
    [SerializeField] private AudioClip inGameBgm;
    [SerializeField] private GameObject MissionPanel;
    [SerializeField] private GameObject Barriers;
    private int completedLevels;
    [SerializeField] private GameObject backGround;
    [SerializeField] private Image fadeImage;
    private float fadeInDuration = 1f;
    private float fadeOutDuration = 1f;
    private float delayBetweenFades = 1f;
    private bool isFading;
    // Start is called before the first frame update
    private void OnEnable()
    {
        Game.SetActive(false);
        mainMenu.SetActive(true);
        completedLevels = PlayerPrefs.GetInt("CompletedLevels");
        mission3Button.interactable = false;
        mission2Button.interactable = false;
        sfxAudioSource.gameObject.SetActive(false);
        
    }

    // Update is called once per frame
    void Update()
    {
        if (completedLevels == 1)
        {
            mission2Button.interactable = true;
        }
        else if (completedLevels == 2)
        {
            mission2Button.interactable = true;
            mission3Button.interactable = true;
        }
    }
    public void OnMission1ButtonClicked()
    {
        Barriers.SetActive(false);
        waveManager.currentMissionNumber = 0;
        player.transform.position = PlayerPostions[0].transform.position;
        player.transform.rotation = PlayerPostions[0].transform.rotation;
        StartCoroutine("LoadGame");
       
    }
    public void OnMission2ButtonClicked()
    {
        Barriers.SetActive(false);
        waveManager.currentMissionNumber = 1;
        player.transform.position = PlayerPostions[1].transform.position;
        player.transform.rotation = PlayerPostions[1].transform.rotation;
        StartCoroutine("LoadGame");
        
    }
    public void OnMission3ButtonClicked()
    {
        Barriers.SetActive(true);
        waveManager.currentMissionNumber = 2;
        player.transform.position = PlayerPostions[2].transform.position;
        player.transform.rotation = PlayerPostions[2].transform.rotation;
        StartCoroutine("LoadGame");
       
    }
    private IEnumerator LoadGame()
    {
        waveManager.gameObject.SetActive(true );
        player.GetComponent<PlayerInputs>().cursorLocked = true;
        Time.timeScale = 1;
        fadeImage.gameObject.SetActive(true);
        mainMenu.SetActive(false);
        StartFadeInOut();
        musicAudioSource.clip = inGameBgm;
         new WaitForSeconds(1f);
        MissionPanel.SetActive(false);
        Game.SetActive(true);
        player.gameObject.SetActive(true);
        backGround.SetActive(false);
         new WaitForSeconds(1f);
        sfxAudioSource.gameObject.SetActive(true);
        musicAudioSource.Play();
        fadeImage.gameObject.SetActive(false);
        yield return null;
    }
    public void StartFadeInOut()
    {
        if (!isFading)
        {
            isFading = true;
            // Set initial alpha value
            fadeImage.color = new Color(fadeImage.color.r, fadeImage.color.g, fadeImage.color.b, 0f);

            // Fade in
            LeanTween.alpha(fadeImage.rectTransform, 1f, fadeInDuration).setEase(LeanTweenType.easeInOutQuad).setOnComplete(() =>
            {
                // Wait for delay between fades
                LeanTween.delayedCall(delayBetweenFades, () =>
                {
                   
                    // Fade out
                    LeanTween.alpha(fadeImage.rectTransform, 0f, fadeOutDuration).setEase(LeanTweenType.easeInOutQuad).setOnComplete(() =>
                    {
                        isFading = false; // Reset flag
                        
                    });
                });
            });
        }
    }
}
