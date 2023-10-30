using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using System.IO;

[System.Serializable]
public class Citydata
{
    public int Gold, population, influence, develop, man, noman, Safety, Commerce, CityCode;
    public string CityName = "도시";

    public Citydata(int cityCode)
    {
        CityCode = cityCode;
        CityName = "도시 " + cityCode;
    }
}

public class Cityifo : MonoBehaviour
{
    public List<Citydata> Cities
    {
        get { return cities; }
    }
    public List<Citydata> cities = new List<Citydata>();

    public int citycodeifom = 2;

    public static Cityifo instance;

    public Citydata Cityifom;

    public string path;
    string filename;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(instance.gameObject);
        }
        DontDestroyOnLoad(this.gameObject);

        path = Application.persistentDataPath;
        filename = "save" + citycodeifom + ".json";
        LoadData(); // 저장된 데이터 로드
    }

    void Start()
    {
        LoadData();
    }

    void Update()
    {
        // Cityifom을 초기화하고 나서 LoadData를 호출
        Cityifom = new Citydata(citycodeifom);
        LoadData();
    }

    public void Save()
    {
        for (int cityCode = 1; cityCode <= 54; cityCode++)
        {
            SaveData(cityCode);
        }
        Debug.Log("저장 완료");
    }

    public void SaveData(int cityCode)
    {
        string data = JsonUtility.ToJson(GetCityData(cityCode));
        File.WriteAllText(Path.Combine(path, "save" + cityCode + ".json"), data);
    }

    public void LoadData()
    {
        for (int code = 1; code <= 54; code++)
        {
            string filePath = Path.Combine(path, "save" + code + ".json");
            if (File.Exists(filePath))
            {
                string data = File.ReadAllText(filePath);
                Citydata loadedCityData = JsonUtility.FromJson<Citydata>(data);

                // 기존 도시 데이터가 있으면 덮어쓰지 않음
                if (GetCityData(code) == null)
                {
                    cities.Add(loadedCityData);
                }
            }
            else
            {
                Citydata city = new Citydata(code)
                {
                    Gold = UnityEngine.Random.Range(100, 1000),
                    population = UnityEngine.Random.Range(1000, 10000),
                    influence = UnityEngine.Random.Range(100, 1000),
                    develop = UnityEngine.Random.Range(1000, 10000),
                    man = UnityEngine.Random.Range(100, 1000),
                    noman = UnityEngine.Random.Range(1000, 10000),
                    Safety = UnityEngine.Random.Range(100, 1000),
                    Commerce = UnityEngine.Random.Range(1000, 10000),
                };
                cities.Add(city);
            }
        }
    }

    private Citydata GetCityData(int cityCode)
    {
        // 도시 데이터를 가져오는 메서드
        foreach (Citydata city in cities)
        {
            if (city.CityCode == cityCode)
            {
                return city;
            }
        }
        return null; // 해당 도시 데이터를 찾을 수 없을 경우
    }

}