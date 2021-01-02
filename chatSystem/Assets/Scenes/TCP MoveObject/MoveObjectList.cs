using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveObjectList : MonoBehaviour
{
    public static MoveObjectList instance;

    public List<MoveObject> list = new List<MoveObject>();

    public MoveObject playerObject;

    private void Awake() => instance = this;

    public MoveObject GetObject(int id)
    {
        for (int i = 0; i < list.Count; i++)
            if (list[i].id == id)
                return list[i];
        return null;
    }

    public bool CheckPlayer(int id)
    {
        for (int i = 0; i < list.Count; i++)
            if (list[i].id == id)
                return true;
        return false;
    }

    public void SetOjectPos(int id, float x, float y)
    {
        for (int i = 0; i < list.Count; i++)
        {
            if (list[i].id == id)
            {
                MoveObject temp = list[i];
                temp.transform.position = new Vector3(x, y, temp.transform.position.z);
                return;
            }
        }

    }

    public void DeleteOject(int id)
    {
        for (int i = 0; i < list.Count; i++)
        {
            if(list[i].id == id)
            {
                MoveObject temp = list[i];
                list.Remove(temp);
                Destroy(temp.gameObject);
                return;
            }
        }
    }
}
