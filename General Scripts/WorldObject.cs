using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class WorldObject : MonoBehaviour
{
    public virtual void OnClickStart(TileObject tileObject) { }
    public virtual void OnClickEnd(TileObject tileObject) { }
}
