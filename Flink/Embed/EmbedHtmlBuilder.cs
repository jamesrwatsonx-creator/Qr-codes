namespace Flink.Embed;

public class EmbedHtmlBuilder(string defaultTheme) {
  private string _descriptionHtml = "";
  private string _imageHtml = "";
  private string _originalUrlHtml = "";
  private string _shortLinkId = "";
  private string _title = "";
  private string _titleHtml = "";

  public EmbedHtmlBuilder WithTitle(string? title) {
    if (title == null) return this;
    _title = title;
    _titleHtml = $"<h2 class=\"primary-foreground-color\">{title.HtmlEncode().Truncate(100)}</h2>";
    return this;
  }

  public EmbedHtmlBuilder WithOriginalUrl(string? originalUrl) {
    if (originalUrl == null) return this;
    _originalUrlHtml = $"<h3>\ud83c\udf10 {originalUrl.HtmlEncode().Truncate(40)}</h3>";
    return this;
  }

  public EmbedHtmlBuilder WithShortLinkId(string shortLinkId) {
    _shortLinkId = shortLinkId;
    return this;
  }

  public EmbedHtmlBuilder WithDescription(string? description) {
    if (description == null) return this;
    _descriptionHtml = $"<p class=\"secondary-foreground-color \">{description.HtmlEncode().Truncate(400)}</p>";
    return this;
  }

  public EmbedHtmlBuilder WithImage(string? imageUrl, string? altText) {
    if (imageUrl == null) return this;
    _imageHtml = $"<img class=\"link-preview-image\" src=\"{imageUrl}\" alt=\"{altText.HtmlEncode()}\" loading=\"lazy\"></img>";
    return this;
  }

  public string Build() {
    return $"""
            <!DOCTYPE html>
            <html lang="en">
            <head>
              <meta charset="UTF-8"/>
              <meta name="viewport" content="width=device-width, initial-scale=1.0"/>
              
              <link rel="icon" type="image/x-icon" href="/static/img/favicon.ico"/>
              <link rel="apple-touch-icon" sizes="180x180" href="/static/img/apple-touch-icon.png"/>
              <link rel="icon" type="image/png" sizes="32x32" href="/static/img/favicon-32x32.png"/>
              <link rel="icon" type="image/png" sizes="16x16" href="/static/img/favicon-16x16.png"/>

              <link rel="stylesheet" href="/themes/{defaultTheme}/embed.css"/>
              <title>{_title.HtmlEncode()}</title>
            </head>
            <body class="primary-background-color">
              <a id="short-link" href="/{_shortLinkId}" class="link-preview-container" target="_blank">
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
