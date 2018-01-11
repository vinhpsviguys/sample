using CoreLib;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


public class DragHandeler : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerDownHandler, IPointerClickHandler
{
    public static GameObject itemBeingDragged = null;
    Sprite _spriteItemClicked;
    Vector3 startPos;
    private Vector3 _tempPos;
    public NewSkill dataSkill;
    // Use this for initialization

    bool checkHeroContainSkill()
    {
        foreach (NewSkill _skill in SplitDataFromServe._heroSkill)
        {
            if (_skill.data["idk"].AsInt == dataSkill.data["idInit"].AsInt) return true;
        }
        return false;
    }
    public void OnBeginDrag(PointerEventData eventData)
    {
        if (checkHeroContainSkill() && !dataSkill.data["type"].Value.Equals("passive"))
        {
            itemBeingDragged = Instantiate(Resources.Load("Prefabs/Skill") as GameObject);
            var group = itemBeingDragged.AddComponent<CanvasGroup>();
            group.blocksRaycasts = false;
            itemBeingDragged.GetComponent<Image>().sprite = _spriteItemClicked;
            itemBeingDragged.GetComponent<Image>().raycastTarget = false;
            itemBeingDragged.transform.SetParent(SkillCharacterUI.Instance._dragItem);
            itemBeingDragged.transform.localScale = Vector3.one;
            itemBeingDragged.GetComponent<RectTransform>().sizeDelta = new Vector2(150f, 150f);
            startPos = transform.position;
        }
        else if (!checkHeroContainSkill() || dataSkill.data["type"].Value.Equals("passive"))
        {
            eventData.pointerDrag = null;
            itemBeingDragged = null;
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (itemBeingDragged != null)
        {
                itemBeingDragged.transform.position = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 1000f));
            _tempPos = itemBeingDragged.transform.localPosition;
            _tempPos.z = 0;
            itemBeingDragged.transform.localPosition = _tempPos;
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (SplitDataFromServe._heroSkill.Contains(dataSkill) && !dataSkill.data["type"].Value.Equals("passive"))
            if (itemBeingDragged != null)
            {
                string tempIdhk = this.dataSkill.data["idhk"].Value;
                if (eventData.pointerEnter.tag.Equals("SkillSlot") && !this.gameObject.tag.Equals("SkillInList"))
                {
                    Transform finishDrag = eventData.pointerEnter.transform;
                    if (eventData.pointerEnter.transform.childCount == 0)
                    {
                        this.transform.parent = null;
                        this.transform.parent = finishDrag;
                        this.transform.localScale = Vector3.one;
                        this.GetComponent<RectTransform>().localPosition = Vector3.zero;
                        PlayerPrefs.SetInt(this.dataSkill.data["idhk"].Value, int.Parse(finishDrag.name));
                        Destroy(itemBeingDragged);
                        itemBeingDragged = null;
                    }
                    else
                    {
                        Destroy(itemBeingDragged);
                        itemBeingDragged = null;
                    }
                }
                else if (eventData.pointerEnter.tag.Equals("SkillSlot") && this.gameObject.tag.Equals("SkillInList"))
                {
                    Transform finishDrag = eventData.pointerEnter.transform;
                    //Debug.Log("wear skill");
                    StartCoroutine(ServerAdapter.EquipSkill(CharacterInfo._instance._baseProperties.idHero, CharacterInfo._instance._baseProperties.idCodeHero, this.dataSkill.data["idhk"].AsInt, result =>
                    {
                        Debug.Log(result.ToString());
                        if (result.StartsWith("Error"))
                        {
                            //Debug.Log("Do nothing");
                            Destroy(itemBeingDragged);
                            itemBeingDragged = null;
                        }
                        else
                        {
                            Debug.Log("wear ok skill level " + this.dataSkill.data["level"].AsInt);
                            GameObject skillObj = Instantiate(Resources.Load("Prefabs/Skill") as GameObject);
                            skillObj.transform.parent = finishDrag;
                            PlayerPrefs.SetInt(this.dataSkill.data["idhk"].Value, finishDrag.GetSiblingIndex());
                            skillObj.transform.localPosition = Vector3.zero;
                            skillObj.GetComponent<RectTransform>().localScale = new Vector3(1f, 1f, 1f);
                            skillObj.transform.GetComponent<Image>().sprite = Resources.Load<Sprite>("Textures/skillAss/" + this.dataSkill.data["sprite"].Value);
                            skillObj.AddComponent<DragHandeler>().dataSkill = this.dataSkill;
                            skillObj.AddComponent<DropHandle>().dataSkill = this.dataSkill;
                            skillObj.tag = "Skill";
                            foreach (NewSkill _tempSkill in SplitDataFromServe._heroSkill)
                            {
                                if (this.dataSkill.data["idhk"].AsInt == _tempSkill.data["idhk"].AsInt)
                                {
                                    _tempSkill.addField("typewear", 1);
                                    break;
                                }
                            }
                            Destroy(itemBeingDragged);
                            itemBeingDragged = null;

                        }
                    }));

                }
                else if (!eventData.pointerEnter.tag.Equals("Skill") && !eventData.pointerEnter.tag.Equals("SkillSlot") && !this.gameObject.tag.Equals("SkillInList"))
                {
                    //Debug.Log("unwear skill");
                    StartCoroutine(ServerAdapter.UnEquipSkill(CharacterInfo._instance._baseProperties.idHero, CharacterInfo._instance._baseProperties.idCodeHero, this.dataSkill.data["idhk"].AsInt, result =>
                     {
                         if (result.StartsWith("Error"))
                         {
                             //Debug.Log("Do nothing");
                             Destroy(itemBeingDragged);
                         }
                         else
                         {
                             //Debug.Log(result);
                             PlayerPrefs.DeleteKey(tempIdhk);
                             foreach (NewSkill _tempSkill in SplitDataFromServe._heroSkill)
                             {
                                 if (this.dataSkill.data["idhk"].AsInt == _tempSkill.data["idhk"].AsInt)
                                 {
                                     _tempSkill.addField("typewear", 0);
                                     break;
                                 }
                             }
                             Destroy(itemBeingDragged);
                             itemBeingDragged = null;
                             Destroy(this.gameObject);

                         }
                     }));
                }
                else
                {
                    Destroy(itemBeingDragged);
                    itemBeingDragged = null;
                }

            }
    }

    public void OnPointerDown(PointerEventData data)
    {
        _spriteItemClicked = this.gameObject.GetComponent<Image>().sprite;
        //SkillCharacterUI.Instance.ShowInfoSkill(this.dataSkill);
    }
    public void SetDataSkill(NewSkill tempSkill)
    {
        this.dataSkill = tempSkill;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        SkillCharacterUI.Instance.ShowInfoSkill(this.dataSkill);
    }
}
