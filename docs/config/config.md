### Flink Configuration

all configurations are optional. Flink works out-of-the-box and is configured with secure and reliable defaults.

#### Proxy Configuration

Flink can route its outgoing network requests through proxy servers defined by the HTTP_PROXY, HTTPS_PROXY, and SOCKS_PROXY environment variables. 
This helps in scenarios where Flink’s IP addresses are blocked or when requests need to be sent through Tor for anonymity or access control reasons.

| Environment Variable | AppSettings Path | Description                                                                   | Default                 |
|:---------------------|:-----------------|:------------------------------------------------------------------------------|:------------------------|
| HTTP_PROXY           | HTTP_PROXY       | the proxy to use for outgoing HTTP requests                                   | null                    |
| HTTPS_PROXY          | HTTPS_PROXY      | the proxy to use for outgoing HTTPS requests (if set, overrides HTTP_PROXY)   | null                    |
| SOCKS_PROXY          | SOCKS_PROXY      | the socks5 proxy to use for outgoing requests (if set, overrides HTTPS_PROXY) | null                    |

##### Examples

```bash
# with username and password and ip address
HTTP_PROXY=http://username:password@127.0.0.1:8080

# with username and password and fqdn
SOCKS_PROXY=socks5://username:password@myproxy.tld:1234

# no authentication, just hostname + port
HTTPS_PROXY=https://localhost:8080
```

#### Database Configuration

Flink allows you to configure its database connection through environment variables or corresponding application settings. 
These settings determine which database backend is used and how the connection to that database is established.

Supported Databases:
- Postgres
- MySql
- Sqlite

| Environment Variable                      | AppSettings Path                         | Description                                                                       | Default                 |
|:------------------------------------------|:-----------------------------------------|:----------------------------------------------------------------------------------|:------------------------|
| DBCONFIGURATION__DBTYPE                   | DbConfiguration:DbType                   | which database type shall be used (enum, either `Postgres`, `MySql` or `Sqlite`)  | Sqlite                  |
| DBCONFIGURATION__SQLITECONNECTIONSTRING   | DbConfiguration:SqliteConnectionString   | format: `Data Source=flink.sqlite`                                                | `Data Source=flink.db ` |
| DBCONFIGURATION__POSTGRESCONNECTIONSTRING | DbConfiguration:PostgresConnectionString | format: `Server=localhost;Port=5432;Database=flink;User Id=flink;Password=flink;` | null                    |
| DBCONFIGURATION__MYSQLCONNECTIONSTRING    | DbConfiguration:MySqlConnectionString    | format: `Server=localhost;Port=3306;Database=flink;Uid=flink;Pwd=flink;`          | null                    |

#### Metrics Configuration

Flink provides a configurable metrics endpoint that can be enabled or disabled through environment variables. 
The metrics endpoint allows external systems such as Prometheus to collect runtime metrics from Flink. 
Access to this endpoint can be protected using HTTP Basic Authentication for security.
Metrics can be enabled or disabled via the `EnableMetrics` setting. 
When metrics are enabled, the endpoint can optionally require authentication, controlled by `RequireBasicAuth`. 
If authentication is enabled, both a username (`BasicAuthUsername`) and password (`BasicAuthPassword`) must be provided. 
The endpoint path itself can be customized through `MetricsEndpointPath`.

By default, metrics are enabled and require authentication with the default credentials `flink` / `flink`, accessible under the `/metrics` path.

| Environment Variable                      | AppSettings Path                         | Description                                                                       | Default                 |
|:------------------------------------------|:-----------------------------------------|:----------------------------------------------------------------------------------|:------------------------|
| METRICSCONFIGURATION__ENABLEMETRICS       | MetricsConfiguration:EnableMetrics       | whether to enable the metrics endpoint or not                                     | true                    |
| METRICSCONFIGURATION__REQUIREBASICAUTH    | MetricsConfiguration:RequireBasicAuth    | whether HTTP BASIC authentication with Username + Password is needed to query     | true                    |
| METRICSCONFIGURATION__BASICAUTHUSERNAME   | MetricsConfiguration:BasicAuthUsername   | the Username that is allowed to query data from the metrics endpoint              | flink                   |
| METRICSCONFIGURATION__BASICAUTHPASSWORD   | MetricsConfiguration:BasicAuthPassword   | the  password for the above given username that is allowed to query               | flink                   |
| METRICSCONFIGURATION__METRICSENDPOINTPATH | MetricsConfiguration:MetricsEndpointPath | the URL segment the metrics endpoint is accessible from                           | /metrics                |

