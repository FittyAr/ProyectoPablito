using System.Threading.Tasks;

namespace ProyectoPablito.Application.Interfaces;

public interface IUserSettingsService
{
    int GetPageSize();
    Task SetPageSizeAsync(int pageSize);
}
