using Services;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicDirector : MonoBehaviour
{
    public AudioEvent mainMusicIntro;
    public AudioEvent mainMusicLoop;
    public AudioEvent endTimeMusic;

    public float EndTimeOffset = 5;
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

        _timeline.AddTimelineEvent(this, () => _audio.Play(endTimeMusic), musicStartDate);
    }

}
