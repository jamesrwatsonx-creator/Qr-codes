### Flink Configuration

all configurations are optional. Flink works out-of-the-box and is configured with secure and reliable defaults.

#### Database Configuration

| Environment Variable                      | AppSettings Path                         | Description                                                                       | Default                 |
|:------------------------------------------|:-----------------------------------------|:----------------------------------------------------------------------------------|:------------------------|
| DBCONFIGURATION__DBTYPE                   | DbConfiguration:DbType                   | which database type shall be used (enum, either `Postgres`, `MySql` or `Sqlite`)  | Sqlite                  |
| DBCONFIGURATION__SQLITECONNECTIONSTRING   | DbConfiguration:SqliteConnectionString   | format: `Data Source=flink.sqlite`                                                | `Data Source=flink.db ` |
| DBCONFIGURATION__POSTGRESCONNECTIONSTRING | DbConfiguration:PostgresConnectionString | format: `Server=localhost;Port=5432;Database=flink;User Id=flink;Password=flink;` | null                    |

#### Metrics Configuration

| Environment Variable                      | AppSettings Path                         | Description                                                                       | Default                 |
|:------------------------------------------|:-----------------------------------------|:----------------------------------------------------------------------------------|:------------------------|
| METRICSCONFIGURATION__ENABLEMETRICS       | MetricsConfiguration:EnableMetrics       | whether to enable the metrics endpoint or not                                     | true                    |
| METRICSCONFIGURATION__REQUIREBASICAUTH    | MetricsConfiguration:RequireBasicAuth    | whether HTTP BASIC authentication with Username + Password is needed to query     | true                    |
| METRICSCONFIGURATION__BASICAUTHUSERNAME   | MetricsConfiguration:BasicAuthUsername   | the Username that is allowed to query data from the metrics endpoint              | flink                   |
| METRICSCONFIGURATION__BASICAUTHPASSWORD   | MetricsConfiguration:BasicAuthPassword   | the  password for the above given username that is allowed to query               | flink                   |
| METRICSCONFIGURATION__METRICSENDPOINTPATH | MetricsConfiguration:MetricsEndpointPath | the URL segment the metrics endpoint is accessible from                           | /metrics                |

**NOTE**: When `RequireBasicAuth` is set to true `BasicAuthUsername` and `BasicAuthPassword` MUST be provided. If `RequireBasicAuth` is set to false, the values in `BasicAuthUsername` and `BasicAuthPassword` are ignored. Please refer to [Prometheus Scrape Configuration Manual](https://prometheus.io/docs/prometheus/latest/configuration/configuration/#scrape_config) to allow Prometheus to query from `Flink`. 

#### WebUI Configuration

| Environment Variable                      | AppSettings Path                         | Description                                                                       | Default                 |
|:------------------------------------------|:-----------------------------------------|:----------------------------------------------------------------------------------|:------------------------|
| WEBUICONFIGURATION__DEFAULTTHEME          | WebUiConfiguration:DefaultTheme          | which Theme to use in case in case query parameter ?theme is not providing theme  | flink-default           |
| WEBUICONFIGURATION__THEMECACHEDURATION    | WebUiConfiguration:ThemeCacheDuration    | mumber of seconds a theme shall be cached in memory (in seconds)                  | 300                     |


#### Spam Protection Configuration

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

Event Notifications are configured in the form of sinks. Multiple sinks can be configured. Each sink requires a URL, a method as well as a payload format.

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
