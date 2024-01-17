using UnityEngine;

[CreateAssetMenu(menuName = "Audio/Audio Multi Event")]
public class AudioMultiEvent : AudioEvent
{
    public AudioClip[] AudioClips;
    [SerializeField]
    private Mode _mode;
    private int _currIdx = 0;

    public override AudioClip GetClip(){
        switch (_mode)
        {
            case Mode.Random:
                return GetRandom();
            case Mode.Ordered:
                return GetNext();
            default: return AudioClips[0];
        }
    }

    private AudioClip GetRandom()
    {
        int randIdx = Random.Range(0, AudioClips.Length);
        return AudioClips[randIdx];
    }

    private AudioClip GetNext()
    {
        if (_currIdx >= AudioClips.Length)
            _currIdx = 0;
        return AudioClips[_currIdx++];
    }

    public enum Mode
    {
        Random, Ordered
    }
}
