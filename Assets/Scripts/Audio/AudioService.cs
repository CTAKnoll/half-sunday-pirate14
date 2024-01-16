using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static Unity.VisualScripting.Member;

namespace Services
{
    public class AudioService : MonoBehaviour, IService
    {
        [SerializeField]
        private AudioSource _sFXSource;

        [SerializeField]
        private AudioSource _musicSource;

        [SerializeField]
        private AudioSource _ambianceSource;

        private AudioSource[] _audioSources = null;
        private void Awake()
        {
            ServiceLocator.RegisterAsService(this);
            _audioSources = new AudioSource[] { _sFXSource, _musicSource, _ambianceSource };
        }

        public void Play(AudioEvent sound)
        {
            if (sound == null)
                return;
            
            var source = _audioSources[(int)sound.channel];

            source.clip = sound.clip;
            source.volume = sound.volume;
            source.loop = sound.loop;
            source.Play();
        }

        public void PlayOneShot(AudioEvent sound)
        {
            if (sound == null)
                return;

            var source = _audioSources[(int)sound.channel];

            source.PlayOneShot(sound.clip, sound.volume);
        }

        public enum Channel
        {
            SFX, Music, Ambiance
        }
    }
}