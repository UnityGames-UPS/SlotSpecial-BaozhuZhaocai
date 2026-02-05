using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
internal class AudioController : MonoBehaviour
{
    [Header("Audio Sources")]
    [SerializeField] private AudioSource bgMusicSource;
    [SerializeField] private AudioSource gameSoundSource;
    // [SerializeField] private AudioSource uiSource;

    [Header("Background")]
    [SerializeField] private AudioClip bgMusic;

    [Header("Game Sounds")]
    [SerializeField] private AudioClip goldenCoin;
    [SerializeField] private AudioClip rocket;
    [SerializeField] private AudioClip rocketBlast;
    [SerializeField] private AudioClip SpinStarts;
    [SerializeField] private AudioClip SpinStops;
    [SerializeField] private AudioClip LightSound;
    [SerializeField] private AudioClip BonusStarted;
    [SerializeField] private AudioClip RepeatSlotWin;
    [SerializeField] private AudioClip WinPopup;

    [Header("UI Sounds")]
    [SerializeField] private AudioClip uiButton;

    [Header("Sound Buttons")]
    [SerializeField] private Button SoundButton;
    [SerializeField] private Button SoundMuteButton;
    [SerializeField] private Button MusicButton;
    [SerializeField] private Button MusicMuteButton;

    private bool isGameMuted = false;
    private bool isMusicMuted = false;

    private void Start()
    {
        if (SoundButton)
        {
            SoundButton.onClick.RemoveAllListeners();
            SoundButton.onClick.AddListener(ToggleGameSound);
        }

        if (MusicButton)
        {
            MusicButton.onClick.RemoveAllListeners();
            MusicButton.onClick.AddListener(ToggleBackgroundMusic);
        }

        if (SoundMuteButton)
        {
            SoundMuteButton.onClick.RemoveAllListeners();
            SoundMuteButton.onClick.AddListener(ToggleGameSound);
        }

        if (MusicMuteButton)
        {
            MusicMuteButton.onClick.RemoveAllListeners();
            MusicMuteButton.onClick.AddListener(ToggleBackgroundMusic);
        }

        PlayBackground();
    }

    private void ToggleGameSound()
    {
        Debug.Log("button pressed!");
        if (!isGameMuted)
        {
            SoundMuteButton.gameObject.SetActive(true);
            SoundButton.gameObject.SetActive(false);
        }
        else
        {
            SoundButton.gameObject.SetActive(true);
            SoundMuteButton.gameObject.SetActive(false);
        }
        isGameMuted = !isGameMuted;
        MuteGame(isGameMuted);
    }

    private void ToggleBackgroundMusic()
    {
        if (!isMusicMuted)
        {
            MusicMuteButton.gameObject.SetActive(true);
            MusicButton.gameObject.SetActive(false);
        }
        else
        {
            MusicButton.gameObject.SetActive(true);
            MusicMuteButton.gameObject.SetActive(false);
        }
        isMusicMuted = !isMusicMuted;
        MuteBackground(isMusicMuted);
    }


    internal void PlayBackground()
    {
        if (!bgMusic) return;

        bgMusicSource.clip = bgMusic;
        bgMusicSource.loop = true;
        if (!bgMusicSource.isPlaying)
            bgMusicSource.Play();
    }

    internal void StopBackground()
    {
        bgMusicSource.Stop();
    }

    internal void PlayGoldenCoin()
    {
        PlayGame(goldenCoin, false);
    }

    internal void PlayRocket()
    {
        PlayGame(rocket, false);
    }
    internal void PlayRocketBlast()
    {
        PlayGame(rocketBlast, false);
    }

    internal void PlaySpinStarts()
    {
        PlayGame(SpinStarts, false);
    }
    internal void PlaySpinStops()
    {
        PlayGame(SpinStops, false);
    }
    internal void PlayLightSound()
    {
        PlayGame(LightSound, false);
    }
    internal void PlayBonusStarted()
    {
        PlayGame(BonusStarted, false);
    }
    internal void PlayRepeatSlotWin()
    {
        PlayGame(RepeatSlotWin, false);
    }
    internal void PlayWin()
    {
        PlayGame(WinPopup, false);
    }

    private void PlayGame(AudioClip clip, bool loop)
    {
        if (!clip) return;

        gameSoundSource.Stop();
        gameSoundSource.clip = clip;
        gameSoundSource.loop = loop;
        gameSoundSource.Play();
    }

    internal void StopGameAudio()
    {
        gameSoundSource.Stop();
        gameSoundSource.loop = false;
    }

    // internal void PlayChip()
    // {
    //     gameSoundSource.PlayOneShot(chipSound);
    // }

    // internal void PlayCardPlaced()
    // {
    //     gameSoundSource.PlayOneShot(cardPlaced);
    // }

    internal void PlayUIButton()
    {
        gameSoundSource.PlayOneShot(uiButton);
    }

    // internal void PlayNavigation()
    // {
    //     uiSource.PlayOneShot(navigation);
    // }

    internal void MuteAll(bool mute)
    {
        bgMusicSource.mute = mute;
        gameSoundSource.mute = mute;
        // uiSource.mute = mute;
    }


    internal void MuteBackground(bool mute) => bgMusicSource.mute = mute;
    internal void MuteGame(bool mute) => gameSoundSource.mute = mute;
    // internal void MuteUI(bool mute) => uiSource.mute = mute;
}
