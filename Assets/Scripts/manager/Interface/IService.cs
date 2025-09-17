using System.Threading.Tasks;

namespace manager.Interface
{
    public interface IService
    {
        Task<bool> Initialize();
    }
}