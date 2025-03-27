using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Woman : MonoBehaviour
{
    public float moveSpeed;
    public float turnSpeed;

    private Rigidbody rb;
    private Animator animator;

    public Transform gunPivot;
    public Transform leftHandMount;
    public Transform rightHandMount;
    public Transform firePoint;
    public LineRenderer lineRenderer;
    public Slider slider;
    public int maxHp = 100;
    private int hp;

    public float leftHandPositionWeight = 1f;
    public float leftHandRotationWeight = 1f;
    public float rightHandPositionWeight = 1f;
    public float rightHandRotationWeight = 1f;

    public bool useIK = false;

    private float gunRange = 4f;
    private float lastFireTime;
    private float fireDelay = 0.2f;
    [HideInInspector]
    public bool isDie = false;

    void Start()
    {
        rb = GetComponent<Rigidbody>();    
        animator = GetComponent<Animator>();

        lineRenderer.positionCount = 2;
        lineRenderer.enabled = false;

        this.hp = this.maxHp;
    }

    
    void Update()
    {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");
        Vector3 dir = new Vector3(h, 0, v);

        Vector3 movement = dir.z * transform.forward * moveSpeed * Time.deltaTime;

        rb.MovePosition(transform.position + movement);

        animator.SetFloat("Move", dir.z);

        float turn = dir.x * turnSpeed * Time.deltaTime;

        transform.rotation *= Quaternion.Euler(0, turn, 0);

        if (Input.GetMouseButton(0))
        {
            if (Time.time > lastFireTime + fireDelay)
            {
                lastFireTime = Time.time;

                StartCoroutine(this.Shoot());
            }
        }
    }

    private IEnumerator Shoot()
    {
        Vector3 startPos = firePoint.position;
        Vector3 endPos = startPos + firePoint.forward * gunRange;

        //�浹 ó�� 
        var dir = endPos - startPos;
        RaycastHit hit;
        if (Physics.Raycast(startPos, dir.normalized, out hit, gunRange))
        {
            endPos = hit.point;
        }


        lineRenderer.SetPosition(0, startPos);
        lineRenderer.SetPosition(1, endPos);
        lineRenderer.enabled = true;

        yield return new WaitForSeconds(0.1f);

        lineRenderer.enabled = false;
    }


    private void OnAnimatorIK(int layerIndex)
    {
        if (!useIK) return;

        //gunPivot��ġ�� ĳ������ ������ �Ȳ�ġ ��Ʈ ��ġ�� ���� �Ѵ� 
        gunPivot.position = animator.GetIKHintPosition(AvatarIKHint.RightElbow);

        //�޼� ��ġ �� ȸ���� ���� ����ġ ���� 
        animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, leftHandPositionWeight);
        animator.SetIKRotationWeight(AvatarIKGoal.LeftHand, leftHandRotationWeight);

        //�޼��� ��ǥ ��ġ �� ȸ�� ���� 
        animator.SetIKPosition(AvatarIKGoal.LeftHand, leftHandMount.position);
        animator.SetIKRotation(AvatarIKGoal.LeftHand, leftHandMount.rotation);

        //������ ��ġ �� ȸ���� ���� ����ġ ���� 
        animator.SetIKPositionWeight(AvatarIKGoal.RightHand, rightHandPositionWeight);
        animator.SetIKRotationWeight(AvatarIKGoal.RightHand, rightHandRotationWeight);

        //�������� ��ǥ ��ġ �� ȸ�� ���� 
        animator.SetIKPosition(AvatarIKGoal.RightHand, rightHandMount.position);
        animator.SetIKRotation(AvatarIKGoal.RightHand, rightHandMount.rotation);
    }

    public void OnDamage(int damage)
    {
        if (isDie) return;

        Debug.Log($"damage: {damage}");
        this.hp -= damage;

        this.slider.value = (float)this.hp / this.maxHp * 100f;


        if (this.hp <= 0) {
            Debug.Log("Die");
            slider.gameObject.SetActive(false);
            isDie = true;
            animator.SetTrigger("Die");
        }
    }
}
