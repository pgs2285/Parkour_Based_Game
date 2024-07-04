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
        // �������� ��û�մϴ�.
        _photonView.RequestOwnership();

        // ������ ��û ��, RPC�� ȣ���Ͽ� �������� ����ϴ�.
        _photonView.RPC("getItem", RpcTarget.AllBuffered, PhotonNetwork.LocalPlayer.ActorNumber);
    }

    #endregion Other Methods
}