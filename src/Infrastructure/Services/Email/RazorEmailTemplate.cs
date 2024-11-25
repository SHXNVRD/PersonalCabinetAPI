using System.Text.RegularExpressions;
using Application.DTOs.Emails;
using Application.Interfaces.Email;
using Infrastructure.RazorTemplates.EmailTemplates.Shared;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using RazorLight;

namespace Infrastructure.Services.Email
{
    public class RazorEmailTemplate : IEmailTemplate
    {
        private readonly IRazorLightEngine _razorEngine;

        public RazorEmailTemplate(IRazorLightEngine razorEngine)
        {
            _razorEngine = razorEngine;
        }

        public async Task<EmailBody> CompileAsync(string templateName, object model)
        {
            var html = await _razorEngine.CompileRenderAsync(templateName, model);
            return new EmailBody(html, string.Empty);
        }
    }
}