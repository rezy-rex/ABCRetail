# ABC Retail Cloud Migration Project

> **Module:** CLDV7112/w – Cloud Development B  
> **Assessment:** Project 1 – Azure Storage Solution  
> **Framework:** ASP.NET Core MVC (.NET 8)  
> **Cloud Platform:** Microsoft Azure

## 📖 Overview
This project demonstrates a scalable, cloud-native architecture for ABC Retail, replacing aging on-premises infrastructure with modern Azure Storage services. The application provides a centralized management portal for handling structured data, multimedia assets, asynchronous order events, and application logs, directly addressing peak-season transaction bottlenecks and legacy middleware limitations.

##  Features
| Service | Implementation |
|---------|----------------|
| **Azure Tables** | CRUD operations for customer profiles & product catalogue using `ITableEntity` |
| **Azure Blobs** | Public container for product images & multimedia with drag-and-drop upload & gallery preview |
| **Azure Queues** | Base64-encoded order/inventory events with peek, enqueue, and dequeue processing |
| **Azure Files** | Managed file share for `.txt`/`.log` creation, file uploads, and inline log viewer |
| **UI/UX** | Elite light-theme design, responsive grid layout, micro-interactions, and toast-style alerts |

##  Tech Stack
- **Backend:** ASP.NET Core MVC (.NET 8), C#
- **Azure SDKs:** `Azure.Data.Tables`, `Azure.Storage.Blobs`, `Azure.Storage.Queues`, `Azure.Storage.Files.Shares`
- **Frontend:** Razor Views, CSS3 (Custom Design System), Vanilla JavaScript
- **Deployment:** Azure App Service (Windows, F1 Free Tier)
- **Tools:** Visual Studio 2022, Git, GitHub, Azure Portal

##  Prerequisites
- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [Visual Studio 2022](https://visualstudio.microsoft.com/) (with ASP.NET & web development workload)
- Active Azure Subscription with Storage Account & App Service provisioned
- Git installed

##  Local Setup
1. **Clone the repository**
   ```bash
   git clone git clone https://github.com/rezy-rex/ABCRetail.git
   cd ABCRetail
