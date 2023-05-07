using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Coordinate3D;

public class EventHandler_Custom : MonoBehaviour
{
    
    //////////////////////////////////////////////////////////////////////
    // field
    //////////////////////////////////////////////////////////////////////
    #region field

    public Hash128 identity;

    #endregion

    // Start is called before the first frame update
    void Start()
    {
        
    }

    //////////////////////////////////////////////////////////////////////
    // override
    //////////////////////////////////////////////////////////////////////
    #region override

    //--------------------------------------------------
    void OnBecameInvisible()
    {
        print("OnBecameInvisible");
    }

    //--------------------------------------------------
    void OnBecameVisible()
    {
        print("OnBecameVisible");

    }

    #endregion

}
