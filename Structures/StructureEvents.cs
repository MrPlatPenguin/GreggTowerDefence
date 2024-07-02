using System;

public class StructureEvents
{
    public delegate void StructureDelegate(Structure structure);
    public static StructureDelegate OnStructureSelected;
    public static StructureDelegate OnStructureMouseOver;
    public static Action OnStructreDeselected;
    public static Action OnStructreMouseExit;

    public static void ResetEvents()
    {
        OnStructureSelected = null;
        OnStructreDeselected = null;
        OnStructureMouseOver = null;
        OnStructreMouseExit = null;
    }
}
