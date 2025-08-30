# BlogBE – Blog Backend API

A containerised backend API for a blog platform, built with **.NET** and orchestrated with **Docker Compose**, deployed to **AWS EC2** with Postgres, MongoDB, and Redis.

---

## 🚀 Live Demo
**Swagger UI**: [http://13.239.35.170/swagger](http://13.239.35.170/swagger)  


---

## 🛠 Tech Stack
- **Backend**: ASP.NET Core Web API
- **Database**: PostgreSQL (relational data)
- **NoSQL Store**: MongoDB (activity logs)
- **Cache**: Redis
- **Containerisation**: Docker & Docker Compose
- **Cloud Hosting**: AWS EC2 (Amazon Linux 2023)
- **API Docs**: Swagger / OpenAPI

---

## 🗂 Architecture Overview
```
[ Client / Browser ]
          |
          v
  http://13.239.35.170:80
          |
          v
+-----------------------------+
|        EC2 Host (AWS)       |
|   Security Group allows 80  |
+-----------------------------+
          |
          v
+-----------------------------+
|   Docker (Compose project)  |
|   Published port: 80 -> 8080|
+-----------------------------+
          |
          v
+-------------------------------------------+
|         Private Docker network            |
|         (auto-created by Compose)         |
|                                           |
|  [ app (ASP.NET Core) :8080 ]             |
|       |            |            |         |
|   5432|       27017|        6379|         |
|       v            v            v         |
|  [ postgres ]  [ mongo ]    [ redis ]     |
|   (no ports)   (no ports)   (no ports)    |
+-------------------------------------------+

Legend:
- Host publishes only app: host:80 -> app:8080
- DB containers do NOT publish ports to host; reachable only inside the Docker network
- Service discovery inside the network uses service names: postgres:5432, mongo:27017, redis:6379

```

---

## ✨ Key Highlights
- **Secure container networking**: DB services are not exposed to the internet — only accessible to the API via service names (`postgres`, `mongo`, `redis`).
- **Environment‑based config**: `.env` file controls connection strings, JWT secrets, and environment settings.
- **Cloud‑ready**: Configured for deployment on AWS EC2 with minimal changes from local dev.
- **Scalable foundation**: Easily extendable to Kubernetes or ECS in the future.

---

## 📚 What I Learned
This project was as much about building a backend as it was about mastering the **end‑to‑end delivery pipeline**.

Key takeaways:
- **Docker Networking**: How containers communicate internally without exposing sensitive ports, and how service discovery works via Compose service names.
- **Environment Management**: Using `.env` files to separate local and cloud configurations cleanly.
- **Cloud Deployment**: Provisioning and securing an EC2 instance, mapping container ports to host ports, and configuring security groups.
- **Service Orchestration**: Running multiple dependent services (API, Postgres, Mongo, Redis) in a single Compose stack with proper startup order.
- **Operational Resilience**: Using restart policies and systemd integration to ensure the app survives EC2 reboots.
- **Security Awareness**: Minimising attack surface by keeping databases internal‑only and exposing only the API.

---

## 📌 Access Instructions
1. Visit the **Swagger UI** link above.
2. Explore available endpoints and try them out directly in the browser.
3. No local setup required for inspection — the live EC2 instance is ready.

---
