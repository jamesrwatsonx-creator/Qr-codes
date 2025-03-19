class FlinkUi {
  urlRegex = /(?:https?:\/\/)?(?:www\.)?[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}(:\d+)?(?:\/[^\s?#]*)*(\?[^\s#]*)?(#[^\s]*)?/;

  constructor(removeUtmQueryParams, removeTrackingQueryParams) {
    this.sanitizers = [
      this.prefixUrlSanitizer,
      removeUtmQueryParams ? this.removeUtmQueryParametersSanitizer : undefined,
      removeTrackingQueryParams ? this.removeTrackingQueryParametersSanitizer : undefined
    ].filter(sanitizers => sanitizers !== undefined)
  }

  isValidURL(inputUrl) {
    return new RegExp(this.urlRegex, 'i').test(inputUrl);
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
