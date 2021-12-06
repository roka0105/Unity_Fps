using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
	protected BulletManager M_Bullet => BulletManager.Instance;

	//public delegate void OnCrushBlock(float dmg);
	//public event OnCrushBlock OnCrush;
	E_BulletType m_type;
	int m_TotalBullet;
	float m_AtkRange;//��Ÿ�
	float m_Atk;
	float m_AtkSpeed;
	float m_CoolTime;//���� ��Ÿ��.
	Vector3 startpos;//���� �� ������ ��ġ.
	Vector3 dir;
	Bullet m_this;
	
	public Vector3 SetStartPos
	{
		set { startpos = value; }
	}
	public Vector3 SetDir
	{
		set
		{
			dir = value;
		}
	}
	public Transform GetBullet
	{
		get { return this.gameObject.transform; }
	}
	public void Init(GunData gundata)
	{
		m_type = gundata.m_type;
		m_TotalBullet = gundata.m_TotalBullet;
		m_AtkRange = gundata.m_AtkRange;
		m_Atk = gundata.m_Atk;
		m_AtkSpeed = gundata.m_AtkSpeed;
		m_CoolTime = gundata.m_CoolTime;
		m_this = gameObject.GetComponent<Bullet>();
	}

	private void Move()
	{
		float distancex = Mathf.Abs(startpos.x - transform.position.x);
		float distancez = Mathf.Abs(startpos.z - transform.position.z);

		//��Ÿ� ����ų� ������ �ε����� despawn
		if (distancex>=m_AtkRange||distancez>=m_AtkRange)
		{
			M_Bullet.DeSpawn((int)m_type,m_this);
		}
		Vector3 temp = dir*m_AtkSpeed*Time.deltaTime;
		this.transform.position += temp;	
	}

	private void OnCollisionEnter(Collision collision)
	{
		int layernumber = collision.gameObject.layer;
		switch(layernumber)
		{
			case (int)E_Layer.Block:
				M_Bullet.DeSpawn((int)m_type, m_this);
				break;
			case (int)E_Layer.CrushBlock:
				//������ �ֱ�
				M_Bullet.DeSpawn((int)m_type, m_this);
				collision.gameObject.GetComponent<Dmginterface>().Dmg(m_Atk);
				//OnCrush(m_Atk);
				break;
			case (int)E_Layer.PlayerBullet:
				M_Bullet.DeSpawn((int)m_type, m_this);
				break;
		}
	}
	private void Update()
	{
		Move();
	}

}
