using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Services;
using Yarn.Unity;
using Utils;

public class IncidentsManager : MonoBehaviour, IService
{
    private Timeline _timeline;
    private AudioService _audio;

    [field: SerializeField]
    public DialogueRunner Dialogue { get; set; }

    private List<string> RandomIncidents;

    public event System.Action<string> spawnedIncident;

    [Header("SFX")]
    public AudioEvent sfx_incidentOpen;
    public AudioEvent sfx_incidentClose;

    // Start is called before the first frame update
    void Awake()
    {
        ServiceLocator.RegisterAsService(this);
        _timeline = ServiceLocator.LazyLoad<Timeline>();
        ServiceLocator.TryGetService(out _audio);
        Dialogue.onDialogueComplete.AddListener(OnIncidentComplete);
    }

    private void Start()
    {
        List<string> nodes = new List<string>(Dialogue.lineProvider.YarnProject.NodeNames.ToList());
        foreach (string nodeName in new List<string>(nodes))
        {
            List<string> tags = Dialogue.GetTagsForNode(nodeName).ToList();
            if (tags.Count > 0)
            {
                if (tags[0] == "competition") // explicitly do not want competitions in our pool
                {
                    nodes.Remove(nodeName);
                    continue;
                }
                _timeline.AddTimelineEvent(this, () => SpawnIncident(nodeName), DateTime.Parse(tags.First()));
                nodes.Remove(nodeName);
            }
        }
        RandomIncidents = nodes;
        _timeline.AddTimelineEvent(this, PullRandomIncident, Timeline.FromNow(0, (int) FloatExtensions.RandomBetween(14, 22)));
    }

    private void PullRandomIncident()
    { 
        SpawnIncident(RandomIncidents[(int)FloatExtensions.RandomBetween(0, RandomIncidents.Count)]);
        _timeline.AddTimelineEvent(this, PullRandomIncident, Timeline.FromNow(0, (int) FloatExtensions.RandomBetween(14, 22)));
    }

    void SpawnIncident(string incYarnNode)
    {
        // TODO: We need to check for dependencies
        spawnedIncident?.Invoke(incYarnNode);
        Dialogue.StartDialogue(incYarnNode);
        _audio.PlayOneShot(sfx_incidentOpen);
        _timeline.SetGameSpeed(Timeline.GameSpeed.Paused);
    }

    void OnIncidentComplete()
    {
        _timeline.RestorePrevSpeed();
        _audio.PlayOneShot(sfx_incidentClose);
    }
}
