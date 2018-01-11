using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LuckyWheelUI : MonoBehaviour {

    public Rigidbody2D _arrow;
    public Transform _spinWheel;

    public GameObject[] _prizeIcon;

    public List<int> prize;
    public List<AnimationCurve> animationCurves;
    private float anglePerItem;
    private int randomTime;
    private int itemNumber;

    private void OnEnable()
    {
        for (int i = 0; i < _prizeIcon.Length; i++)
        {
            _prizeIcon[i].transform.localEulerAngles = new Vector3(0, 0, 0);
        }
    }


    public void StartSpinWheel()
    {

        randomTime = Random.Range(13, 15);
        itemNumber = RandomPrize();
        float maxAngle = 360 * randomTime + (itemNumber * anglePerItem - anglePerItem / 2);

        StartCoroutine(SpinTheWheel(2.5f * randomTime, maxAngle));
    }

    int RandomPrize()
    {
        int prizeNumber = 0;
        int randomNumber = Random.Range(1, 70);
        if (1 <= randomNumber && randomNumber <= 10)
        {
            prizeNumber = 0;
        }
        else if (10 < randomNumber && randomNumber < 20)
        {
            prizeNumber = 1;
        }
        else if (20 < randomNumber && randomNumber <= 30)
        {
            prizeNumber = 2;
        }
        else if (30 < randomNumber && randomNumber <= 40)
        {
            prizeNumber = 3;
        }
        else if (40 < randomNumber && randomNumber <= 50)
        {
            prizeNumber = 4;
        }
        else if (50 < randomNumber && randomNumber <= 60)
        {
            prizeNumber = 5;
        }
        else if (60 < randomNumber && randomNumber <= 70)
        {
            prizeNumber = 6;
        }


        return prizeNumber;
    }

    IEnumerator SpinTheWheel(float time, float maxAngle)
    {

        _arrow.isKinematic = false;
        float timer = 0.0f;
        float startAngle = transform.eulerAngles.z;
        maxAngle = maxAngle - startAngle;

        int animationCurveNumber = Random.Range(0, animationCurves.Count);
        Debug.Log("Animation Curve No. : " + animationCurveNumber);

        while (timer < time)
        {
            //to calculate rotation
            float angle = maxAngle * animationCurves[animationCurveNumber].Evaluate(timer / time);
            _spinWheel.eulerAngles = new Vector3(0.0f, 0.0f, angle + startAngle);
            for (int i = 0; i < _prizeIcon.Length; i++)
            {
                _prizeIcon[i].transform.rotation = Quaternion.identity;
            }
            timer += Time.deltaTime + 0.05f;
            yield return 0;
        }

        _spinWheel.eulerAngles = new Vector3(0.0f, 0.0f, maxAngle + startAngle);


        _arrow.isKinematic = true;
      
    }


    public void CloseThisDialog()
    {
        this.gameObject.SetActive(false);
    }
}
