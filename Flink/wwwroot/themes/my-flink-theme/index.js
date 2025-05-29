const flinkApi = new FlinkApiClient();

const input = document.getElementById('long-link-input');
const button = document.getElementById('flinkify-button');
const output = document.getElementById('short-link-input');

// how many characters shall the flinkified URL have?
const linkPrefixLength = 4;


// The logic is simple - once the button is clicked ...
// ... we read the value from the input textbox,
// ... post it to the Flink backend
// ... show the shortened URL from the flink backend in the output textbox

button.addEventListener('click', () => {
  const longUrl = input.value.trim();

  flinkApi
    .postLink(longUrl, "my-flink-theme", null, linkPrefixLength)
    .then(async data => output.value = await flinkApi.getShortLinkHref(data['id']));
});
