using Services;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

[CreateAssetMenu(menuName = "Audio/Audio Event")]
public class AudioEvent : ScriptableObject
{
    public AudioClip clip;

    public bool loop;

    [Range(0, 1)]
    public float volume = 1;
    public AudioMixerGroup mixerGroup;
    public AudioService.Channel channel;

    [Min(1)]
    [Tooltip("This is the maximum number of times this sound can be played concurrently before culled")]
    public int maxConcurrent = 10;

    [Min(0f)]
    [Tooltip("After this event has been triggerd, it cannot be triggered again until after this interval expires")]
    public float minInterval = 0f;
}
