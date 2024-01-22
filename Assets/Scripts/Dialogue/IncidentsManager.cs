using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Services;
using Yarn.Unity;
using UI.Dialogue;

public class IncidentsManager : MonoBehaviour, IService
{
    private Timeline _timeline;

    [field: SerializeField]
    public DialogueRunner Dialogue { get; set; }

    [SerializeField]
    private IncidentTimeline[] _incidents;

    public event System.Action<string> spawnedIncident;

    // Start is called before the first frame update
    void Awake()
    {
        ServiceLocator.RegisterAsService(this);
        _timeline = ServiceLocator.GetService<Timeline>();
    }

    private void Start()
    {
        foreach(var inc in _incidents)
        {
            _timeline.AddTimelineEvent(this, () => SpawnIncident(inc.nodeName), Timeline.FromStart(0, inc.monthsAfterStart));
        }
    }

    void SpawnIncident(string incYarnNode)
    {
        spawnedIncident?.Invoke(incYarnNode);
    }

    [System.Serializable]
    public struct IncidentTimeline
    {
        public string nodeName;
        public int monthsAfterStart;
    }
}
