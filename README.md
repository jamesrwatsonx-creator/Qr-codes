![flink-logo](flink-logo.png)

Flink is a simple, Free, Libre, and Open Source URL Shortener built with love and a ASP.NET Core 8 Web API. Why yet another URL Shortener, you might ask yourself? Well, because `flink` has a unique feature set, and demands around scalabilty, simplicity, security and traceability that no other URL shortener was able to fullfil.

## Features

- 🚀 get a production-grade `flink` instance up and running in less then 1 minute
- 📱 easy generation of QR Codes for shortened links
- ⌨️ batch short link creation from your terminal (using `curl`, `wget` or any other board util)
- 😎 swagger (OpenAPI) interface allowing you to build on top of `flink`
- 🖥️ runs on Linux, Windows, Mac
- 🐳 deployable as container (podman and docker)
- 📈 keep track of how often short links are visited through built-in Prometheus support



## Quickstart Guide

```bash
# if you are using podman
podman run -p 8080:8080 quay.io/flink-shortener/flink

# alternatively if you use docker
docker run -p 8080:8080 quay.io/flink-shortener/flink

# create a short link pointing to blog.rtrace.io with the id "awesomeblog" (obviously you change the the link and id to whatever you want)
curl -X "POST" \
    -H "Content-Type: application/json" \
    -d '{ "uri": "https://blog.rtrace.io", "creator": "ruffy", "preferredId": "awesomeblog" }' \
    'http://localhost:8080/to'

# Now it's time to explore!

# open up your browser, navigate to http://localhost:8080/to/awesomeblog
# want the QR Code, navigate to http://localhost:8080/qr/awesomeblog
# want to use Grafana & Promethues to understand how often you clicked the link, navigate to http://localhost:8080/metrics
# the Swagger OpenAPI documentation can be found http://localhost:8080/swagger/index.html
```

## Roadmap

- [ ] In-Memory caching to improve performance and load-reduction on the sqlite database
- [ ] In-Memory caching for generated QR codes
- [ ] Variable QR Code sizes (attach a URL parameter specifying height in px)
- [ ] Scalability through Federation
- [ ] Load/Performance tests (unscientific test of `flink` on a Raspberry PI 4B, handles ~15.000 requests/s)
- [ ] Optional authentication for creating short links (to prevent potential abuse)

## Contributing

Contributions to `flink` are welcome! If you have any ideas for improvements or new features, feel free to open an issue or submit a pull request.

## License

Flink is licensed under the [MIT License](LICENSE), allowing you to use, modify, and distribute the software freely.
