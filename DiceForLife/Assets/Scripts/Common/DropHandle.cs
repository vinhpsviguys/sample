
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using CoreLib;

public class DropHandle : MonoBehaviour, IDropHandler
{

    public NewSkill dataSkill;


    public void OnDrop(PointerEventData eventData)
    {
      
        if (GetDropObject(eventData) != null && GetDropObject(eventData).tag=="Skill")
        {
            Debug.Log("switch skill slot");
            Sprite dropSprite = gameObject.GetComponent<Image>().sprite;
            Sprite dragSprite= GetDropObject(eventData).GetComponent<Image>().sprite;
            NewSkill dropSkill = gameObject.GetComponent<DropHandle>().dataSkill;
            NewSkill dragSkill = GetDropObject(eventData).GetComponent<DragHandeler>().dataSkill;
            int indexDropSkill = PlayerPrefs.GetInt(gameObject.GetComponent<DropHandle>().dataSkill.data["idhk"].Value);
            int indexDragSkill = PlayerPrefs.GetInt(GetDropObject(eventData).GetComponent<DragHandeler>().dataSkill.data["idhk"].Value);

            PlayerPrefs.SetInt(GetDropObject(eventData).GetComponent<DropHandle>().dataSkill.data["idhk"].Value, indexDropSkill);
            PlayerPrefs.SetInt(gameObject.GetComponent<DropHandle>().dataSkill.data["idhk"].Value, indexDragSkill);
            this.gameObject.GetComponent<Image>().sprite= dragSprite;
            GetDropObject(eventData).GetComponent<Image>().sprite = dropSprite;
            this.gameObject.GetComponent<DragHandeler>().SetDataSkill(dragSkill);
            this.gameObject.GetComponent<DropHandle>().SetDataSkill(dragSkill);
            GetDropObject(eventData).GetComponent<DragHandeler>().SetDataSkill(dropSkill);
            GetDropObject(eventData).GetComponent<DropHandle>().SetDataSkill(dropSkill);
           
        } else if (GetDropObject(eventData) != null && GetDropObject(eventData).tag == "SkillInList")
        {
            Sprite dragSprite = GetDropObject(eventData).GetComponent<Image>().sprite;
            NewSkill dragSkill = GetDropObject(eventData).GetComponent<DragHandeler>().dataSkill;
            NewSkill dropSkill = gameObject.GetComponent<DropHandle>().dataSkill;
            int indexDropSkill = PlayerPrefs.GetInt(gameObject.GetComponent<DropHandle>().dataSkill.data["idhk"].Value);
            if (dragSkill.data["idInit"].AsInt != dropSkill.data["idInit"].AsInt && !checkHeroWearedSkill(dragSkill))
            {
                StartCoroutine(ServerAdapter.UnEquipSkill(CharacterInfo._instance._baseProperties.idHero, CharacterInfo._instance._baseProperties.idCodeHero, dropSkill.data["idhk"].AsInt, result =>
                {
                    Debug.Log(result.ToString());
                    if (result.StartsWith("Error"))
                    {
                        Debug.Log("Do nothing");
                    }
                    else
                    {
                        Debug.Log("remove old skill ok");
                        foreach (NewSkill _tempSkill in SplitDataFromServe._heroSkill)
                        {
                            if (dropSkill.data["idhk"].AsInt == _tempSkill.data["idhk"].AsInt)
                            {
                                _tempSkill.addField("typewear", 0);
                                break;
                            }
                        }
                        StartCoroutine(ServerAdapter.EquipSkill(CharacterInfo._instance._baseProperties.idHero, CharacterInfo._instance._baseProperties.idCodeHero, dragSkill.data["idhk"].AsInt, result1 =>
                        {
                            Debug.Log(result1.ToString());
                            if (result1.StartsWith("Error"))
                            {
                                Debug.Log("Do nothing");
                            }
                            else
                            {
                                Debug.Log("change skill ok");
                                foreach (NewSkill _tempSkill in SplitDataFromServe._heroSkill)
                                {
                                    if (dragSkill.data["idhk"].AsInt == _tempSkill.data["idhk"].AsInt)
                                    {
                                        _tempSkill.addField("typewear", 1);
                                        break;
                                    }
                                }
                                PlayerPrefs.DeleteKey(dropSkill.data["idhk"].Value);
                                PlayerPrefs.SetInt(dragSkill.data["idhk"].Value, indexDropSkill);
                                this.gameObject.GetComponent<Image>().sprite = dragSprite;
                                this.gameObject.GetComponent<DragHandeler>().SetDataSkill(dragSkill);
                                this.gameObject.GetComponent<DropHandle>().SetDataSkill(dragSkill);
                            }
                        }));
                    }
                }));
                
            } else
            {
                Debug.Log("Do nothing");
            }
        }
    }

  
    private GameObject GetDropObject(PointerEventData data)
    {
        GameObject originalObj = data.pointerDrag as GameObject;
        if (originalObj.GetComponent<DragHandeler>() == null)
            return null;
        return originalObj;
    }

    public void SetDataSkill(NewSkill tempSkill)
    {
        this.dataSkill = tempSkill;
    }

    bool checkHeroWearedSkill(NewSkill  tempSkill)
    {
        bool check = false;
        foreach (NewSkill _skill in SplitDataFromServe._heroSkill)
        {
            if (_skill.data["idk"].AsInt == tempSkill.data["idInit"].AsInt && _skill.data["typewear"]==1)
            {
                check = true;
                break;
            }
        }
        return check;
    }
}
