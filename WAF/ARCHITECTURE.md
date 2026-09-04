# WAF (Web Application Firewall) Mimarisi

## Genel Akış

```
                  HTTP Request
                       │
                       ▼
              ┌─────────────────┐
              │ WAF Middleware  │
              │                 │
              │ Request Inspect │
              └────────┬────────┘
                       │
                       ▼
              ┌─────────────────┐
              │  WAF Engine     │
              └────────┬────────┘
                       │
                       ▼
              ┌─────────────────┐
              │ SQLi Detection  │
              │      Rule       │
              └────────┬────────┘
                       │
                 ┌─────┴─────┐
                 ▼           ▼
              BLOCK        ALLOW
                │             │
                ▼             ▼
             403          Spring Boot
                           Backend
```

## Bileşenler

### 1. WAF Middleware
- HTTP isteklerini keser ve ilk incelemeyi gerçekleştirir
- İstek parametrelerini, header'larını ve body'sini toplar
- Veriyi WAF Engine'e iletir

### 2. WAF Engine
- Tanımlı kuralları uygulamaya koyan merkezi bileşen
- Kural seti (Rule Set) üzerinden istekleri değerlendirir
- Karar vermeyi koordine eder (BLOCK/ALLOW)

### 3. Detection Rules

#### SQLi Detection Rule (Mevcut ⭐)
- SQL Injection saldırılarını tespit etmek için özel kural
- Şüpheli SQL sözdizimi desenlerini arar
- Parametre ve input validasyonu yapar

#### Diğer Detection Rules (Gelecek ⏳)
- XSS (Cross-Site Scripting) Detection
- CSRF Protection
- Path Traversal Detection
- Command Injection Detection
- Rate Limiting & DoS Protection

### 4. Karar Noktası (Decision Point)
- **BLOCK**: Kötü niyetli olduğu tespit edilen istekler 403 Forbidden yanıtı alır
- **ALLOW**: Güvenli istekler Spring Boot backend'e iletilir

## Teknoloji Stack
- **Framework**: Spring Boot
- **WAF Bileşeni**: Custom Java Implementation veya Spring Security entegrasyonu
- **Detection Engine**: Rule-based pattern matching
- **Karar Mekanizması**: Real-time filtering

## Mevcut Durumu (Sprint 1)
- ✅ **SQLi Detection**: Aktif ve çalışan
- ⏳ Diğer tehdit tespitleri: Gelecek versiyonlarda eklenecek
