# 💱 Currency Converter API

A secure, scalable and observable currency conversion API built with **.NET 8** for the BambooCard technical task.

---

## ⚙️ Getting Started

### Prerequisites:
- [.NET 8 SDK](https://dotnet.microsoft.com/en-us/download)
- Docker

---

### 1. Run with Docker Compose

docker compose up -d --build

---

### 2. Run the API

dotnet restore
dotnet run

---

### 3. Run Jaeger for Tracing (via Docker)

docker run -d --name jaeger -e COLLECTOR_ZIPKIN_HOST_PORT=:9411 -p 6831:6831/udp -p 16686:16686 jaegertracing/all-in-one:1.50

---

### 4. Run Seq for Log (via Docker)
docker run -d --name seq -e ACCEPT_EULA=Y  -p 5341:80 datalust/seq:latest

---

### 5. How to get a JWT token
POST /api/auth/token
Content-Type: application/json

{
  "userId": "test123",
  "role": "Admin",
}

---
