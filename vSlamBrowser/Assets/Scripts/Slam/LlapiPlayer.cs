using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Dissonance;
using System;

public class LlapiPlayer : MonoBehaviour, IDissonancePlayer
{
    public NetworkPlayerType networkPlayerType = NetworkPlayerType.Remote;
    public bool _isTracking = false;
    public bool IsTracking
    {
        get
        {
            return _isTracking;
        }
    }
    DissonanceComms _comms = null;

    public string _playerId;

    // This property implements the PlayerId part of the interface
    public string PlayerId { get { return _playerId; } }


    public Vector3 Position
    {
        get
        {
            if (transform != null)
            {
                return transform.position;
            }
            return Vector3.zero;
        }
    }

    public Quaternion Rotation
    {
        get
        {
            return transform.rotation;
        }
    }

    public NetworkPlayerType Type
    {
        get
        {
            return networkPlayerType;
        }
        set
        {
            networkPlayerType = value;
        }
    }
    public void SetPlayerId(string dissId)
    {
        _comms = FindObjectOfType<DissonanceComms>();
        if (_comms != null )
        {
            _playerId = dissId;
            _comms.TrackPlayerPosition(this);
           // _comms.
            _isTracking = true;
        }

    }
    private void OnDestroy()
    {
        if (_comms != null)
        {
            _comms.StopTracking(this);
        }
   }
    // Use this for initialization
    void Start () {
    }

    // Update is called once per frame
    void Update () {
		
	}
}
