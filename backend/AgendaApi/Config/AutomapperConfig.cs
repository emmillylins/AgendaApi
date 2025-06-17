using Application.DTOs;
using Domain.Entities;
using AutoMapper;

namespace AgendaApi.Config
{
    public class AutomapperConfig : Profile
    {
        public AutomapperConfig()
        {
            CreateMap<AgendaDTO, Agenda>().ReverseMap();
            CreateMap<CreateAgendaDTO, Agenda>().ReverseMap();
        }
    }
}
