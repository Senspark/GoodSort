using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using Game.Audio;
using UnityEngine;
using manager.Interface;

namespace manager
{
    public class DefaultAudioManager : IAudioManager
    {
        private readonly Dictionary<Audio, List<(AudioClip clip, float volume)>> clipsInfo = new();
        private readonly IDataManager dataManager;
        private bool initialized;
        private Audio CurrentMusic { set; get; } = Audio.None;

        public DefaultAudioManager(IDataManager dataManager)
        {
            this.dataManager = dataManager;
        }
        
        public Task<bool> Initialize()
        {
            return initialized ? Task.FromResult(true) : InitializeImpl();
        }

        public bool MusicEnabled
        {
            get => dataManager.GetInt("music_enabled", 1) == 1;
            set
            {
                dataManager.SetInt("music_enabled", value ? 1 : 0);
                if(value)
                {
                    AudioPlayer.Instance.UnmuteMusic();
                }
                else
                {
                    AudioPlayer.Instance.MuteMusic();
                }
            }
        }

        public bool SoundEnabled
        {
            get => dataManager.GetInt("sound_enabled", 1) == 1;
            set
            {
                dataManager.SetInt("sound_enabled", value ? 1 : 0);
                if(value)
                {
                    AudioPlayer.Instance.UnmuteSound();
                }
                else
                {
                    AudioPlayer.Instance.MuteSound();
                }
            }
        }
        
        public void PlayMusic(Audio audio)
        {
            if (!initialized || audio == CurrentMusic)
            {
                return;
            }
            CurrentMusic = audio;
            var (clip, volume) = GetRandomClip(audio);
            if (clip == null)
            {
                return;
            }
            AudioPlayer.Instance.PlayMusic(clip, volume);
        }

        public void StopMusic()
        {
            if (!initialized)
            {
                return;
            }
            CurrentMusic = Audio.None;
            AudioPlayer.Instance.StopMusic();
        }

        public void PauseMusic()
        {
            AudioPlayer.Instance.PauseMusic();
        }

        public void ResumeMusic()
        {
            AudioPlayer.Instance.ResumeMusic();
        }

        public void PlaySound(Audio audio)
        {
            if (!initialized)
            {
                return;
            }
            var (clip, volume) = GetRandomClip(audio);
            if (clip == null)
            {
                return;
            }
            AudioPlayer.Instance.PlaySound(clip, volume);
        }
        
        private async Task<bool> InitializeImpl()
        {
            var MusicVolume = 0.4f;
            var SoundVolume = 0.6f;
            await Task.WhenAll(
                LoadAudioClipAsync(Audio.Music1, ("Music1", MusicVolume)),
                LoadAudioClipAsync(Audio.Music2, ("Music2", MusicVolume))
            );
            
            initialized = true;
            return true;
        }

        private async Task LoadAudioClipAsync(Audio audio, params (string path, float volume)[] paths)
        {
            var items = await Task.WhenAll(Enumerable.Select(paths, async key => {
                var path = $"Audio/{key.path}";
                var clip = await Resources.LoadAsync<AudioClip>(path) as AudioClip;
                return (clip, key.volume);
            }).ToArray());
            var infos = GetClipsInfo(audio);
            infos.AddRange(items);
        }

        private List<(AudioClip, float)> GetClipsInfo(Audio audio)
        {
            if(clipsInfo.TryGetValue(audio, out var info))
            {
                return info;
            }
            var clips = new List<(AudioClip, float)>();
            clipsInfo.Add(audio, clips);
            return clips;
        }
        
        // get randomized clip from audio
        private (AudioClip, float) GetRandomClip(Audio audio)
        {
            var clips = GetClipsInfo(audio);
            return clips[Random.Range(0, clips.Count)];
        }
    }
}