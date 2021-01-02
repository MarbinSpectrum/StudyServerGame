using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MoveObject : MonoBehaviour
{
    public Canvas canvas;
    public TextMeshProUGUI text;
    public string nickName;
    public int id;

    public void Awake()
    {
        canvas.worldCamera = Camera.main;
        MoveObjectList.instance.list.Add(this);
    }

    void Update()
    {
        text.text = nickName;
        if (ClientMoveObject.instance.clientid != id)
            return;
        float x = Input.GetAxisRaw("Horizontal");
        float y = Input.GetAxisRaw("Vertical");
        transform.position += new Vector3(x, y, 0) * 10 * Time.deltaTime;
        if (x != 0 || y != 0)
            ClientMoveObject.instance.Send($"PlayerPos|{id}|{ transform.position.x}|{ transform.position.y}");
    }
}
