using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PoolInfo
{
    public bool IsUIobj = false;
    public GameObject ObjectToSpawn;
    public int InitialCount = 1;
    public int MaxCount = -1; // -1 -> infinite
}

public struct PoolSpawnData
{
    public int CreatedCount;
    public PoolInfo PoolInfo;
    public Transform Container;
    public List<GameObject> SpawnedObjects;

    public PoolSpawnData (PoolInfo i_PoolInfo)
    {
        CreatedCount = 0;
        PoolInfo = i_PoolInfo;
        Container = null;
        SpawnedObjects = new List<GameObject>();
    }

    public void IncrementCount ()
    {
        CreatedCount++;
    }

    public void AddObject (GameObject i_Obj)
    {
        SpawnedObjects.Add(i_Obj);
    }
}

public class PoolMgr : MonoBehaviour
{
    private static PoolMgr m_Instance;
    public static PoolMgr Instance
    {
        get => m_Instance;
    }

    [SerializeField] private List<PoolInfo> m_PoolInfos = new List<PoolInfo>();
    private Dictionary<string, PoolSpawnData> m_SpawnData = new Dictionary<string, PoolSpawnData>();

    
    private void Awake()
    {
        if (m_Instance == null)
        {
            m_Instance = this;
        }
        else if (m_Instance != this)
        {
            Destroy(this);
        }

        InitPool();
    }

    public bool IsInPool (string i_ObjName)
    {
        return m_SpawnData.ContainsKey(i_ObjName);
    }

    private void InitPool ()
    {
        for (int i = 0; i < m_PoolInfos.Count; i++)
        {
            string objName = m_PoolInfos[i].ObjectToSpawn.name;
            if (!m_SpawnData.ContainsKey(objName))
            {
                PoolSpawnData newSpawnData = new PoolSpawnData(m_PoolInfos[i]);
                m_SpawnData.Add(objName, newSpawnData);
                GameObject container = new GameObject(objName + "_Pool");
                container.transform.SetParent(transform);

                newSpawnData.Container = container.transform;
                m_SpawnData[objName] = newSpawnData;

                for (int j = 0; j < m_PoolInfos[i].InitialCount; j++)
                {
                    CreateObject(m_PoolInfos[i].ObjectToSpawn);
                }
            }
        }
    }

    public GameObject Spawn (string i_ObjName, Vector3 i_Position = default, Quaternion i_Rotation = default, RectTransform rect = default)
    {
        if (!m_SpawnData.ContainsKey(i_ObjName))
        {
            Debug.LogError("Can't spawn " + i_ObjName + " since it doesn't exist in Pool Info!");
            return null;
        }

        GameObject availableObj = null;

        int spawnedCount = m_SpawnData[i_ObjName].SpawnedObjects.Count;
        if (spawnedCount == 0)
        {
            int maxCount = m_SpawnData[i_ObjName].PoolInfo.MaxCount;
            if (maxCount >= 0 && m_SpawnData[i_ObjName].CreatedCount >= maxCount)
            {
                Debug.LogError("Can't spawn more object (" + i_ObjName + ") since the limit has been reached!");
                return null;
            }

            availableObj = CreateObject(m_SpawnData[i_ObjName].PoolInfo.ObjectToSpawn);
        }
        else
        {
            availableObj = m_SpawnData[i_ObjName].SpawnedObjects[0];
        }

        m_SpawnData[i_ObjName].SpawnedObjects.RemoveAt(0);

        availableObj.SetActive(true);
        if (m_SpawnData[i_ObjName].PoolInfo.IsUIobj)
        {
            availableObj.transform.SetParent(rect);
            availableObj.transform.localPosition = rect.anchoredPosition;
        }
        else
        {
            availableObj.transform.position = i_Position;
            availableObj.transform.rotation = i_Rotation;
        }
        IPoolable[] poolables = availableObj.GetComponents<IPoolable>();
        for (int i = 0; i < poolables.Length; i++)
        {
            poolables[i].OnSpawn();
        }

        return availableObj;
    }

    public void Despawn (GameObject i_Obj)
    {
        if (i_Obj != null)
        {
            IPoolable[] poolables = i_Obj.GetComponents<IPoolable>();
            for (int i = 0; i < poolables.Length; i++)
            {
                poolables[i].OnDespawn();
            }
            AddToPool(i_Obj);
        }
    }

    private GameObject CreateObject (GameObject i_Prefab)
    {
        if (i_Prefab == null)
        {
            Debug.LogError("You are trying to create a null object");
            return null;
        }
        
        GameObject newObj = Instantiate(i_Prefab);
        newObj.name = newObj.name.Replace("(Clone)", "");
        PoolSpawnData temp = m_SpawnData[i_Prefab.name];
        temp.CreatedCount++;
        m_SpawnData[i_Prefab.name] = temp;
        AddToPool(newObj);

        return newObj;
    }

    private void AddToPool (GameObject i_Obj)
    {
        if (i_Obj == null)
        {
            Debug.LogError("You are trying to add to pool a null object");
            return;
        }

        if (!m_SpawnData.ContainsKey(i_Obj.name))
        {
            Debug.LogError("Can't add to pool" + i_Obj.name + "since it doesn't exist in the pool");
            return;
        }

        i_Obj.SetActive(false);
        i_Obj.transform.SetParent(m_SpawnData[i_Obj.name].Container);
        m_SpawnData[i_Obj.name].AddObject(i_Obj);
    }
}