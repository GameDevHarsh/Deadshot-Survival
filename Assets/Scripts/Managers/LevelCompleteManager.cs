using Core.Inputs;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class LevelCompleteManager : MonoBehaviour
{
    public GameObject CongratPanel;
    public WaveManager waveManager;
    public UiManager uiManager;
    // Start is called before the first frame update

    private void OnEnable()
    {
        CongratPanel.SetActive(false);
    }
    private void OnTriggerEnter(Collider other)
    {
        if(other.transform.CompareTag("Player"))
        {
            Cursor.lockState = CursorLockMode.None;
            other.gameObject.SetActive(false);
            uiManager.musicAudioSource.clip = uiManager.mainMenuBGM;
            uiManager.sfxAudioSource.gameObject.SetActive(false);
            uiManager.musicAudioSource.Play();
            Time.timeScale = 0;
            CongratPanel.SetActive(true);
            if(waveManager.currentMissionNumber==0)
            {
                PlayerPrefs.SetInt("CompletedLevels",1);
            }
            else
                if(waveManager.currentMissionNumber==1)
            {
                PlayerPrefs.SetInt("CompletedLevels", 2);
            }
            waveManager.gameObject.SetActive(false);

        }
    }
}
