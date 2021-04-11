using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flood : Object
{
    private int mActiveCount;

    public override void OnCollision(GameObject _Other, ref Tile _TargetTile, out bool _CheckOneMoreTime)
    {
        _CheckOneMoreTime = false;
    }

    public override bool OnSlide(InputManager.SlideDirection _Direction, int _Distance = 5)
    {
        return false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Replica") && collision.GetComponent<Adapt_Fish>().CheckTier())
        {
            GameObject Effect = ObjectManager.Instance.CreateOutTile(EnumObjects.Fish_Effect);
            Effect.transform.position = transform.position;
        }
    }
}
