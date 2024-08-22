using Google.Protobuf.Protocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThirdPersonController : MonoBehaviour
{
    [SerializeField] private Transform characterBody;
    [SerializeField] private Transform cameraArm;
    [SerializeField] private Camera _camera3pov;
    [SerializeField] private Camera _camera1pov;
    [SerializeField] private SkillDescription _skillDescription;

    public float speed = 5.0f;
    public float sensitivity = 5f;
    public float zoomSpeed = 20f; // 줌 속도
    public float minFov = 15f; // 최소 FOV (최대 줌인)
    public float maxFov = 90f; // 최대 FOV (최대 줌아웃)

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

        if (!HatcheryManager.Instance.myPlayer.isDead && HatcheryManager.Instance.myPlayer.canMove)
            Move();

        if (Input.GetKeyDown(KeyCode.F4)) 
            changePerspective();

        if (Input.GetKeyDown(KeyCode.LeftBracket))
            sensitivity -= sensitivity >= 0.5f ? 0.5f : 0;

        if (Input.GetKeyDown(KeyCode.RightBracket))
            sensitivity += 0.5f;

        if (Input.GetKeyDown(KeyCode.Mouse1)){
            _skillDescription.useSkill();
        }
            
    }

    private void LookAround()
    {
        // 마우스 이동으로 카메라 회전
        Vector2 mouseDelta = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
        Vector3 camAngle = cameraArm.rotation.eulerAngles;

        float x = camAngle.x - mouseDelta.y * sensitivity;
        float y = camAngle.y + mouseDelta.x * sensitivity;

        if (x < 180.0f)
        {
            x = Mathf.Clamp(x, -1.0f, 70.0f);
        }
        else
        {
            x = Mathf.Clamp(x, 335.0f, 361.0f);
        }

        cameraArm.rotation = Quaternion.Euler(x, y, camAngle.z);

        // 줌인 줌아웃(FOV 조절방식)
        float scrollInput = Input.GetAxis("Mouse ScrollWheel");
        float fov = _camera3pov.fieldOfView;

        fov -= scrollInput * zoomSpeed;
        fov = Mathf.Clamp(fov, minFov, maxFov); // 최소 및 최대값으로 제한

        _camera3pov.fieldOfView = fov;
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

    void changePerspective()
    {
        if(_camera3pov.gameObject.activeSelf)
        {
            _camera3pov.gameObject.SetActive(false);
            _camera1pov.gameObject.SetActive(true);
        }
        else
        {
            _camera3pov.gameObject.SetActive(true);
            _camera1pov.gameObject.SetActive(false);
        }
    }
}