using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Unity.VisualScripting;
using UnityEngine;

public class Weapon : MonoBehaviour, IWeapon
{
    #region Variables

    protected PhotonView _photonView;

    #endregion Variables

    #region Unity Methods

    void Awake()
    {
        _photonView = GetComponent<PhotonView>();
    }

    #endregion Unity Methods

    #region Other Methods

    [PunRPC]
    void getItem(int playerID)
    {
        Debug.Log(pv.OwnerActorNr +" pID : " +playerID);
        if (pv.OwnerActorNr == playerID)
        {
            Debug.Log("11");
            transform.parent = GameObject.FindWithTag("EquipPoint").transform;
            gameObject.GetComponent<FloatingRotationgObject>().enabled = false;
            transform.localPosition = Vector3.zero;
            transform.localRotation = Quaternion.identity;
        }
    }

    public void onCollision()
    {
        // 소유권을 요청합니다.
        _photonView.RequestOwnership();

        // 소유권 요청 후, RPC를 호출하여 아이템을 얻습니다.
        _photonView.RPC("getItem", RpcTarget.AllBuffered, PhotonNetwork.LocalPlayer.ActorNumber);
    }

    #endregion Other Methods
}