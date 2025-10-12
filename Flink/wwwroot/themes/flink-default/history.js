const localHistory = new FlinkLocalHistory();

function renderRow(entry) {
  const createdAt = new Date(entry.createdAt).toLocaleString();
  return `
    <td class="col col-img"><img src="/links/${entry['shortLinkId']}/thumbnail" alt="123" loading="lazy"/></td>
    <td class="col col-title">
      <strong>${entry['metaData']['title'] ?? "none"}</strong>
      <p>${entry['metaData']['description'] ?? ''}</p>
      <p><span>🌐</span> <a href="${entry['shortenedLink']['uri']}" target="_blank">${entry['shortenedLink']['uri']}</a></p>
      <p><span>🗓️</span> ${createdAt}</p>
      <div class="col col-actions">
        <a href="/links/${entry['shortLinkId']}" target="_blank" class="btn btn-action">🔗</a>
        <a href="/links/${entry['shortLinkId']}/qr" target="_blank" class="btn btn-action">🤳</a>
        <a href="/links/${entry['shortLinkId']}/embed" target="_blank" class="btn btn-action">🪪</a>
        <a data-flink-id="${entry['shortLinkId']}" class="btn btn-action btn-remove-action">🗑️</a>
      </div>
    </td>`;
}

function renderHistory() {
  const container = document.getElementById("history-table-body");
  container.innerHTML = "";

  localHistory.read().forEach(entry => {
    const tableRowElement = document.createElement("tr");
    tableRowElement.innerHTML = renderRow(entry);
    container.appendChild(tableRowElement);
  });
}


(function () {
  window.addEventListener("storage", renderHistory);
  renderHistory();
  const removeButtons = document.getElementsByClassName('btn-remove-action');
  for (let i = 0; i < removeButtons.length; i++) {
    removeButtons[i].addEventListener('click', async (event) => {
      console.log(event.target.attributes['data-flink-id'].value);
      localHistory.remove(event.target.attributes['data-flink-id'].value);
      renderHistory()
    })
  }
})();
