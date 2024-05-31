using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class ClimbController : MonoBehaviour
{
    EnvironmentScanner envScanner;
    PlayerController playerController;

    ClimbPoint currentPoint;
    public void Awake()
    {
        envScanner = GetComponent<EnvironmentScanner>();        
        playerController = GetComponent<PlayerController>();
    }

    private void Update()
    {

        if (!playerController.IsHanging) // Idle To Hang
        {

            if (Input.GetButton("Jump") && !playerController.InAction)
            {
                if (envScanner.ClimbLedgeCheck(transform.forward, out RaycastHit ledgeHit))
                {
                    currentPoint = ledgeHit.transform.GetComponent<ClimbPoint>();
                    playerController.SetControl(false);
                    StartCoroutine(JumpToLedge("IdleToHang", ledgeHit.transform, 0.41f, 0.54f));
                }
            }
        }
        else // Ledge to Ledge
        {
            float h= Mathf.Round(Input.GetAxisRaw("Horizontal"));
            float v= Mathf.Round(Input.GetAxisRaw("Vertical"));

            Vector2 inputDir = new Vector2(h,v);

            if(playerController.InAction || inputDir == Vector2.zero) return;

            var neighbour = currentPoint.GetNeighbour(inputDir);
            if(neighbour == null) return;
            
            if(neighbour.connectionType == ConnectionType.Jump && Input.GetButton("Jump"))  // 다른곳으로 이동가능한 블력일떄
            {
                currentPoint = neighbour.point;
                if(neighbour.direction.y == 1)
                    StartCoroutine(JumpToLedge("HangHopUp", currentPoint.transform, 0.35f, 0.65f));
                else if(neighbour.direction.y == -1)
                    StartCoroutine(JumpToLedge("HangHopDown", currentPoint.transform, 0.31f, 0.65f));
                else if(neighbour.direction.x == 1)
                    StartCoroutine(JumpToLedge("HangHopRight", currentPoint.transform, 0.20f, 0.50f));
                else if(neighbour.direction.x == -1)
                    StartCoroutine(JumpToLedge("HangHopLeft", currentPoint.transform, 0.20f, 0.50f));
            }else if(neighbour.connectionType == ConnectionType.Move)
            {
                currentPoint = neighbour.point;
                if (neighbour.direction.x == 1)
                    StartCoroutine(JumpToLedge("ShimmyRight", currentPoint.transform, 0f, 0.38f, handOffset : new Vector3(0.25f, 0.05f, 0.1f)));
                else if (neighbour.direction.x == -1)
                    StartCoroutine(JumpToLedge("ShimmyLeft", currentPoint.transform, 0.0f, 0.38f, AvatarTarget.LeftHand, handOffset: new Vector3(0.25f, 0.05f, 0.1f)));
            }
        }
    }

    IEnumerator JumpToLedge(string anim, Transform ledge, float matchStartTime, float matchTargetTime,
        AvatarTarget hand = AvatarTarget.RightHand,
        Vector3? handOffset = null) // 변수뒤의 물음표는 null이 가능하게
    {
        MatchTargetParams matchParams = new MatchTargetParams()
        {
            pos = GetHandPos(ledge, hand, handOffset),
            bodyPart = hand,
            startTime = matchStartTime,
            targetTime = matchTargetTime,
            posWeight = Vector3.one
        };
        Quaternion targetRotation = Quaternion.LookRotation(-ledge.forward);
        yield return playerController.DoAction(anim, matchParams, targetRotation, true);

        playerController.IsHanging = true;
    }

    Vector3 GetHandPos(Transform ledge, AvatarTarget hand, Vector3? handOFfset)
    {
        Vector3 offVal = (handOFfset != null) ? handOFfset.Value : new Vector3(0.25f, 0.1f, 0.1f);
        

        

        var hDir = hand == AvatarTarget.RightHand ? ledge.right : -ledge.right;
        return ledge.position + ledge.forward * offVal.z + Vector3.up * offVal.y - hDir * offVal.x;
    }
}
