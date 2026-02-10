using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UMS.DAL.Entities;
using UMS.Service.DTOs.ExamDTOs;

namespace UMS.Service.Profiles
{
    public class ExamProfile:Profile
    {
        public ExamProfile()
        {
            CreateMap<Exam, ExamGetDTO>();
            CreateMap<Exam, ExamCreateDTO>();
            CreateMap<ExamCreateDTO, Exam>()
                .ForMember(dest => dest.Id, opt => opt.Ignore());
            CreateMap<ExamPatchDTO, Exam>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForAllMembers(opt =>
                opt.Condition((src, dest, srcMember) => srcMember != null));
            CreateMap<Exam, ExamPatchDTO>();
        }
    }
}
