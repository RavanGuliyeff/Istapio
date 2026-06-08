using AutoMapper;
using Istapio.Application.Models.DTOs.JobPost;
using Istapio.Domain.Entities;

namespace Istapio.Application.Profiles;

public class JobPostProfile : Profile
{
    public JobPostProfile()
    {
        CreateMap<JobPost, GetJobPostDto>();
        CreateMap<JobPost, CreateJobPostDto>();
        CreateMap<JobPost, UpdateJobPostDto>();
    }
}