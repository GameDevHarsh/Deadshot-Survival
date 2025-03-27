using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ThirdPersonHealth : MonoBehaviour
{
    [SerializeField] private float health = 100;
    [SerializeField] private float damageAmmount;
    [SerializeField]
    private TextMeshProUGUI healthText;
    [SerializeField]
    private Slider healthBarSlider;
    [SerializeField]
    private Image bloodSplash;
    [SerializeField] private GameObject LosePanel;
    [SerializeField] private WaveManager waveManager;
    [SerializeField] private UiManager uiManager;
    private float fadeInDuration = 0f;
    private float fadeOutDuration = 1f;
    private float delayBetweenFades = 1f;
    private bool isFading;

    private void OnEnable()
    {
        health = 100;
        LosePanel.SetActive(false);
        this.gameObject.SetActive(true);
        bloodSplash.color = new Color(bloodSplash.color.r, bloodSplash.color.g, bloodSplash.color.b, 0f);
        bloodSplash.gameObject.SetActive(true);
    }

    public void DamagePl()
    {
        if (health > 0)
        {
            health -= damageAmmount;
            StartFadeInOut();
        }
    }
    private void Update()
    {
        healthBarSlider.value = Mathf.Lerp(healthBarSlider.value, health, 2 * Time.deltaTime);
        healthText.text = health.ToString();
        if (health - damageAmmount < 0)
        {
            bloodSplash.gameObject.SetActive(false);
            LosePanel.SetActive(true);
            waveManager.gameObject.SetActive(false);
            uiManager.musicAudioSource.clip= uiManager.mainMenuBGM;
            uiManager.sfxAudioSource.gameObject.SetActive(false);
            uiManager.musicAudioSource.Play();
            Time.timeScale = 0;
            Cursor.lockState = CursorLockMode.None;
            this.gameObject.SetActive(false);
        }
    }
    public void StartFadeInOut()
    {
        if (!isFading)
        {
            isFading = true;
            // Set initial alpha value
            bloodSplash.color = new Color(bloodSplash.color.r, bloodSplash.color.g, bloodSplash.color.b, 0f);

            // Fade in
            LeanTween.alpha(bloodSplash.rectTransform, 1f, fadeInDuration).setEase(LeanTweenType.easeInOutQuad).setOnComplete(() =>
            {
                // Wait for delay between fades
                LeanTween.delayedCall(delayBetweenFades, () =>
                {
                    // Fade out
                    LeanTween.alpha(bloodSplash.rectTransform, 0f, fadeOutDuration).setEase(LeanTweenType.easeInOutQuad).setOnComplete(() =>
                    {
                        isFading = false; // Reset flag
                    });
                });
            });
        }
    }
}

