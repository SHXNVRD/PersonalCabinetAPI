using Application.Interfaces;
using Domain.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Infrastructure.Data.IdentityValidators;

public class UserPhoneNumberValidator<TUser> : IUserValidator<TUser>
    where TUser : User
{
    private readonly IUnitOfWork _unitOfWork;

    public UserPhoneNumberValidator(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<IdentityResult> ValidateAsync(UserManager<TUser> manager, TUser user)
    {
        List<IdentityError> errors = [];

        var owner = await _unitOfWork.UserRepository.GetByPhoneNumber(user.PhoneNumber);

        if (owner != null && owner.Id != user.Id)
            errors.Add(new IdentityError
            {
                Code = "DuplicatePhone",
                Description = $"Phone number {user.PhoneNumber} is already taken."
            });
        
        return errors?.Count > 0
            ? IdentityResult.Failed(errors.ToArray())
            : IdentityResult.Success;
    }
}