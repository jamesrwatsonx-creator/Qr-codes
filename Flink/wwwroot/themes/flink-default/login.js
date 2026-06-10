const api = new FlinkApiClient();
const container = document.getElementById("div-login-providers");

function createProviderButton(provider) {
  const btn = document.createElement("button");
  btn.className = "btn-login-provider";

  if (provider.buttonColor) {
    btn.style.backgroundColor = provider.buttonColor;
  }

  const img = document.createElement("img");
  img.src = provider.imageUrl;
  img.alt = provider.displayName;

  const span = document.createElement("span");
  span.innerText = `Login with ${provider.displayName}`;

  btn.appendChild(img);
  btn.appendChild(span);

  btn.onclick = () => {
    window.location.href = provider.loginUrl;
  };

  return btn;
}

async function loadProviders() {
  try {
    const providers = await api.getAuthProviders();

    if (!providers || providers.length === 0) {
      container.innerHTML = "<p>No authentication providers configured.</p>";
      return;
    }

    providers.forEach(p => {
      container.appendChild(createProviderButton(p));
    });

  } catch (err) {
    console.error(err);
    document.getElementById("error-popover").innerText = "Failed to load login providers";
    document.getElementById("error-popover").classList.add("error-show");
  }
}

loadProviders();
