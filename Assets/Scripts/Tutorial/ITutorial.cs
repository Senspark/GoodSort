namespace Tutorial
{
    public enum TutorialType
    {
        Onboarding
    }

    public enum TutorialActionType
    {
        OnClear
    }
    
    public interface ITutorial
    {
        public void Start();
        public void Finish();
        public bool CheckStart();
        public bool CheckFinish();
        public string GetCurrentTargetName();
    }
    
    public class Step 
    {
        public string targetObjectName; 
        public TutorialActionType actionType; 
        public string eventParameter;
        public System.Action OnStepStart;
    }

    public static class TutorialEvents
    {
        public static System.Action<TutorialActionType, string> OnActionReached;
        public static void SendEvent(TutorialActionType actionType, string param = "") => OnActionReached?.Invoke(actionType, param);
    }
}