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

    public Room room_Cp;

    public Hash128 identity;

    bool m_isVisible;

    #endregion

    //////////////////////////////////////////////////////////////////////
    // properties
    //////////////////////////////////////////////////////////////////////
    #region properties

    public bool isVisible
    {
        get { return m_isVisible; }
        set
        {
            if (m_isVisible != value)
            {
                if(!room_Cp)
                {
                    return;
                }

                room_Cp.ChangedWallVisibleState(identity, value);
                m_isVisible = value;
            }
        }
    }

    #endregion

    //////////////////////////////////////////////////////////////////////
    // methods
    //////////////////////////////////////////////////////////////////////
    #region methods

    // Start is called before the first frame update
    void Start()
    {
        
    }

    void LateUpdate()
    {
        // SetVisibleState();
    }

    void SetVisibleState()
    {
        float dot = Vector3.Dot(transform.position - Camera.main.transform.position, transform.up);
        if(dot > 0f)
        {
            isVisible = true;
        }
        else
        {
            isVisible = false;
        }
    }

    #endregion

}
