using UnityEngine;
using Photon.Pun;
using Unity.VisualScripting;

public class ItemPickup : MonoBehaviourPunCallbacks
{
    private bool isPicked = false;
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && other.GetComponent<CharacterController>() != null && !isPicked)
        {
            PhotonView playerPhotonView = other.GetComponent<PhotonView>();
            PhotonView itemPhotonView = GetComponent<PhotonView>();

            if (playerPhotonView != null && itemPhotonView != null && playerPhotonView.IsMine)
            {
                Transform attachPoint = FindAttachPoint(other.transform, "EquipPoint");
                if (attachPoint != null)
                {
                    // 아이템 객체의 RPC 메서드를 호출합니다.
                    itemPhotonView.RPC("PickupItem", RpcTarget.AllBuffered, playerPhotonView.ViewID, itemPhotonView.ViewID); 
                    //Debug.Log(other.GetComponent<Weapon>());
                    this.GetComponent<Weapon>().ownerID = playerPhotonView.ViewID;
                }
            }
            
            other.GetComponent<PlayerController>().Weapon = this.gameObject;
            this.gameObject.GetComponent<Collider>().enabled = false;

        }
    }

    Transform FindAttachPoint(Transform parent, string attachPointName)
    {
        foreach (Transform child in parent)
        {
            if (child.name == attachPointName)
            {
                return child;
            }
            Transform result = FindAttachPoint(child, attachPointName);
            if (result != null)
            {
                return result;
            }
        }
        return null;
    }

    [PunRPC]
    void PickupItem(int playerViewID, int itemViewID)
    {
        PhotonView playerPhotonView = PhotonView.Find(playerViewID);
        PhotonView itemPhotonView = PhotonView.Find(itemViewID);

        if (playerPhotonView != null && itemPhotonView != null)
        {
            Transform attachPoint = FindAttachPoint(playerPhotonView.transform, "EquipPoint");
            if (attachPoint != null)
            {           isPicked = true;
                itemPhotonView.transform.SetParent(attachPoint);
                itemPhotonView.transform.localPosition = Vector3.zero;
                itemPhotonView.transform.localRotation = Quaternion.identity;
            }
        }
        

        this.enabled = false;
    }
}