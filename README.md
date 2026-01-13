# Remoview 🎬 (BLM4531 Project)
Video Linki:
PDF Linki:

Remoview; film listeleme, film detay görüntüleme, puan verme, yorum yapma ve favorilere ekleme özellikleri sunan bir web uygulamasıdır.  
Proje; **ASP.NET Core Web API (Backend)** + **Blazor (Frontend)** + **PostgreSQL (Database)** mimarisiyle geliştirilmiştir.

Bu proje kapsamında ayrıca **moderasyon (onay/red) sistemi** eklenmiştir:
- Kullanıcıların eklediği **filmler** ve **yorumlar** ilk aşamada **Pending** olarak kaydedilir.
- **SuperAdmin** panelinden onaylanan içerikler (**Approved**) ana sayfada ve film detaylarında görünür.
- Reddedilen içerikler (**Rejected**) yayınlanmaz.

---

## İçerik / Modüller

- ✅ Kullanıcı kayıt & giriş (JWT)
- ✅ Film listeleme (yalnızca Approved)
- ✅ Film detay sayfası (yalnızca Approved + yorumlarda Approved)
- ✅ Film ekleme (Pending)
- ✅ Filme yorum ekleme (Pending)
- ✅ Filme puan verme (Approved filmler için)
- ✅ Favori sistemi (ekle/çıkar, profil ekranında görüntüle)
- ✅ Moderasyon paneli (SuperAdmin):
  - Pending film/yorum listeleme
  - Approve / Reject işlemleri
  - Moderasyon meta alanları (ModeratedByUserId, ModeratedAtUtc, ModerationNote)

---

## Kullanılan Teknolojiler

**Backend**
- ASP.NET Core Web API
- Entity Framework Core (Code First)
- PostgreSQL (Npgsql Provider)
- JWT Authentication

**Frontend**
- Blazor (Interactive Server)
- HttpClient ile API tüketimi

**Database**
- PostgreSQL
- EF Core Migrations

---

## Kurulum ve Çalıştırma

### 1) Gereksinimler
- .NET SDK (8.x önerilir)
- PostgreSQL (14+ önerilir)
- Visual Studio 2022 / VS Code (opsiyonel)
- Git

### 2) Veritabanı Ayarı
PostgreSQL üzerinde bir veritabanı oluşturun. Örnek:
- DB Name: `removiewdb`

Backend projesindeki `appsettings.json` (veya `appsettings.Development.json`) içine bağlantı ayarını girin:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=removiewdb;Username=postgres;Password=YOUR_PASSWORD"
  }
}
