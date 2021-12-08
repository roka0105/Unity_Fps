using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
public enum E_Layer
{
	None=-1,
	Minimap=6,
	Block,
	Player,
	CrushBlock,
	PlayerBullet,
	Max

}
#region ������ ����ü ���� *���߿� �ش��ϴ� Ŭ���� ���� ��ġ�ϱ�*
public struct StageData
{
	public BlockData m_BlockData;
	public MapData m_MapData;
	public PlayerData m_PlayerData;
	public EnemyData m_EnemyData;
}
public struct GunData
{
	public E_BulletType m_type;//enum���� ���߿� �ٲٱ� �ӽ÷� int
	public int m_TotalBullet;
	public float m_AtkRange;//��Ÿ�
	public float m_Atk;
	public float m_AtkSpeed;
	public float m_CoolTime;//���� ��Ÿ��.
}
public struct EnemyData
{
	public int m_TotalEnemy;
	public int type;//�̰͵� enum���� ���߿� ���� �������� ������ ���.
	public float m_Range;//�ν� ��Ÿ�
	public float m_AtkRange;
	public float m_Hp;
	public float m_Speed;
	public float m_Atk;
	public float m_AtkSpeed;
	public SkillData m_Skill;
}
public struct PlayerData
{
	public float m_Hp;
	public float m_Speed;
	public float m_AtkRange;//�⺻�����̶� ������ 1�� ��.
	public float m_Atk;
	public float m_AtkSpeed;
	public SkillData m_Skill;
}
public struct MapData
{
	public Vector3 m_Startpos;
	public Vector3 m_Endpos;
	public List<Vector3> m_CantRespawnPos;
}
public struct BlockData
{
	public float m_Hp;//�μ����� ��
	public float m_Speed;//�����̴� ��
}
public struct SkillData
{
	public float m_SkillCoolTime;
	public float m_SkillDmg;
	public float m_SkillRange;
	public float m_MaxChargeSkill;
	public float m_SkillDelay;//���ӻ��Ǵ� ��ų ������.
}
#endregion
public class _GameManager : Singleton<_GameManager>
{
	public NavMeshSurface surface;
	MapManager M_Map => MapManager.Instance;
	PlayerManager M_Player => PlayerManager.Instance;
	CameraManager M_Camera => CameraManager.Instance;
	BulletManager M_Bullet => BulletManager.Instance;

	MonsterManager M_Monster => MonsterManager.Instance;
	#region �� �Ŵ����� �� ����
	[SerializeField]
	Vector3 m_Startpos;
	[SerializeField]
	Vector3 m_Endpos;
	[SerializeField]
	Vector3 m_RespawnPos;

	public bool testflag;
	public GameObject Colliderparent;
	int nowstage;

	List<StageData> m_StageDataList;
	public GameObject ColliderParent
	{
		get { return Colliderparent; }
	}
	public Vector3 GetStartPos
	{
		get { return m_Startpos; }
	}
	public Vector3 GetEndPos
	{
		get { return m_Endpos; }
	}
	#endregion
	#region �� �Ŵ����� �� ����
	[SerializeField]
	private float InitBlockHP;
	[SerializeField]
	private int InitBlockROW;
	[SerializeField]
	private int InitBlockCOL;
	[SerializeField]
	private Block BlockPrefeb;
	#region ������Ƽ
	public int Stageindex
	{
		get { return nowstage; }
		set { nowstage = value; }
	}
	public Block BlockOriginal
	{
		get => BlockPrefeb;
	}
	public float GetBlockHP
	{
		get => InitBlockHP;
	}
	public int GetBlockRow
	{
		get => InitBlockROW;
	}
	public int GetBlockCol
	{
		get => InitBlockROW;
	}
	#endregion
	#endregion
	#region �ѿ� ���� ��������
	public int m_GunTypeCount;
	public List<GameObject> GunPrefeblist;
	public List<Bullet> BulletPrefeblist;

