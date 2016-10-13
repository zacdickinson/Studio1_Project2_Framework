using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ResponseInputLogic
{
    public Response DetermineResponse(string _playerInput, List<Response> _responses)
    {
        //Loops through the list of video cips to find a match using the videoName.
        for (int index = 0; index < _responses.Count; index++)
        {
            if (_responses[index].ID == _playerInput)
            {
                return _responses[index];
            }
        }
        return null;
    }
}
