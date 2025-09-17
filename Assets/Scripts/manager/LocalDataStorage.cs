using UnityEngine;
using manager.Interface;

namespace manager
{
    public class LocalDataStorage: IDataStorage
    {
        public int GetInt(string key, int defaultValue)
        {
            return PlayerPrefs.GetInt(key, defaultValue);
        }
        
        public float GetFloat(string key, float defaultValue)
        {
            return PlayerPrefs.GetFloat(key, defaultValue);
        }
        
        public string GetString(string key, string defaultValue)
        {
            return PlayerPrefs.GetString(key, defaultValue);
        }
        
        public void SetInt(string key, int value)
        {
            PlayerPrefs.SetInt(key, value);
        }
        
        public void SetFloat(string key, float value)
        {
            PlayerPrefs.SetFloat(key, value);
        }
        
        public void SetString(string key, string value)
        {
            PlayerPrefs.SetString(key, value);
        }
    }
}