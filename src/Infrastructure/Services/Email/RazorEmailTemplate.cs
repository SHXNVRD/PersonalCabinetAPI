using System.Text.RegularExpressions;
using Application.DTOs.Emails;
using Application.Interfaces.Email;
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
            var htmlContent = await _razorEngine.CompileRenderAsync(templateName + ".cshtml", model);
            var plainTextContent = ExtractTextFromHtml(htmlContent);
            
            return new EmailBody(htmlContent, plainTextContent);
        }

        private string ExtractTextFromHtml(string html)
        {
            var text = Regex.Replace(html, "(<.*?>\\s*)+", " ");
            text = Regex.Replace(text, @"\s+", " ").Trim();
            
            return text;
        }
    }
}