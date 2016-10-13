using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class IncidentInputLogic
{
    public Incident DetermineIncident(string _playerInput, List<Incident> _incidents)
    {
        //Loops through the list of video cips to find a match using the videoName.
        for (int index = 0; index < _incidents.Count; index++)
        {
            if (_incidents[index].ID == _playerInput)
            {
                return _incidents[index];
            }
        }
        //Currently returns a null if it cant find anything.
        return null;
    }
}
