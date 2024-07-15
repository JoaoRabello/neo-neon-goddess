using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class cursor_script : MonoBehaviour
{

  
    private SpriteRenderer rend;
    public Sprite activeCursor;
    Vector3 cursorpos;



    public Vector2 hotSpot = Vector2.zero;

    public  void SetCursor(Sprite anotherCursor)
    {


        rend.sprite = anotherCursor;

    }
    // Start is called before the first frame update
    private void Awake()
    {
        Cursor.visible = false;
    }
 
   

    void Start()
    {
            Cursor.visible = false;
          rend = GetComponent<SpriteRenderer>();
          rend.sprite = activeCursor;
    }

    // Update is called once per frame
    void Update()
    {
        cursorpos.x = Camera.main.ScreenToWorldPoint(Input.mousePosition).x;
        cursorpos.y = Camera.main.ScreenToWorldPoint(Input.mousePosition).y;
        cursorpos.z = -1;
        transform.position = cursorpos;
 
    }
}
