public class HeroRunestone
{
    internal int idhg;
    internal int idh;
    internal string idcode;
    internal int idig;
    internal int quantity;
    internal int level;
    internal long timemili;
    public HeroRunestone()
    {

    }
    public HeroRunestone(Item _item)
    {
        idhg = int.Parse(_item.getValue("idhg").ToString());
        idh = int.Parse(_item.getValue("idh").ToString());
        idcode = _item.getValue("idcode").ToString();
        idig = int.Parse(_item.getValue("idig").ToString());
        quantity = int.Parse(_item.getValue("quantity").ToString());
        level = int.Parse(_item.getValue("level").ToString());
        timemili = long.Parse(_item.getValue("timemili").ToString());
    }
    public HeroRunestone(int idig, int quantity, int level)
    {
        idhg = 0;
        idh = CharacterInfo._instance._baseProperties.idHero;
        idcode = CharacterInfo._instance._baseProperties.idCodeHero;
        this.idig = idig;
        this.quantity = quantity;
        this.level = level;
        timemili = 0;
    }
}
