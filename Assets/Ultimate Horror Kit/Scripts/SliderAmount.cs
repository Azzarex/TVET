using UnityEngine;
using UnityEngine.UI;

public class SliderAmount : MonoBehaviour
{

    public Slider slider;
    private Text myText;
    [HideInInspector]
    public float CurrentSliderValue;
    private void Awake()
    {
        myText = GetComponent<Text>();
    }
    void Update()
    {
        CurrentSliderValue = slider.value;
        myText.text = CurrentSliderValue.ToString();
    }
}
