using AutoMapper;
using eVote360.Application.Abstractions.Repositories;
using eVote360.Application.Abstractions.Services;
using eVote360.Application.DTOs.Request;
using eVote360.Application.DTOs.Response;
using eVote360.Application.Results;
using eVote360.Domain.Entities.Citizen;
using eVote360.Domain.ValueObjects;

namespace eVote360.Application.Services;

public class CitizenService : ICitizenService
{
    private readonly ICitizenRepository _repository;
    private readonly IMapper _mapper;

    public CitizenService(ICitizenRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<Result<IEnumerable<CitizenResponseDto>>> GetAllAsync()
    {
        var citizens = await _repository.GetAllAsync();
        var dtos = _mapper.Map<IEnumerable<CitizenResponseDto>>(citizens);
        return Result<IEnumerable<CitizenResponseDto>>.Success(dtos);
    }

    public async Task<Result<CitizenResponseDto>> GetByIdAsync(int id)
    {
        var citizen = await _repository.GetByIdAsync(id);
        if (citizen == null)
            return Result<CitizenResponseDto>.Failure("Ciudadano no encontrado");

        var dto = _mapper.Map<CitizenResponseDto>(citizen);
        return Result<CitizenResponseDto>.Success(dto);
    }

    public async Task<Result<CitizenResponseDto>> CreateAsync(CitizenCreateRequestDto dto)
    {
        var exists = await _repository.GetByNationalIdAsync(dto.NumeroDocumento);
        if (exists != null)
            return Result<CitizenResponseDto>.Failure("Ya existe un ciudadano con ese número de documento");

        var citizen = new Citizen
        {
            Nombre = dto.Nombre,
            Apellido = dto.Apellido,
            Email = new EmailAddress(dto.Email),
            NumeroDocumento = new NationalId(dto.NumeroDocumento),
            IsActive = true
        };

        await _repository.AddAsync(citizen);
        var responseDto = _mapper.Map<CitizenResponseDto>(citizen);
        return Result<CitizenResponseDto>.Success(responseDto);
    }

    public async Task<Result<CitizenResponseDto>> UpdateAsync(CitizenUpdateRequestDto dto)
    {
        var citizen = await _repository.GetByIdAsync(dto.Id);
        if (citizen == null)
            return Result<CitizenResponseDto>.Failure("Ciudadano no encontrado");

        var exists = await _repository.ExistsDifferentCitizenWithNationalIdAsync(dto.Id, dto.NumeroDocumento);
        if (exists)
            return Result<CitizenResponseDto>.Failure("Ya existe otro ciudadano con ese número de documento");

        citizen.Nombre = dto.Nombre;
        citizen.Apellido = dto.Apellido;
        citizen.Email = new EmailAddress(dto.Email);
        citizen.NumeroDocumento = new NationalId(dto.NumeroDocumento);

        await _repository.UpdateAsync(citizen);
        var responseDto = _mapper.Map<CitizenResponseDto>(citizen);
        return Result<CitizenResponseDto>.Success(responseDto);
    }

    public async Task<Result> ToggleActiveAsync(int id)
    {
        var citizen = await _repository.GetByIdAsync(id);
        if (citizen == null)
            return Result.Failure("Ciudadano no encontrado");

        citizen.IsActive = !citizen.IsActive;
        await _repository.UpdateAsync(citizen);
        return Result.Success();
    }
}