	List<GunData> m_GunDataList;
	#endregion
	#region ����
	public Monster m_MonsterPrefeb;
	public Monster MonsterPrefeb
	{
		get { return m_MonsterPrefeb; }
	}
	#endregion
	//�������� ����Ʈ���� �������� ���� ���� �о�ͼ� ���� ���� ���·� �����ϱ�.
	void LoadStageData()
	{
		LoadMapdata();
	}
	void LoadMapdata()
	{
		if (!testflag)
		{
			m_Startpos = m_StageDataList[nowstage].m_MapData.m_Startpos;
			m_Endpos = m_StageDataList[nowstage].m_MapData.m_Endpos;
		}
	}
	[ContextMenu("�� ����")]
	void CreateMap()
	{
		if (m_StageDataList.Count <= 0)
		{
			Debug.Log("stagedata list null");
			return;
		}
		LoadStageData();
		M_Map.CreateFixBlock();
		//Update NavMesh
		surface.BuildNavMesh();
		while(true)
		{
		
			RandomPlayerPos();	
			
			GameObject player = M_Player.GetObjPlayer;
		
			if (!RaycastSpawn(player))
			{
				break;
			}
		}
		for(int i=0;i< m_StageDataList[nowstage].m_EnemyData.m_TotalEnemy;++i)
		{
			while (true)
			{
				Monster item = RandomEnemyPos();
				if (!RaycastSpawn(item.gameObject))
				{
					break;
				}
			}
		}
	}
	[ContextMenu("�� ����")]
	void DeleteMap()
	{
		//blocklist type 0:ground 1:move 2:crush 3:wall
		for (int i = 0; i < M_Map.BlocklistTypeCount; ++i)
		{
			M_Map.DeleteBlocklist(i);
		}
		M_Player.SetActivePlayer(false);
	}
	//���ӿ��� �����ƴ��� üũ ���̸� �ٽ� ��ġ �ޱ�.
	bool RaycastSpawn(GameObject obj)
	{
		int mask = 1 << 7;
		float radius = obj.transform.localScale.x/2;
		RaycastHit hitinfo;

		if(Physics.Raycast(obj.transform.position,transform.forward,out hitinfo,radius,mask))
		{
			return true;
		}
        if(Physics.Raycast(obj.transform.position, -transform.forward, out hitinfo, radius, mask))
		{
			return true;
		}
		if(Physics.Raycast(obj.transform.position, transform.right, out hitinfo, radius, mask))
		{
			return true;
		}
		if(Physics.Raycast(obj.transform.position, -transform.right, out hitinfo, radius, mask))
		{
			return true;
		}
		return false;
	}
	void RandomPlayerPos()
	{
		float x;
		float z;
		float y = 0;	
		x = Random.Range(m_Startpos.x +0.5f, m_Endpos.x-0.5f );
		y = m_RespawnPos.y;
		z = Random.Range(m_Startpos.z- 0.5f, m_Endpos.z+ 0.5f);

		M_Player.SetPos(x, y, z);

		M_Player.SetActivePlayer(true);
		M_Camera.Movecheck = true;
	}
	Monster RandomEnemyPos()
	{
		float x;
		float z;
		float y = 0;
		x = Random.Range(m_Startpos.x + 0.5f, m_Endpos.x - 0.5f);
		y = m_RespawnPos.y;
		z = Random.Range(m_Startpos.z - 0.5f, m_Endpos.z + 0.5f);

		Monster item=M_Monster.Spawn(E_MonsterType.Nomal);
		item.SetPos(x, y, z);
		return item;
	}
	public void AddCantRespawnList(Vector3 pos)
	{
		m_StageDataList[nowstage].m_MapData.m_CantRespawnPos.Add(pos);
	}
	public GunData GetGunData(int index)
	{
		return m_GunDataList[index];
	}
	public StageData GetStageData()
	{
		return m_StageDataList[nowstage];
	}
	private void Start()
	{
		
		m_RespawnPos.y = 2;
		m_StageDataList = new List<StageData>();
		m_GunDataList = new List<GunData>();
		StageData temp1 = new StageData();
		//StageData temp2 = new StageData();
		//StageData temp3=new StageData();
		#region �� ���� �ʱ�ȭ

		temp1.m_MapData.m_Startpos.x = -30;
		temp1.m_MapData.m_Startpos.z = 23;
		temp1.m_MapData.m_Endpos.x = 30;
		temp1.m_MapData.m_Endpos.z = -23;
		temp1.m_MapData.m_CantRespawnPos = new List<Vector3>();

		#endregion

		#region �������� �� ���� �ʱ�ȭ
		temp1.m_BlockData.m_Hp = 20;
		#endregion

		#region �÷��̾� ���� �ʱ�ȭ
		temp1.m_PlayerData.m_Atk = 5;
		temp1.m_PlayerData.m_AtkSpeed = 1.2f;
		temp1.m_PlayerData.m_AtkRange = 1f;
		temp1.m_PlayerData.m_Hp = 100;
		temp1.m_PlayerData.m_Speed = 3f;
		//temp1.m_PlayerData.m_SkillCoolTime = 5f;
		//temp1.m_PlayerData.m_SkillRange = 15f;
		
		#endregion
		
		#region �� ���� �ʱ�ȭ
		//* �ӽ÷� �Ѱ��� Ÿ���� ���� ����Ϸ��� ���⼭ �ʱ�ȭ �ϴµ� ������ �� Ÿ�Ժ��� pool�����Ҷ� �⺻�� �����ؾ���.
		//�������� �������� �����;��Ұ� �������� 1������ � Ÿ���� ���Ͱ� ���������� ���Ѱ�.
		temp1.m_EnemyData.m_TotalEnemy = 5;
		temp1.m_EnemyData.m_Speed = 2.5f;
		temp1.m_EnemyData.m_Atk = 10;
		temp1.m_EnemyData.m_AtkSpeed = 2f;
		temp1.m_EnemyData.m_Hp = 15;
		temp1.m_EnemyData.m_AtkRange = 4f;
		temp1.m_EnemyData.m_Range = 10f;
		temp1.m_EnemyData.m_Skill.m_SkillCoolTime = 5f;
		temp1.m_EnemyData.m_Skill.m_SkillRange = 10f;
		#endregion
		#region �� ��������
		m_GunTypeCount = BulletPrefeblist.Count;//GunPrefeblist.Count; ��  ���� ���� ���ذ�����.
		GunData gunData = new GunData();
		gunData.m_type = E_BulletType.Nomal;
		gunData.m_TotalBullet = 10;//������ �䱸�ϴ� �Ѿ� maxġ.
		gunData.m_Atk = 8f;
		gunData.m_AtkRange = 10f;
		gunData.m_AtkSpeed = 3f;
		gunData.m_CoolTime = 4f;//�����ð�
		#endregion
		m_StageDataList.Add(temp1);
		m_GunDataList.Add(gunData);

		M_Bullet.Init();

	}
}
