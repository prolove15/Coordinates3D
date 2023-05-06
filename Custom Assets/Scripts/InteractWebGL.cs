using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;

namespace Coordinate3D
{

public class InteractWebGL : MonoBehaviour
{
    
    //////////////////////////////////////////////////////////////////////
    // import
    //////////////////////////////////////////////////////////////////////
    #region import
    
    [DllImport("__Internal")]
    private static extern void Hello();

    [DllImport("__Internal")]
    private static extern void HelloString(string str);

    [DllImport("__Internal")]
    private static extern void PrintFloatArray(float[] array, int size);

    [DllImport("__Internal")]
    private static extern int AddNumbers(int x, int y);

    [DllImport("__Internal")]
    private static extern string StringReturnValueFunction();

    [DllImport("__Internal")]
    private static extern void BindWebGLTexture(int texture);

    #endregion

    //////////////////////////////////////////////////////////////////////
    // types
    //////////////////////////////////////////////////////////////////////
    #region types
    
    public enum GameState_En
    {
        Nothing, Inited, Play
    }

    #endregion
    
    //////////////////////////////////////////////////////////////////////
    // fields
    //////////////////////////////////////////////////////////////////////
    #region fields
    
    //--------------------------------------------------
    Controller controller_Cp;

    FurnitureManager furnitureManager_Cp;

    //--------------------------------------------------
    public GameState_En gameState;

    #endregion
    
    //////////////////////////////////////////////////////////////////////
    // properties
    //////////////////////////////////////////////////////////////////////
    #region properties
    
    //--------------------------------------------------
    

    #endregion
    
    //////////////////////////////////////////////////////////////////////
    // methods
    //////////////////////////////////////////////////////////////////////
    
    //--------------------------------------------------
    void Start()
    {
        
    }

    //--------------------------------------------------
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Alpha1))
        {
            CallingJavascriptFunction();
        }
    }
    
    //////////////////////////////////////////////////////////////////////
    // init
    //////////////////////////////////////////////////////////////////////
    #region init
    
    //--------------------------------------------------
    public void Init()
    {
        InitOpponents();

        gameState = GameState_En.Inited;
    }

    //--------------------------------------------------
    void InitOpponents()
    {
        controller_Cp = GameObject.FindWithTag("GameController").GetComponent<Controller>();

        furnitureManager_Cp = controller_Cp.furnitureManager_Cp;
    }

    #endregion

    //--------------------------------------------------
    public void Play()
    {
        gameState = GameState_En.Play;
    }

    //--------------------------------------------------
    void CallingJavascriptFunction()
    {
        Hello();
        
        HelloString("Helloblight");
        
        // float[] myArray = new float[10];
        // PrintFloatArray(myArray, myArray.Length);
        
        // int result = AddNumbers(5, 7);
        // Debug.Log(result);
        
        // Debug.Log(StringReturnValueFunction());
        
        // var texture = new Texture2D(0, 0, TextureFormat.ARGB32, false);
        // BindWebGLTexture((int)texture.GetNativeTexturePtr());
    }

    //--------------------------------------------------
    public void OnClick_FurnitureBtn(int index_pr)
    {
        if(gameState != GameState_En.Play)
        {
            return;
        }

        if(index_pr == 1)
        {
            furnitureManager_Cp.InstantFurniture();
        }
        else if(index_pr == 2)
        {
            furnitureManager_Cp.InstantWallFurniture();
        }
    }
}

}