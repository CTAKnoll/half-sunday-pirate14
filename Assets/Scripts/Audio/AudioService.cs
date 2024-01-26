using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

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

        public enum Channel
        {
            SFX, Music, Ambiance
        }

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

            source.clip = sound.GetClip();
            source.volume = sound.volume;
            source.loop = sound.loop;
            if(sound.randomizePitch)
            {
                source.pitch += UnityEngine.Random.Range(-1 * sound.pitchVariance, sound.pitchVariance);
            }
            source.Play();
        }

        public void PlayOneShot(AudioEvent sound)
        {
            if (sound == null)
                return;

            var source = _audioSources[(int)sound.channel];
            if (sound.randomizePitch)
            {
                source.pitch += UnityEngine.Random.Range(-1 * sound.pitchVariance, sound.pitchVariance);
            }

            source.PlayOneShot(sound.GetClip(), sound.volume);
        }

    }
}