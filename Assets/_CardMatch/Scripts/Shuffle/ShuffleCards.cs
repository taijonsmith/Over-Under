using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShuffleCards : MonoBehaviour 
{
    [SerializeField] private GameObject card1;
    [SerializeField] private GameObject card2;
    [SerializeField] private GameObject card3;

    [SerializeField] private float InitialSpeed = 0.9f;
    [SerializeField] private float IncSpeed = 2.0f;
    [SerializeField] private int NumberOfShuffles = 10;

    [SerializeField] private float TimeScale = 1;

    GameObject g1, g2, g3;

    private float maxYMove = 2.1f;
    private float maxXMove = 2.0f;
    private bool shuffle = false;
    private float Speed;
   
	// Use this for initialization
	void Start () 
    {
        if (TimeScale <= 0) TimeScale = 1;
        Time.timeScale = TimeScale;
        g1 = card1;
        g2 = card2;
        g3 = card3;
        if (NumberOfShuffles <= 0) NumberOfShuffles = 3;
	}
	
	// Update is called once per frame
	void Update () 
    {

	}

    public void Shuffle()
    {
        if (shuffle) return; // Shuffle in progress

        Speed = InitialSpeed;
        StartCoroutine(ShufffleAni());
    }

    private IEnumerator ShufffleAni()
    {
        Vector3 p1, p2, p3;
        float XMove, YMove, x1, x2, x3, y1, y2, y3;
        GameObject g;
        
        float rand;

        shuffle = true;

        for (int i = 0; i < NumberOfShuffles; i++)
        {
            // Determine swap(g1,g2), sawp(g2,g3), or swap(g1, g3) using random number
            rand = Random.value;
            if (rand <= 1/3.0f)
            {
                #region swap (g1,g2)
                // move g2 up
                y2 = g2.gameObject.transform.position.y;
                YMove = 0;
                while (YMove < maxYMove)
                {
                    YMove += Speed * Time.deltaTime;
                    YMove = Mathf.Min(YMove, maxYMove);
                    p2 = g2.gameObject.transform.position;
                    p2.y = y2 + YMove;
                    g2.gameObject.transform.position = p2;
                    yield return null;
                }

                // move g2 left, move g1 right
                x1 = g1.gameObject.transform.position.x;
                x2 = g2.gameObject.transform.position.x;
                XMove = 0;
                while (XMove < maxXMove)
                {
                    XMove += Speed * Time.deltaTime;
                    XMove = Mathf.Min(XMove, maxXMove);
                    p2 = g2.gameObject.transform.position;
                    p2.x = x2 - XMove;
                    g2.gameObject.transform.position = p2;
                    p1 = g1.gameObject.transform.position;
                    p1.x = x1 + XMove;
                    g1.gameObject.transform.position = p1;

                    yield return null;
                }

                // move g2 down
                y2 = g2.gameObject.transform.position.y;
                YMove = 0;
                while (YMove < maxYMove)
                {
                    YMove += Speed * Time.deltaTime;
                    YMove = Mathf.Min(YMove, maxYMove);
                    p2 = g2.gameObject.transform.position;
                    p2.y = y2 - YMove;
                    g2.gameObject.transform.position = p2;
                    yield return null;
                }
 
                // Swap g1 and g2
                g = g1;
                g1 = g2;
                g2 = g;

                #endregion swap(g1,g2)
            }
            else if (rand <= 2 / 3.0f)
            {
                #region swap (g2,g3)
                // move g2 up
                y2 = g2.gameObject.transform.position.y;
                YMove = 0;
                while (YMove < maxYMove)
                {
                    YMove += Speed * Time.deltaTime;
                    YMove = Mathf.Min(YMove, maxYMove);
                    p2 = g2.gameObject.transform.position;
                    p2.y = y2 + YMove;
                    g2.gameObject.transform.position = p2;
                    yield return null;
                }

                // move g2 right, move g3 left
                x3 = g3.gameObject.transform.position.x;
                x2 = g2.gameObject.transform.position.x;
                XMove = 0;
                while (XMove < maxXMove)
                {
                    XMove += Speed * Time.deltaTime;
                    XMove = Mathf.Min(XMove, maxXMove);
                    p2 = g2.gameObject.transform.position;
                    p2.x = x2 + XMove;
                    g2.gameObject.transform.position = p2;
                    p3 = g3.gameObject.transform.position;
                    p3.x = x3 - XMove;
                    g3.gameObject.transform.position = p3;

                    yield return null;
                }

                // move g2 down
                y2 = g2.gameObject.transform.position.y;
                YMove = 0;
                while (YMove < maxYMove)
                {
                    YMove += Speed * Time.deltaTime;
                    YMove = Mathf.Min(YMove, maxYMove);
                    p2 = g2.gameObject.transform.position;
                    p2.y = y2 - YMove;
                    g2.gameObject.transform.position = p2;
                    yield return null;
                }

                // Swap g2 and g3
                g = g2;
                g2 = g3;
                g3 = g;

                #endregion swap(g2,g3)
            }
            else
            {
                #region swap (g1,g3)

                // move g1 up, move g3 down
                y2 = g1.gameObject.transform.position.y;
                y3 = g3.gameObject.transform.position.y;
                YMove = 0;
                while (YMove < maxYMove)
                {
                    YMove += Speed * Time.deltaTime;
                    YMove = Mathf.Min(YMove, maxYMove);
                    p1 = g1.gameObject.transform.position;
                    p1.y = y2 + YMove;
                    g1.gameObject.transform.position = p1;
                    p3 = g3.gameObject.transform.position;
                    p3.y = y3 - YMove;
                    g3.gameObject.transform.position = p3;
                    yield return null;
                }

                // move g1 right, move g3 left
                x1 = g1.gameObject.transform.position.x;
                x3 = g3.gameObject.transform.position.x;
                XMove = 0;
                while (XMove < 2*maxXMove)
                {
                    XMove += Speed * Time.deltaTime;
                    XMove = Mathf.Min(XMove, 2*maxXMove);
                    p1 = g1.gameObject.transform.position;
                    p1.x = x1 + XMove;
                    g1.gameObject.transform.position = p1;
                    p3 = g3.gameObject.transform.position;
                    p3.x = x3 - XMove;
                    g3.gameObject.transform.position = p3;

                    yield return null;
                }

                // move g1 down, move g3 up
                y1 = g1.gameObject.transform.position.y;
                y3 = g3.gameObject.transform.position.y;
                YMove = 0;
                while (YMove < maxYMove)
                {
                    YMove += Speed * Time.deltaTime;
                    YMove = Mathf.Min(YMove, maxYMove);
                    p1 = g1.gameObject.transform.position;
                    p1.y = y1 - YMove;
                    g1.gameObject.transform.position = p1;
                    p3 = g3.gameObject.transform.position;
                    p3.y = y3 + YMove;
                    g3.gameObject.transform.position = p3;
                    yield return null;
                }

                // Swap g1 and g3
                g = g1;
                g1 = g3;
                g3 = g;

                #endregion swap(g1,g3)
            }
            Speed += IncSpeed;
        }

        shuffle = false;

        yield return null;
    }
}
