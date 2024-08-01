using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Data.SqlClient;
using ClinicManagement.Models;
using System.Threading.Tasks;
using ClinicManagement.Services;

namespace ClinicManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PatientsController : ControllerBase
    {
        private readonly IPatientService _patientService;

        public PatientsController(IPatientService patientService)
        {
            _patientService = patientService;
        }
        // Create Patient 
        [HttpPost("CreatePatient")]
        public async Task<IActionResult> CreatePatient([FromBody] PatientCreationDto patientDto)
        {
            if (patientDto == null)
            {
                return BadRequest("Patient data is required.");
            }
            try
            {
                var result = await _patientService.CreatePatientAsync(patientDto);

                if (result > 0)
                {
                    return Ok(new { Message = "Patient created successfully." });
                }
                else
                {
                    return StatusCode(StatusCodes.Status500InternalServerError, "Error occurred while creating the patient.");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }

        }

        // Seclect Patients by Search 
        [HttpPost("GetPatients")]
        public async Task<IActionResult> GetPatients([FromBody] GetPatientsRequest request)
        {
            var patients = await _patientService.GetPatientsAsync(request.SearchTerm, request.PageIndex, request.PageSize);
            return Ok(patients);
        }

        // Select Patient By Id 
        [HttpPost("GetPatientById")]
        public async Task<IActionResult> GetPatientsById([FromQuery] int Id)
        {
            var patients = await _patientService.GetPatientByIdAsync(Id);
            return Ok(patients);
        }

        // Update patient
        [HttpPost("UpdatePatient")]
        public async Task<IActionResult> UpdatePatient([FromBody] UpdatePatientDto patientDto)
        {
            if (patientDto == null)
            {
                return BadRequest("Invalid patient data.");
            }

            try
            {
                await _patientService.UpdatePatientAsync(patientDto);
                return Ok("Patient updated successfully.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // Deactive Patient
        [HttpPost("DeactivatePatient")]
        public async Task<IActionResult> DeactivatePatient([FromBody] DeactivatePatientDto deactivatePatientDto)
        {
            if (deactivatePatientDto == null || deactivatePatientDto.PatientId <= 0 || string.IsNullOrEmpty(deactivatePatientDto.InactiveReason))
            {
                return BadRequest("Invalid patient data.");
            }
            try
            {
                await _patientService.DeactivatePatientAsync(deactivatePatientDto);
                return Ok("Patient deactivated successfully.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}
