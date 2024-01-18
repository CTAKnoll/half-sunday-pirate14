using Services;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

[CreateAssetMenu(menuName = "Audio/Audio Event")]
public class AudioEvent : ScriptableObject
{
    [SerializeField]
    private AudioClip _clip;

    public virtual AudioClip GetClip()
    {
        return _clip;
    }

    public bool loop;

    [Range(0, 1)]
    public float volume = 1;
    public AudioMixerGroup mixerGroup;
    public AudioService.Channel channel;

    [Header("Pitch")]
    public bool randomizePitch = false;
    [Range(0, 0.5f)]
    public float pitchVariance = 0;

}
