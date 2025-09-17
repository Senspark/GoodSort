using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Game.Audio
{
    [DefaultExecutionOrder(-1)]
    [DisallowMultipleComponent]
    public class AudioPlayer : MonoBehaviour
    {
        public static AudioPlayer Instance { get; private set; }
        [Header("Music Settings")]
        [SerializeField] private AudioSource musicSource;
        
        [Header("Sound Settings")]
        [SerializeField] private int soundSourceCount = 5;
        
        private readonly List<AudioSource> soundSources = new();
        private bool musicMuted;
        private bool soundMuted;
        
        private void Awake()
        {
            if(Instance != null && Instance != this)
            {
                Destroy(this);
                return;
            }
            Instance = this;
            DontDestroyOnLoad(gameObject);
            if (musicSource == null)
            {
                musicSource = gameObject.AddComponent<AudioSource>();
                musicSource.loop = true;
            }
            for (int i = 0; i < soundSourceCount; i++)
            {
                var source = gameObject.AddComponent<AudioSource>();
                source.loop = false;
                soundSources.Add(source);
            }
        }

        private void OnDestroy()
        {
            if (Instance == this)
            {
                Instance = null;
            }
        }
        
        public void PlayMusic(AudioClip clip, float volume, bool isLoop = true)
        {
            if (clip == null)
            {
                return;
            }
            musicSource.clip = clip;
            musicSource.volume = volume;
            musicSource.loop = isLoop;
            musicSource.Play();
        }
        
        public void StopMusic() => musicSource.Stop();
        public void PauseMusic() => musicSource.Pause();
        public void ResumeMusic() => musicSource.UnPause();
        
        public void PlaySound(AudioClip clip, float volume)
        {
            if (clip == null)
            {
                return;
            }

            foreach (var source in soundSources.Where(source => !source.isPlaying))
            {
                source.clip = clip;
                source.volume = volume;
                source.Play();
                return;
            }
        }

        public void MuteMusic()
        {
            musicMuted = true;
            musicSource.mute = true;
        }
        
        public void UnmuteMusic()
        {
            musicMuted = false;
            musicSource.mute = false;
        }
        
        public void MuteSound()
        {
            soundMuted = true;
            foreach (var source in soundSources)
            {
                source.mute = true;
            }
        }
        
        public void UnmuteSound()
        {
            soundMuted = false;
            foreach (var source in soundSources)
            {
                source.mute = false;
            }
        }
        
        
    }
}