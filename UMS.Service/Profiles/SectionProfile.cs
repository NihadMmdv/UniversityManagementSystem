using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UMS.DAL.Entities;
using UMS.Service.DTOs.SectionDTOs;

namespace UMS.Service.Profiles
{
    public class SectionProfile : Profile
    {
        public SectionProfile()
        {
            CreateMap<Section, SectionCreateDTO>()
                .ForMember(
                    dest => dest.LessonIds,
                    opt => opt.MapFrom(src => src.Lessons.Select(l => l.Id)));
            CreateMap<Section, SectionGetDTO>()
                .ForMember(
                    dest => dest.LessonIds,
                    opt => opt.MapFrom(src => src.Lessons.Select(l => l.Id)));
            CreateMap<SectionCreateDTO, Section>()
                .ForMember(dest => dest.Lessons, opt => opt.Ignore())
                .ForMember(dest => dest.Students, opt => opt.Ignore())
                .ForMember(dest => dest.Id, opt => opt.Ignore());
            CreateMap<SectionPatchDTO, Section>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForAllMembers(opt =>
                opt.Condition((src, dest, srcMember) => srcMember != null));
            CreateMap<Section, SectionPatchDTO>();
        }
    }

}
