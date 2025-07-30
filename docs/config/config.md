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
- [Flink-Blocklist](https://gitlab.com/rtraceio/web/flink/-/raw/main/flink-blocklist.txt)
