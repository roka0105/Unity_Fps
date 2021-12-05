using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapManager : Singleton<MapManager>
{
	public enum E_BlocklistType
	{
		None = -1,
		Ground,
		Move,
		Crush,
		Wall,
		Max,
	}
	
	protected _GameManager M_Game => _GameManager.Instance;
	protected BlockManager M_Block => BlockManager.Instance;

	protected Dictionary<int, List<Block>> Blocklist;
	int m_Xlimit_index;
	int m_Zlimit_index;
	int m_Ylimit_index;
	int m_TotalBlockCount;
	GameObject ColliderParent;
	Vector3 start;
	Vector3 end;
	Vector3 startwall;
	Vector3 endwall;
	Vector3 Groundsize;
	public int BlocklistTypeCount
	{
		get { return (int)E_BlocklistType.Max; }
	}
	public int XxZBlockCount
	{
		get { return m_TotalBlockCount; }
	}
	public void CreateFixBlock()
	{
		start = M_Game.GetStartPos;
		end = M_Game.GetEndPos;
		Groundsize.x = Mathf.Abs(start.x - end.x);
		Groundsize.y = 1f;
		Groundsize.z = Mathf.Abs(start.z - end.z);
		m_Ylimit_index = 5;
		m_Xlimit_index = (int)(Mathf.Abs(start.x-end.x)) / 2 + 1;
		m_Zlimit_index = (int)(Mathf.Abs(start.z-end.z)) / 2 + 1;
		m_TotalBlockCount = m_Xlimit_index * m_Zlimit_index;

		CreateGround();
		CreateWall();
	}
	public void DeleteBlocklist(int _type)
	{
		if (Blocklist[_type] == null || Blocklist[_type].Count <= 0)
			return;
		for (int i = 0; i < Blocklist[_type].Count; ++i)
		{
			//block type 0:nomal 1:move 2:crush
			Block item = Blocklist[_type][i];
			item.gameObject.layer = -1;
			item.P_Position = -1;
			if (item == null)
			{
				Debug.Log("MapManager: block list null!");
				return;
			}
			if(_type==3)
			{
				M_Block.DeSpawn(0, item);
			}
			else
			M_Block.DeSpawn(_type, item);
		}
		Blocklist[_type].Clear();
		Blocklist[_type] = null;
	}
	void CreateGround()
	{
		if (Blocklist[(int)E_BlocklistType.Ground] != null)
			return;

		Vector3 currentpos = start;
		for (int i = 0; i < m_Zlimit_index; ++i)
		{
			for (int j = 0; j < m_Xlimit_index; ++j)
			{
				//block type 0:nomal 1:move 2:crush
				Block item = M_Block.Spawn(0);
				if (item == null)
				{
					Debug.Log("block spawn null!");
					return;
				}
				currentpos.x = start.x + j * 2-1f;
				currentpos.z = start.z - i * 2+1f;
				//position 1:ground 2:wall
				item.P_Position = 1;

				item.transform.position = currentpos;
				item.gameObject.layer = 9;
				item.gameObject.SetActive(true);
				if (Blocklist[(int)E_BlocklistType.Ground] == null)
					Blocklist[(int)E_BlocklistType.Ground] = new List<Block>();
				Blocklist[(int)E_BlocklistType.Ground].Add(item);
			}
		}
		GameObject collider = new GameObject("MapCollider",typeof(BoxCollider));
		BoxCollider setcollider = collider.GetComponent<BoxCollider>();
		setcollider.size = Groundsize;
		
		collider.transform.SetParent(ColliderParent.transform);
	}

	void CreateWall()
	{
		if (Blocklist[(int)E_BlocklistType.Wall] != null || Blocklist[(int)E_BlocklistType.Crush] != null)
			return;

		start = M_Game.GetStartPos;
		end = M_Game.GetEndPos;
		startwall = start;
		endwall = end;
		m_Ylimit_index = 5;
		m_Xlimit_index = (int)(Mathf.Abs(start.x) + Mathf.Abs(end.x)) / 2 + 1;
		m_Zlimit_index = (int)(Mathf.Abs(start.z) + Mathf.Abs(end.z)) / 2 + 1;
		m_TotalBlockCount = m_Xlimit_index * m_Zlimit_index;
		float tempz = start.z - m_Zlimit_index * 2;
		float tempx = start.x + m_Xlimit_index * 2;
		int type = 0;
		#region 하단&오른쪽 벽
		startwall.z = tempz;
		startwall.x = tempx;
		SideZWall(tempz, false);
		SideXWall(tempx, false);
		#endregion
		#region 중간&중간 벽
		tempz = start.z - m_Zlimit_index;
		tempx = start.x + m_Xlimit_index;
		SideZWall(tempz, true);
		SideXWall(tempx, true);
		#endregion
		#region 상단&왼쪽 벽
		tempz = start.z;
		tempx = start.x;
		SideZWall(tempz, false);
		SideXWall(tempx, false);
		#endregion
	}

	void SideZWall(float z, bool create_crush)
	{
		Vector3 currentpos;
		currentpos.y = m_Ylimit_index;
		currentpos.x = 0;
		currentpos.z = z;
		int[] RandomIndex = new int[2];
		RandomIndex[0] = -1;
		RandomIndex[1] = -1;
		bool nowtype;
		float distance;
		#region 벽에 부셔지는 블럭 생성을 위한 위치 선정 (다음 방으로 이동할 문)
		if (create_crush)
		{
			RandomIndex[0] = Random.Range(1, m_Xlimit_index / 2);
			RandomIndex[1] = Random.Range(m_Xlimit_index / 2 + 1, m_Xlimit_index);
		}
		#endregion
		for (int i = 0; i < m_Xlimit_index; ++i)
		{
			for (int j = 0; j < m_Ylimit_index; ++j)
			{
				Block item = null;
				//선정된 위치에 문 설치.
				if ((i == RandomIndex[0] || i == RandomIndex[1]) && j <= 3)
				{
					item = M_Block.Spawn(2);
					nowtype = true;
				}
				else
				{
					item = M_Block.Spawn(0);
					nowtype = false;
				}
				if (item == null)
				{
					Debug.Log("block spawn null!");
					return;
				}
				currentpos.x = start.x + i * 2;
				currentpos.z = start.z + 1;
				currentpos.y = start.y + j;
				item.transform.position = currentpos;
				item.gameObject.SetActive(true);
				item.gameObject.layer = 7;
				//position 1:ground 2:wall
				item.P_Position = 2;
				if (create_crush && nowtype)
				{
					if (Blocklist[(int)E_BlocklistType.Crush] == null)
						Blocklist[(int)E_BlocklistType.Crush] = new List<Block>();

					Blocklist[(int)E_BlocklistType.Crush].Add(item);
				}
				else
				{
					if (Blocklist[(int)E_BlocklistType.Wall] == null)
						Blocklist[(int)E_BlocklistType.Wall] = new List<Block>();

					Blocklist[(int)E_BlocklistType.Wall].Add(item);
				}
			}
		}
		endwall.x = currentpos.x;
		endwall.y = currentpos.y;
	}
	void SideXWall(float x, bool create_crush)
	{
		Vector3 currentpos;
		currentpos.y = m_Ylimit_index;
		currentpos.x = x;
		int[] RandomIndex = new int[2];
		RandomIndex[0] = -1;
		RandomIndex[1] = -1;
		bool nowtype;
		float distance;
		#region 벽에 부셔지는 블럭 생성을 위한 위치 선정 (다음 방으로 이동할 문)
		if (create_crush)
		{
			RandomIndex[0] = Random.Range(1, m_Zlimit_index / 2);
			RandomIndex[1] = Random.Range(m_Zlimit_index / 2 + 1, m_Zlimit_index);
		}
		#endregion
		for (int i = 0; i <= m_Zlimit_index; ++i)
		{
			for (int j = 0; j < m_Ylimit_index; ++j)
			{
				Block item = null;
				//선정된 위치에 문 설치.
				if ((i == RandomIndex[0] || i == RandomIndex[1]) && j <= 3)
				{
					item = M_Block.Spawn(2);
					nowtype = true;
				}
				else
				{
					item = M_Block.Spawn(0);
					nowtype = false;
				}

				if (item == null)
				{
					Debug.Log("block spawn null!");
					return;
				}
				currentpos.z = start.z - i * 2;
				currentpos.x = start.x - 1;
				currentpos.y = start.y + j;
				item.transform.position = currentpos;
				item.gameObject.SetActive(true);
				item.gameObject.layer = 7;
				//position 1:ground 2:wall
				item.P_Position = 2;
				//crush block 일시.
				if (create_crush&&nowtype)
				{
					if (Blocklist[(int)E_BlocklistType.Crush] == null)
						Blocklist[(int)E_BlocklistType.Crush] = new List<Block>();

					Blocklist[(int)E_BlocklistType.Crush].Add(item);
				}
				else
				{
					if (Blocklist[(int)E_BlocklistType.Wall] == null)
						Blocklist[(int)E_BlocklistType.Wall] = new List<Block>();

					Blocklist[(int)E_BlocklistType.Wall].Add(item);
				}

			}
		}
	}
	private void Awake()
	{
		ColliderParent = M_Game.ColliderParent;
		Blocklist = new Dictionary<int, List<Block>>();
		//ground block list
		Blocklist.Add((int)E_BlocklistType.Ground, null);
		//crush block list
		Blocklist.Add((int)E_BlocklistType.Move, new List<Block>());
		//move block list
		Blocklist.Add((int)E_BlocklistType.Crush, null);
		//wall block list
		Blocklist.Add((int)E_BlocklistType.Wall, null);
	}
}
