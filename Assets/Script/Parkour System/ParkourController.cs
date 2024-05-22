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
            if (hitData.forwardHitFound) // ���� �߰ߵȰ� �ִٸ�
            {
                foreach(ParkourAction action in parkourActions)
                {
                    if(action.CheckIfPossible(hitData, gameObject.transform)) // ������ �����ȿ� �ֳ� üũ
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

        playerController.SetControl(false); //�߷� �� �ݸ��� ������ ����� �ö��� ���ϹǷ�, �ϴ� �̰��� ��Ȱ��ȭ ���ִ� �ڵ�

        animator.CrossFade(action.AnimName, 0.2f); // cross fade�� �ִϸ��̼��� �ް��ϰ� �ٲ�� ������� �ʰ� ���� �Լ��� ���� �ڿ����� �������, �ι��� �μ��� fade out �ð�
        yield return null; //  �� �������� �ѱ����ν� ��ȯ

        var animState = animator.GetNextAnimatorStateInfo(0); // 0�� ���̾��� ��ȯ������ ������.
        if(animState.IsName(action.AnimName))
        {
            Debug.LogError("�ִϸ��̼��� �������� �ʴ´�.");
        }

        float timer = 0f;
        while(timer <= animState.length)
        { // �ִϸ��̼ǵ���
            timer += Time.deltaTime;
            if(action.RotateToObstacle)
            {
                transform.rotation = Quaternion.RotateTowards(transform.rotation, action.TargetRotation, playerController.RotationSpeed * Time.deltaTime);
            }//�ִϸ��̼� ���ൿ�� ������Ʈ

            if (action.EnableTargetMatching)
                MatchTarget(action);

            if(animator.IsInTransition(0) && timer> 1.0f) // Vault�������� �پ�Ѱ� ���߿� �����ϴ� ��ȯ���϶� �߷��� �������� ����. ���� ��ȯ���� break�ϸ� �ȵǴ� 0.5���� �������� ��������
                break;

            yield return null;
        }

        yield return new WaitForSeconds(action.PostActionDelay); // �ִϸ��̼��� 2���� ����� ��� ��Ʈ�ѷ� �ѱ������ �� ������

        playerController.SetControl(true); // �߷� �� collisionȰ��ȭ
        inAction = false;
    }

    void MatchTarget(ParkourAction action)
    {
        if (animator.isMatchingTarget) return;

        animator.MatchTarget(action.MatchPos, transform.rotation, action.MatchBodyPart,
            new MatchTargetWeightMask(action.MatchPosWeight, 0),// vector�� xyz�� 1�ΰ͸� ��ġ�� match��Ų��. rotation�� match�Ƚ�ų�Ŵ� 0
            action.MatchStartTime, action.MatchTargetTime);
    }
}
