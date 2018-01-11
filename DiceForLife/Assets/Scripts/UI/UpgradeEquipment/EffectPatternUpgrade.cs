using UnityEngine;

public class EffectPatternUpgrade : MonoBehaviour
{
    [SerializeField] private GameObject _fireCircleEff,_maskObject,_parSuccess;
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

    private bool isShowSuccess;
    private float _timeShowSuccess;
    public  float _timeRolling = 2f;
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
        if(isShowSuccess)
        {
            _timeShowSuccess += Time.deltaTime;
            //_parSuccess.transform.Rotate(0, 0, 3);
            if (_timeShowSuccess >= 1f)
            {
                isShowSuccess = false;
                _parSuccess.SetActive(false);
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
    internal void ShowParticleSuccess()
    {
        isShowSuccess = true;
        _parSuccess.SetActive(true);
        _timeShowSuccess = 0;
    }
}
