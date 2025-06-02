# BugBucks‑Sandbox

**Amaç**

*FinTech & e‑ticaret* odaklı senaryolarda modern teknolojileri deneyebileceğim ve showcase olarak kullanabileceğim **mikro‑servis tabanlı bir sandbox** oluşturmak.


---

## Tech Stack (Docker Compose)

| Katman                | Bileşen                 | Sürüm                                  | Amaç                                     |
|-----------------------| ----------------------- | -------------------------------------- | ---------------------------------------- |
| **Framework**         | .NET 9 / ASP.NET Core   | **9.0**                                | Mikro‑servis, Minimal API & gRPC |
| **Veritabanı**        | MariaDB                 | **11.7**                               | Transactional data (Identity, Order etc.) |
| **Message Broker**    | RabbitMQ *(management)* | **4**                                  | Event‑driven integration, retry/DLQ      |
| **Logging**           | Serilog → Seq • Loki    | `latest`                               | Structured logging & querying            |
| **Tracing / Metrics** | OpenTelemetry Collector | `otel/opentelemetry-collector-contrib` | OTLP gRPC/HTTP alıcı, Prometheus scrape  |
|                       | Tempo                   | `grafana/tempo:latest`                 | Dağıtık izleme (traces)                  |
|                       | Prometheus              | `prom/prometheus`                      | Time‑series metrics & alerting           |
| **Dashboard**         | Grafana                 | `grafana/grafana`                      | Log / trace / metric dashboards          |
| **Arama**             | Elasticsearch + Kibana  | **7.17.28**                            | Full‑text search & analytics (optional)  |
| **Secrets**           | HashiCorp Vault         | `latest`                               | Secret management (PoC)                  |

---

## Servisler

| Host Port | Servis            | Açıklama                                            |
| --------- | ----------------- | --------------------------------------------------- |
| **6000**  | `gateway.api`     | Tek giriş noktası (YARP tabanlı reverse‑proxy)      |
| **6001**  | `identityservice` | JWT-based authentication / authorization            |
| **6002**  | `checkoutservice` | Sipariş akışı & ödeme orkestrasyonu (saga + outbox) |
| **6003**  | `orderservice`    | Sipariş yönetimi                                    |
| **6004**  | `paymentservice`  | Mock ödeme servisi                                  |

> Her servis container içinde `8080` üzerinde hem REST hem gRPC sunar; host port eşlemeleri üstte.

---

## Hızlı Başlangıç

```bash
# Clone
$ git clone https://github.com/deniz-dalkilic/BugBucks-Sandbox.git \
    && cd BugBucks-Sandbox/docker

# Start the stack
$ docker compose up -d --build

# Health check
$ curl http://localhost:6000/health
```

İlk kalkışta **MariaDB** içinde şema dump’ları otomatik uygulanır.

---

## Dashboard & Araçlar

| URL                                              | İçerik                                   |
| ------------------------------------------------ |------------------------------------------|
| [http://localhost:5341](http://localhost:5341)   | **Seq** — structured log viewer          |
| [http://localhost:3000](http://localhost:3000)   | **Grafana** — dashboards (*admin/admin*) |
| [http://localhost:15672](http://localhost:15672) | **RabbitMQ** yönetim UI (guest/guest)    |
| [http://localhost:9200](http://localhost:9200)   | Elasticsearch (tek node)                 |
| [http://localhost:5601](http://localhost:5601)   | Kibana                                   |

---

## Yol Haritası

* Gerçek zamanlı ekonomik veri entegrasyonu (TCMB, TÜİK, etc.)
* Sadakat / kampanya modülü (müşteri segmentasyonu)
* AsyncAPI dokümantasyonu + NATS Streaming PoC
* Terraform & GitHub Actions ile tam CI/CD (opsiyonel K8s)
