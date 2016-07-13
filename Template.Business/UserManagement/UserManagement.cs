using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autofac;
using Template.Repository;
using DevOne.Security.Cryptography.BCrypt;
using Template.Business.Support;
using System.Data.Entity.Validation;

namespace Template.Business.Security
{
    public class UserManagement
    {
        public static CustomIdentity Login(string email, string password)
        {
            using(var lifetimescope = IoCContainer.BeginLifetimeScope())
            using (var repository = lifetimescope.Resolve<IRepository<Users>>())
            {
                try
                {
                    var existingUser = repository.GetSingle<Users>(x =>
                        x.Email.Equals(email, StringComparison.InvariantCultureIgnoreCase) &&
                        x.UserMembership.Any());

                    if (existingUser != null)
                    {
                        var correctPassword = BCryptHelper.CheckPassword(password, existingUser.UserMembership.FirstOrDefault().PasswordHash);
                        if(correctPassword)
                            return new CustomIdentity(true, existingUser.Email) 
                            { 
                                Roles = existingUser.UserRoles.Select(x => x.Roles.RoleName).ToList(),
                                IsActive = existingUser.IsActive
                            };
                        else
                            return new CustomIdentity(false, String.Empty);
                    }
                    else
                    {
                        //user does not exist 
                        return new CustomIdentity(false, String.Empty);
                    }
                }
                catch (Exception e)
                {
                    Logger.LogEvent("GetIdentity", e);
                    return new CustomIdentity(false, String.Empty);
                }
            }
        }

        public static bool Register(string email, string password)
        {
            using(var lifetimescope = IoCContainer.BeginLifetimeScope())
            using (var repository = lifetimescope.Resolve<IRepository<Users>>())
            {
                try
                {
                    var user = new Users
                    {
                        Email = "auzunel@hotmail.com",
                        FirstName = "Ali",
                        LastName = "Uzunel",
                        IsActive = true,
                        CreatedOn = DateTime.UtcNow,
                        CreatedBy = 1
                    };

                    var salt = BCryptHelper.GenerateSalt();
                    user.UserMembership.Add(new UserMembership
                    {
                        PasswordSalt = salt,
                        PasswordHash = BCryptHelper.HashPassword(password, salt),
                        IsLocked = false,
                        CreatedOn = DateTime.UtcNow,
                        CreatedBy = 1
                    });

                    user.UserRoles.Add(new UserRoles
                    {
                        RoleId = 1
                    });

                    repository.Add(user);
                }
                catch (Exception e)
                {
                    Logger.LogEvent("UserManagement.Register", e);
                    return false;
                }
                //catch (DbEntityValidationException e)
                //{
                //    foreach (var eve in e.EntityValidationErrors)
                //    {
                //        foreach (var ve in eve.ValidationErrors)
                //        {
                //            Console.WriteLine("- Property: \"{0}\", Error: \"{1}\"",
                //                ve.PropertyName, ve.ErrorMessage);
                //        }
                //    }
                //    throw;
                //}
            }
            return true;
        }

        public static CustomIdentity Load(string email)
        {
            using (var lifetimescope = IoCContainer.BeginLifetimeScope())
            using (var repository = lifetimescope.Resolve<IRepository<Users>>())
            {
                try
                {
                    var existingUser = repository.GetSingle<Users>(x =>
                        x.Email.Equals(email, StringComparison.InvariantCultureIgnoreCase) &&
                        x.UserMembership.Any());

                    if (existingUser != null)
                    {
                        return new CustomIdentity(true, existingUser.Email)
                        {
                            Roles = existingUser.UserRoles.Select(x => x.Roles.RoleName).ToList(),
                            IsActive = existingUser.IsActive
                        };
                    }
                    else
                    {
                        //user does not exist 
                        return new CustomIdentity(false, String.Empty);
                    }
                }
                catch (Exception e)
                {
                    Logger.LogEvent("GetIdentity", e);
                    return new CustomIdentity(false, String.Empty);
                }
            }
        }
    }
}
