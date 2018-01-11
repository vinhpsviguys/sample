/*
 * Đây là đơn vị chỉ số của thuộc tính
 * Bao gốm giá trị cộng tuyệt đối, cộng theo %
 */



/*
 * Đây là class thể hiện các dòng thuộc tính của trang bị
 */
public class PropertiesBonus
{
    internal PROPERTYTYPE _typeProperty ;
    internal float _valueProperty;
    internal string GetNameEquipt()
    {
        return "";
    }
    public PropertiesBonus()
    {

    }
    public PropertiesBonus(PROPERTYTYPE _type,float _valuePro)
    {
        _typeProperty = _type;
        _valueProperty = _valuePro;
    }
}
