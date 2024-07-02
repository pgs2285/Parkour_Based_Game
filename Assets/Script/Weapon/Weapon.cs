using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
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
    void getAndDestroy()
    {
        Destroy(gameObject);
    }
    public void onCollision()
    {
        _photonView.RPC("getAndDestroy", RpcTarget.AllBuffered);
    }
    #endregion Other Methods


}
