using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CityNameUpdater : MonoBehaviour
{
    public float updateInterval = 0.01f;
    private float timeSinceLastUpdate;
    public TextMeshProUGUI cityNameText;
    public TextMeshProUGUI cityGold;
    public TextMeshProUGUI cityMil;
    public TextMeshProUGUI citypopulation;
    public TextMeshProUGUI citydevelop;
    public TextMeshProUGUI cityinfluence;
    public TextMeshProUGUI citySafety;
    public TextMeshProUGUI cityCommerce;
    public TextMeshProUGUI cityman;
    public TextMeshProUGUI citynoman;
    public Cityifo cityifoScript;
    public int citycodeifom;

    void Start()
    {
        cityNameText.text = "낙양";
        timeSinceLastUpdate = 0.0f;
    }

    void Update()
    {
        timeSinceLastUpdate += Time.deltaTime;
        if (timeSinceLastUpdate >= updateInterval)
        {
            if (cityifoScript != null)
            {
                // 도시 데이터 업데이트 메서드 호출
                UpdateCityData(citycodeifom);
            }
            else
            {
                Debug.LogError("cityifoScript is not assigned.");
            }

            timeSinceLastUpdate = 0.0f;
        }
    }
    public void CityName(string city)
    {
        cityNameText.text = city; // 문자열을 TextMeshProUGUI에 할당
    }

    public void UpdateCityData(int citycodeifo)
    {
        citycodeifom = citycodeifo;
        if (cityNameText != null && cityifoScript != null)
        {
            List<Citydata> cityList = cityifoScript.Cities;
            // 원하는 도시 데이터에 접근하여 업데이트합니다.
            Citydata cityData = cityList[citycodeifo]; // 예: 리스트의 첫 번째 도시 데이터

            // 도시 이름을 TextMeshProUGUI에 업데이트
            cityGold.text = "Gold: " + cityData.Gold.ToString();
            cityMil.text = "Military: " + cityData.influence.ToString();
            citypopulation.text = "Population: " + cityData.population.ToString();
            citydevelop.text = "Development: " + cityData.develop.ToString();
            cityinfluence.text = "Influence: " + cityData.influence.ToString();
            citySafety.text = "Safety: " + cityData.Safety.ToString();
            cityCommerce.text = "Commerce: " + cityData.Commerce.ToString();
            cityman.text = "Male: " + cityData.man.ToString();
            citynoman.text = "Female: " + cityData.noman.ToString();
        }
        else
        {
            Debug.LogError("cityNameText or cityifoScript is not assigned.");
        }
    }
}