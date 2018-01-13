using UnityEngine;

public class EffectPatternUpgrade : MonoBehaviour
{
    [SerializeField] private GameObject _fireCircleEff, _maskObject;

    [SerializeField] private GameObject _effSuccess, _effFailed;
    [SerializeField] private Transform _totalPattern, _1stPattern, _2ndPattern;
    private bool isRolling;
    private Vector3 _1stTarget = new Vector3(0, 0, 360 * 2.5f);
    private Vector3 _2ndTarget = new Vector3(0, 0, 360 * -3);
    private Vector3 _totalTarget = new Vector3(0, 0, 360 * 1);

    private float _giatocQuay;
    private float _MaxSpeedRotate = 4f;
    // Use this for initialization
    private bool isRollUp;
    private bool isRollDown;

    private bool isShowEffectResult, isSuccess;
    private float _timeShowSuccess;
    public float _timeRolling = 2f;
    void Start()
    {
        ResetPattern();
    }
    private void Update()
    {
#if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.R)) StartRolling();
#endif

        if (isRolling)
        {
            if (isRollUp)
            {
                _giatocQuay += _MaxSpeedRotate * Time.deltaTime / 0.3f;
                if (_giatocQuay > 8) isRollUp = false;
            }
            if (isRollDown)
            {
                _giatocQuay -= _MaxSpeedRotate * Time.deltaTime / 0.3f;
                if (_giatocQuay <= 0)
                {
                    ResetPattern();
                }
            }
            //_totalPattern.Rotate(0, 0, _giatocQuay);
            _1stPattern.Rotate(0, 0, _giatocQuay * 1.2f);
            _2ndPattern.Rotate(0, 0, -_giatocQuay * 1.5f);
        }
        if (isShowEffectResult)
        {
            _timeShowSuccess -= Time.deltaTime;
            //_parSuccess.transform.Rotate(0, 0, 3);
            if (_timeShowSuccess <= 0f)
            {
                isShowEffectResult = false;
                if (isSuccess) _effSuccess.SetActive(false);
                else _effFailed.SetActive(false);
            }
        }
    }
    // Update is called once per frame
    void ResetPattern()
    {
        isRolling = false;
        _1stPattern.gameObject.SetActive(false);
        _2ndPattern.gameObject.SetActive(false);
        _fireCircleEff.SetActive(false);
        _maskObject.SetActive(false);
    }
    internal void StartRolling()
    {
        isRolling = true;
        _giatocQuay = 0;
        isRollUp = true;
        isRollDown = false;
        _1stPattern.gameObject.SetActive(true);
        _2ndPattern.gameObject.SetActive(true);

        _fireCircleEff.SetActive(true);
        _maskObject.SetActive(true);
    }
    internal void EndRolling()
    {
        isRollDown = true;
    }
    internal void ShowEffectUpgrade(bool isSuccess)
    {
        this.isShowEffectResult = true;
        this.isSuccess = isSuccess;
        if (isSuccess)
        {
            _timeShowSuccess = 0.9f;
            _effSuccess.SetActive(true);
        }
        else
        {
            _timeShowSuccess = 0.9f;
            _effFailed.SetActive(true);
        }
    }
}
