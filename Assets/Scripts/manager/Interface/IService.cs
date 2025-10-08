using Cysharp.Threading.Tasks;

namespace manager.Interface
{
    public interface IService
    {
        UniTask<bool> Initialize();
    }
}