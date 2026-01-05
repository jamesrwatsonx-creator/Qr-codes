using System.Security.Cryptography;

namespace Flink.ShortLinks;

public static class ShortLinkIdGenerator {
  private static readonly char[] UsableCharacters = [
    'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'k', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z',
    'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'J', 'K', 'L', 'M', 'N', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z',
    '0', '1', '2', '3', '4', '5', '6', '7', '8', '9'
  ];

  public static string CreateRandomId(int length) {
    var randomChars = Enumerable.Range(0, length)
      .Select(_ => UsableCharacters[RandomNumberGenerator.GetInt32(0, UsableCharacters.Length - 1)]);

    var id = string.Join("", randomChars);
    
    return SpecialShortLinkIds.ReservedShortLinkIds.Contains(id.ToLowerInvariant())
      ? CreateRandomId(length)
      : id;
  }
}
