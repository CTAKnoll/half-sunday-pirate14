using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Services;
using Yarn.Unity;

public class IncidentsManager : MonoBehaviour
{
    private Timeline _timeline;
    [SerializeField]
    private DialogueRunner _dialogue;

    [SerializeField]
    private IncidentTimeline[] _incidents;


    // Start is called before the first frame update
    void Awake()
    {
        _timeline = ServiceLocator.GetService<Timeline>();
    }

    private void Start()
    {
        foreach(var inc in _incidents)
        {
            _timeline.AddTimelineEvent(this, () => _dialogue.StartDialogue(inc.nodeName), Timeline.FromStart(0, inc.monthsAfterStart));
        }
    }

    [System.Serializable]
    public struct IncidentTimeline
    {
        public string nodeName;
        public int monthsAfterStart;
    }
}
