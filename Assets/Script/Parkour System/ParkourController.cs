using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ParkourController : MonoBehaviour
{
    [SerializeField] List<ParkourAction> parkourActions;

    bool inAction;
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
        if(Input.GetButton("Jump") && !inAction)
        {
            var hitData = environmentScanner.ObstacleCheck();
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
    }
    IEnumerator DoParkourAction(ParkourAction action)
    {
        inAction = true;

        playerController.SetControl(false); //중력 및 콜리더 떄문에 계단을 올라가지 못하므로, 일단 이것을 비활성화 해주는 코드

        animator.CrossFade(action.AnimName, 0.2f); // cross fade는 애니메이션이 급격하게 바뀌면 어색하지 않게 블렌딩 함수를 통해 자연스래 만들어줌, 두번쨰 인수는 fade out 시간
        yield return null; //  한 프레임을 넘김으로써 전환

        var animState = animator.GetNextAnimatorStateInfo(0); // 0번 레이어의 전환정보를 가져옴.
        if(animState.IsName(action.AnimName))
        {
            Debug.LogError("애니메이션이 존재하지 않는다.");
        }

        float timer = 0f;
        while(timer <= animState.length)
        { // 애니메이션동안
            timer += Time.deltaTime;
            if(action.RotateToObstacle)
            {
                transform.rotation = Quaternion.RotateTowards(transform.rotation, action.TargetRotation, playerController.RotationSpeed * Time.deltaTime);
            }//애니메이션 진행동안 업데이트

            if (action.EnableTargetMatching)
                MatchTarget(action);

            if(animator.IsInTransition(0) && timer> 1.0f) // Vault같은것은 뛰어넘고 공중에 착지하니 전환중일때 중력을 돌려놓기 위함. 시작 전환때는 break하면 안되니 0.5같은 작은값을 조건으로
                break;

            yield return null;
        }

        yield return new WaitForSeconds(action.PostActionDelay); // 애니메이션이 2개가 연결된 경우 컨트롤러 넘기기전에 더 딜레이

        playerController.SetControl(true); // 중력 및 collision활성화
        inAction = false;
    }

    void MatchTarget(ParkourAction action)
    {
        if (animator.isMatchingTarget) return;

        animator.MatchTarget(action.MatchPos, transform.rotation, action.MatchBodyPart,
            new MatchTargetWeightMask(action.MatchPosWeight, 0),// vector의 xyz중 1인것만 위치에 match시킨다. rotation은 match안시킬거니 0
            action.MatchStartTime, action.MatchTargetTime);
    }
}
