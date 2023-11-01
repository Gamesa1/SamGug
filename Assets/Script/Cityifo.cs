using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using System.IO;
using UnityEngine.UI;

[System.Serializable]
public class Citydata
{
    public int Gold, population, influence, develop, man, noman, Safety, Commerce, CityCode;
    public string CityName = "도시";
    public int cityinfluence; // 세력 번호를 저장하는 변수
    public string cityinfluencname; // 세력 이름을 저장하는 변수

    public Citydata(int cityCode)
    {
        CityCode = cityCode;
        CityName = "도시 " + cityCode;
    }
}



public class Cityifo : MonoBehaviour
{

    public int PlayerInfluence;
    public string PlayerInfluencess;

    private Vector3 originalScale;

    public float highlightedScale = 1.2f; // 강조된 스케일

    private float updateInterval = 0.1f; // 세력 업데이트 주기 (예: 10초)
    private float timeSinceLastUpdate = 0.0f;
    private Dictionary<int, Color> influenceColors = new Dictionary<int, Color>();
    [SerializeField]
    private Button[] cityButtons = new Button[54];
    public Dictionary<int, string> influenceNames = new Dictionary<int, string>();
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

    private Button lastHighlightedButton; // 이전에 강조된 버튼을 추적하기 위한 변수

    private void Awake()
    {
        // 세력 번호와 세력 이름을 매핑하는 딕셔너리 초기화
        influenceNames.Add(1, "유비");
        influenceNames.Add(2, "손책");
        influenceNames.Add(3, "조조");
        influenceNames.Add(4, "원소");
        influenceNames.Add(5, "원술");
        influenceNames.Add(6, "동탁");
        influenceNames.Add(10, "재야");


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

        TextAsset cityInfluenceText = Resources.Load<TextAsset>("cityInfluence");
        
        TextAsset cityInfluencecolorText = Resources.Load<TextAsset>("cityInfluencecolor");

        if (cityInfluenceText != null)
        {
            string cityInfluenceData = cityInfluenceText.text;
            string[] lines = cityInfluenceData.Split('\n');

            foreach (string line in lines)
            {
                // 각 라인을 파싱하여 도시 번호와 세력 코드를 얻습니다.
                string[] parts = line.Split(':');
                if (parts.Length == 2)
                {
                    if (int.TryParse(parts[0], out int cityCode) && int.TryParse(parts[1], out int cityInfluence))
                    {
                        // 해당 도시 번호를 가진 Citydata 객체를 찾아서 cityinfluence를 설정합니다.
                        Citydata city = cities.Find(c => c.CityCode == cityCode);
                        if (city != null)
                        {
                            city.cityinfluence = cityInfluence;
                        }
                    }
                }
            }
        }

        
        if (cityInfluencecolorText != null)
        {
            string cityInfluencecolorData = cityInfluencecolorText.text;
            string[] lines = cityInfluencecolorData.Split('\n');

            foreach (string line in lines)
            {
                string[] parts = line.Split(':');
                if (parts.Length == 2)
                {
                    if (int.TryParse(parts[0], out int cityInfluence))
                    {
                        string[] rgbValues = parts[1].Split(',');
                        if (rgbValues.Length == 3 && int.TryParse(rgbValues[0], out int red) && int.TryParse(rgbValues[1], out int green) && int.TryParse(rgbValues[2], out int blue))
                        {
                            Color color = new Color(red / 255f, green / 255f, blue / 255f);
                            influenceColors[cityInfluence] = color;
                        }
                    }
                }
            }
        }

        Save();
        originalScale = cityButtons[0].transform.localScale;

        LoadData();

        for (int i = 0; i < cityButtons.Length; i++)
        {
            int cityCode = i + 1; // 도시 코드 (예: 1, 2, 3, ...)

            // 각 버튼에 대한 클릭 이벤트 함수 등록
            cityButtons[i].onClick.AddListener(() => ChangeButtonColor(cityCode));
        }
    }

