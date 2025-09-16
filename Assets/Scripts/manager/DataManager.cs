namespace manager
{
    public interface IDataManager
    {
        public int GetInt(string key, int defaultValue);
        public void SetInt(string key, int value);
        
        public string GetString(string key, string defaultValue);
        public void SetString(string key, string value);
        
        public float GetFloat(string key, float defaultValue);
        public void SetFloat(string key, float value);
    }
}