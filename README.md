# WAF Simulator

A simple Web Application Firewall (WAF) simulator built to demonstrate common web attack detection and request filtering.
The project uses a reverse proxy architecture where incoming requests pass through the WAF before reaching a Spring Boot backend application. 
The WAF inspects request paths, query strings, and bodies to detect SQL Injection, Cross-Site Scripting (XSS), Path Traversal, and Command Injection attempts.
Malicious requests are blocked with detailed JSON responses, while safe requests are forwarded to the backend. 
The system also tracks repeated attacks by IP address and applies a temporary ban after multiple malicious requests within a defined time window.

## Architecture

```text
Angular Frontend
       ↓
.NET WAF
       ↓
Spring Boot Backend
```

## Features

* SQL Injection detection
* XSS detection
* Path Traversal detection
* Command Injection detection
* Rate Limiting
* Temporary IP banning based on repeated attack attempts
* JSON responses for blocked requests
* Simple web interface for testing attacks

<img width="1828" height="872" alt="Ekran görüntüsü 2026-09-28 231736" src="https://github.com/user-attachments/assets/12aee12f-d213-4de2-98b4-0853c65feda6" />

<img width="1813" height="855" alt="Ekran görüntüsü 2026-09-28 231803" src="https://github.com/user-attachments/assets/bb06dec8-4239-44da-ab4d-938813181345" />

<img width="900" height="751" alt="Ekran görüntüsü 2026-09-28 231812" src="https://github.com/user-attachments/assets/82310a89-dade-4011-ac87-cb58c87b7fcb" />

<img width="602" height="607" alt="Ekran görüntüsü 2026-09-28 231747" src="https://github.com/user-attachments/assets/48c4ad93-5709-4ed6-b8e0-4fe1743e72a1" />


<img width="781" height="747" alt="Ekran görüntüsü 2026-09-28 232000" src="https://github.com/user-attachments/assets/c8deec41-ebb9-4b30-97eb-f381a532b33c" />


## Technologies

* **.NET / ASP.NET Core**
* **Angular**
* **Spring Boot**


