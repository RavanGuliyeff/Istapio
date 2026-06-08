using AutoMapper;
using Istapio.Application.Models.DTOs.Company;
using Istapio.Domain.Entities;

namespace Istapio.Application.Profiles;

public class CompanyProfile : Profile
{
    public CompanyProfile()
    {
        CreateMap<Company, GetCompanyDto>();
        CreateMap<Company, CreateCompanyDto>();
        CreateMap<Company, UpdateCompanyDto>();
    }
}