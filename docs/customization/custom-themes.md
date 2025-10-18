# Custom Themes

Flink comes with a default theme that works right out of the box. However, if you want to customize its appearance to align with your brand or online presence, Flink supports custom themes. Creating a your own theme is simple. All you need to know is a little bit HTML, CSS, and JS.

## Creating a new theme

### Step 1 - Setting up the development environment

First, create a new folder to store all the necessary files for your Flink theme. Each theme requires a name. The name of the theme is the parent-directory of its files. In this guide, let's choose `my-flink-theme`. Open your terminal and run the following command:

```bash
# create an empty directory that stores the theme files
theme_directory="$HOME/Documents/my-flink-theme"
mkdir -p $theme_directory

# grab the latest version of
docker pull quay.io/rtraceio/flink:latest

# spin up docker / podman and mount the newly created directory in the wwwroot
docker run \
  --rm \
  --name flink-theme-dev \
  -e WEBUICONFIGURATION__DEFAULTHEME=my-flink-theme \
  -e WEBUICONFIGURATION__THEMECACHEDURATION=0 \
  -p 8080:8080 \
  -v "$theme_directory:/app/wwwroot/themes/my-flink-theme" \
  "quay.io/rtraceio/flink:latest"
```


#### Pitfall with Volume Mounts

On Ubuntu/Debian-based distributions (especially when using AppArmor), you need to pass `:U` as parameter to the Volume Mount to properly handle permissions.

```bash
docker run \
  --rm \
  --name flink-theme-dev \
  -e WEBUICONFIGURATION__DEFAULTHEME=my-flink-theme \
  -e WEBUICONFIGURATION__THEMECACHEDURATION=0 \
  -p 8080:8080 \
  -v "$theme_directory:/app/wwwroot/themes/my-flink-theme:U" \
  "quay.io/rtraceio/flink:latest"
```

On RHEL-based distributions (especially when using SELinux), you need to pass `:Z` as parameter to the Volume Mount to properly handle permissions.

```bash
docker run \
  --rm \
  --name flink-theme-dev \
  -e WEBUICONFIGURATION__DEFAULTHEME=my-flink-theme \
  -e WEBUICONFIGURATION__THEMECACHEDURATION=0 \
  -p 8080:8080 \
  -v "$theme_directory:/app/wwwroot/themes/my-flink-theme:Z" \
  "quay.io/rtraceio/flink:latest"
```


With the environment-variable `WEBUICONFIGURATION__DEFAULTHEME` we tell Flink to use the `my-flink-theme` as the default theme whevever Flink is visited. With the environment-variable `WEBUICONFIGURATION__THEMECACHEDURATION` we tell Flink not to cache the theme in-memory and always read the latest version of the theme from disk. In a production environment you should
not set this variable to `0` (seconds) - for our theming development environment `0` (seconds) is fine, so we don't have to wait until the cache expires to see our changes in action. 
Now our development environment is set up, and we can start working on the theme. 

### Step 2 - Creating the HTML structure

Every theme requires at least an `index.html` file, which serves as the main entry page when visitors access Flink (e.g., https://flink.is/). Inside your newly created directory, create a file named `index.html` and use the following starter template:

```html
<!DOCTYPE html>
<html lang="en">
<head>
  <meta charset="UTF-8">
  <title>my-flink-theme (replace with your own title)</title>
</head>
<body>
    <!-- this is where the users enters the long link -->
    <input id="long-link-input">

    <!-- this is the button that is pressed to trigger flink -->
    <button id="flinkify-button">flinkify</button>

    <!-- this textbox is where we want to see the flinkified link -->
    <input id="short-link-input">

<!-- Flink JS -->
<script src="/static/flink-core-api.js"></script>
<script src="/static/flink-core-ui.js"></script>
</body>
</html>
```

You can now open this `index.html` in your browser to preview it - visit [http://localhost:8080/](http://localhost:8080). You should see something similar to the following.

![HTML only looked at in a Web Browser](/docs/customization/assets/my-flink-theme-html-only.png)

### Step 3 - Styling the theme

As you might have noticed, the design of our theme is not quite state of the art - and frankly, ugly AF. Let's improve it with some styling! To enhance the appearance of our page, we'll create a Cascading Style Sheet (CSS) file. In the same directory as `index.html`, create a new file called `theme.css` (you can name it differently, but remember the name for later). Again here's some rudimentary template for you to get started with.

```css
body {
    margin: 15px auto;
    background-color: #AAAAAF;
}

input {
    display: block;
    width: 80%;
    height: 50px;
    margin: 10px auto;
    border-radius: 5px;
    text-align: center;
}

button {
    display: block;
    width: 50%;
    height: 50px;
    margin: 10px auto;
}
```

Now, we need to link the `theme.css` file to our `index.html`. Modify the `<head>` section as follows:

```html
<head>
  <meta charset="UTF-8">
  <title>my-flink-theme (replace with your own title)</title>
  <link rel="stylesheet" href="/themes/my-flink-theme/index.css"/>
</head>
```

Again, visit [http://localhost:8080/](http://localhost:8080) and look at our
stunning, preak-frontend-design theme we have created.


![HTML and CSS looked at in a Web Browser](/docs/customization/assets/my-flink-theme-html-and-css.png)

### Step 4 - making the frontend functional with JS

So far, we have created a simple HTML file and some rudimentary styling for it. I am sure you have pressed the `flinkify` button already, and noticed that it didn't work yet.
And this is exactly what we'll be working on in this step. Let's create a new file called `my-flink-theme.js` (you're free to name it however you like, just be sure to remember it to link it into our index.html later).

```js
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
      .then(data => {
        const flinkifiedUrl = flinkApi.getShortLinkHref(data['id']);
        output.value = flinkifiedUrl;
      });
});
```

Now we have rudimentary JS in place that makes our theme functional. The final step is to link it in our `index.html`. This time we add it closer to the end of the file.

```html
<!-- Flink JS -->
<script src="/static/flink-core-api.js"></script>
<script src="/static/flink-core-ui.js"></script>

<!-- Insert your Themes JS here -->
<script src="/themes/my-flink-theme/my-flink-theme.js"></script>

</body>
</html>
```

visit [http://localhost:8080/](http://localhost:8080) - and flinkify some links.

## Sites for a Theme

- `index.html` landing page on '/'
- `stats.html` statistics page on '/stats'
- `history.html` (local) history page on '/history'
- `countdown.html` links that are not yet active '/cd'
- `password.html` links that are password protected '/pw'

## Improvement ideas

Now that we created our rudimentary theme, it's your turn to improve it and adapt it to your liking. The following ideas

- improve UI/UX
  - styling
  - add a nice logo
  - brand it to your liking
  - input validation
  - error handling
  - show embeds
  - show metadata
  - about instance info
  - add ARIA labels (to improve screenreader experience)
  - add Password form (for when a client wants a link to have a password)
  - add support for activation and/or expiration time
  - create a page that allows editing

- SEO
  - Twitter cards
  - OpenGraph
  - Meta Tags

Also make sure to find some inspiration in the already existing themes for `Flink`. You can find them in the [Flink Themes](https://gitlab.com/rtraceio/web/flink/-/tree/main/Flink/wwwroot/themes).