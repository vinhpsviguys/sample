using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingPortalLevel : MonoBehaviour {

    public GameObject _tabCharacter, _tabPet;
    public Button _characterBtn, _petBtn;
    public Sprite _stateOn, _stateOff;

    float hp = 0.5f;
    float xp = 0.5f;

    float hunger = 0.5f;
    float xpBoots = 0.5f;

    public InputField hpField;
    public InputField xpField;
    public InputField hungerField;
    public InputField xpBootsField;

    public Slider hpSlider;
    public Slider xpSlider;
    public Slider hungerSlider;
    public Slider xpBootsSlider;

    private void OnEnable()
    {
        hpField.text = "50";
        xpField.text = "50";
        hungerField.text = "50";
        xpBootsField.text = "50";

        hpSlider.value = 0.5f;
        xpSlider.value = 0.5f;
        hungerSlider.value = 0.5f;
        xpBootsSlider.value = 0.5f;

        OpenCharacterTab();
        _characterBtn.onClick.AddListener(OpenCharacterTab);
        _petBtn.onClick.AddListener(OpenPetTab);

        hpField.onValueChange.AddListener(delegate { ValueHPBarChange(); });
        xpField.onValueChange.AddListener(delegate { ValueXPBarChange(); });
        hungerField.onValueChange.AddListener(delegate { ValuehungerBarChange(); });
        xpBootsField.onValueChange.AddListener(delegate { ValuexpBootsBarChange(); });

        hpSlider.onValueChanged.AddListener(delegate { ValueHPInputFieldChange(); });
        xpSlider.onValueChanged.AddListener(delegate { ValueXPInputFieldChange(); });
        hungerSlider.onValueChanged.AddListener(delegate { ValuehungerInputFieldChange(); });
        xpBootsSlider.onValueChanged.AddListener(delegate { ValuexpBootsInputFieldChange(); });
    }
    private void OnDisable()
    {
        _characterBtn.onClick.RemoveListener(OpenCharacterTab);
        _petBtn.onClick.RemoveListener(OpenPetTab);

        hpField.onValueChange.RemoveListener(delegate { ValueHPBarChange(); });
        xpField.onValueChange.RemoveListener(delegate { ValueXPBarChange(); });
        hungerField.onValueChange.RemoveListener(delegate { ValuehungerBarChange(); });
        xpBootsField.onValueChange.RemoveListener(delegate { ValuexpBootsBarChange(); });

        hpSlider.onValueChanged.RemoveListener(delegate { ValueHPInputFieldChange(); });
        xpSlider.onValueChanged.RemoveListener(delegate { ValueXPInputFieldChange(); });
        hungerSlider.onValueChanged.RemoveListener(delegate { ValuehungerInputFieldChange(); });
        xpBootsSlider.onValueChanged.RemoveListener(delegate { ValuexpBootsInputFieldChange(); });
    }

    public void OpenCharacterTab()
    {
        _characterBtn.image.sprite = _stateOn;
        _characterBtn.transform.GetChild(0).GetComponent<Text>().color = new Color(96f / 255f, 25f / 255f, 22f / 255f, 1f);
        _petBtn.image.sprite = _stateOff;
        _petBtn.transform.GetChild(0).GetComponent<Text>().color = new Color(255f / 255f, 232f / 255f, 170f / 255f, 1f);
        _tabCharacter.gameObject.SetActive(true);
        _tabPet.gameObject.SetActive(false);

       
    }
    public void OpenPetTab()
    {
        _characterBtn.image.sprite = _stateOff;
        _characterBtn.transform.GetChild(0).GetComponent<Text>().color = new Color(255f / 255f, 232f / 255f, 170f / 255f, 1f);
        _petBtn.image.sprite = _stateOn;
        _petBtn.transform.GetChild(0).GetComponent<Text>().color = new Color(96f / 255f, 25f / 255f, 22f / 255f, 1f);
        _tabCharacter.gameObject.SetActive(false);
        _tabPet.gameObject.SetActive(true);
    }

    void ValueHPBarChange()
    {
        hpSlider.value = int.Parse(hpField.text) / 100f;
    }
    void ValueHPInputFieldChange()
    {
        hpField.text = (hpSlider.value * 100f).ToString();
    }
    void ValueXPBarChange()
    {
        xpSlider.value= int.Parse(xpField.text) / 100f;
    }
    void ValueXPInputFieldChange()
    {
        xpField.text = (xpSlider.value * 100f).ToString();
    }
    void ValuehungerBarChange()
    {
        hungerSlider.value = int.Parse(hungerField.text) / 100f;
    }
    void ValuehungerInputFieldChange()
    {
        hungerField.text = (hungerSlider.value * 100f).ToString();
    }
    void ValuexpBootsBarChange()
    {
        xpBootsSlider.value = int.Parse(xpField.text) / 100f;
    }
    void ValuexpBootsInputFieldChange()
    {
        xpBootsField.text = (xpBootsSlider.value * 100f).ToString();
    }
    public void CloseThisDialog()
    {
        this.gameObject.SetActive(false);
    }
    public void SaveThisDialog()
    {
        this.gameObject.SetActive(false);
    }
}
