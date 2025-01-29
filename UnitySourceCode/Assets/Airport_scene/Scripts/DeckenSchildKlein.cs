using UnityEngine;
using System.Collections;

public class DeckenSchildKlein : MonoBehaviour {
//--------------------------------------------------------------------------------------------------------------------------
//--------------------------------------------------------------------------------------------------------------------------
//--------------------------------------------------------------------------------------------------------------------------

    public int matIndex;
    public Texture texture1;
    public Texture texture2;

    public enum SchildType
    {
        GateA,
        GateB,
        GateC,
        GateD,
        GateE,
        GateF,
        GateG,
        GateH,
        GateI,
        GateJ,
        Info,
        Locker,
        LostNFound,
        TicketKontrolle,
        Shop,
        GeldAutomat,
        GepaeckKontrolle,
        PassKontrolle,
        Raucherzone,
        Nichtraucher,
        Aufzug,
        Wartezone,
        Treppe,
        Wickelraum,
        Rolltreppe,
        Briefkasten,
        Telefon,
        WC,
        Ruheraum,
        DurchVerboten,
        Kaffee,
        Cocktail,
    }
    public SchildType schildType;
//--------------------------------------------------------------------------------------------------------------------------
//--------------------------------------------------------------------------------------------------------------------------
//--------------------------------------------------------------------------------------------------------------------------
float[] xoffset =
{
    0.0f, 0.25f, 0.5f, 0.75f,
    0.0f, 0.25f, 0.5f, 0.75f,
    0.0f, 0.25f, 0.5f, 0.75f,
    0.0f, 0.25f, 0.5f, 0.75f,

    0.0f, 0.25f, 0.5f, 0.75f,
    0.0f, 0.25f, 0.5f, 0.75f,
    0.0f, 0.25f, 0.5f, 0.75f,
    0.0f, 0.25f, 0.5f, 0.75f,
};
float[] yoffset =
{
    0.0f, 0.0f, 0.0f, 0.0f,
    0.25f, 0.25f, 0.25f, 0.25f,
    0.5f, 0.5f, 0.5f, 0.5f,
    0.75f, 0.75f, 0.75f, 0.75f,

    0.0f, 0.0f, 0.0f, 0.0f,
    0.25f, 0.25f, 0.25f, 0.25f,
    0.5f, 0.5f, 0.5f, 0.5f,
    0.75f, 0.75f, 0.75f, 0.75f,
};
int[] textureID =
{
    0,0,0,0,
    0,0,0,0,
    0,0,0,0,
    0,0,0,0,

    1,1,1,1,
    1,1,1,1,
    1,1,1,1,
    1,1,1,1,
};
//--------------------------------------------------------------------------------------------------------------------------
//--------------------------------------------------------------------------------------------------------------------------
//--------------------------------------------------------------------------------------------------------------------------
void Update()
{
    if(gameObject.GetComponent<Renderer>().enabled == true)
    {
        //-- set textfield
        if(textureID[(int)schildType] == 0)
            gameObject.transform.GetComponent<Renderer>().materials[matIndex].mainTexture = texture1;         
        else
            gameObject.transform.GetComponent<Renderer>().materials[matIndex].mainTexture = texture2;         

        gameObject.transform.GetComponent<Renderer>().materials[matIndex].mainTextureOffset = new Vector2(xoffset[(int)schildType], -yoffset[(int)schildType]);
    }
}    
//--------------------------------------------------------------------------------------------------------------------------
//--------------------------------------------------------------------------------------------------------------------------
//--------------------------------------------------------------------------------------------------------------------------
}
