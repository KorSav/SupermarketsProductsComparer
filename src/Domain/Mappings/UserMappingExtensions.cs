using Microsoft.AspNetCore.Identity;
using program.Models;
using program.Models.User;

namespace program.Domain.Mappings;

public static class UserMappingExtensions{
    public static User ToUser(this UserLogInViewModel userViewModel){
        return new User(){
            Name = userViewModel.Name,
            Email = userViewModel.Email,
            PasswordHash = userViewModel.Password
        };
    }

    public static User ToUser(this UserRegisterViewModel userViewModel){
        PasswordHasher<string> passwordHasher = new();
        return new User(){
            Name = userViewModel.Name,
            Email = userViewModel.Email,
            PasswordHash = passwordHasher.HashPassword(userViewModel.Name, userViewModel.Password)
        };
    }
}