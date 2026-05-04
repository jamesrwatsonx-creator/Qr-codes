namespace Flink.Embed;

public class EmbedHtmlBuilder(string defaultTheme) {
  private string _imageHtml = "";
  private string _shortLinkId = "";
  private string _title = "";
  private string _description = "";
  private string _shortLinkUrl = "";
  private string _imageUrl = "";

  public EmbedHtmlBuilder WithTitle(string? title) {
    if (title == null) return this;
    _title = title;
    return this;
  }

  public EmbedHtmlBuilder WithShortUrl(string? originalUrl) {
    if (originalUrl == null) return this;
    _shortLinkUrl = originalUrl;
    return this;
  }

  public EmbedHtmlBuilder WithShortLinkId(string shortLinkId) {
    _shortLinkId = shortLinkId;
    return this;
  }

  public EmbedHtmlBuilder WithDescription(string? description) {
    if (description == null) return this;
    _description = description;
    return this;
  }

  public EmbedHtmlBuilder WithImage(string? imageUrl, string? altText) {
    if (imageUrl == null) return this;
    _imageUrl = imageUrl;
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

              <meta content="Shortened URL" name="keywords">
              <meta content="{_description.HtmlEncode()}" name="description">
              <meta content="{_title.HtmlEncode()}" property="og:title"/>
              <meta content="{_description.HtmlEncode()}" property="og:description"/>
              <meta content="{_imageUrl}" property="og:image"/>
              <meta content="website" property="og:type"/>
              <meta content="en_US" property="og:locale"/>
              <meta content="summary_large_image" name="twitter:card"/>
              <meta content="{_title.HtmlEncode()}" name="twitter:title"/>
              <meta content="{_description.HtmlEncode()}" name="twitter:description"/>
              <meta content="{_imageUrl}" name="twitter:image"/>

              <link rel="stylesheet" href="/themes/{defaultTheme}/embed.css"/>
              <title>{_title.HtmlEncode()}</title>
            </head>
            <body class="primary-background-color">
            <a id="short-link" href="/{_shortLinkId}" class="link-preview-container" target="_blank" rel="noopener noreferrer" aria-label="{_title}">
              <article class="link-preview">
                {_imageHtml}
                <div class="link-content">
                  <h3 class="link-title">{_title.HtmlEncode().Truncate(80)}</h3>
                  <p class="link-description">{_description.HtmlEncode().Truncate(300)}</p>
                  <span class="link-url">🌐 {_shortLinkUrl.HtmlEncode()?.Replace("https://", "").Replace("http://", "")}</span>
                </div>
              </article>
            </a>
            </body>
            </html>
            """;
  }
}
