class FlinkLocalHistory {
  historyKey = 'flink-local-history';

  add(shortLinkId, shortenedLink, metaData) {
    const history = this.read();
    history.push({
      'createdAt': new Date().getTime(),
      'shortLinkId': shortLinkId,
      'shortenedLink': shortenedLink,
      'metaData': metaData
    })
    localStorage.setItem(this.historyKey, JSON.stringify(history));
  }

  remove(shortLinkId) {
    const history = this.read();
    const index = history.findIndex(entry => entry.shortLinkId === shortLinkId);
    if (index !== -1) {
      history.splice(index, 1);
    }
    localStorage.setItem(this.historyKey, JSON.stringify(history));
  }

  read() {
    const historyContent = localStorage.getItem(this.historyKey);
    return historyContent !== null ? JSON.parse(historyContent) : [];
  }
}
