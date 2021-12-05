using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockManager : Singleton<BlockManager>
{
	public enum E_BlockType
	{
		None = -1,
		Nomal,
		Move,
		Crush,
		Max,
	}
	public enum E_BlockPosition
	{
		None=-1,
		Ground,
		Wall,
		Max
	}

	Transform BlockPoolParent;
	_GameManager M_Game = _GameManager.Instance;
	MapManager M_Map = MapManager.Instance;
	Dictionary<E_BlockType, MemoryPool<Block>> BlockMemorypool;
	Material Blue;
	Material Pink;
	private void Awake()
	{
		BlockPoolParent = GameObject.Find("BlockPool").transform;
		BlockMemorypool = new Dictionary<E_BlockType, MemoryPool<Block>>();
		BlockMemorypool.Add(E_BlockType.Nomal, new MemoryPool<Block>(M_Game.BlockOriginal, M_Map.XxZBlockCount, BlockPoolParent));
		BlockPoolParent = GameObject.Find("MoveBlockPool").transform;
		BlockMemorypool.Add(E_BlockType.Move, new MemoryPool<Block>(M_Game.BlockOriginal, 200, BlockPoolParent));
		BlockPoolParent = GameObject.Find("CrushBlockPool").transform;
		BlockMemorypool.Add(E_BlockType.Crush, new MemoryPool<Block>(M_Game.BlockOriginal, 200, BlockPoolParent));
		Blue = Resources.Load<Material>("Material/Sky");
		Pink = Resources.Load<Material>("Material/Pink");
	}
	
	public Block Spawn(int _type)
	{
		Block item = null;
	
		switch ((E_BlockType)_type)
		{
			case E_BlockType.Nomal:
				item = BlockMemorypool[E_BlockType.Nomal].Spawn();
				if (item.P_Type == (int)E_BlockType.None)
				{
					item.P_Type = (int)E_BlockType.Nomal;
					if(item.name!= "NomalBlock")
					item.name = "NomalBlock";
				}
				break;
			case E_BlockType.Move:
				item = BlockMemorypool[E_BlockType.Move].Spawn();
				
				if (item.P_Type == (int)E_BlockType.None)
				{
					item.P_Type = (int)E_BlockType.Move;
					if (item.name != "MoveBlock")
						item.name = "MoveBlock";
				}
				break;
			case E_BlockType.Crush:
				item = BlockMemorypool[E_BlockType.Crush].Spawn();
				item.P_Type =(int)E_BlockType.Crush;
				item.Init(M_Game.GetStageData());
				if (item.P_Type == (int)E_BlockType.None)
				{
					item.P_Type = (int)E_BlockType.Crush;
					if (item.name != "CrushBlock")
						item.name = "CrushBlock";
				}
				int count = item.transform.childCount;
				Transform[] tempitem =item.GetComponentsInChildren<Transform>();
				for (int i = 1; i < count+1; ++i)
				{   
					if (i % 2 == 0)
					{
						tempitem[i].GetComponent<MeshRenderer>().material = Pink;
					}
					else
					{
						tempitem[i].GetComponent<MeshRenderer>().material= Blue;
					}

				}

				break;
		}
		return item;
	}
	public void DeSpawn(int _type, Block obj)
	{
		switch ((E_BlockType)_type)
		{
			case E_BlockType.Nomal:
				BlockMemorypool[E_BlockType.Nomal].DeSpawn(obj);
				break;
			case E_BlockType.Move:
				BlockMemorypool[E_BlockType.Move].DeSpawn(obj);
				break;
			case E_BlockType.Crush:
				BlockMemorypool[E_BlockType.Crush].DeSpawn(obj);
				//������Ʈ hp�� �������� �ʱ�ȭ �۾� �Լ�ȣ��. �Լ� ������ block����.
				break;
		}
	}
}
