namespace Tutorial
{
    public enum TutorialType
    {
        Onboarding
    }

    public enum TutorialActionType
    {
        OnClick,
        OnDrag,
        OnDrop
    }
    
    public interface ITutorial
    {
        public void Start();
        public void Finish();
        public bool CheckStart();
        public bool CheckFinish();
    }

    public interface TutorialData
    {
        
    }

    public static class TutorialEvents
    {
        public static System.Action<TutorialActionType, string> OnActionReached;
        public static void SendEvent(TutorialActionType actionType, string param = "") => OnActionReached?.Invoke(actionType, param);
    }
}