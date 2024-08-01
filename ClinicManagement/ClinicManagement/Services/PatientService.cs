using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;
using ClinicManagement.Models;
using Microsoft.Extensions.Configuration;
using Dapper;

namespace ClinicManagement.Services
{
    public class PatientService : IPatientService
    {
        private readonly string _connectionString;

        public PatientService(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }
        public async Task<int> CreatePatientAsync(PatientCreationDto patientDto)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                var patientId = -1;
                using (var command = new SqlCommand("sp_InsertPatient", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@FirstName", patientDto.FirstName);
                    command.Parameters.AddWithValue("@LastName", patientDto.LastName);
                    command.Parameters.AddWithValue("@Gender", patientDto.Gender);
                    command.Parameters.AddWithValue("@DateOfBirth", patientDto.DateOfBirth);
                    command.Parameters.AddWithValue("@IsActive", patientDto.IsActive);

                    var result = await command.ExecuteScalarAsync();
                    patientId = Convert.ToInt32(result);
                }

                foreach (var contact in patientDto.ContactInfos)
                {
                    using (var contactCommand = new SqlCommand("sp_InsertContactInfo", connection))
                    {
                        contactCommand.CommandType = CommandType.StoredProcedure;
                        contactCommand.Parameters.AddWithValue("@PatientId", patientId);
                        contactCommand.Parameters.AddWithValue("@ContactType", contact.ContactType);
                        contactCommand.Parameters.AddWithValue("@ContactDetail", contact.ContactDetail);

                        await contactCommand.ExecuteNonQueryAsync();
                    }
                }

                foreach (var address in patientDto.Addresses)
                {
                    using (var addressCommand = new SqlCommand("sp_InsertAddress", connection))
                    {
                        addressCommand.CommandType = CommandType.StoredProcedure;
                        addressCommand.Parameters.AddWithValue("@PatientId", patientId);
                        addressCommand.Parameters.AddWithValue("@AddressType", address.AddressType);
                        addressCommand.Parameters.AddWithValue("@Street", address.Street);
                        addressCommand.Parameters.AddWithValue("@City", address.City);
                        addressCommand.Parameters.AddWithValue("@State", address.State);
                        addressCommand.Parameters.AddWithValue("@ZipCode", address.ZipCode);
                        addressCommand.Parameters.AddWithValue("@Country", address.Country);

                        await addressCommand.ExecuteNonQueryAsync();
                    }
                }

                return patientId;
            }
        }

        public async Task<IEnumerable<GetPatientDto>> GetPatientsAsync(string searchTerm, int pageIndex, int pageSize)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var parameters = new
                {
                    SearchTerm = searchTerm,
                    PageIndex = pageIndex,
                    PageSize = pageSize
                };

                await connection.OpenAsync();

                var result = await connection.QueryAsync(
                    "EXEC sp_GetPatients @SearchTerm, @PageIndex, @PageSize",
                    parameters,
                    commandType: CommandType.Text
                );

                var patientResults = result.Select(row => new
                {
                    PatientId = (int)row.PatientId,
                    FirstName = (string)row.FirstName,
                    LastName = (string)row.LastName,
                    Gender = (string)row.Gender,
                    DateOfBirth = (DateTime)row.DateOfBirth,
                    IsActive = (bool)row.IsActive,
                    ContactId = (int?)row.ContactInfoID,
                    ContactType = (string)row.ContactType,
                    ContactDetail = (string)row.ContactDetail,
                    AddressId = (int?)row.AddressID,
                    AddressType = (string)row.AddressType,
                    Street = (string)row.Street,
                    City = (string)row.City,
                    State = (string)row.State,
                    ZipCode = (string)row.ZipCode,
                    Country = (string)row.Country,
                    TotalRecords = (int)row.TotalRecords
                }).ToList();

                var patientDictionary = patientResults
                    .GroupBy(p => new
                    {
                        p.PatientId,
                        p.FirstName,
                        p.LastName,
                        p.Gender,
                        p.DateOfBirth,
                        p.IsActive,
                        p.TotalRecords
                    })
                    .Select(g => new GetPatientDto
                    {
                        PatientId = g.Key.PatientId,
                        FirstName = g.Key.FirstName,
                        LastName = g.Key.LastName,
                        Gender = g.Key.Gender,
                        DateOfBirth = g.Key.DateOfBirth,
                        IsActive = g.Key.IsActive,
                        TotalRecords = g.Key.TotalRecords,
                        ContactInfos = g
                            .Where(p => p.ContactId.HasValue)
                            .GroupBy(p => new { p.ContactId, p.ContactType, p.ContactDetail })
                            .Select(cg => new GetContactInfoDto
                            {
                                ContactInfoID = cg.Key.ContactId.Value,
                                ContactType = cg.Key.ContactType,
                                ContactDetail = cg.Key.ContactDetail
                            }).ToList(),
                        Addresses = g
                            .Where(p => p.AddressId.HasValue)
                            .GroupBy(p => new { p.AddressId, p.AddressType, p.Street, p.City, p.State, p.ZipCode, p.Country })
                            .Select(ag => new GetAddressDto
                            {
                                AddressID = ag.Key.AddressId.Value,
                                AddressType = ag.Key.AddressType,
                                Street = ag.Key.Street,
                                City = ag.Key.City,
                                State = ag.Key.State,
                                ZipCode = ag.Key.ZipCode,
                                Country = ag.Key.Country
                            }).ToList()
                    }).ToList();

