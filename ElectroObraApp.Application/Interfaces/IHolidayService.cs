using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ElectroObraApp.Application.Interfaces;

public interface IHolidayService
{
    Task<List<HolidayModel>> GetHolidaysAsync(int year);
}

public class HolidayModel
{
    public DateTime Date { get; set; }
    public string Name { get; set; } = string.Empty;
}
