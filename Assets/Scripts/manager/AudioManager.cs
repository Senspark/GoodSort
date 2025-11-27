using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Cysharp.Threading.Tasks;

using JetBrains.Annotations;

using manager.Interface;

using Newtonsoft.Json;

using Sirenix.OdinInspector;

using UnityEngine;

namespace Senspark {
    public interface IAudioInfo {
        /// <summary>
        /// Base volume.
        /// </summary>
        float Volume { get; }

        /// <summary>
        /// Loads the audio clip.
        /// </summary>
        [ItemNotNull]
        [NotNull]
        Task<AudioClip> Load();
    }

    public class AudioInfo : IAudioInfo {
        [NotNull]
        private readonly string _path;

        public float Volume { get; }

        public AudioInfo([NotNull] string path, float volume = 1f) {
            _path = path;
            Volume = volume;
        }

        public async Task<AudioClip> Load() {
            var clip = await Resources.LoadAsync<AudioClip>(_path) as AudioClip;
            if (clip == null) {
                throw new ArgumentNullException($"Could not load {_path}");
            }
            return clip;
        }
    }

    public class ScriptableAudioInfo : ScriptableObject, IAudioInfo {
        [Required]
        [SerializeField]
        private AudioClip _audioClip;

        [SerializeField]
        private float _volume;

        public float Volume => _volume;

        public Task<AudioClip> Load() {
            return Task.FromResult(_audioClip);
        }
    }

    public class AudioManager : IAudioManager, IService {
        private class Data {
            [JsonProperty("music_enabled")]
            public bool IsMusicEnabled { get; set; }

            [JsonProperty("sound_enabled")]
            public bool IsSoundEnabled { get; set; }

            [JsonProperty("music_volume")]
            public float MusicVolume { get; set; }

            [JsonProperty("sound_volume")]
            public float SoundVolume { get; set; }
        }

        [NotNull]
        private readonly IDataManager _dataManager;

        [NotNull]
        private readonly string _dataKey;

        [NotNull]
        private readonly Dictionary<Enum, IAudioInfo> _infos;

        [NotNull]
        private readonly Dictionary<Enum, AudioClip> _clips;

        private UniTask _initializer;
        private AudioPlayer _musicPlayer;
        private AudioPlayer _soundPlayer;
        private Data _data;

        [CanBeNull]
        private Enum _music;

        public UniTask Initialize()
        {
            return _initializer = _initializer.Status == UniTaskStatus.Succeeded ? _initializer : InitializeImpl();
        }

        async UniTask<bool> IService.Initialize()
        {
            await Initialize();
            return true;
        }

        public bool IsMusicEnabled {
            get => _data?.IsMusicEnabled ?? true;
            set {
                if (_data == null) {
                    return;
                }
                if (_data.IsMusicEnabled == value) {
                    return;
                }
                _data.IsMusicEnabled = value;
                SaveData();
                UpdateMusic();
            }
        }

        public bool IsSoundEnabled {
            get => _data?.IsSoundEnabled ?? true;
            set {
                if (_data == null) {
                    return;
                }
                if (_data.IsSoundEnabled == value) {
                    return;
                }
                _data.IsSoundEnabled = value;
                SaveData();
                UpdateSound();
            }
        }

        public float MusicVolume {
            get => _data?.MusicVolume ?? 1f;
            set {
                if (_data == null) {
                    return;
                }
                if (Mathf.Approximately(_data.MusicVolume, value)) {
                    return;
                }
                _data.MusicVolume = value;
                SaveData();
                UpdateMusic();
            }
        }

        public float SoundVolume {
            get => _data?.SoundVolume ?? 1f;
            set {
                if (_data == null) {
                    return;
                }
                if (Mathf.Approximately(_data.SoundVolume, value)) {
                    return;
                }
                _data.SoundVolume = value;
                SaveData();
                UpdateSound();
            }
        }

        public AudioManager(
            [NotNull] IDataManager dataManager,
            [NotNull] string dataKey,
            [NotNull] Dictionary<Enum, IAudioInfo> infos
        ) {
            _dataManager = dataManager;
            _dataKey = dataKey;
            _infos = infos;
            _clips = new Dictionary<Enum, AudioClip>();
        }


        private async UniTask InitializeImpl() {
            await _dataManager.Initialize();
            const string path = "Prefabs/Auxiliary/AudioPlayer";
            var prefab = (AudioPlayer) await Resources.LoadAsync<AudioPlayer>(path);
            _musicPlayer = UnityEngine.Object.Instantiate(prefab);
            _musicPlayer.IsLoop = true;
            _soundPlayer = UnityEngine.Object.Instantiate(prefab);
            UnityEngine.Object.DontDestroyOnLoad(_musicPlayer.gameObject);
            UnityEngine.Object.DontDestroyOnLoad(_soundPlayer.gameObject);
            LoadData();
            UpdateMusic();
            UpdateSound();
            await Task.WhenAll(_infos.Select((Func<KeyValuePair<Enum, IAudioInfo>, Task>) (async entry => {
                var clip = await entry.Value.Load();
                _clips.Add(entry.Key, clip);
            })));
        }

        private void LoadData() {
            _data = _dataManager.Get(_dataKey,
                new Data {
                    IsMusicEnabled = true, //
                    IsSoundEnabled = true,
                    MusicVolume = 1,
                    SoundVolume = 1,
                });
        }

        private void SaveData() {
            _dataManager.Set(_dataKey, _data);
        }

        private void UpdateMusic() {
            _musicPlayer.IsMuted = !IsMusicEnabled;
            _musicPlayer.Volume = MusicVolume;
        }

        private void UpdateSound() {
            _soundPlayer.IsMuted = !IsSoundEnabled;
            _soundPlayer.Volume = SoundVolume;
            if (IsSoundEnabled) {
                // OK.
            } else {
                _soundPlayer.Stop();
            }
        }

        public void PlayMusic(Enum id, float volume) {
            var player = _musicPlayer;
            if (player == null) {
                return;
            }
            if (id != null) {
                _music = id;
            }
            if (_music == null) {
                return;
            }
            var clip = _clips[_music];
            var finalVolume = _infos[_music].Volume * volume;
            player.PlayMusic(clip, finalVolume);
        }

        public void StopMusic() {
            var player = _musicPlayer;
            if (player == null) {
                return;
            }
            player.Stop();
        }

        public void PlaySound(Enum id, float volume) {
            var player = _soundPlayer;
            if (player == null) {
                return;
            }
            var clip = _clips[id];
            var finalVolume = _infos[id].Volume * volume;
            player.PlaySound(clip, finalVolume);
        }
        
        public void PlaySoundOnPitch(Enum id, float pitch) {
            var player = _soundPlayer;
            if (player == null) {
                return;
            }
            var clip = _clips[id];
            var finalVolume = _infos[id].Volume;
            player.PlaySound(clip, finalVolume, pitch);
        }
    }
}