                return patientDictionary;
            }
        }
        public async Task<GetPatientDto> GetPatientByIdAsync(int patientId)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var parameters = new
                {
                    PatientId = patientId
                };

                await connection.OpenAsync();

                var result = await connection.QueryAsync(
                    "EXEC sp_GetPatientById @PatientId",
                    parameters,
                    commandType: CommandType.Text
                );

                var patientResults = result.Select(row => new
                {
                    PatientId = (int)row.PatientId,
                    FirstName = (string)row.FirstName,
                    LastName = (string)row.LastName,
                    Gender = (string)row.Gender,
                    DateOfBirth = (DateTime)row.DateOfBirth,
                    IsActive = (bool)row.IsActive,
                    ContactId = (int?)row.ContactInfoID,
                    ContactType = (string)row.ContactType,
                    ContactDetail = (string)row.ContactDetail,
                    AddressId = (int?)row.AddressID,
                    AddressType = (string)row.AddressType,
                    Street = (string)row.Street,
                    City = (string)row.City,
                    State = (string)row.State,
                    ZipCode = (string)row.ZipCode,
                    Country = (string)row.Country
                }).ToList();

                var patientDto = patientResults
                    .GroupBy(p => new
                    {
                        p.PatientId,
                        p.FirstName,
                        p.LastName,
                        p.Gender,
                        p.DateOfBirth,
                        p.IsActive
                    })
                    .Select(g => new GetPatientDto
                    {
                        PatientId = g.Key.PatientId,
                        FirstName = g.Key.FirstName,
                        LastName = g.Key.LastName,
                        Gender = g.Key.Gender,
                        DateOfBirth = g.Key.DateOfBirth,
                        IsActive = g.Key.IsActive,
                        ContactInfos = g
                            .Where(p => p.ContactId.HasValue)
                            .GroupBy(p => new { p.ContactId, p.ContactType, p.ContactDetail })
                            .Select(cg => new GetContactInfoDto
                            {
                                ContactInfoID = cg.Key.ContactId.Value,
                                ContactType = cg.Key.ContactType,
                                ContactDetail = cg.Key.ContactDetail
                            }).ToList(),
                        Addresses = g
                            .Where(p => p.AddressId.HasValue)
                            .GroupBy(p => new { p.AddressId, p.AddressType, p.Street, p.City, p.State, p.ZipCode, p.Country })
                            .Select(ag => new GetAddressDto
                            {
                                AddressID = ag.Key.AddressId.Value,
                                AddressType = ag.Key.AddressType,
                                Street = ag.Key.Street,
                                City = ag.Key.City,
                                State = ag.Key.State,
                                ZipCode = ag.Key.ZipCode,
                                Country = ag.Key.Country
                            }).ToList()
                    })
                    .FirstOrDefault();

                return patientDto;
            }
        }

        public async Task UpdatePatientAsync(UpdatePatientDto patientDto)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        var updatePatientParams = new
                        {
                            PatientId = patientDto.PatientId,
                            FirstName = patientDto.FirstName,
                            LastName = patientDto.LastName,
                            Gender = patientDto.Gender,
                            DateOfBirth = patientDto.DateOfBirth                       
                        };
                        await connection.ExecuteAsync("sp_UpdatePatient", updatePatientParams, transaction, commandType: CommandType.StoredProcedure);

                        foreach (var contact in patientDto.ContactInfos)
                        {
                            var updateContactParams = new
                            {
                                ContactInfoID = contact.ContactInfoID,
                                ContactType = contact.ContactType,
                                ContactDetail = contact.ContactDetail
                            };
                            await connection.ExecuteAsync("sp_UpdateContactInfo", updateContactParams, transaction, commandType: CommandType.StoredProcedure);
                        }

                        foreach (var address in patientDto.Addresses)
                        {
                            var updateAddressParams = new
                            {
                                AddressID = address.AddressID,
                                AddressType = address.AddressType,
                                Street = address.Street,
                                City = address.City,
                                State = address.State,
                                ZipCode = address.ZipCode,
                                Country = address.Country
                            };
                            await connection.ExecuteAsync("sp_UpdateAddress", updateAddressParams, transaction, commandType: CommandType.StoredProcedure);
                        }

                        transaction.Commit();
                    }
                    catch (Exception)
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }
        }

        public async Task DeactivatePatientAsync(DeactivatePatientDto deactivatePatientDto)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                var parameters = new
                {
                    PatientId = deactivatePatientDto.PatientId,
                    InactiveReason = deactivatePatientDto.InactiveReason
                };
                await connection.ExecuteAsync("sp_DeactivatePatient", parameters, commandType: CommandType.StoredProcedure);

            }
        }
    }
}
