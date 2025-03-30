using Flink.SpamProtection.Ports.BusinessLogic;
using FluentAssertions;

namespace Flink.UnitTests.SpamProtection;

public class EasyListParserTests {
  private const string EasyListHeaderExample = """
                                               [Adblock Plus 2.0]
                                               ! Version: 202503301046
                                               ! Title: EasyList
                                               ! Last modified: 30 Mar 2025 10:46 UTC
                                               ! Expires: 4 days (update frequency)
                                               ! *** easylist:template_header.txt ***
                                               ! 
                                               ! Please report any unblocked adverts or problems
                                               ! in the forums (https://forums.lanik.us/)
                                               ! or via e-mail (easylist@protonmail.com).
                                               ! 
                                               ! Homepage: https://easylist.to/
                                               ! Licence: https://easylist.to/pages/licence.html
                                               ! GitHub issues: https://github.com/easylist/easylist/issues
                                               ! GitHub pull requests: https://github.com/easylist/easylist/pulls
                                               ! 
                                               ! -----------------------General advert blocking filters-----------------------!
                                               ! *** easylist:easylist/easylist_general_block.txt ***
                                               -ad-manager/$~stylesheet
                                               -ad-sidebar.$image
                                               """;

  private const string FanboyListHeaderExample = """
                                                 [Adblock Plus 2.0]
                                                 ! Checksum: QObDVAla+z9y3urSYJsRHg
                                                 ! Version: 202503301040
                                                 ! Title: Fanboy's Annoyance List
                                                 ! Last modified: 2025-03-30 10:40 UTC
                                                 ! Expires: 9 days (update frequency)
                                                 ! Homepage: https://easylist.to/
                                                 ! License: http://creativecommons.org/licenses/by/3.0/
                                                 !
                                                 ! Please report any unblocked content or problems by email or in our forums
                                                 ! Email: easylist@protonmail.com
                                                 ! Forums: https://forums.lanik.us/
                                                 !
                                                 !--------------------------General blocking rules-----------------------------!
                                                 ! fanboy_annoyance_general_block.txt
                                                 """;

  [Theory]
  [InlineData(FanboyListHeaderExample)]
  [InlineData(EasyListHeaderExample)]
  public void ValidEasyListHeader_ParseHeader_SuccessfulParseResults(string lines) {
    // Act
    var parseResult = EasyListParser.ParseHeader("", lines);
    // Assert
    parseResult.IsSuccess.Should().BeTrue();
  }

  [Theory]
  [InlineData("||domain.tld^", "domain.tld")]
  [InlineData("||sub.domain.tld^", "sub.domain.tld")]
  [InlineData("||domain.co.uk^", "domain.co.uk")]
  public void ValidEasyListExcerpt_ParseDomains_ReturnsDomainsFromExcerpt(string input, string expectedDomain) {
    // Act
    var parseResult = EasyListParser.ParseForbiddenDomains(input);
    // Assert
    parseResult.Should().Contain(expectedDomain);
  }
}
