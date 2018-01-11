using UnityEngine;
using UnityEngine.UI;

public class ItemHeroSelect : MonoBehaviour
{
    internal int myIdHero;
    //public Image boundImg;
    public Image myIcon;
    public void OnSelected()
    {
        //Debug.Log("enter" + myIdHero);
        this.PostEvent(EventID.ItemHeroSelected, myIdHero);
    }
}
