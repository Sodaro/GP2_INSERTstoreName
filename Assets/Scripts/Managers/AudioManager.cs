using UnityEngine.Audio;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class AudioManager : MonoBehaviour
{
    
    public Sound[] sounds;
    

    public static AudioManager Instance;

    bool musicIsPlaying = false;

    private bool _isNightLevel = false;
 
    private string _activeMusic;

    Component[] audioSources;

    #region VoiceLines
    [Header("Voice Lines")]
    [SerializeField] private AudioSource _voiceAudioSource;
    private Queue<Sound> _voiceQueue;
    #endregion

    private HashSet<PathFollowAI> _guardsInCombat;

    public void AddToCombat(PathFollowAI guard)
    {
        if (_guardsInCombat.Contains(guard))
            return;

        if (_guardsInCombat.Count == 0)
        {
            _voiceAudioSource.Stop();
            PlayAggroMusic();
        }
            

        _guardsInCombat.Add(guard);

    }

    public void RemoveFromCombat(PathFollowAI guard)
    {
        if (!_guardsInCombat.Contains(guard))
            return;

        _guardsInCombat.Remove(guard);
        if (_guardsInCombat.Count == 0)
        {
            PlayMusic();
        }
    }

    private void ResetGuardsInCombat()
    {
        _guardsInCombat.Clear();
    }



    private void OnEnable()
    {
        SceneManager.activeSceneChanged += ChangedActiveScene;
        MainMenu.onLevelChanged += DisableSFXSounds;
        MainMenu.onLevelChanged += ResetGuardsInCombat;
        //LoadingScreen.onLoadingScreenFinished += PlayMusic;
    }
    private void OnDisable()
    {
        SceneManager.activeSceneChanged -= ChangedActiveScene;
        MainMenu.onLevelChanged -= DisableSFXSounds;
        MainMenu.onLevelChanged -= ResetGuardsInCombat;
        //LoadingScreen.onLoadingScreenFinished -= PlayMusic;
        //LoadingScreen.onLoadingScreenFinished -= PlayAmbience;
    }
    private void DisableSFXSounds()
    {
        foreach (var source in audioSources)
        {
            AudioSource audio = source as AudioSource;

            audio.Stop();
        }
        _voiceQueue.Clear();
    }

    public void PlayMusic()
    {
        LoadingScreen.onLoadingScreenFinished -= PlayMusic;
        Stop("AggroMusic");
        if (_isNightLevel)
        {
            Stop("InGameMusic");
            PlaySFX("NightLevel");
            _activeMusic = "NightLevel";
        }
        else
        {
            Stop("NightLevel");
            PlaySFX("InGameMusic");
            _activeMusic = "InGameMusic";
        }

    }

    public void PlayAggroMusic()
    {
        Stop(_activeMusic);
        PlaySFX("AggroMusic");
    }

    //private void PlayNightMusic()
    //{
    //    LoadingScreen.onLoadingScreenFinished -= PlayNightMusic;
    //    Stop("InGameMusic");
    //    PlaySFX("NightLevel");
    //}
    private void PlayAmbience()
    {
        LoadingScreen.onLoadingScreenFinished -= PlayAmbience;
        PlaySFX("StoreNoise");
    }

    private void Awake()
    {



        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
            
        }
        else
        {
            Instance = this;
        }

        DontDestroyOnLoad(gameObject);

        _voiceQueue = new Queue<Sound>();
        _guardsInCombat = new HashSet<PathFollowAI>();

        foreach (Sound s in sounds)
        {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;

            s.source.volume = s.volume;
            s.source.pitch = s.pitch;
            s.source.loop = s.loop;
            s.source.outputAudioMixerGroup = s.output;
        }


        
        
    }

    private void Start()
    {
        audioSources = GetComponents<AudioSource>();

        if (!musicIsPlaying)
        {
            PlaySFX("InGameMusic");
            musicIsPlaying = true;
        }
    }


    private void Update()
    {
        if (_voiceQueue.Count > 0 && _voiceAudioSource.isPlaying == false)
        {
            PlayVoiceLine(_voiceQueue.Dequeue());
        }
    }

    public void SetVolumeMusic(float newMusicVolume)
    {

        
        foreach (AudioSource a in audioSources)
        {

            a.volume = newMusicVolume;

            Debug.LogWarning(a.volume);

        }
    }

    private void ChangedActiveScene(Scene current, Scene next)
    {
        string activeScene = SceneManager.GetActiveScene().name;
        //Debug.Log(activeScene);
        LoadingScreen.onLoadingScreenFinished += PlayMusic;

        if (activeScene == "MainMenu2" || activeScene == "Level1B" || activeScene == "Level2B" || activeScene == "Level3B")
        {
            Stop("StoreNoise");
            _isNightLevel = true;
        }
        else
        {
            _isNightLevel = false;
            LoadingScreen.onLoadingScreenFinished += PlayAmbience;
            Stop("StoreNoise");
        }
    }

    private Sound FindSound(string name)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        if (s == null)
        {
            Debug.LogWarning("Sound: " + name + " not found!");
            return null;
        }
        return s;
    }

    public void PlayVoiceLine(string name, bool interrupt = false)
    {
        Sound sound = FindSound(name);
        if (interrupt == true || !_voiceAudioSource.isPlaying)
        {
            PlayVoiceLine(sound);
        }
        else
        {
            _voiceQueue.Enqueue(sound);
        }
    }
    public void PlayVoiceLineRandom(bool interrupt = false, params string[] sounds)
    {
        if (sounds.Length == 0)
        {
            Debug.Log($"PlayVoiceLineRandom received empty array");
            return;
        }
        
        int index = UnityEngine.Random.Range(0, sounds.Length);
        
        PlayVoiceLine(sounds[index], interrupt);
    }

    private void PlayVoiceLine(Sound sound)
    {
        _voiceAudioSource.Stop();
        _voiceAudioSource.clip = sound.clip;
        _voiceAudioSource.volume = sound.volume;
        _voiceAudioSource.pitch = sound.pitch;
        _voiceAudioSource.Play();
    }

    public void PlaySFXRandom(params string[] sounds)
    {
        int index = UnityEngine.Random.Range(0, sounds.Length);
        PlaySFX(sounds[index]);
    }

    public void PlaySFX(string name)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        if (s == null)
        {
            Debug.LogWarning("Sound: " + name + " not found!");
            return;
        }
            

        s.source.Play();
    }

    public void Stop(string name)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        if (s == null)
        {
            Debug.LogWarning("Sound: " + name + " not found!");
            return;
        }


        s.source.Stop();
    }
}
