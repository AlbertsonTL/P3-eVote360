using AutoMapper;
using eVote360.Application.Abstractions.Repositories;
using eVote360.Application.Abstractions.Services;
using eVote360.Application.Common.Security;
using eVote360.Application.DTOs.Request;
using eVote360.Application.DTOs.Response;
using eVote360.Application.Results;
using eVote360.Domain.Entities;
using eVote360.Domain.ValueObjects;

namespace eVote360.Application.Services;

public class UserService : IUserService
{
    private readonly IUsuarioRepository _repository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IMapper _mapper;

    public UserService(IUsuarioRepository repository, IPasswordHasher passwordHasher, IMapper mapper)
    {
        _repository = repository;
        _passwordHasher = passwordHasher;
        _mapper = mapper;
    }

    public async Task<Result<IEnumerable<UserResponseDto>>> GetAllAsync()
    {
        var users = await _repository.GetAllAsync();
        var dtos = _mapper.Map<IEnumerable<UserResponseDto>>(users);
        return Result<IEnumerable<UserResponseDto>>.Success(dtos);
    }

    public async Task<Result<UserResponseDto>> GetByIdAsync(int id)
    {
        var user = await _repository.GetByIdAsync(id);
        if (user == null)
            return Result<UserResponseDto>.Failure("Usuario no encontrado");

        var dto = _mapper.Map<UserResponseDto>(user);
        return Result<UserResponseDto>.Success(dto);
    }

    public async Task<Result<UserResponseDto>> CreateAsync(UserCreateRequestDto dto)
    {
        var exists = await _repository.GetByUsernameAsync(dto.NombreUsuario);
        if (exists != null)
            return Result<UserResponseDto>.Failure("Ya existe un usuario con ese nombre de usuario");

        var user = new Usuario
        {
            Nombre = dto.Nombre,
            Apellido = dto.Apellido,
            Email = new EmailAddress(dto.Email),
            NombreUsuario = dto.NombreUsuario,
            PasswordHash = _passwordHasher.HashPassword(dto.Password),
            Rol = dto.Rol,
            IsActive = true
        };

        await _repository.AddAsync(user);
        var responseDto = _mapper.Map<UserResponseDto>(user);
        return Result<UserResponseDto>.Success(responseDto);
    }

    public async Task<Result<UserResponseDto>> UpdateAsync(UserUpdateRequestDto dto)
    {
        var user = await _repository.GetByIdAsync(dto.Id);
        if (user == null)
            return Result<UserResponseDto>.Failure("Usuario no encontrado");

        var exists = await _repository.ExistsAsync(u => u.NombreUsuario == dto.NombreUsuario && u.Id != dto.Id);
        if (exists)
            return Result<UserResponseDto>.Failure("Ya existe otro usuario con ese nombre de usuario");

        user.Nombre = dto.Nombre;
        user.Apellido = dto.Apellido;
        user.Email = new EmailAddress(dto.Email);
        user.NombreUsuario = dto.NombreUsuario;
        user.Rol = dto.Rol;

        if (!string.IsNullOrEmpty(dto.Password))
            user.PasswordHash = _passwordHasher.HashPassword(dto.Password);

        await _repository.UpdateAsync(user);
        var responseDto = _mapper.Map<UserResponseDto>(user);
        return Result<UserResponseDto>.Success(responseDto);
    }

    public async Task<Result> ToggleActiveAsync(int id)
    {
        var user = await _repository.GetByIdAsync(id);
        if (user == null)
            return Result.Failure("Usuario no encontrado");

        user.IsActive = !user.IsActive;
        await _repository.UpdateAsync(user);
        return Result.Success();
    }
}
