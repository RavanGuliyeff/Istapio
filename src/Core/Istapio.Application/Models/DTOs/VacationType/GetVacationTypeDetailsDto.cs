namespace Istapio.Application.Models.DTOs.VacationType;

public record GetVacationTypeDetailsDto(
    Guid Id,
    string Name,
    ICollection<GetVacationTypeJobPostDto> JobPosts
);
