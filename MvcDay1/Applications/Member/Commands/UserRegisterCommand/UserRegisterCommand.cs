using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using MailKit.Net.Smtp;
using MailKit.Security;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.Extensions.Configuration;
using MimeKit;
using MvcDay1.Data;
using MvcDay1.Models;

namespace MvcDay1.Applications.Member.Commands.UserRegisterCommand
{
    public class UserRegisterCommandValidator : AbstractValidator<UserRegisterCommand>
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public UserRegisterCommandValidator(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
            RuleFor(c => c.Email)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithMessage("อีเมลห้ามว่าง")
                .EmailAddress()
                .WithMessage("อีเมลไม่ถูกต้อง")
                .MustAsync(NoDuplicateEmail)
                .WithMessage("อีเมลซ้ำในระบบ");

            RuleFor(c => c.Username)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithMessage("รหัสผู้ใช้ห้ามว่าง")
                .MustAsync(NoDuplicateUsername)
                .WithMessage("รหัสผู้ใช้ซ้ำในระบบ");

            RuleFor(c => c.FirstName)
                .NotEmpty()
                .WithMessage("ชื่อห้ามว่าง");

            RuleFor(c => c.LastName)
                .NotEmpty()
                .WithMessage("นามสกุลห้ามว่าง");

            RuleFor(c => c.Password)
                .NotEmpty()
                .WithMessage("รหัสผ่านห้ามว่าง");

            RuleFor(c => c.ConfirmPassword)
                .Equal(c => c.Password)
                .When(c => !string.IsNullOrEmpty(c.Password))
                .WithMessage("รหัสผ่านยืนยันไม่ตรงกัน");

        }

        private async Task<bool> NoDuplicateEmail(string? email, CancellationToken token)
        {
            var exist = await _userManager.FindByEmailAsync(email);
            if (exist != null)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        private async Task<bool> NoDuplicateUsername(string? username, CancellationToken token)
        {
            var exist = await _userManager.FindByNameAsync(username);
            if (exist != null)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
    }

    public class UserRegisterCommand : IRequest<bool>
    {
        public string? Email { get; set; }
        public string? Username { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Password { get; set; }
        public string? ConfirmPassword { get; set; }

        public class UserRegisterCommandHandler : IRequestHandler<UserRegisterCommand, bool>
        {
            private readonly UserManager<ApplicationUser> _userManager;
            private readonly IConfiguration _configuration;

            public UserRegisterCommandHandler(UserManager<ApplicationUser> userManager, IConfiguration configuration)
            {
                _userManager = userManager;
                _configuration = configuration;
            }

            public async Task<bool> Handle(UserRegisterCommand request, CancellationToken cancellationToken)
            {
                var appUser = new ApplicationUser()
                {
                    UserName = request.Username,
                    Email = request.Email,
                    FirstName = request.FirstName ?? "",
                    LastName = request.LastName ?? "",
                    EmailConfirmed = true
                };
                var result = await _userManager.CreateAsync(appUser, request.Password);

                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(appUser, "Moderator");
                    EmailSettingModel settings = new EmailSettingModel();
                    _configuration.GetSection("Email").Bind(settings);

                    var message = new MimeMessage();
                    var bodyBuilder = new BodyBuilder();

                    message.From.Add(new MailboxAddress(settings!.User, settings!.User));
                    message.To.Add(new MailboxAddress(request.FirstName + " " + request.LastName, request.Email));

                    message.Subject = "การลงทะเบียนสำเร็จ";
                    bodyBuilder.HtmlBody = $"รหัสผู้ใช้ คือ {request.Username} <br/>รหัสผ่าน คือ {request.Password}";
                    message.Body = bodyBuilder.ToMessageBody();

                    var client = new SmtpClient();
                    client.ServerCertificateValidationCallback = (s, c, h, e) => true;
                    await client.ConnectAsync(settings!.Host, settings!.Port, SecureSocketOptions.StartTls, cancellationToken);
                    await client.AuthenticateAsync(settings!.User, settings!.Password, cancellationToken);
                    await client.SendAsync(message, cancellationToken);
                    await client.DisconnectAsync(true, cancellationToken);

                    return true;
                }
                else
                {
                    throw new ValidationException("ไม่สามารถสร้างบัญชีได้ รหัสผ่านอาจไม่ผ่านนโยบายความปลอดภัย");
                }

            }
        }
    }
}
