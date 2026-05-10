using System.Threading.Tasks;

namespace ElectroObraApp.Application.Interfaces;

public interface IUserSettingsService
{
    int GetPageSize();
    Task SetPageSizeAsync(int pageSize);
}

