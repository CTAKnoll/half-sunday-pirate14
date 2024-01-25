using Services;
using System.Collections;
using System.Collections.Generic;
using UI.Mission;
using UnityEngine;

public class IncidentsBar : MonoBehaviour
{
    [SerializeField]
    private IncidentWidgetView _incidentWidgetPrefab;
    private List<IncidentWidgetController> _controllers = new();
    private IncidentsManager incManager;
    private AudioService _audio;

    public AudioEvent sfx_openIncident;
    public AudioEvent sfx_closeIncident;


    private void Start()
    {
        ServiceLocator.TryGetService(out incManager);
        ServiceLocator.TryGetService(out _audio);

        incManager.spawnedIncident += OnSpawnIncident;
    }

    private void OnSpawnIncident(string node)
    {
        IncidentWidgetView view = Instantiate(_incidentWidgetPrefab, transform);
        var controller = new IncidentWidgetController(view, node);
        _controllers.Add(controller);

        controller.started += OnIncidentOpen;
        controller.completed += OnIncidentComplete;
    }

    private void OnIncidentOpen()
    {
        foreach(var controller in _controllers) 
        {
            controller.Hide();
        }
        _audio.PlayOneShot(sfx_openIncident);
        gameObject.SetActive(false);
    }

    private void OnIncidentComplete(IncidentWidgetController inc)
    {
        gameObject.SetActive(true);
        _audio.PlayOneShot(sfx_closeIncident);
        foreach (var controller in _controllers)
        {
            controller.Show();
        }
        inc.started -= OnIncidentOpen;
        _controllers.Remove(inc);
    }
}
