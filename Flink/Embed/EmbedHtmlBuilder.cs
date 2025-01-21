namespace Flink.Embed;

public class EmbedHtmlBuilder {
  private string _title = "";
  private string _titleHtml = "";
  private string _shortLinkId = "";
  private string _originalUrlHtml = "";
  private string _imageHtml = "";
  private string _descriptionHtml = "";

  public EmbedHtmlBuilder WithTitle(string? title) {
    if (title == null) return this;
    _title = title;
    _titleHtml = $"<h2 class=\"primary-foreground-color\">{title}</h2>";
    return this;
  }

  public EmbedHtmlBuilder WithOriginalUrl(string? originalUrl) {
    if (originalUrl == null) return this;
    _originalUrlHtml = $"<h3>\ud83c\udf10 {originalUrl}</h3>";
    return this;
  }

  public EmbedHtmlBuilder WithShortLinkId(string shortLinkId) {
    _shortLinkId = shortLinkId;
    return this;
  }

  public EmbedHtmlBuilder WithDescription(string? description) {
    if (description == null) return this;
    _descriptionHtml = $"<p class=\"secondary-foreground-color \">{description}</p>";
    return this;
  }
  
  public EmbedHtmlBuilder WithImage(string? imageUrl, string? altText) {
    if (imageUrl == null) return this;
    _imageHtml = $"<img class=\"link-preview-image\" src=\"{imageUrl}\" alt=\"{altText}\"></img>";
    return this;
  }

  public string Build() {
    return $"""
             <!DOCTYPE html>
             <html lang="en">
             <head>
               <meta charset="UTF-8">
               <meta name="viewport" content="width=device-width, initial-scale=1.0">
               <link rel="stylesheet" href="/static/embed.css"/>
               <title>{_title}</title>
             </head>
             <body class="primary-background-color">
               <a id="short-link" href="/to/{_shortLinkId}" class="link-preview-container">
                 <div class="link-preview primary-background-color">
                   {_imageHtml}
                   <div class="link-content">
                     {_titleHtml}
                     {_originalUrlHtml}
                     {_descriptionHtml}
                   </div>
                 </div>
               </a>
             </body>
             </html>
             """;
  }
}
