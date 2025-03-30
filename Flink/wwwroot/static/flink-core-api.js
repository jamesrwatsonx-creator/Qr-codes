class FlinkInfo {
  constructor(applicationName, applicationRepository, applicationVersion, description) {
    this.applicationName = applicationName;
    this.applicationRepository = applicationRepository;
    this.applicationVersion = applicationVersion;
    this.description = description;
  }
}

class ShortLink {
  constructor(id, creationDate, creator, uri) {
    this.id = id;
    this.creationDate = creationDate;
    this.creator = creator;
    this.uri = uri
  }
}

class ShortLinkMeta {
  constructor(id, title, description, author, image, httpStatusCode, documentSize, lastUpdate) {
    this.id = id;
    this.title = title;
    this.description = description;
    this.author = author;
    this.image = image;
    this.httpStatusCode = httpStatusCode;
    this.documentSize = documentSize;
    this.lastUpdate = lastUpdate;
  }
}

class FlinkApiClient {
  getShortLinkHref(shortLinkId) {
    return `${location.protocol}//${location.host}/${shortLinkId}`;
  }

  getMetaHref(shortLinkId) {
    return `${location.protocol}//${location.host}/links/${shortLinkId}/meta`;
  }

  getQrCodeHref(shortLinkId, height = null) {
    const heightQueryParam = height !== null ? `?height=${height}` : '';
    return `${location.protocol}//${location.host}/links/${shortLinkId}/qr${heightQueryParam}`;
  }

  getEmbedHref(shortLinkId) {
    return `${location.protocol}//${location.host}/links/${shortLinkId}/embed`;
  }

  async getInfo() {
    return new Promise((resolve, reject) => {
      const xhr = new XMLHttpRequest();
      xhr.open('GET', '/info', true);
      xhr.setRequestHeader("Content-Type", "application/json; charset=UTF-8");
      xhr.responseType = "json";

      xhr.onload = () => {
        if (xhr.status === 200) {
          resolve(new FlinkInfo(
            xhr.response.applicationName,
            xhr.response.applicationRepository,
            xhr.response.applicationVersion,
            xhr.response.description));
        } else {
          reject(new Error(`Request failed with status ${xhr.status}`));
        }
      };

      xhr.onerror = () => reject(new Error("Network error"));
      xhr.send();
    });
  }


  async postLink(uri, creator, preferredId, preferredIdLength) {
    return new Promise((resolve, reject) => {
      const xhr = new XMLHttpRequest();
      xhr.open('POST', `/links`, true);
      xhr.setRequestHeader("Content-Type", "application/json; charset=UTF-8");
      xhr.responseType = "json";

      xhr.onload = () => {
        if (xhr.status === 201) {
          resolve(new ShortLink(
            xhr.response.id,
            xhr.response.creationDate,
            xhr.response.creator,
            xhr.response.uri));
        } else {
          reject(new Error(`Request failed with status ${xhr.status}`));
        }
      };

      xhr.onerror = () => reject(new Error("Network error"));
      xhr.send(JSON.stringify({
        'uri': uri,
        'creator': creator,
        'preferredId': preferredId,
        'preferredIdLength': preferredIdLength
      }));
    });
  }

  async getMeta(shortLinkId) {
    return new Promise((resolve, reject) => {
      const xhr = new XMLHttpRequest();
      xhr.open('GET', this.getMetaHref(shortLinkId), true);
      xhr.setRequestHeader("Content-Type", "application/json; charset=UTF-8");
      xhr.responseType = "json";

      xhr.onload = () => {
        if (xhr.status === 200) {
          resolve(new ShortLinkMeta(
            xhr.response.id,
            xhr.response.title,
            xhr.response.description,
            xhr.response.author,
            xhr.response.image,
            xhr.response.httpStatusCode,
            xhr.response.documentSize,
            xhr.response.lastUpdate));
        } else {
          reject(new Error(`Request failed with status ${xhr.status}`));
        }
      };

      xhr.onerror = () => reject(new Error("Network error"));
      xhr.send();
    });
  }

  async retryGetMeta(shortLinkId, interval = 300) {
    while (true) {
      try {
        const meta = await this.getMeta(shortLinkId);
        if (!meta) {
          await new Promise(resolve => setTimeout(resolve, interval));
          continue;
        }
        return meta;
      } catch (error) {
        await new Promise(resolve => setTimeout(resolve, interval));
      }
    }
  }
}
