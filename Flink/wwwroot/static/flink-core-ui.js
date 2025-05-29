class FlinkUi {
  urlRegex = /(?:https?:\/\/)?(?:www\.)?[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}(:\d+)?(?:\/[^\s?#]*)*(\?[^\s#]*)?(#[^\s]*)?/;
  recursionDetectionRegexes = [
    new RegExp(`${window.location.host}/[0-9a-zA-Z]+$`, 'i'),
    new RegExp(`${window.location.host}/links/[0-9a-zA-Z]+$`, 'i')
  ];

  constructor(removeUtmQueryParams, removeTrackingQueryParams) {
    this.sanitizers = [
      this.prefixUrlSanitizer,
      removeUtmQueryParams ? this.removeUtmQueryParametersSanitizer : undefined,
      removeTrackingQueryParams ? this.removeTrackingQueryParametersSanitizer : undefined
    ].filter(sanitizers => sanitizers !== undefined)
  }

  parseDateTime(inputValue) {
    if (!inputValue)
      return null;
    if (inputValue === "")
      return null;

    const date = new Date(inputValue);
    if (isNaN(date.getTime()))
      return null;
    return date;
  }

  convertToIsoDateTimeString(inputValue) {
    const date = this.parseDateTime(inputValue);
    if (date === null) return null;
    return date.toISOString().replace(/\.\d{3}Z$/, 'Z');
  }

  isValidURL(inputUrl) {
    const isUrl = new RegExp(this.urlRegex, 'i').test(inputUrl);
    const indicationForRecursion = this.recursionDetectionRegexes.some(r => r.test(inputUrl));
    return isUrl && !indicationForRecursion
  }

  sanitizeUrl(inputUrl) {
    return this.sanitizers.reduce((url, sanitizer) => sanitizer(url), inputUrl);
  }

  prefixUrlSanitizer(inputUrl) {
    const hasSchemaPrefix = [
      inputUrl.startsWith("http://"),
      inputUrl.startsWith("https://")
    ].reduce((acc, val) => acc || val, false);
    return hasSchemaPrefix ? inputUrl : `https://${inputUrl}`;
  }

  removeTrackingQueryParametersSanitizer(url) {
    try {
      let urlObj = new URL(url);
      let params = urlObj.searchParams;

      const trackingParams = [
        'utm_', 'fbclid', 'gclid', 'dclid', 'msclkid', 'mc_eid', 'twclid', 'igshid',
        '_hsenc', '_hsmi', 'ref', 'sourceid', 'trk', 'trkID', 'trkInfo', 'yclid',
        'sessid', 'PHPSESSID', 'token', 'auth', 'redirect'
      ];

      [...params.keys()].forEach(key => {
        if (trackingParams.some(tp => key.startsWith(tp))) {
          params.delete(key);
        }
      });

      return urlObj.origin + urlObj.pathname + (params.toString() ? '?' + params.toString() : '');
    } catch (error) {
      console.error("Invalid URL:", error);
      return url;
    }
  }

  removeUtmQueryParametersSanitizer(url) {
    try {
      let urlObj = new URL(url);
      let params = urlObj.searchParams;

      [...params.keys()]
        .forEach(key => {
          if (key.startsWith('utm_')) {
            params.delete(key);
          }
        });

      return urlObj.origin + urlObj.pathname + (params.toString() ? '?' + params.toString() : '');
    } catch (error) {
      console.error("Invalid URL:", error);
      return url;
    }
  }
}
