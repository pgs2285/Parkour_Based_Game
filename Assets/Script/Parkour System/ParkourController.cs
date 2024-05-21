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
            }
            yield return null;
        }

        playerController.SetControl(true); // 중력 및 collision활성화
        inAction = false;
    }
}
