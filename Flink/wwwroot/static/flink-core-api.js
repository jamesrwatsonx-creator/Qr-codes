class FlinkInfo {
  constructor(applicationName, applicationRepository, applicationVersion, description) {
    this.applicationName = applicationName;
    this.applicationRepository = applicationRepository;
    this.applicationVersion = applicationVersion;
    this.description = description;
  }
}

class ShortLink {
  constructor(id, creationDate, creator, uri, preferredId = null, preferredIdLength = 4, activeUntil = null, activeFrom = null, password = null) {
    this.id = id;
    this.creationDate = creationDate;
    this.creator = creator;
    this.uri = uri
    this.preferredId = preferredId;
    this.preferredIdLength = preferredIdLength;
    this.activeUntil = activeUntil;
    this.activeFrom = activeFrom;
    this.password = password;
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

class AuthProvider {
  constructor(id, displayName, imageUrl, loginUrl, buttonColor = null) {
    this.id = id;
    this.displayName = displayName;
    this.imageUrl = imageUrl;
    this.loginUrl = loginUrl;
    this.buttonColor = buttonColor;
  }
}

class FlinkApiClient {
  async getPasswordQueryParam(plainTextPassword = null) {
    if (plainTextPassword === null || plainTextPassword?.trim() === "") return '';
    const hashedPassword = await this.hashPassword(plainTextPassword);
    return `?password=${hashedPassword}`;
  }

  async getShortLinkHref(shortLinkId, plainTextPassword = null) {
    const passwordQueryParam = await this.getPasswordQueryParam(plainTextPassword);
    return `${location.protocol}//${location.host}/${shortLinkId}${passwordQueryParam}`;
  }

  async getMetaHref(shortLinkId, plainTextPassword = null) {
    const passwordQueryParam = await this.getPasswordQueryParam(plainTextPassword);
    return `${location.protocol}//${location.host}/links/${shortLinkId}/meta${passwordQueryParam}`;
  }

  async getQrCodeHref(shortLinkId, height = null) {
    const heightQueryParam = height !== null ? `?height=${height}` : '';
    return `${location.protocol}//${location.host}/links/${shortLinkId}/qr${heightQueryParam}`;
  }

  async getEmbedHref(shortLinkId, plainTextPassword = null) {
    const passwordQueryParam = await this.getPasswordQueryParam(plainTextPassword);
    return `${location.protocol}//${location.host}/links/${shortLinkId}/embed${passwordQueryParam}`;
  }

  async hashPassword(plaintextPassword) {
    const encoder = new TextEncoder();
    const data = encoder.encode(plaintextPassword);
    const hashBuffer = await crypto.subtle.digest('SHA-256', data);
    const hashArray = Array.from(new Uint8Array(hashBuffer));
    return hashArray.map(b => b.toString(16).padStart(2, '0')).join('');
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


  async postLink(uri, creator, preferredId = null, preferredIdLength = 4, activeFrom = null, activeUntil = null, plainTextPassword = null) {
    const hashedPassword = plainTextPassword === null ? null : await this.hashPassword(plainTextPassword);

    return new Promise((resolve, reject) => {
      const xhr = new XMLHttpRequest();
      xhr.open('POST', `/links`, true);
      xhr.setRequestHeader("Content-Type", "application/json; charset=UTF-8");
      xhr.responseType = "json";

      xhr.onload = () => {
        if (xhr.status === 201) {
          resolve(new ShortLink(
            xhr.response.data.id,
            xhr.response.data.creationDate,
            xhr.response.data.creator,
            xhr.response.data.uri,
            xhr.response.data.activeFrom,
            xhr.response.data.activeUntil));
        } else {
          reject(new Error(`Request failed with status ${xhr.status}`));
        }
      };

      xhr.onerror = () => reject(new Error("Network error"));
      xhr.send(JSON.stringify({
        'uri': uri,
        'creator': creator,
        'preferredId': preferredId,
        'preferredIdLength': preferredIdLength,
        'activeUntil': activeUntil,
        'activeFrom': activeFrom,
        'password': hashedPassword
      }));
    });
  }

  async checkPassword(shortLinkId, plainTextPassword = null) {
    const href = await this.getMetaHref(shortLinkId, plainTextPassword);
    return new Promise((resolve, reject) => {
      const xhr = new XMLHttpRequest();
      xhr.open('GET', href, true);
      xhr.setRequestHeader("Content-Type", "application/json; charset=UTF-8");
      xhr.responseType = "json";

      xhr.onload = () => {
        if (xhr.status === 200) {
          console.log("Password correct");
          resolve(true);
        } else if (xhr.status === 401) {
          console.log("Password incorrect");
          resolve(false);
        } else {
          reject(new Error(`Request failed with status ${xhr.status}`));
        }
      };

      xhr.onerror = () => reject(new Error("Network error"));
      xhr.send();
    });
  }

  async getMeta(shortLinkId, plainTextPassword = null) {
    const href = await this.getMetaHref(shortLinkId, plainTextPassword);
    return new Promise((resolve, reject) => {
      const xhr = new XMLHttpRequest();
      xhr.open('GET', href, true);
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

  async retryGetMeta(shortLinkId, plainTextPassword = null, interval = 300) {
    while (true) {
      try {
        const meta = await this.getMeta(shortLinkId, plainTextPassword);
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

  async getAuthProviders() {
    return new Promise((resolve, reject) => {
      const xhr = new XMLHttpRequest();
      xhr.open('GET', '/auth/providers', true);
      xhr.responseType = "json";

      xhr.onload = () => {
        if (xhr.status === 200) {
          const providers = xhr.response.data.map(p =>
            new AuthProvider(
              p.id,
              p.displayName,
              p.imageUrl,
              p.loginUrl,
              p.buttonColor
            )
          );
          resolve(providers);
        } else {
          reject(new Error(`Failed to load providers (${xhr.status})`));
        }
      };

      xhr.onerror = () => reject(new Error("Network error"));
      xhr.send();
    });
  }

  async getCurrentUser() {
    return new Promise((resolve, reject) => {
      const xhr = new XMLHttpRequest();
      xhr.open('GET', '/auth/me', true);
      xhr.responseType = "json";

      xhr.onload = () => {
        if (xhr.status === 200) {
          resolve(xhr.response.data);
        } else if (xhr.status === 401) {
          resolve(null);
        } else {
          reject(new Error(`Failed (${xhr.status})`));
        }
      };

      xhr.onerror = () => reject(new Error("Network error"));
      xhr.send();
    });
  }

  async logout() {
    return new Promise((resolve, reject) => {
      const xhr = new XMLHttpRequest();
      xhr.open('POST', '/auth/logout', true);

      xhr.onload = () => {
        if (xhr.status === 200) resolve();
        else reject(new Error(`Logout failed (${xhr.status})`));
      };

      xhr.onerror = () => reject(new Error("Network error"));
      xhr.send();
    });
  }
}
