namespace ClinicManagement.Models
{
    public class PatientCreationDto
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Gender { get; set; }
        public DateTime DateOfBirth { get; set; }
        public bool IsActive { get; set; }
        public List<ContactInfoDto> ContactInfos { get; set; }
        public List<AddressDto> Addresses { get; set; }
    }

    public class ContactInfoDto
    {
        public string ContactType { get; set; }
        public string ContactDetail { get; set; }

    }

    public class AddressDto
    {
        public string AddressType { get; set; }
        public string Street { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string ZipCode { get; set; }
        public string Country { get; set; }

    }

    public class GetPatientsRequest
    {
        public string SearchTerm { get; set; }
        public int PageIndex { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }

    public class GetPatientDto
    {
        public int PatientId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Gender { get; set; }
        public DateTime DateOfBirth { get; set; }
        public bool IsActive { get; set; }
        public List<GetContactInfoDto> ContactInfos { get; set; }
        public List<GetAddressDto> Addresses { get; set; }
        public int TotalRecords { get; set; }
    }
    public class GetContactInfoDto
    {
        public int ContactInfoID { get; set; }  
        public string ContactType { get; set; }
        public string ContactDetail { get; set; }

    }
    public class GetAddressDto
    {
        public int AddressID { get; set; }
        public string AddressType { get; set; }
        public string Street { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string ZipCode { get; set; }
        public string Country { get; set; }

    }

    public class UpdatePatientDto
    {
        public int PatientId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Gender { get; set; }
        public DateTime DateOfBirth { get; set; }
        public List<UpdateContactInfoDto> ContactInfos { get; set; }
        public List<UpdateAddressDto> Addresses { get; set; }
    }
    public class UpdateContactInfoDto
    {
        public int ContactInfoID { get; set; }
        public string ContactType { get; set; }
        public string ContactDetail { get; set; }

    }
    public class UpdateAddressDto
    {
        public int AddressID { get; set; }
        public string AddressType { get; set; }
        public string Street { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string ZipCode { get; set; }
        public string Country { get; set; }

    }
    public class DeactivatePatientDto 
    {
        public int PatientId { get; set; }
        public string InactiveReason { get; set; }

    }
}
