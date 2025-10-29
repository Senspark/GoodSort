using Senspark;
using UnityEngine;

namespace manager.Interface
{
    public enum Audio
    {
        None,
        MenuMusic,
        GameMusic,
        ClickButton,
        CloseDialog,
        CoinFly,
        PutGoods,
        Match,
        LevelComplete,
    }
    [Service(nameof(IAudioManager))]
    public interface IAudioManager : IService
    {
        public bool MusicEnabled { get; set; }
        public bool SoundEnabled { get; set; }
        public void PlayMusic(Audio audio);
        public void StopMusic();
        public void PauseMusic();
        public void ResumeMusic();
        public void PlaySound(Audio audio);
    }
}