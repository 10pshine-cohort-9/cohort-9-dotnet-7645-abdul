using System.Reflection;

namespace TaskManagement.Infrastructure.Email.Services;

public sealed class EmailTemplateService
{
    private readonly string _templatePath;

    public EmailTemplateService()
    {
        _templatePath = Path.Combine(
            Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!,
            "Email",
            "Templates");
    }

    public async Task<string> GetTemplateAsync(
        string templateName,
        Dictionary<string, string> placeholders)
    {
        var file = Path.Combine(_templatePath, templateName);

        if (!File.Exists(file))
            throw new FileNotFoundException(file);

        var html = await File.ReadAllTextAsync(file);

        foreach (var item in placeholders)
        {
            html = html.Replace(
                $"{{{{{item.Key}}}}}",
                item.Value);
        }

        html = html.Replace(
            "{{CurrentYear}}",
            DateTime.UtcNow.Year.ToString());

        return html;
    }
}