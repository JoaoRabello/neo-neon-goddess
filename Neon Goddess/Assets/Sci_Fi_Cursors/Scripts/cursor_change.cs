using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cursor_change : MonoBehaviour
{
    GameObject mycursor_object;
    private SpriteRenderer rend;
    // Start is called before the first frame update
    void Start()
    {
        mycursor_object = GameObject.FindGameObjectWithTag("CursorObject");
        rend = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnMouseEnter()
    {
       mycursor_object.GetComponent<cursor_script>().SetCursor(rend.sprite);
    }
}
