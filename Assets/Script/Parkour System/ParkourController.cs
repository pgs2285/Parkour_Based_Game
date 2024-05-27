using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ParkourController : MonoBehaviour
{
    [SerializeField] List<ParkourAction> parkourActions;
    [SerializeField] ParkourAction jumpDownAction;
    
    EnvironmentScanner environmentScanner;
    Animator animator;
    PlayerController playerController;

    private void Awake()
    {
        environmentScanner = GetComponent<EnvironmentScanner>();
        animator = GetComponent<Animator>();
        playerController = GetComponent<PlayerController>();
    }

    private void Update()
    {
        var hitData = environmentScanner.ObstacleCheck();
        if (Input.GetButton("Jump") && !playerController.InAction)
        {
            
            if (hitData.forwardHitFound) // 만약 발견된게 있다면
            {
                foreach(ParkourAction action in parkourActions)
                {
                    if(action.CheckIfPossible(hitData, gameObject.transform)) // 가능한 범위안에 있나 체크
                    {
                        StartCoroutine(DoParkourAction(action));
                        break; 
                    }
                }

            }
        }

        if(playerController.IsOnLedge && !playerController.InAction && !hitData.forwardHitFound)
        {
            bool shouldJump = true;
            if (playerController.LedgeData.height > 1 && !Input.GetButton("Jump"))
                shouldJump = false;
            if(shouldJump && playerController.LedgeData.angle <= 50)
            {
                playerController.IsOnLedge = false;
                StartCoroutine(DoParkourAction(jumpDownAction));
            }

        }
    }
    IEnumerator DoParkourAction(ParkourAction action)
    {
        playerController.SetControl(false); //중력 및 콜리더 떄문에 계단을 올라가지 못하므로, 일단 이것을 비활성화 해주는 코드
        MatchTargetParams matchParams = null;
        if (action.EnableTargetMatching)
        {
            matchParams = new MatchTargetParams()
            {
                pos = action.MatchPos,
                bodyPart = action.MatchBodyPart,
                posWeight = action.MatchPosWeight,
                startTime = action.MatchStartTime,
                targetTime = action.MatchTargetTime
            };
        }

        yield return playerController.DoAction(action.AnimName, matchParams, action.TargetRotation, action.RotateToObstacle,
            action.PostActionDelay, action.Mirror);
        playerController.SetControl(true); // 중력 및 collision활성화
    }

    void MatchTarget(ParkourAction action)
    {
        if (animator.isMatchingTarget) return;

        animator.MatchTarget(action.MatchPos, transform.rotation, action.MatchBodyPart,
            new MatchTargetWeightMask(action.MatchPosWeight, 0),// vector의 xyz중 1인것만 위치에 match시킨다. rotation은 match안시킬거니 0
            action.MatchStartTime, action.MatchTargetTime);
    }
}
