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
            if (hitData.forwardHitFound)
            {
                foreach(ParkourAction action in parkourActions)
                {
                    if(action.CheckIfPossible(hitData, gameObject.transform))
                    {
                        StartCoroutine(DoParkourAction(action));
                        Debug.Log("Found Parkour" + action.AnimName);
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
        yield return new WaitForSeconds(animState.length);
        playerController.SetControl(true);
        inAction = false;
    }
}
