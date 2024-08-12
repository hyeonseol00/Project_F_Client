using Google.Protobuf.Protocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThirdPersonController : MonoBehaviour
{
    float speed = 5.0f;

    [SerializeField] private Transform characterBody;
    [SerializeField] private Transform cameraArm;

    Animator animator;

    // Start is called before the first frame update
    void Start()
    {
        animator = characterBody.GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        LookAround();

        if (!HatcheryManager.Instance.myPlayer.isDead)
            Move();
    }

    private void LookAround()
    {
        Vector2 mouseDelta = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
        Vector3 camAngle = cameraArm.rotation.eulerAngles;

        float x = camAngle.x - mouseDelta.y;

        if (x < 180.0f)
        {
            x = Mathf.Clamp(x, -1.0f, 70.0f);
        }
        else
        {
            x = Mathf.Clamp(x, 335.0f, 361.0f);
        }

        cameraArm.rotation = Quaternion.Euler(x, camAngle.y + mouseDelta.x, camAngle.z);
    }

	private void Move()
	{
        Vector2 moveInput = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")).normalized;

		animator.SetFloat("Move", moveInput.magnitude);

		bool isMove = moveInput.magnitude != 0;
		if (isMove)
        {
            Vector3 lookForward = new Vector3(cameraArm.forward.x, 0.0f, cameraArm.forward.z).normalized;
            Vector3 lookRight = new Vector3(cameraArm.right.x, 0.0f, cameraArm.right.z).normalized;
            Vector3 moveDir = lookForward * moveInput.y + lookRight * moveInput.x;

            characterBody.forward = lookForward;
            transform.position += moveDir * Time.deltaTime * speed;

            var pos = transform.position;
            var rot = characterBody.transform.rotation.eulerAngles;
			TransformInfo tr = new TransformInfo { PosX = pos.x, PosY = pos.y, PosZ = pos.z, Rot = rot.y };
			C_MoveAtHatchery response = new C_MoveAtHatchery { Transform = tr };
			GameManager.Network.Send(response);
		}
	}
}