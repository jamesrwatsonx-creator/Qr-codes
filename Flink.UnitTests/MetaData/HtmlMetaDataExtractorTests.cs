using Flink.MetaData;
using FluentAssertions;

namespace Flink.UnitTests.MetaData;

public class HtmlMetaDataExtractorTests {
  [Fact]
  public void BlogHtmlContent_PassedToMetaDataExtractor_ReturnsPredefinedMetaDataDict() {
    var metaData = HtmlMetaDataExtractor.ExtractMetadata(SamplePayloads.FocusriteArticleHead);
    metaData.Should().ContainKeys("title", "og:title", "og:description", "author", "twitter:title", "twitter:description", "description");
  }

  [Fact]
  public void InvalidHtmlContent_PassedToMetaDataExtractor_ReturnsEmptyMetaDataDict() {
    var metaData = HtmlMetaDataExtractor.ExtractMetadata("<invalidHTML<");
    metaData.Should().BeEmpty();
  }
}
