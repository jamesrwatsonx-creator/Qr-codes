# Installation

Flink is a C# / ASP.NET WebAPI packaged as multiple executable artifacts, so it can be run virtually everywhere. This page lists the recommended installation methods of `Flink`.

![Flink Deployment](../assets/flink-setup.drawio.svg)

## Docker

Docker is a simple and popular method to spin up and operate workloads. The following installation guide assumes you have `docker` installed. If you have not, please refer to the [Docker Install Manual](https://docs.docker.com/engine/install/). While Docker is fully supported, `Flink` recommends [Podman](https://podman.io/) over Docker. The commands below will use the docker CLI, however the CLIs of docker and podman can be used interchangeably. You can download and install podman from the [Podman Installation Docs](https://podman.io/docs/installation)


### Docker (default settings)

| Time to set up | Production-readiness |
|:---------------|:---------------------|
| < 1m           | ★★☆☆☆                |

```bash
docker run --name flink -p 8080:8080 quay.io/rtraceio/flink:latest
```

**NOTE**: Flink will default to store Shortlinks in a SQLite database. Once the container is stopped (or crashes), all shortened Links are lost.

### Docker (with volume for database)

| Time to set up | Production-readiness |
|:---------------|:---------------------|
| < 5m           | ★★★★☆                |

```bash
# Create a directory on your host where the SQLite Database shall be stored
host_directory="/home/flink/flink_data"
mkdir -p $host_directory

docker run --name flink -p 8080:8080 -v "$host_directory/flink.db:/app/flink.db" \ --env-file docker.env quay.io/rtraceio/flink:latest
```

**NOTE**: The [.env File](/docs/install/docker.env) defaults to the name `flink.db` for the SQLite database. If you change it in the `.env` file you will need to change it here too.

**Recommendation**: to prevent data-loss, Flink recommends you to create frequent backups of the `flink.db` (e.g. using daily cronjob/systemd timer).


## Docker-Compose

Docker-compose - alternatively podman-compose - are simple container orchestration frameworks. `Flink` support different Databases besides SQLite (including PostgreSQL and MySQL/MariaDB). Productive installs with high loads might run into limitations with SQLite or require more `Flink` instances/replicas. `Flink` then recommends to spin up multiple `Flink` instances in front of the same database.


### docker-compose with PostgreSQL

| Time to set up | Production-readiness |
|:---------------|:---------------------|
| < 5m           | ★★★★★                |

```yaml
version: '3.9'

services:
  postgres:
    image: postgres:16
    container_name: postgres
    environment:
      POSTGRES_USER: flink
      POSTGRES_PASSWORD: flink
      POSTGRES_DB: flinkdb
    ports:
      - '5432:5432'
    volumes:
      - pgdata:/var/lib/postgresql/data

  flink:
    image: quay.io/rtraceio/flink:latest
    container_name: flink
    ports:
      - '8080:8080'
    environment:
      DbConfiguration__DbType: Postgres
      DbConfiguration__PostgresConnectionString: Host=postgres;Port=5432;Database=flinkdb;Username=flink;Password=flink
    depends_on:
      - postgres
    restart: unless-stopped

volumes:
  pgdata:
```

## Kubernetes

### Kubernetes Deployment with SQLite

| Time to set up | Production-readiness |
|:---------------|:---------------------|
| < 15m          | ★★★★☆                |


```yaml
apiVersion: apps/v1
kind: Deployment
metadata:
  name: flink
  labels:
    app: flink
    sourcerepository: quay.io
    app.kubernetes.io/name: flink
    app.kubernetes.io/instance: flinkURLShortener
    app.kubernetes.io/version: latest
    app.kubernetes.io/component: webapp
    app.kubernetes.io/part-of: flink
    app.kubernetes.io/managed-by: kubectl
spec:
  replicas: 1
  selector:
    matchLabels:
      app: flink
  template:
    metadata:
      labels:
        app: flink
    spec:
      containers:
      - name: flink
        image: quay.io/rtraceio/flink:latest
        imagePullPolicy: Always
        ports:
        - containerPort: 8080
        livenessProbe:
          httpGet:
            path: /
            port: 8080
          initialDelaySeconds: 15
          periodSeconds: 10
        readinessProbe:
          httpGet:
            path: /
            port: 8080
          initialDelaySeconds: 5
          periodSeconds: 10
        env:
        - name: DBCONFIGURATION__DBTYPE
          value: "Sqlite"
        - name: DBCONFIGURATION__SQLITECONNECTIONSTRING
          value: "Data Source=/app/data/flink-live.sqlite;"
        resources:
          limits:
            memory: "256Mi"
        volumeMounts:
        - mountPath: /app/data
          name: flink-vol
      volumes:
      - name: flink-vol
        persistentVolumeClaim:
          claimName: <your-pvc-name>
```

- Add PVC
- Add Service (Loadbalancer, ClusterIP)
- Add Ingress Resource

**Warning**: Using SQLite as database it is NOT recommended to scale the `ReplicaCount` to > 1. There is a (high!) chance 2 `Flink` instances will concurrently write to the
Database and thus corrupt the SQLite file. SQLite is NOT made for multi-session access.
