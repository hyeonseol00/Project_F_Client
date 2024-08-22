using Google.Protobuf.Protocol;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Enemy : MonoBehaviour
{
	public float speed;

	[SerializeField] Animator animator;
	[SerializeField] UIMonsterInformation monsterUI;
	[SerializeField] RectTransform winPopup;
    [SerializeField] TextMeshProUGUI winMessage;
    [SerializeField] RectTransform btns;

    Vector3 unitVector;
	bool isDead = false;

	private void Update()
	{
		Move();
	}

	private void OnTriggerEnter(Collider other)
	{
		if (other.tag == "Weapon" || other.tag == "Bullet")
		{
			// Attack attack = other.GetComponent<Attack>();
			// curHealth -= attack.damage;
			// monsterUI.SetCurHP(curHealth);

			C_AttackBoss attackBossPacket = new C_AttackBoss { };

			GameManager.Network.Send(attackBossPacket);

			// animator.SetBool("Hit", true);
		}
	}

	public void HitAnimation()
	{
		animator.SetBool("Hit", true);
	}

	public void SetCoordinates(Vector3 move, Vector3 eRot)
	{
		if (isDead)
			return;

		var pos = move;
		pos.y = transform.position.y;
		transform.position = pos;
		transform.rotation = Quaternion.Euler(eRot);
	}

	public void SetUnitVector(Vector3 vec)
	{
		unitVector = vec;
	}

	private void Move()
	{
		if (isDead)
			return;

		var lastPos = transform.position;
		transform.position += unitVector * Time.deltaTime * speed;

		animator.SetFloat("Move", Vector3.Distance(lastPos, transform.position));

		if (transform.position.y <= -30.0f)
		{
			var pos = new Vector3(transform.position.x, -19.5f, transform.position.z);
			transform.position = pos;
		}

	}

	public void Dead(S_KillBoss pkt)
	{
		isDead = true;
		animator.SetBool("Die", true);

		winMessage.text = pkt.Message;

		var btnComponents = btns.GetComponentsInChildren<Button>();
		for (int i = 0; i < btnComponents.Length; i++) 
		{
			var btnText = btnComponents[i].GetComponentInChildren<TextMeshProUGUI>();
			btnText.text = pkt.BtnTexts[i];

			if (HatcheryManager.Instance.myPlayer.PlayerId == pkt.PlayerId)
                btnComponents[i].interactable = true;
            else
				btnComponents[i].interactable = false;
        }

        winPopup.gameObject.SetActive(true);
	}

	public void ConfirmReward(S_HatcheryConfirmReward pkt)
    {
		StartCoroutine(SetConfirmUI(pkt));
    }

	public IEnumerator SetConfirmUI(S_HatcheryConfirmReward pkt)
    {
        winMessage.text = pkt.Message;

        var btnComponents = btns.GetComponentsInChildren<Button>();
        for (int i = 0; i < btnComponents.Length; i++)
        {
            var btnText = btnComponents[i].GetComponentInChildren<TextMeshProUGUI>();
            btnText.text = pkt.BtnTexts[i];
        }

        var btnColors = btnComponents[pkt.SelectedBtn].colors;
        btnColors.disabledColor = UnityEngine.Color.green;
        btnComponents[pkt.SelectedBtn].colors = btnColors;

        yield return new WaitForSeconds(5.0f);

		HatcheryManager.Instance.LeaveHatchery();
    }

	public void OffBtnsInteractable()
    {
        var btnComponents = btns.GetComponentsInChildren<Button>();
        for (int i = 0; i < btnComponents.Length; i++)
            btnComponents[i].interactable = false;
    }
}
