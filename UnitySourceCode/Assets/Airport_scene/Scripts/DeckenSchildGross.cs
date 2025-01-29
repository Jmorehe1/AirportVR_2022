using UnityEngine;
using System.Collections;

public class DeckenSchildGross : MonoBehaviour {
//--------------------------------------------------------------------------------------------------------------------------
//--------------------------------------------------------------------------------------------------------------------------
//--------------------------------------------------------------------------------------------------------------------------

    public int matIndexMitte;
    public int matIndexPfeilLinks;
    public int matIndexPfeilRechts;
    public Texture texture1;
    public Texture texture2;

    public enum PfeilType
    {
        Oben,
        RechtsOben,
        Rechts,
        Links,
        LinksOben,
        KeinPfeil,
    }
    public PfeilType pfeilLinks;
    public PfeilType pfeilRechts;

    public enum TextType
    {
        Gepaeckausgabe,
        Passkontrolle,
        Ausgang,
        Toiletten,
        Flugsteig,
        Zentralbereich,
        Parkplaetze,
        Diverses,
        Flugscheine,
        CheckIn,
        Reisebuero,
        AnbindungBahnEtc,
    }
    public TextType textType;

//--------------------------------------------------------------------------------------------------------------------------
//--------------------------------------------------------------------------------------------------------------------------
//--------------------------------------------------------------------------------------------------------------------------
float[] yoffset =
{
    1.0f / 6.0f * 0.0f,
    1.0f / 6.0f * 1.0f,
    1.0f / 6.0f * 2.0f,
    1.0f / 6.0f * 3.0f,
    1.0f / 6.0f * 4.0f,
    1.0f / 6.0f * 5.0f,

    1.0f / 6.0f * 0.0f,
    1.0f / 6.0f * 1.0f,
    1.0f / 6.0f * 2.0f,
    1.0f / 6.0f * 3.0f,
    1.0f / 6.0f * 4.0f,
    1.0f / 6.0f * 5.0f,
};
int[] textureID =
{
    0,
    0,
    0,
    0,
    0,
    0,
    1,
    1,
    1,
    1,
    1,
    1,
};
//--------------------------------------------------------------------------------------------------------------------------
//--------------------------------------------------------------------------------------------------------------------------
//--------------------------------------------------------------------------------------------------------------------------
void Update()
{
    if(gameObject.GetComponent<Renderer>().enabled == true)
    {
        //-- set textfield
        if(textureID[(int)textType] == 0)
            gameObject.transform.GetComponent<Renderer>().materials[matIndexMitte].mainTexture = texture1;         
        else
            gameObject.transform.GetComponent<Renderer>().materials[matIndexMitte].mainTexture = texture2;         

        gameObject.transform.GetComponent<Renderer>().materials[matIndexMitte].mainTextureOffset = new Vector2(0.0f, -yoffset[(int)textType]);
 
        //-- set pfeil links
        gameObject.transform.GetComponent<Renderer>().materials[matIndexPfeilLinks].mainTextureOffset = new Vector2(0.0f, -yoffset[(int)pfeilLinks]);
    
        //-- set pfeil rechts
        gameObject.transform.GetComponent<Renderer>().materials[matIndexPfeilRechts].mainTextureOffset = new Vector2(0.0f, -yoffset[(int)pfeilRechts]);
    }
}    
//--------------------------------------------------------------------------------------------------------------------------
//--------------------------------------------------------------------------------------------------------------------------
//--------------------------------------------------------------------------------------------------------------------------
}
