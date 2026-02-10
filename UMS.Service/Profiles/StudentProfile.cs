using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UMS.DAL.Entities;
using UMS.Service.DTOs.StudentDTOs;

namespace UMS.Service.Profiles
{
    public class StudentProfile:Profile
    {
        public StudentProfile()
        {
            CreateMap<Student, StudentGetDTO>();
            CreateMap<Student, StudentCreateDTO>();
            CreateMap<StudentCreateDTO, Student>()
                .ForMember(dest => dest.Id, opt => opt.Ignore());
            CreateMap<StudentPatchDTO, Student>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForAllMembers(opt =>
                opt.Condition((src, dest, srcMember) => srcMember != null));
            CreateMap<Student, StudentPatchDTO>();
        }
    }
}
