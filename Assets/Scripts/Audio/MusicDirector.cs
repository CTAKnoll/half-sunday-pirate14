using Services;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class MusicDirector : MonoBehaviour
{
    public AudioEvent mainMusicIntro;
    public AudioEvent mainMusicLoop;
    public AudioEvent endTimeMusic;

    [Space]
    [SerializeField]
    private AudioMixerSnapshot _mainSnapshot;
    [SerializeField]
    private AudioMixerSnapshot _endTimesSnapshot;

    public float EndTimeOffset = -15;
    private Timeline _timeline;
    private AudioService _audio;

    private void Start()
    {
        ServiceLocator.TryGetService(out _timeline);    
        ServiceLocator.TryGetService(out _audio);

        EnqueueEndTimes();
    }

    private void EnqueueEndTimes()
    {
        var musicStartDate = _timeline.GetDateBeforeInRealtime(
            endTimeMusic.GetClip().length + EndTimeOffset, 
            Timeline.CRASH_DATE);
        Debug.Log($"Timespan {_timeline.GetTimespanFromSeconds(endTimeMusic.GetClip().length + EndTimeOffset)}");
        Debug.Log($"Enqueuing endtime music [{endTimeMusic.GetClip().name}] for {musicStartDate}");

        _timeline.AddTimelineEvent(this, TransitionToEnd, musicStartDate);
    }

    private void TransitionToEnd()
    {
        _endTimesSnapshot.TransitionTo(2.5f);
        void StartSong()
        {
            _audio.Play(endTimeMusic);
        }
        Invoke("StartSong", 1);
    }

}
