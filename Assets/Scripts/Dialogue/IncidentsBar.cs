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

    private void Start()
    {
        var incManager = ServiceLocator.GetService<IncidentsManager>();

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
    }

    private void OnIncidentComplete(IncidentWidgetController inc)
    {
        foreach (var controller in _controllers)
        {
            controller.Show();
        }
        
        inc.started -= OnIncidentOpen;
        _controllers.Remove(inc);
    }
}
