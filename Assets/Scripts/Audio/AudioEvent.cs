using Services;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

[CreateAssetMenu(menuName = "Audio/Audio Event")]
public class AudioEvent : ScriptableObject
{
    public AudioClip clip;
    public virtual AudioClip[] GetClips() => new AudioClip[] { clip };

    public bool loop;

    [Range(0, 1)]
    public float volume = 1;
    public AudioMixerGroup mixerGroup;
    public AudioService.Channel channel;

}

[CreateAssetMenu(menuName = "Audio/Audio Multi Event")]
public class AudioMultiEvent : AudioEvent
{
    public AudioClip[] AudioClips;
    new public AudioClip clip => AudioClips[0];
    public override AudioClip[] GetClips() => AudioClips;

}
