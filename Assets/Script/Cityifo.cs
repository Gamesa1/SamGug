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
    private Cityifo cityifo;
    public int Gold, population, influence, develop, man, noman, Safety, Commerce, CityCode, Agriculture;
    public string CityName = "도시";
    public string cityinfluencname; // 세력 이름을 저장하는 변수
    public int cityinfluence;

    public Citydata(int cityCode)
    {
        CityCode = cityCode;

        // cityCodematch.txt 파일을 읽어옵니다.
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
                        if (parsedCityCode == cityCode) // 현재 도시 코드와 일치하는 도시 이름을 찾았다면
                        {
                            CityName = parts[1].Trim(); // 도시 이름을 얻습니다.
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
}


[System.Serializable]
public class Playdata
{
    public int action;
    public int playerinCity; //플레이어가 있는 도시
    public int playCity; 

    public Playdata()
    {
        action = 1;
        playerinCity = 1;
        playCity = 1;
    }

}


public class Cityifo : MonoBehaviour
{
    [SerializeField]
    public Playdata playData;

    public int PlayerInfluence;
    public string PlayerInfluencess;

    public GameObject CityInCurrect;

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

    public int citycodeifom;
    public static Cityifo instance;

    public Citydata Cityifom;
    public bool PlayerMove = false;

    public string path;

    private Button lastHighlightedButton; // 이전에 강조된 버튼을 추적하기 위한 변수

    private void Awake()
    {
        influenceNames.Add(1, "유비");
        influenceNames.Add(2, "손책");
        influenceNames.Add(3, "조조");
        influenceNames.Add(4, "원소");
        influenceNames.Add(5, "원술");
        influenceNames.Add(6, "동탁");
        influenceNames.Add(10, "재야");
        // 세력 번호와 세력 이름을 매핑하는 딕셔너리 초기화
        

        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(instance.gameObject);
        }
        DontDestroyOnLoad(this.gameObject);
        if(playData.playerinCity == 0 || playData.playCity == 0){
            playData = new Playdata();
        }
        path = Application.persistentDataPath;
        LoadData(); // 저장된 데이터 로드
    }

    void Start()
    {
        playData = LoadPlayData();

        string copyFilePath = Path.Combine(Application.persistentDataPath, "cityInfluence_copy.txt");
        TextAsset cityInfluenceTextOrg = Resources.Load<TextAsset>("cityInfluence");
        TextAsset cityInfluencecolorText = Resources.Load<TextAsset>("cityInfluencecolor");

        string originalFilePath = Path.Combine(Application.dataPath, "Resources/cityInfluence.txt");

        if(!File.Exists(copyFilePath)){
            if (File.Exists(originalFilePath))
            {
                string cityInfluenceData = File.ReadAllText(originalFilePath);
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
            else
            {
                Debug.LogError("cityInfluence.txt 파일이 존재하지 않습니다.");
            }
        }
        CopyCityInfluenceFileToStreamingAssets();
        if (File.Exists(copyFilePath))
        {
            string cityInfluenceData = File.ReadAllText(copyFilePath);
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
        LoadCopiedCityInfluenceFileFromStreamingAssets();
        UpdateCityInfluenceFileFromJson();
        //플레이어 세팅
        if(PlayerInfluence == 10 ){

        }
        LoadData();
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
        SavePlayData(playData);
        Debug.Log("저장 완료");
    }

    public void SaveData(int cityCode)
    {
        string data = JsonUtility.ToJson(GetCityData(cityCode));
        File.WriteAllText(Path.Combine(path, "save" + cityCode + ".json"), data);
    }

    public void LoadData()
    {
        LoadPlayData();
        for (int code = 1; code <= 54; code++)
        {
            Citydata cityData = GetCityData(code);
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
                    Agriculture =UnityEngine.Random.Range(1000, 10000),
                    man = UnityEngine.Random.Range(100, 1000),
                    noman = UnityEngine.Random.Range(1000, 10000),
                    Safety = UnityEngine.Random.Range(100, 1000),
                    Commerce = UnityEngine.Random.Range(1000, 10000),
                };
                cities.Add(city);
            }
        }

    }

    
    public void SavePlayData(Playdata playData)
    {
        string data = JsonUtility.ToJson(playData);
        File.WriteAllText(Path.Combine(path, "playData.json"), data);
        Debug.Log("Play data 저장 완료");
    }

    public Playdata LoadPlayData()
    {
        string filePath = Path.Combine(path, "playData.json");
        if (File.Exists(filePath))
        {
            string data = File.ReadAllText(filePath);
            return JsonUtility.FromJson<Playdata>(data);
        }
        else
        {
            Debug.LogError("playData.json 파일이 존재하지 않습니다.");
            return null;
        }
    }
/**
    private Playdata GetPalyData()
    {
        foreach(Playdata playdata in playData)
        {
            return playdata;
        }
    } **/



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
    void CopyCityInfluenceFileToStreamingAssets()
    {
        string originalFilePath = Path.Combine(Application.dataPath, "Resources/cityInfluence.txt");
        string copyFilePath = Path.Combine(Application.persistentDataPath, "cityInfluence_copy.txt");
        if(!File.Exists(copyFilePath)){
            if (File.Exists(originalFilePath))
            {
                // 원본 파일의 내용을 읽어와서 복사본 파일에 저장
                string cityInfluenceData = File.ReadAllText(originalFilePath);
                File.WriteAllText(copyFilePath, cityInfluenceData);

                Debug.Log("cityInfluence.txt를 Application.persistentDataPath 폴더에 복사했습니다.");
            }
            else
            {
                Debug.LogError("cityInfluence.txt 파일이 존재하지 않습니다.");
            }
        }
        else
        {
            Debug.LogError("cityInfluence_copy.txt  파일이 존재합니다.");
        }
    }

    void LoadCopiedCityInfluenceFileFromStreamingAssets()
    {
        string copyFilePath = Path.Combine(Application.persistentDataPath, "cityInfluence_copy.txt");

        if (File.Exists(copyFilePath))
        {
            // 복사본 파일의 내용을 읽어옴
            string cityInfluenceData = File.ReadAllText(copyFilePath);
        }
        else
        {
            Debug.LogError("cityInfluence_copy.txt 파일을 로드할 수 없습니다.");
        }
    }

    void UpdateCityInfluenceFileFromJson()
    {
        string copyFilePath = Path.Combine(Application.persistentDataPath, "cityInfluence_copy.txt");

        // 파일이 존재하는지 확인
        if (File.Exists(copyFilePath))
        {
            string[] lines = File.ReadAllLines(copyFilePath);

            for (int i = 0; i < cities.Count; i++)
            {
                int cityCode = cities[i].CityCode;
                int cityInfluence = cities[i].cityinfluence;

                // 해당 도시 코드에 대한 라인을 찾아서 업데이트
                for (int j = 0; j < lines.Length; j++)
                {
                    string[] parts = lines[j].Split(':');
                    if (parts.Length == 2)
                    {
                        if (int.TryParse(parts[0], out int currentCityCode) && currentCityCode == cityCode)
                        {
                            // 새로운 세력 코드로 라인을 업데이트
                            lines[j] = cityCode + ":" + cityInfluence;
                            break;
                        }
                    }
                }
            }
            // 업데이트된 내용을 파일에 쓰기
            File.WriteAllLines(copyFilePath, lines);
        }
        else
        {
            Debug.LogError("cityInfluence_copy.txt 파일이 존재하지 않습니다.");
        }
    }

    //여기서부터 게임 플레이에 지장이 가는 버튼 함수

    public void ChangePlayCity(int newCity) // 지금 보고있는 플레이어 도시 버튼누르면 바꾸기
    {
        playData.playCity = newCity;
    }

    //인사
    public void ClickMove(){
        PlayerMove = true;
    }
    
    public void Move(int where){
        if(PlayerMove)
        {
            playData.playerinCity = where;
            PlayerMove = false;
        }
    }

    // 여기서 부터는 내정

    public void Internaltechnology()
    {
        int cityCode = playData.playerinCity;
        
        Citydata cityData = GetCityData(cityCode);
        if(playData.playerinCity == playData.playCity)
        {
            if(cityData != null)
            {
                cityData.develop += 10;
            }
            else
            {
                Debug.LogError("도시 데이터를 찾을 수 없습니다. 도시 코드를 확인해 주세요.");
            }
        }
        else
        {
            CityInCurrect.SetActive(true);
        }
    }

    public void InternalAgriculture(){
        int cityCode = playData.playerinCity;
        
        Citydata cityData = GetCityData(cityCode);
        if(cityCode == playData.playCity)
        {
            if(cityData != null)
            {
                cityData.Agriculture += 10;
            }
            else
            {
                Debug.LogError("도시 데이터를 찾을 수 없습니다. 도시 코드를 확인해 주세요.");
            }
        }
        else
        {
            CityInCurrect.SetActive(true);
        }
    }

}

