using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;
using ClinicManagement.Models;
using Microsoft.Extensions.Configuration;
namespace ClinicManagement.Services
{
    public interface IPatientService
    {
        Task<int> CreatePatientAsync(PatientCreationDto patientDto);
        Task<IEnumerable<GetPatientDto>> GetPatientsAsync(string searchTerm, int pageIndex, int pageSize);
        Task<GetPatientDto> GetPatientByIdAsync(int patientId);
        Task UpdatePatientAsync(UpdatePatientDto patientDto);
        Task DeactivatePatientAsync(DeactivatePatientDto deactivatePatientDto);
    }
}
