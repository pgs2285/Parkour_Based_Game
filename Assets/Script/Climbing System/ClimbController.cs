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
            }
        }
    }

    IEnumerator JumpToLedge(string anim, Transform ledge, float matchStartTime, float matchTargetTime)
    {
        MatchTargetParams matchParams = new MatchTargetParams()
        {
            pos = GetHandPos(ledge),
            bodyPart = AvatarTarget.RightHand,
            startTime = matchStartTime,
            targetTime = matchTargetTime,
            posWeight = Vector3.one
        };
        Quaternion targetRotation = Quaternion.LookRotation(-ledge.forward);
        yield return playerController.DoAction(anim, matchParams, targetRotation, true);

        playerController.IsHanging = true;
    }

    Vector3 GetHandPos(Transform ledge)
    {
        return ledge.position + ledge.forward * 0.1f + Vector3.up * 0.1f - ledge.right * 0.25f;
    }
}