**NOTE**: When `RequireBasicAuth` is set to true `BasicAuthUsername` and `BasicAuthPassword` MUST be provided. If `RequireBasicAuth` is set to false, the values in `BasicAuthUsername` and `BasicAuthPassword` are ignored. Please refer to [Prometheus Scrape Configuration Manual](https://prometheus.io/docs/prometheus/latest/configuration/configuration/#scrape_config) to allow Prometheus to query from `Flink`. 

#### WebUI Configuration

These settings control the appearance and caching behavior of the Flink Web UI. They define global, 
theme-independent parameters that apply to any configured theme.  The `DefaultTheme` setting determines 
which theme is used when no theme is specified through the `?theme` query parameter in the URL. 
This ensures the Web UI always loads with a consistent appearance even when no explicit theme is requested.

The `ThemeCacheDuration` setting defines how long a theme is cached in memory, measured in seconds. 
Caching improves performance by reducing the need to repeatedly load theme resources from disk.

| Environment Variable                      | AppSettings Path                         | Description                                                                       | Default                 |
|:------------------------------------------|:-----------------------------------------|:----------------------------------------------------------------------------------|:------------------------|
| WEBUICONFIGURATION__DEFAULTTHEME          | WebUiConfiguration:DefaultTheme          | which Theme to use in case in case query parameter ?theme is not providing theme  | flink-default           |
| WEBUICONFIGURATION__THEMECACHEDURATION    | WebUiConfiguration:ThemeCacheDuration    | mumber of seconds a theme shall be cached in memory (in seconds)                  | 300                     |

Both settings apply globally across all themes and are not tied to any specific theme configuration.

#### Spam Protection Configuration


Flink includes a built-in spam protection mechanism to help prevent the creation and distribution of malicious or unwanted shortened URLs. 
This feature analyzes submitted links against one or more blocklists to detect and block spam, phishing, or advertising domains before they are shortened.
The spam protection system can be enabled or disabled through the `Enabled` setting. When enabled, 
Flink checks every submitted URL against the configured blocklists specified in the `BlockLists` setting. 
These blocklists follow the **AdblockPlus 2.0** format and can include any number of third-party or custom lists.

By default, Flink uses a curated set of well-known community-maintained blocklists. 
Administrators can extend or replace these defaults by providing their own blocklist URLs.

| Environment Variable                      | AppSettings Path                         | Description                                                                       | Default                 |
|:------------------------------------------|:-----------------------------------------|:----------------------------------------------------------------------------------|:------------------------|
| SPAMPROTECTIONCONFIGURATION__ENABLED      | SpamProtectionConfiguration:Enabled      | boolean, indicating whether Flink Spam Protection is enabled                      | true                    |
| SPAMPROTECTIONCONFIGURATION__BLOCKLISTS   | SpamProtectionConfiguration:BlockLists   | AdblockPlus2.0 compliant blocklist URLs (e.g. easylist)                           | see Block List defaults |

##### Default Block Lists

- [EasyList](https://easylist.to/easylist/easylist.txt)
- [Fanboy-Annoyance](https://secure.fanboy.co.nz/fanboy-annoyance.txt)
- [Malware-Filter Phishing List](https://malware-filter.gitlab.io/malware-filter/phishing-filter-agh.txt)
- [Phishing.Database Domains](https://github.com/Phishing-Database/Phishing.Database/blob/master/phishing-domains-ACTIVE.adblock)
- [Flink-Blocklist](https://gitlab.com/rtraceio/web/flink/-/raw/main/flink-blocklist.txt)

#### Event Notification Configuration

Flink provides an event-driven notification system that allows you to send real-time updates to external services 
whenever specific events occur within the application. Notifications are delivered to one or more **sinks**, 
each representing an external endpoint that receives event data. This enables integration with chat platforms, 
alerting tools, or custom webhooks for monitoring and automation purposes (e.g. [n8n](https://n8n.io/))

Each sink requires a **URL**, an HTTP **method** (typically `POST`), and a **payload format** defining 
how the notification data is structured. Multiple sinks can be configured independently, 
allowing different events to trigger different destinations. For example, one sink might send 
startup notifications to an internal monitoring system, while another pushes link-related events to a Slack or Discord channel.

Flink currently supports the following payload formats (and webhook formats):
- [ntfy](https://ntfy.sh) (use method: `POST`)
- [mattermost](https://mattermost.com) (use method: `POST`)
- [slack](https://slack.com) (use method: `POST`)
- [rocketchat](https://rocket.chat) (use method: `POST`)
- [discord](https://discord.com) (use method: `POST`)

Flink currently supports the following events:
- `FlinkBooted` (fired when a Flink Instance starts up) 
- `FlinkLinkShortened` (fired when Flink shortens a link)
- `FlinkLinkDeleted` (fired when Flink deletes a link)

| Environment Variable                                            | AppSettings Path                                             | Description                                                       | Default |
|:----------------------------------------------------------------|:-------------------------------------------------------------|:------------------------------------------------------------------|:--------|
| EVENTNOTIFICATIONCONFIGURATION__SINKS__<index>__NAME            | EventNotificationConfiguration:Sinks:<index>:name            | a unique name given for the Sink                                  |         |
| EVENTNOTIFICATIONCONFIGURATION__SINKS__<index>__URL             | EventNotificationConfiguration:Sinks:<index>:url             | a valid webhook                                                   | none    |
| EVENTNOTIFICATIONCONFIGURATION__SINKS__<index>__METHOD          | EventNotificationConfiguration:Sinks:<index>:method          | whether to HTTP PUT, POST, GET                                    | POST    |
| EVENTNOTIFICATIONCONFIGURATION__SINKS__<index>__PAYLOADFORMAT   | EventNotificationConfiguration:Sinks:<index>:payloadFormat   | which payload format should be used (e.g. `ntfy`, `slack`)        | none    |
| EVENTNOTIFICATIONCONFIGURATION__SINKS__<index>__EVENTS__<index> | EventNotificationConfiguration:Sinks:<index>:events:<index>  | array of which events should be fired (e.g. `FlinkLinkShortened`) | none    |

**NOTE: ** Index counting starts with 0.

##### appsettings.json example

```json
{
  "EventNotificationConfiguration": {
    "Sinks": [
      {
        "name": "ntfy-service-alerts",
        "url": "https://ntfy.rtrace.io/flink-moderation-stream",
        "method": "POST",
        "payloadFormat": "ntfy",
        "events": [
          "FlinkBooted"
        ]
      },
      {
        "name": "discord-notifier",
        "url": "https://discordapp.com/api/webhooks/123412341234123/<someSecretToken>",
        "method": "POST",
        "payloadFormat": "discord",
        "events": [
          "FlinkLinkShortened",
          "FlinkLinkDeleted"
        ]
      }
    ]
  }
}
```

##### environment variables example

```txt
EVENTNOTIFICATIONCONFIGURATION__SINKS__0__NAME='first-slack-messages'
EVENTNOTIFICATIONCONFIGURATION__SINKS__0__URL='https://slack.com/<webhookurl-for-channel1>'
EVENTNOTIFICATIONCONFIGURATION__SINKS__0__METHOD='POST'
EVENTNOTIFICATIONCONFIGURATION__SINKS__0__PAYLOADFORMAT='slack'
EVENTNOTIFICATIONCONFIGURATION__SINKS__0__EVENTS__0='FlinkBooted'

EVENTNOTIFICATIONCONFIGURATION__SINKS__1__NAME='slack-messages'
EVENTNOTIFICATIONCONFIGURATION__SINKS__1__URL='https://slack.com/<webhookurl-for-channel2>'
EVENTNOTIFICATIONCONFIGURATION__SINKS__1__METHOD='POST'
EVENTNOTIFICATIONCONFIGURATION__SINKS__1__PAYLOADFORMAT='slack'
EVENTNOTIFICATIONCONFIGURATION__SINKS__1__EVENTS__0='FlinkLinkShortened'
EVENTNOTIFICATIONCONFIGURATION__SINKS__1__EVENTS__1='FlinkLinkDeleted'
```
