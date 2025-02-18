using AutoMapper;
using E_CommerceApplication.BLL.DTO;
using E_CommerceApplication.BLL.Models;

namespace E_CommerceApplication.Helpers
{
    public class MappingProfiles:Profile
    {
        public MappingProfiles()
        {
            //CreateMap<Assessments, AssessmentsDto>()
            //    .ReverseMap();


            CreateMap<Contact, ContactDTO>().ReverseMap();

        }
    }
}
