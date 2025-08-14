
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SettingUI : BaseUI
{
    public override CanvasLayer Layer => CanvasLayer.Popup;
    
    public override bool IsEnabled => gameObject.activeSelf;

    [SerializeField] private Slider masterVolumeSlider;
    [SerializeField] private Slider bgmVolumeSlider;
    [SerializeField] private Slider sfxVolumeSlider;

    [SerializeField] private Button masterVolumeMuteButton;
    [SerializeField] private Button bgmVolumeMuteButton;
    [SerializeField] private Button sfxVolumeMuteButton;

    [SerializeField] private TextMeshProUGUI masterVolumeMuteText;
    [SerializeField] private TextMeshProUGUI bgmVolumeMuteText;
    [SerializeField] private TextMeshProUGUI sfxVolumeMuteText;

    [SerializeField] private GameObject settingInfo;
    [SerializeField] private GameObject controllerInfo;
    [SerializeField] private Button gameOverButton;

    private SettingManager _settingManager;

    private PlayerController _player;

    
    private void Awake()
    {
        SetSliders();
        
        SetMuteButtons(masterVolumeMuteButton, SoundType.MasterVolume, masterVolumeMuteText);
        SetMuteButtons(bgmVolumeMuteButton, SoundType.BGMVolume, bgmVolumeMuteText);
        SetMuteButtons(sfxVolumeMuteButton, SoundType.SFXVolume, sfxVolumeMuteText);

        gameOverButton.onClick.AddListener(OnClickGameOverButton);
    }

    public void Init(PlayerController player)
    {
        _settingManager = GameManager.Instance.SettingManager;
        
        _player = player;
    }

    public override void Enable()
    {
        gameObject.SetActive(true);
        controllerInfo.SetActive(false);

        GetSoundValues();

        Time.timeScale = 0f;

        AudioManager.Instance.Play(key:"SettingOpenClip");
        _player.InputController.EnableSettingUIInputs();
        _player.InputController.OnClosePopupUI += Disable;
    }

    public override void Disable()
    {
        gameObject.SetActive(false);
        AudioManager.Instance.Play(key: "SettingOpenClip");

        Time.timeScale = 1f;

        _player.InputController.EnablePlayerInputs();
        _player.InputController.OnClosePopupUI -= Disable;
    }

    private void SetSliders()
    {
        masterVolumeSlider.onValueChanged.AddListener((v) => { _settingManager.SetVolume(SoundType.MasterVolume, v);});
        bgmVolumeSlider.onValueChanged.AddListener((v) => { _settingManager.SetVolume(SoundType.BGMVolume, v);});
        sfxVolumeSlider.onValueChanged.AddListener((v) => { _settingManager.SetVolume(SoundType.SFXVolume, v);});
    }

    private void SetMuteButtons(Button button, SoundType type, TextMeshProUGUI muteText)
    {
        button.onClick.AddListener(() =>
        {
            _settingManager.ToggleSoundMute(type);
            muteText.text = _settingManager.IsMuted(type) ? "비활성화" : "활성화";

            AudioManager.Instance.Play(key: "SettingClickClip");
        });
    }

    private void GetSoundValues()
    {
        masterVolumeSlider.value = _settingManager.GetVolume(SoundType.MasterVolume);
        bgmVolumeSlider.value = _settingManager.GetVolume(SoundType.BGMVolume);
        sfxVolumeSlider.value = _settingManager.GetVolume(SoundType.SFXVolume);

        masterVolumeMuteText.text = _settingManager.IsMuted(SoundType.MasterVolume) ? "비활성화" : "활성화";
        bgmVolumeMuteText.text = _settingManager.IsMuted(SoundType.BGMVolume) ? "비활성화" : "활성화";
        sfxVolumeMuteText.text= _settingManager.IsMuted(SoundType.SFXVolume) ? "비활성화" : "활성화";
    }


    private void OnClickGameOverButton()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
    Application.Quit();
#endif
    }
}