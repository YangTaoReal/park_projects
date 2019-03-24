using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

[System.Serializable]
public enum WeaterType
{
    Sun,  //晴
    Rain, //雨
}

[System.Serializable]
public class TimeRangWeather
{
    public float from;
    public float to;
    public WeaterType weaterType;
    public bool clearCloud;
}


[System.Serializable]
public class WeatherData
{
    public TimeRangWeather[] data = new TimeRangWeather[0];
    public long nextRandomWeatherTime;
    public bool allSunOrRain;
}



public class Weather : MonoBehaviour {

    private static Weather _instance = null;
    public static Weather Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = GameObject.FindObjectOfType<Weather>();
            }
            return _instance;
        }
    }

    public delegate void WeatherEvent();
    public WeatherEvent OnWeatherChange;

    public bool enableCloud = true; //云层开关
    public GameObject cloudPanel;
    public GameObject rain;
    WeatherData weatherData{
        get{
            return data;
        }
        set{
            data = value;
        }
    }


    public float timeUnit = 1; //时间段（单位：小时)
    public int minSun;
    public int maxSun;

    Light _sun = null;
    Light sun{
        get
        {
            if(_sun == null)
            {
                _sun = GameObject.FindObjectOfType<Light>();
            }

            return _sun;
        }
    }


    WeatherData data = new WeatherData();
    int curIdx = -1;

	// Use this for initialization
	void Start () {
        ReadData();
	}

    //是否晴天
    public bool IsSunDay{
        get{
            return GetCurWeatherType() == WeaterType.Sun;
        }
    }

    //是否雨天
    public bool IsRainDay
    {
        get
        {
            return GetCurWeatherType() == WeaterType.Rain;
        }
    }

    //雨天数量
    public int RainDayCount{
        get{
            int ret = 0;
            for (int i = 0; i < data.data.Length; i++)
            {
                if (data.data[i].weaterType == WeaterType.Rain)
                {
                    ret++;
                }
            }

            return ret;
        }
    }

    // 清理云层
    public void ClearCloud()
    {
        if (curIdx >= 0 && curIdx < data.data.Length)
        {
            data.data[curIdx].clearCloud = true;
            //data.data[curIdx].weaterType = WeaterType.Sun;
            WeatherChange();
        } 
    }

    //获取当前天气
    public TimeRangWeather GetCurWeather(){
        if(curIdx >= 0 && curIdx < data.data.Length){
            return data.data[curIdx];
        }
        else{
            return null;
        }
    }

    //获取当前天气
    public WeaterType GetCurWeatherType()
    {
        if (curIdx >= 0 && curIdx < data.data.Length)
        {
            return data.data[curIdx].weaterType;
        }
        else
        {
            return WeaterType.Sun;
        }
    }

    //获取下一时段天气
    public TimeRangWeather GetNextWeather()
    {
        if (curIdx+1 >= 0 && curIdx+1 < data.data.Length)
        {
            return data.data[curIdx+1];
        }
        else
        {
            return null;
        }
    }

    //获取剩余全天天气
    public List<TimeRangWeather> GetAllDayWeather()
    {
        List<TimeRangWeather> ret = new List<TimeRangWeather>();
        for (int i = 0; i < data.data.Length; i++){
            if(i >= curIdx){
                ret.Add(data.data[i]);
            }
        }

        return ret;
    }

    //导出字符串制数据
    public string SerializeToString()
    {
        return Convert.ToBase64String(Serialize());
    }

    //导出二进制数据
    public byte[] Serialize()
    {
        BinaryFormatter bf = new BinaryFormatter();
        MemoryStream ms = new MemoryStream();
        bf.Serialize(ms, data);
        var ret = ms.ToArray();
        ms.Close();

        return ret;
    }

    //导入字符串制数据
    public bool DeserializeFromString(string str)
    {
        return Deserialize(Convert.FromBase64String(str));
    }

    //导入二进制数据
    public bool Deserialize(byte[] _data)
    {
        if (_data == null || _data.Length == 0) return false;

        MemoryStream inData = new MemoryStream(_data);
        BinaryFormatter bf = new BinaryFormatter();
        data = (WeatherData)bf.Deserialize(inData);
        inData.Close();

        return true;
    }

    static bool RandomBool
    {
        get{
            if (UnityEngine.Random.Range(0, 100) < 50)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }

    void RandomNextDayWeather(){
        
        DateTime now = DateTime.Now;
        var weekDay = now.DayOfWeek;

        bool allSunOrRain = false;
        if(weekDay == DayOfWeek.Sunday || weekDay == DayOfWeek.Saturday){
            if (data.allSunOrRain) //前一天是特殊天气
            {
                allSunOrRain = false;
            }
            else   //前一天是正常天气
            {
                if (weekDay == DayOfWeek.Sunday)
                {
                    allSunOrRain = true;
                }
                else if (RandomBool)
                {
                    allSunOrRain = true;
                }
            }
        }


        data.nextRandomWeatherTime = DateTime.Now.AddDays(1.0f).Ticks;
        data.allSunOrRain = allSunOrRain;

        if (allSunOrRain)
        {
            RandomSpecial();
        }
        else{
            RandomNomarl();
        }

        SaveData();
    }

    void RandomNomarl()
    {
        int allUnit = (int)(24 / timeUnit);
        
        int sunNum = UnityEngine.Random.Range(minSun, maxSun + 1);
        List<int> allList = new List<int>();
        List<int> sunList = new List<int>();
        for (int i= 0; i < allUnit; i++){
            allList.Add(i);
        }
        int idxTemp;

        while(sunNum > 0 && allList.Count > 0){
            idxTemp = UnityEngine.Random.Range(0, allList.Count);
            sunList.Add(allList[idxTemp]);
            allList.RemoveAt(idxTemp);
            sunNum--;
        }

        List<WeaterType> typeList = new List<WeaterType>();
        for (int i = 0; i < allUnit;i++){
            typeList.Add(WeaterType.Rain);
        }
        for (int i = 0; i < sunList.Count; i++)
        {
            typeList[sunList[i]] = WeaterType.Sun;
        }

        data.data = new TimeRangWeather[allUnit];
        for (byte i = 0; i < allUnit; i++)
        {
            var t = new TimeRangWeather();
            t.from = i*timeUnit;
            t.to =  (i + 1)*timeUnit;
            t.weaterType = typeList[i];
            t.clearCloud = false;

            data.data[i] = t;
        }
    }

    void RandomSpecial(){

        int allUnit = (int)(24 / timeUnit);
        
        WeaterType sunOrRain = WeaterType.Sun;
        if (RandomBool)
        {
            sunOrRain = WeaterType.Rain;
        }
        else
        {
            sunOrRain = WeaterType.Sun;
        }

        data.data = new TimeRangWeather[allUnit];
        for (byte i = 0; i < allUnit; i++)
        {
            var t = new TimeRangWeather();
            t.from = i * timeUnit;
            t.to = (i + 1) * timeUnit;
            t.weaterType = sunOrRain;
            t.clearCloud = false;

            data.data[i] = t;
        }
    }

    // Update is called once per frame
    private void FixedUpdate()
    {
        DateTime now = DateTime.Now;
        int tempIdx = (int)(now.Hour / timeUnit);

        if (now.Ticks >= data.nextRandomWeatherTime)
        {
            RandomNextDayWeather();

            curIdx = tempIdx;
            WeatherChange();
        }


        if (curIdx != tempIdx)
        {
            curIdx = tempIdx;
            WeatherChange();
        }

        var dataCur = GetCurWeather();
        if (dataCur.weaterType == WeaterType.Rain)
        {
            if (sun && sun.intensity > 0.5f)
            {
                sun.intensity -= 0.005f;
            }
        }
        else if (dataCur.weaterType == WeaterType.Sun)
        {
            if (sun && sun.intensity < 1f)
            {
                sun.intensity += 0.005f;
            }
        }
    }

    void WeatherChange(){
        var dataCur = GetCurWeather();
        if(dataCur.weaterType == WeaterType.Rain){
            //if(sun) sun.intensity = 0.5f;
            rain.gameObject.SetActive(true);
            if(dataCur.clearCloud == false && enableCloud){
                cloudPanel.gameObject.SetActive(true);
            }
            else{
                cloudPanel.gameObject.SetActive(false);
            }
        }else if(dataCur.weaterType == WeaterType.Sun){
            //if (sun) sun.intensity = 1.0f;
            rain.gameObject.SetActive(false);
            cloudPanel.gameObject.SetActive(false);
        }

        SaveData();

        if (OnWeatherChange != null)
        {
            OnWeatherChange();
        }
    }

    void ReadData()
    {
        var save = PlayerPrefs.GetString("weatherData", "");
        if (string.IsNullOrEmpty(save))
            return;

        DeserializeFromString(save);
    }

    void SaveData()
    {
        var save = SerializeToString();
        if (string.IsNullOrEmpty(save))
            return;

        PlayerPrefs.SetString("weatherData", save);
    }
}
