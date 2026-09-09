using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "PermanentCredit", menuName = "Permanent/PermanentCredit")]
public class PermanentCredit : ScriptableObject
{
    [SerializeField]private int _permanentCredit;

    private Text _permanentCreditText;

    public void Set_Initialize ()
    {
        _permanentCredit = 0;
        _permanentCreditText = null;
    }

   
    public int Set_CreditUp { set => _permanentCredit += value;}
    public int Set_CreditDown { set => _permanentCredit -= value;}

    public void Set_CreditTextModeDebuff()
    {
        _permanentCreditText.text = "+" + _permanentCredit.ToString();    
    }

    public void Set_CreditTextModeBuff()
    {
        _permanentCreditText.text = "-" + _permanentCredit.ToString();
    }


    public Text Set_PermanentText { set =>  _permanentCreditText = value; }


    public int Get_PermanentCredit { get => _permanentCredit; }
}