    void Update()
    {

        //플레이어 세팅
        if(PlayerInfluence == 10 ){

        }

        TextAsset cityInfluenceloadText = Resources.Load<TextAsset>("cityInfluenceload");
        if (cityInfluenceloadText != null)
        {
            string cityInfluenceloadData = cityInfluenceloadText.text;
            string[] lines = cityInfluenceloadData.Split('\n');

            foreach (string line in lines)
            {
                // 각 라인을 파싱하여 도시 번호와 세력 코드를 얻습니다.
                string[] parts = line.Split(":");
                if (parts.Length == 2)
                {
                    if (int.TryParse(parts[0], out int cityinfluence))
                    {
                        // 해당 도시 번호를 가진 모든 Citydata 객체를 찾아서 cityinfluencname을 설정합니다.
                        foreach (Citydata city in cities)
                        {
                            if (city.cityinfluence == cityinfluence)
                            {
                                city.cityinfluencname = parts[1];
                            }
                        }
                    }
                }
            }
        }
        if (cityInfluenceloadText != null)
        {
            string cityInfluenceloadData = cityInfluenceloadText.text;
            string[] lines = cityInfluenceloadData.Split('\n');

            foreach (string line in lines)
            {
                string[] parts = line.Split(':');
                if (parts.Length == 2)
                {
                    if (int.TryParse(parts[0], out int playerInfluence))
                    {
                        string playerInfluencess = parts[1].Trim(); // 세력 이름을 얻습니다.

                        // 여기서 PlayerInfluence와 PlayerInfluencess 변수에 값을 할당합니다.
                        PlayerInfluence = playerInfluence;
                        PlayerInfluencess = playerInfluencess;
                    }
                }
            }
        }
        
        // Cityifom을 초기화하고 나서 LoadData를 호출
        Cityifom = new Citydata(citycodeifom);
        
        timeSinceLastUpdate += Time.deltaTime;

        if (timeSinceLastUpdate >= updateInterval)
        {
            UpdateCityColors();
            timeSinceLastUpdate = 0.0f;
        }
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
                    if (influenceNames.ContainsKey(loadedCityData.cityinfluence))
                    {
                        loadedCityData.cityinfluencname = influenceNames[loadedCityData.cityinfluence]; // 세력 이름을 할당
                    }
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

    private void ChangeButtonColor(int cityCode)
    {
        // cityCode에 해당하는 버튼의 이미지 컴포넌트 색상 변경
        if (lastHighlightedButton != null)
        {
            RestoreButtonColor(lastHighlightedButton);
        }

        // 현재 버튼을 강조
        HighlightButton(cityButtons[cityCode - 1], cityCode);
        lastHighlightedButton = cityButtons[cityCode - 1];
    }

    private void HighlightButton(Button button, int cityCode)
    {
        // 버튼을 강조하는 코드를 여기에 작성
        Image buttonImage = button.image;
        if (buttonImage != null)
        {
            Citydata cityData = GetCityData(cityCode);
            if (influenceColors.TryGetValue(cityData.cityinfluence, out Color influenceColor))
            {
                buttonImage.color = influenceColor;
                buttonImage.transform.localScale = originalScale * highlightedScale;
            }
        }
    }

    private void RestoreButtonColor(Button button)
    {
        // 버튼을 원래대로 복원하는 코드를 여기에 작성
        Image buttonImage = button.image;
        if (buttonImage != null)
        {
            buttonImage.transform.localScale = originalScale;
        }
    }

    private IEnumerator RemoveHighlightAfterDelay(ButtonHighlight buttonHighlight)
    {
        yield return new WaitForSeconds(1.0f); // 1초 대기

        // 강조 표시 제거 (크기 원래대로)
        buttonHighlight.Unhighlight();
    }
    
    private void UpdateCityColors()
    {
        // 현재 세력 색 정보를 가져옴
        foreach (Citydata city in cities)
        {
            if (influenceColors.TryGetValue(city.cityinfluence, out Color influenceColor))
            {
                SetButtonColor(city.CityCode, influenceColor);
            }
        }
    }

    private void SetButtonColor(int cityCode, Color influenceColor)
    {
        if (cityCode >= 1 && cityCode <= cityButtons.Length)
        {
            // 도시 코드가 버튼 배열의 인덱스와 일치하면 해당 버튼의 이미지 색상 변경
            Image buttonImage = cityButtons[cityCode - 1].image;
            if (buttonImage != null)
            {
                buttonImage.color = influenceColor;
            }
            
        }
    }
}