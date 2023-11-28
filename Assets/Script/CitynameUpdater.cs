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
    public TextMeshProUGUI cityAgriculture;
    public TextMeshProUGUI citySafety;
    public TextMeshProUGUI cityCommerce;
    public TextMeshProUGUI cityman;
    public TextMeshProUGUI citynoman;
    public TextMeshProUGUI cityinfluenceCity;
    
    public Cityifo cityifoScript;
    public int citycodeifom;

    public TextMeshProUGUI PlayerInfluences;
    public TextMeshProUGUI PlayerCity;
    public TextMeshProUGUI Action;

    void Start()
    {
        Playdata playData = cityifoScript.playData;
        TextAsset cityCodeMatchText = Resources.Load<TextAsset>("cityCodematch");
        if (cityCodeMatchText != null)
        {
            string cityCodeMatchData = cityCodeMatchText.text;
            string[] lines = cityCodeMatchData.Split('\n');

            foreach (string line in lines)
            {
                // 각 라인을 파싱하여 도시 코드와 도시 이름을 얻습니다.
                string[] parts = line.Split(':');
                if (parts.Length == 2)
                {
                    if (int.TryParse(parts[0], out int parsedCityCode))
                    {
                        if (parsedCityCode == playData.playerinCity) // 현재 도시 코드와 일치하는 도시 이름을 찾았다면
                        {
                            cityNameText.text  = parts[1].Trim(); // 도시 이름을 얻습니다.
                            break;
                        }
                    }
                }
            }
        }
        else
        {
            Debug.LogError("cityCodematch.txt 파일이 존재하지 않습니다.");
        }
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

    public void UpdateCityData(int citycodeifo)
    {
        Playdata playData = cityifoScript.playData;
        TextAsset cityCodeMatchText = Resources.Load<TextAsset>("cityCodematch");
        
        citycodeifom = citycodeifo;
        if (cityNameText != null && cityifoScript != null)
        {
            List<Citydata> cityList = cityifoScript.Cities;
            // 원하는 도시 데이터에 접근하여 업데이트합니다.
            Citydata cityData = cityList[citycodeifo]; // 예: 리스트의 첫 번째 도시 데이터

            // 도시 이름을 TextMeshProUGUI에 업데이트
            cityGold.text =  cityData.Gold.ToString();
            cityMil.text = cityData.influence.ToString();
            citypopulation.text =  cityData.population.ToString();
            citydevelop.text = cityData.develop.ToString();
            cityinfluence.text =  cityData.influence.ToString();
            cityAgriculture.text =  cityData.Agriculture.ToString();
            citySafety.text = cityData.Safety.ToString();
            cityCommerce.text =  cityData.Commerce.ToString();
            cityman.text =  cityData.man.ToString();
            citynoman.text =  cityData.noman.ToString();
            cityinfluenceCity.text =  cityData.cityinfluencname;
            
            PlayerInfluences.text = "세력: "+ cityifoScript.PlayerInfluencess;
            Action.text = "행동력: " + playData.action.ToString();
            cityNameText.text = cityData.CityName;
            if (cityCodeMatchText != null)
            {
                string cityCodeMatchData = cityCodeMatchText.text;
                string[] lines = cityCodeMatchData.Split('\n');

                foreach (string line in lines)
                {
                    // 각 라인을 파싱하여 도시 코드와 도시 이름을 얻습니다.
                    string[] parts = line.Split(':');
                    if (parts.Length == 2)
                    {
                        if (int.TryParse(parts[0], out int parsedCityCode))
                        {
                            if (parsedCityCode == playData.playerinCity) // 현재 도시 코드와 일치하는 도시 이름을 찾았다면
                            {
                                PlayerCity.text  = "주인공 위치 도시: "+parts[1].Trim(); // 도시 이름을 얻습니다.
                                break;
                            }
                        }
                    }
                }
            }
            else
            {
                Debug.LogError("cityCodematch.txt 파일이 존재하지 않습니다.");
            }
        }
        else
        {
            Debug.LogError("cityNameText or cityifoScript is not assigned.");
        }
    }
}