using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
public class NetworkManager : MonoBehaviourPunCallbacks
{   
    public int screenWidth = 1920;
    public int screenHeight = 1080;
    void Awake()
    {
        Screen.SetResolution(screenHeight,screenWidth, false);
        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster() =>
        PhotonNetwork.JoinOrCreateRoom("Room", new RoomOptions { MaxPlayers = 4 }, null);

    public override void OnJoinedRoom()
    {
        
        GameObject target = PhotonNetwork.Instantiate("Player", new Vector3(0, 3.5f, 0), Quaternion.identity);
        Camera.main.GetComponent<CameraController>().followTarget = target.transform;
    }
}
